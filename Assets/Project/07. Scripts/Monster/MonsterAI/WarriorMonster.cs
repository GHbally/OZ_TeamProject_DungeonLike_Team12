using UnityEngine;

// 근접 공격 몬스터
public class WarriorMonster : MonsterBase
{
    [Header("근접 공격")]

    // 플레이어에게 줄 데미지
    public float damage = 10f;

    // 몇 초마다 데미지를 줄지
    public float damageInterval = 1f;

    // 공격 가능한 거리
    public float attackRange = 0.8f;

    // 데미지 시간을 계산하는 타이머
    private float damageTimer;

    // 몬스터가 생성될 때 실행
    protected override void OnEnable()
    {
        // MonsterBase의 초기화 실행
        base.OnEnable();

        // 바로 공격할 수 있도록 타이머를 채움
        damageTimer = damageInterval;
    }

    // 전사는 항상 플레이어를 추적만 하므로 상태 변경이 필요 없음
    protected override void UpdateState()
    {
        currentState = MonsterState.Chase;
    }

   
    protected override void AttackLogic()
    {
        // 전사는 별도의 공격 패턴이 없으므로 비워둠
    }

    // 매 프레임 실행
    protected override void Update()
    {
        // 부모 실행
        // 플레이어 추적, 사망 체크 등을 수행
        base.Update();

        // 플레이어가 없으면 종료
        if (player == null)
            return;

        // 죽은 상태면 종료
        if (currentState == MonsterState.Dead)
            return;

        // 플레이어와 현재 거리 계산
        float distance =
            Vector2.Distance(transform.position, player.position);

        // 공격 범위 안에 들어왔는지 확인
        if (distance <= attackRange)
        {
            // 타이머 증가
            damageTimer += Time.deltaTime;

            // 공격 시간이 되었는지 확인
            if (damageTimer >= damageInterval)
            {
                // 타이머 초기화
                damageTimer = 0f;

                // PlayerBase 가져오기
                PlayerBase playerBase =
                    player.GetComponent<PlayerBase>();

                // 플레이어가 존재하면
                if (playerBase != null)
                {
                    // 플레이어에게 데미지 전달
                    playerBase.TakeDamage(damage);

                    Debug.Log("전사 몬스터 공격");
                }
            }
        }
        else
        {
            // 공격 범위 밖으로 나가면
            // 다시 바로 공격할 수 있도록 타이머를 채워둠
            damageTimer = damageInterval;
        }
    }
}
