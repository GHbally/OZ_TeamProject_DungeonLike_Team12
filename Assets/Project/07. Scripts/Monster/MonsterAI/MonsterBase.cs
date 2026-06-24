//[몬스터 부모 클래스]
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour, IDamageable1
{
    [Header("몬스터 능력치")]
    public MonsterType monsterType;
    public float maxHp = 100;                   //최대 체력
    protected float currentHp;                  //현재 체력
    public float moveSpeed = 3f;                //이동속도
    public Transform player;                    //플레이어 위치
    public MonsterState currentState;           //현재 몬스터 상태

    [Header("몬스터 스프라이트 제어")]
    [SerializeField] protected float monsterScale = 1.0f; //몬스터 기본 크기 설정용(인스펙터 조절)
    protected Animator animator;                //에셋 애니메이션 제어용 컨포넌트 변수
    protected Transform visualTransform;        //외형 이미지를 담고 있는 자식 오브젝트 위치 변수
    protected SpriteRenderer spriteRenderer;    //페이드 아웃용

    [Header("몬스터 분리")]
    // 주변 몬스터를 탐색할 범위
    public float separationRadius = 1.0f;
    // 몬스터끼리 밀어내는 힘
    public float separationForce = 1.5f;

    protected Rigidbody2D rb;

    protected bool isDead;                      //몬스터 죽은 상태

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //자식 오브젝트 Visual과 그 안의 애니메이터 찾아오기
        visualTransform = transform.Find("Visual");

        if (visualTransform != null)
        {
            //Visual 자식 아래에서 애니메이터 찾아오기
            animator = visualTransform.GetComponentInChildren<Animator>();
        }
    }

    protected virtual void OnEnable() 
    {
        currentHp = maxHp;                  //체력 초기화
        currentState = MonsterState.Chase;  //추적시작

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

        if (currentState == MonsterState.Chase)
        {
            animator.SetBool("1_Move", true);
        }
        else
        {
            animator.SetBool("1_Move", false);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (currentState == MonsterState.Dead) return; //죽었으면 로직 정지

        //추적 상태일 때만 플레이어를 쫓아감
        if (currentState == MonsterState.Chase && player != null)
        {
            MoveTowardsPlayer();
        }
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

        Debug.Log(
            $"{name} 피해: {damageInfo.Damage}, " +
            $"치명타: {damageInfo.IsCritical}, " +
            $"남은 체력: {currentHp}"
        );

        if (currentHp <= 0f)
        {
            Death();
        }
    }


    //[사망 메서드]
    protected virtual void Death()
    {
        if (currentState == MonsterState.Dead) return;
        currentState = MonsterState.Dead; //죽은 상태로

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

        if (spriteRenderer != null)
        {
            float duration = 0.5f;
            float elapsed = 0f;
            Color originalColor = spriteRenderer.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
                spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        //소멸 후 경험치 드랍
        if (DropManager.Instance != null)
        {
            DropManager.Instance.DropExp(transform.position);

            //힐 구슬
            if (Random.value < 0.1f)
            {
                DropManager.Instance.DropHealOrb(transform.position); //힐 구슬 구현 예정
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
            //몬스터 사망했으니 개체수 1 줄이기(웨이브당 총 몬스터 수랑 연동)
            waveManager.MonsterDead();
        }
    }
}
