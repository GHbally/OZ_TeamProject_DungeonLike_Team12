using UnityEngine;


//플레이어 발견 - 사거리 밖 - 추적
//사거리 진입 - 정지 - 공격
//플레이어 멀어짐 - 다시 추적

public class ArcherMonster : MonsterBase
{
    [Header("원거리 몬스터 설정")]
    public float attackRange = 7f;      //사거리
    public float attackCooldown = 2f;   //공격주기
    private float attackTimer;          //연사 속도 타이머

    public float attackPreDelay = 0.5f; //선공격 딜레이

    protected override void OnEnable()
    {
        base.OnEnable();    //부모의 체력, 플레이어 추적 받아오기
        attackTimer = 0f;   //화살 타이머 0으로 시작하게 세팅
    }


    //사거리별 상태변화 로직
    protected override void UpdateState()
    {
        if (player == null) return;
        //현재 아처 위치와 플레이어 위치 사이의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);

        //사거리 안이면
        if (distance <= attackRange)
        {
            currentState = MonsterState.Attack; //공격상태로
        }
        //아니면
        else
        {
            currentState = MonsterState.Chase;  //추적상태로
        }     
    }

    //원거리 공격
    protected override void AttackLogic()
    {
        attackTimer += Time.deltaTime;  //화살 쿨타임 타이머에 시간을 계속 더해줌

        //쿨타임 마다
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;       //타이머 0으로 초기화하고
            StartAttackMotion();    //공격 모션
        }
    }

    void StartAttackMotion()
    {
        if (animator != null)
        {
            animator.SetTrigger("6_Other");

            Invoke(nameof(Shoot), attackPreDelay);
        }
    }

    //투사체 발사
    void Shoot()
    {
        //오브젝트 풀링으로 화살 가져오기
        GameObject arrow = PoolManager.Instance.GetArrow();

        if (arrow == null) return;  //화살이 있을때만 실행

        //화살 생성 위치
        arrow.transform.position = transform.position + new Vector3(0, 0.5f, 0);

        Vector3 targetPosition = player.position + new Vector3(0, 0.6f, 0);

        //플레이어 방향 계산
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;

        arrow.GetComponent<Arrow>().Initialized(dir);

        //Debug.Log("원거리 몬스터 발사");
    }
}
