//[몬스터 부모 클래스]
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour, IDamageable1
{
    [Header("몬스터 능력치")]
    public MonsterType monsterType;
    public float maxHp = 100;             //최대 체력
    protected float currentHp;            //현재 체력
    public float moveSpeed = 3f;        //이동속도
    public Transform player;            //플레이어 위치
    public MonsterState currentState;   //현재 몬스터 상태

    [Header("몬스터 분리")]
    // 주변 몬스터를 탐색할 범위
    public float separationRadius = 1.0f;
    // 몬스터끼리 밀어내는 힘
    public float separationForce = 1.5f;

    protected Rigidbody2D rb;

    protected bool isDead;                //몬스터 죽은 상태

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void OnEnable() 
    {
        currentHp = maxHp;                  //체력 초기화
        currentState = MonsterState.Chase;  //추적시작

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;   //플레이어 위치 연결
        }
    }

    protected virtual void Update()
    {
        if (currentState == MonsterState.Dead) return; //죽었으면 로직 정지

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
        float stopDistance = 0.8f;
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


    //사망
    protected virtual void Death()
    {
        currentState = MonsterState.Dead; //죽은 상태로

        //경험치 드랍
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
