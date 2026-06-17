using UnityEngine;


//플레이어 발견 - 사거리 밖 - 추적
//사거리 진입 - 정지 - 공격
//플레이어 멀어짐 - 다시 추적

public class ArcherMonster : MonsterBase
{
    [Header("원거리 몬스터 설정")]
    public float attackRange = 7f; //사거리
    public float attackCooldown = 2f; // 공격주기
    private float attackTimer;

    protected override void OnEnable()
    {
        base.OnEnable();
        attackTimer = 0f;
    }


    //플레이어 추적
    protected override void Chase()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        //사거리 안
        if (distance <= attackRange)
        {
            currentState = MonsterState.Attack;

            Debug.Log($"{name}Attack 상태 진입");
            return;
        }
        //플레이어 방향 계산
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        //플레이어 추적
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);

    }

    //원거리 공격
    protected override void Attack()
    {
        if(player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);


        //플레이어가 사거리 밖으로 나감
        if(distance>attackRange)
        {
            currentState= MonsterState.Chase;
            return;
        }

        attackTimer += Time.deltaTime;
        if(attackTimer>=attackCooldown)
        { 
            attackTimer = 0f;
            Shoot();
        }
    }

    //투사체 발사
    void Shoot()
    {
        GameObject arrow = PoolManager.Instance.GetArrow();

        if (arrow == null) return;

        //총알 생성 위치
        arrow.transform.position = transform.position;

        //플레이어 방향 계산
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;

        arrow.GetComponent<Arrow>().Initialized(dir);

        Debug.Log("원거리 몬스터 발사");
    }
}
