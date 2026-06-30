//[몬스터 부모 클래스]
using System.Collections.Generic;               //List 사용을 위해 추가
using UnityEngine;
using System.Collections;

public abstract class MonsterBase : MonoBehaviour, IDamageable1
{
    [Header("몬스터 능력치")]
    public MonsterType monsterType;
    public float maxHp = 100;                   //최대 체력
    protected float currentHp;                  //현재 체력
    public float moveSpeed = 3f;                //이동속도

    //추가: 풀링 재사용 시 능력치가 계속 누적되는 것을 막기 위한 기본값 저장 변수
    private float baseMaxHp;                    // 원래 최대 체력 저장
    private float baseMoveSpeed;                // 원래 이동속도 저장

    public Transform player;                    //플레이어 위치
    public MonsterState currentState;           //현재 몬스터 상태

    [Header("몬스터 스프라이트 제어")]
    [SerializeField] protected float monsterScale = 1.0f; //몬스터 기본 크기 설정용(인스펙터 조절)
    protected Animator animator;                //에셋 애니메이션 제어용 컨포넌트 변수
    protected Transform visualTransform;        //외형 이미지를 담고 있는 자식 오브젝트 위치 변수

    //단일 SpriteRenderer대신 자식 모든 스프라이트를 담을 리스트로 변경
    protected List<SpriteRenderer> monsterRenderers = new List<SpriteRenderer>();
    protected SpriteRenderer mainSpriteRenderer;    //사망 페이드 아웃용

    [Header("몬스터 분리")]
    // 주변 몬스터를 탐색할 범위
    public float separationRadius = 1.0f;
    // 몬스터끼리 밀어내는 힘
    public float separationForce = 1.5f;

    [Header("피격 연출 설정")]
    [SerializeField] private Color hitColor = new Color(1.0f, 0.3f, 0.3f, 1f);
    [SerializeField] private float hitFlashDuration = 0.1f; //깜빡이는 시간
    private Coroutine hitCoroutine; //중복 피격 시 코루틴 제어용 변수

    [Header("레이캐스트 설정")]
    public float rayDistance = 0.5f;     // 몬스터 크기에 따라 조절
    public LayerMask obstacleLayer;      // 맵 오브젝트 레이어 선택 (인스펙터에서 지정)

    [Header("피격 경직")]
    [SerializeField] private float hitStunTime = 0.1f; //경직시간
    private bool isHitStun = false; //현재 경직 중인지

    protected Rigidbody2D rb;
    protected bool isDead;                      //몬스터 죽은 상태
    public bool isBossSummonedMonster = false; // 이 몬스터가 최종보스가 소환한 몬스터인지 확인하는 변수

    private bool isStuck = false; // 현재 끼어있는지 상태 변수

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //자식 오브젝트 Visual과 그 안의 애니메이터 찾아오기

        // 추가: 인스펙터에서 설정한 원래 능력치를 저장
        baseMaxHp = maxHp;
        baseMoveSpeed = moveSpeed;

        visualTransform = transform.Find("Visual");

