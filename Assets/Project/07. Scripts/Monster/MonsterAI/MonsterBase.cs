//[몬스터 부모 클래스]
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("몬스터 능력치")]
    public MonsterType monsterType;
    public int maxHp = 100;             //최대 체력
    protected int currentHp;            //현재 체력
    public float moveSpeed = 3f;        //이동속도
    public Transform player;            //플레이어 위치
    public MonsterState currentState;   //현재 몬스터 상태
    protected Rigidbody2D rb;

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

    //[몬스터 이동 메서드]
    private void MoveTowardsPlayer()
    {
        Vector2 dir = ((Vector2)player.position - rb.position).normalized; //방향 계산
        //부드러운 이동
        rb.MovePosition(rb.position + dir * moveSpeed * Time.fixedDeltaTime);
    }

    //[자식 클래스에게 토스할 메서드]
    protected abstract void UpdateState();  //어느 사거리에서 상태를 전환할지 (원거리몹 수행 또는 보스 패턴)
    protected abstract void AttackLogic();  //어느 타이밍에 원거리 공격할지 (원거리몹 수행)

    //[피격 메서드]
    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
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