        if (visualTransform != null)
        {
            //Visual 자식 아래에서 애니메이터 찾아오기
            animator = visualTransform.GetComponentInChildren<Animator>();

            //Visual 자식들 중에서 모든 SpriteRenderer를 긁어오기 (스켈레탈 구조 대응)
            SpriteRenderer[] allRenderers = visualTransform.GetComponentsInChildren<SpriteRenderer>();

            foreach (SpriteRenderer sr in allRenderers)
            {
                string objName = sr.gameObject.name.ToLower();
                //그림자나 눈이 이름에 포함된 스프라이트는 피격 연출에서 제외!
                if (objName.Contains("shadow") || objName.Contains("eye"))
                {
                    continue;
                }

                monsterRenderers.Add(sr);

                //사망 페이드 아웃 때 사용할 메인 스프라이트 Renderer 하나를 임시 저장
                if (mainSpriteRenderer == null) mainSpriteRenderer = sr;
            }
        }
    }

    protected virtual void OnEnable() 
    {
        // 추가: 풀에서 다시 꺼낼 때마다 원래 능력치로 초기화
        maxHp = baseMaxHp;
        moveSpeed = baseMoveSpeed;


        currentHp = maxHp;                  //체력 초기화
        currentState = MonsterState.Chase;  //추적시작

        isDead = false;                     //사망 상태 초기화
        isBossSummonedMonster = false;

        ResetRenderersColor();              //풀에서 나올때 피격 색상 초기화

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.linearVelocity = Vector2.zero;
        }

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.enabled = true;
        }

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;   //플레이어 위치 연결

            Debug.Log("플레이어 찾음 : " + player.name);
        }
        else 
        {
            // 못 찾았을 때 콘솔 오류 출력
            Debug.LogError("Player 태그를 가진 오브젝트를 찾지 못했습니다.");
        }
    }

    protected virtual void Update()
    {
        if (currentState == MonsterState.Dead) return; //죽었으면 로직 정지

        HandleSpriteAndAnimation(); //애니메이션 & 방향전환 처리

        /////////////////////////////테스트 공격/////////////////////////////////
        if (Input.GetKeyDown(KeyCode.K))
        {
            Death();
        }

        //거리를 재서 Chase나 Attack으로 상태 바꿔주는 애 (자식이 구현)
        UpdateState();

        //아처 활 쿨타임 같이 시간 연산만 실행 (자식이 구현) +마법사몹 추가하면 같이 써도 될듯
        if(currentState == MonsterState.Attack)
        {
            AttackLogic();
        }
    }

    public void ApplyStageMultiplier
    (
        float hpMultiplier,
        float speedMultiplier
    )
    {
        // 수정: 기존 maxHp에 계속 곱하지 않고, 원래 체력 기준으로 계산
        maxHp = baseMaxHp * hpMultiplier;

        // 증가된 최대 체력으로 현재 체력 설정
        currentHp = maxHp;

        // 수정: 기존 moveSpeed에 계속 곱하지 않고, 원래 속도 기준으로 계산
        moveSpeed = baseMoveSpeed * speedMultiplier;
    }

    //스프라이트 제어용 메서드
    private void HandleSpriteAndAnimation()
    {
        if (player == null || visualTransform == null) return;

        //방향전환(Flip)
        //플레이어가 몬스터보다 오른쪽에 있다면
        if (player.position.x > transform.position.x)
        {
            //오른쪽 보기(에셋기준 설정)
            visualTransform.localScale = new Vector3(-monsterScale, monsterScale, 1f);
        }
        else //왼쪽이라면
        {
            //왼쪽 보기(에셋기준 설정)
            visualTransform.localScale = new Vector3(monsterScale, monsterScale, 1f);
        }

        if (animator == null) return;

        if (currentState == MonsterState.Chase)
        {
            animator.SetBool("1_Move", true);
        }
        else
        {
            animator.SetBool("1_Move", false);
        }
    }
    // 물리 연산이 일정한 시간 간격으로 실행되는 함수
    protected virtual void FixedUpdate()
    {
        // 몬스터가 죽었으면 더 이상 이동하지 않음
        if (currentState == MonsterState.Dead)return;

        if (isHitStun) return; //경직 상태에서는 몬스터 움직임 멈춤

        // 이동 방향을 저장할 변수
        Vector2 moveDir = Vector2.zero;

        // 현재 상태가 추적이고 플레이어가 존재하면
        if (currentState == MonsterState.Chase && player != null)
        {
            // 플레이어를 향하는 방향 계산
            // normalized : 방향만 남기고 길이를 1로 만들어 일정한 속도로 이동
            moveDir = ((Vector2)player.position - rb.position).normalized;
        }

        // 주변 몬스터를 피해가는 방향 계산
        Vector2 separationDir = GetSeparationDirection();

        // 플레이어를 향하는 방향과 몬스터끼리 밀어내는 방향을 합침
        // separationForce가 클수록 더 강하게 밀려남

        Vector2 finalDir =(moveDir + separationDir * separationForce).normalized;

        // 이동할 방향이 존재하면
        if (finalDir != Vector2.zero)
        {
            // --- [끼임 체크 추가] ---
            // 몬스터 위치에 이미 벽(Object 레이어)이 있는지 확인
            Collider2D hitCol = Physics2D.OverlapPoint(rb.position, obstacleLayer);

            // 겹쳐 있다면 레이캐스트를 쏘지 않고 이동을 강제로 허용 (탈출)
            if (hitCol != null)
            {
                // 끼어있는 상태라면 벽을 뚫고 나올 수 있도록 이동 로직 강제 실행
                rb.MovePosition(rb.position + finalDir * moveSpeed * Time.fixedDeltaTime);
            }
            else
            {
                // 벽이 없을 때만 레이캐스트 정상 동작
                ContactFilter2D filter = new ContactFilter2D();
                filter.SetLayerMask(obstacleLayer);
                filter.useLayerMask = true;

                RaycastHit2D[] hits = new RaycastHit2D[1];
                int hitCount = Physics2D.Raycast(rb.position + (finalDir * 0.1f), finalDir, filter, hits, rayDistance);

                if (hitCount == 0)
                {
                    rb.MovePosition(rb.position + finalDir * moveSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
    
    private Vector2 GetSeparationDirection()
    {
        // separationRadius 안에 있는 모든 Collider 검색
        Collider2D[] nearby =Physics2D.OverlapCircleAll(
                transform.position,
                separationRadius);

        // 최종적으로 밀려날 방향을 저장
        Vector2 separation = Vector2.zero;

        // 주변의 모든 Collider 검사
        foreach (Collider2D col in nearby)
        {
            // 부모 오브젝트에서 MonsterBase 가져오기
            MonsterBase monster =
                col.GetComponentInParent<MonsterBase>();

            // 몬스터가 아니면 건너뜀
            if (monster == null)continue;

            // 자기 자신이면 건너뜀
            if (monster == this) continue;

            // 상대 몬스터 반대 방향 계산
            Vector2 away = rb.position - (Vector2)monster.transform.position;

            // 두 몬스터 사이 거리 계산
            float distance = away.magnitude;

            // 너무 가까우면
            if (distance > 0.01f && distance < separationRadius)
            {
                // 가까울수록 더 강하게 밀어냄
                separation += away.normalized * (separationRadius - distance);
            }
        }

        // 여러 방향을 하나의 방향으로 합쳐 반환
        // normalized를 사용하는 이유는 이동속도가 너무 빨라지는 것을 막기 위해서
        return separation.normalized;
    }
    ///////////////////플레이어 추적 이동////////////////////////
    private void MoveTowardsPlayer()
    {
        //방향 계산
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;

        ///////////////몬스터 분리/////////////////
        //주변의 모든 콜라이더를 검색
        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        //몬스터를 밀어내는 방향을 저장
        Vector2 separation = Vector2.zero;

        foreach (Collider2D col in nearby)
        {
            //자기 자신은 제외
            if(col.gameObject == gameObject) continue;

            //몬스터베이스가 붙어있는 몬스터만 검사
            MonsterBase monster = col.GetComponent<MonsterBase>();

            if (monster != null)
            {
                //상대 몬스터 반대 방향을 계산
                Vector2 away = (Vector2)(transform.position - col.transform.position);
                float distance = away.magnitude;

                /*******************
                 필요한 이유
                전사 A     전사 B
                A와 B의 거리가 너무 가까워짐
                서로 반대 방향으로 살짝 이동하려고 함
                완전히 겹치는 현상 감소
                *********************/
                // 다른 몬스터와 너무 가까운 경우
                if (distance > 0.01f && distance < separationRadius)
                {
                    //상대 몬스터 반대 방향으로 밀어내기
                    Vector2 push = away.normalized * (separationRadius - distance);
                    //밀어내는 방향을 누적
                    separation += push;
                }
            }

        }

        //플레이어 추적방향과 몬스터 분리 방향
        Vector2 finalDir = (dir + separation * separationForce).normalized;

        // 플레이어 근처까지 오면 멈춤
        float stopDistance = 0.1f;
        //플레이어와의 거리 계산
        float distanceToPlayer = Vector2.Distance(rb.position, player.position);

        if (distanceToPlayer > stopDistance)
        {
            rb.MovePosition
            (
                rb.position +
                finalDir * moveSpeed * Time.fixedDeltaTime
            );
        }
    }

    //[자식 클래스에게 토스할 메서드]
    protected abstract void UpdateState();  //어느 사거리에서 상태를 전환할지 (원거리몹 수행 또는 보스 패턴)
    protected abstract void AttackLogic();  //어느 타이밍에 원거리 공격할지 (원거리몹 수행)

    //[피격 메서드]
    //public virtual void TakeDamage1(int damage)
    //{
    //    currentHp -= damage;
    //    if (currentHp <= 0)
    //    {
    //        Death();
    //    }
    //}

    //[피격 메서드]
    public virtual void TakeDamage(DamageInfo1 damageInfo)
    {
        if (isDead)
        {
            return;
        }
        currentHp -= damageInfo.Damage;

        StartCoroutine(HitStunCo()); //피격 경직 시작

        Debug.Log(
            $"{name} 피해: {damageInfo.Damage}, " +
            $"치명타: {damageInfo.IsCritical}, " +
            $"남은 체력: {currentHp}"
        );



        //피격 시 연출 코루틴 재생(이미 도는 중이면 끄고 새로 시작)
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitFlashCo());

        if (currentHp <= 0f)
        {
            //사망 직전 플래시 강제 시행
            if (hitCoroutine != null) StopCoroutine(hitCoroutine);

            foreach (SpriteRenderer sr in monsterRenderers)
            {
                if (sr != null) sr.color = hitColor; //인스펙터에서 지정한 몬스터 피격 색상
            }

            Death();
        }
    }

    private IEnumerator HitStunCo()
    {
        isHitStun = true; // 이동 정지

        yield return new WaitForSeconds(hitStunTime);

        isHitStun = false; // 다시 이동
    }

    //[피격 연출 코루틴]
    private System.Collections.IEnumerator HitFlashCo()
    {
        //지정된 피격 색상으로 변경
        foreach (SpriteRenderer sr in monsterRenderers)
        {
            if (sr != null) sr.color = hitColor;
        }

        //지정된 시간만큼 대기
        yield return new WaitForSeconds(hitFlashDuration);

        //원래 색상(원래 흰색)으로 원상복구
        ResetRenderersColor();
    }

    //[렌더러 색을 기본값으로 돌려놓는 메서드]
    private void ResetRenderersColor()
    {
        foreach (SpriteRenderer sr in monsterRenderers)
        {
            if (sr != null) sr.color = Color.white;
        }
    }


    //[사망 메서드]
    protected virtual void Death()
    {
        isDead = true;

        //죽을 때 피격코루틴 도는중이면 정지하고 리셋
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);

        if (currentState == MonsterState.Dead) return;
        currentState = MonsterState.Dead; //죽은 상태로

        if (HUDController.Instance != null)
        {
            HUDController.Instance.UpdateKillCount();
        }

        //시체가 플레이어를 따라오지 못하게 물리랑 충돌 잠금
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        //서서히 사라지는 사망 시퀀스 코루틴
        StartCoroutine(DeathSequenceCo());
    }

    //[사망 시퀀스 코루틴]
    private System.Collections.IEnumerator DeathSequenceCo()
    {
        if (animator != null)
        {
            animator.SetBool("1_Move", false);
            animator.SetTrigger("4_Death");
        }

        //사망 애니메이션 끝날때까지 대기
        yield return new WaitForSeconds(0.8f);

        //페이드아웃을 위해 색상을 원래 화이트로 초기화
        ResetRenderersColor();

        //모든 신체파츠를 동시에 서서히 투명하게 만듦
        if (monsterRenderers.Count > 0)
        {
            float duration = 0.5f;
            float elapsed = 0f;

            //페이드아웃 시작 전 모든 파츠의 원래 컬러들 기억하기
            List<Color> originalColors = new List<Color>();
            foreach (SpriteRenderer sr in monsterRenderers)
            {
                if (sr != null) originalColors.Add(sr.color);
                else originalColors.Add(Color.white);
            }

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

                //모든 파츠의 알파값을 매 프레임 동시에 깎아내림
                for (int i = 0; i < monsterRenderers.Count; i++)
                {
                    if (monsterRenderers[i] != null)
                    {
                        Color orig = originalColors[i];
                        monsterRenderers[i].color = new Color(orig.r, orig.g, orig.b, alpha);
                    }
                }
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        // 소멸 후 경험치 또는 힐 구슬 중 하나만 드랍
        if (DropManager.Instance != null)
        {
            if (Random.value < 0.1f)
            {
                DropManager.Instance.DropHealOrb(transform.position);
            }
            else
            {
                DropManager.Instance.DropExp(transform.position);
            }
        }

        //오브젝트 풀링 연동
        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.ReturnMonster(gameObject);
        }

        //스테이지 웨이브 관리하는 매니저 찾아서
        var waveManager = FindFirstObjectByType<WaveManager>();

        if (waveManager != null)
        {
            // 일반 몬스터만 WaveManager에 알림
            if (!isBossSummonedMonster)
            {
                waveManager.MonsterDead(transform.position);
            }
        }
    }
}
