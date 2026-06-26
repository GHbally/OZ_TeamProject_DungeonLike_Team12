using UnityEngine;

// 궁수 스킬
// 3레벨에 동서남북으로 화살발사, 4레벨에 관통 2회, 5레벨부터 가까운 적에게 유도되는 기능을 가진다.

public class ChaserArrow : SkillProjectileBase
{
    [Header("궁수 화살 레벨 설정")]
    [SerializeField] private int piercingUnlockLevel = 4;
    [SerializeField] private int chaseUnlockLevel = 5;

    [Header("관통 설정")]
    [SerializeField] private int normalMaxHitCount = 1;
    [SerializeField] private int piercingMaxHitCount = 3;

    [Header("유도 설정")]
    [SerializeField] private float chaseRange = 10f;
    [SerializeField] private float chaseTurnSpeed = 8f;
    [SerializeField] private LayerMask chaseEnemyLayer;

    // 유도 대상 탐색 결과를 담는 배열
    // 매 프레임 새 배열을 만들지 않기 위해 미리 만들어 둠
    private readonly Collider2D[] chaseResults = new Collider2D[16];

    private int archerLevel;

    protected override void OnInitialized()
    {
        // pool 에서 처음 꺼내졌을 때 기본 설정을 적용
        // 실제 레벨은 AutoAttackController에서 SetupAcherArrow()로 전달
        ApplyLevelSetting();
    }

    // AutoAttackController가 화살을 발사한 직후 호출
    // 현재 궁수 스킬 레벨을 전달받아서 관통, 유도 여부를 결정
    public void SetupArcherArrow(int newArcherLevel)
    {
        archerLevel = newArcherLevel;

        // 레벨에 따라 관통 횟수를 다시 적용
        ApplyLevelSetting();
    }

    private void ApplyLevelSetting()
    {
        // 4레벨 이상이면 관통 2회
        // 3명의 적까지 피해를 줄 수 있음.
        if(archerLevel >= piercingUnlockLevel)
        {
            SetMaxHitCount(piercingMaxHitCount);
            return;
        }

        // 4레벨 미만이면 일반화살
        SetMaxHitCount(normalMaxHitCount);
    }

    protected override void UpdateMovement()
    {
        // 5레벨 이상이면 이동하기 전에 가까운 적으로 방향을 틀어줌
        if(archerLevel >= chaseUnlockLevel)
        {
            UpdateChaseDirection();
        }

        // 실제 이동은 부모 클래스의 기본 투사체 이동을 사용
        base.UpdateMovement();
    }

    private void UpdateChaseDirection()
    {
        Transform nearestTarget = FindNearestEnemy();

        if(nearestTarget == null)
        {
            return;
        }

        Vector2 chaseDirection = (nearestTarget.position - transform.position).normalized;

        if(chaseDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        // 현재 방향에서 목표 방향으로 부드럽게 회전한다.
        // chaseTurnSpeed가 높을 수록 빠르게 적을 따라감

        Vector2 newDirection = Vector2.Lerp(
            moveDirection, 
            chaseDirection, 
            chaseTurnSpeed * Time.deltaTime
            ).normalized;

        SetMoveDirection(newDirection);
    }
    private Transform FindNearestEnemy()
    {
        int count = Physics2D.OverlapCircleNonAlloc(
            transform.position,
            chaseRange,
            chaseResults,
            chaseEnemyLayer
        );

        Transform nearestTarget = null;
        float nearestDistanceSqr = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            Collider2D targetCollider = chaseResults[i];

            if(targetCollider == null)
            {
                continue;
            }

            IDamageable1 damageble = targetCollider.GetComponentInParent<IDamageable1>();

            if(damageble == null)
            {
                continue;
            }

            Vector2 direction = targetCollider.transform.position - transform.position;

            float distanceSqr = direction.sqrMagnitude;

            if (distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr = distanceSqr;
                nearestTarget = targetCollider.transform;
            }
        }
        return nearestTarget;
    }

    protected override void OnBeforeRelease()
    {
        // Pool로 돌오가기 전에 이전 레벨 정보를 초기화 한다.
        // Pooling 오브젝트는 재사용되므로 상태가 남으면 안된다.
        archerLevel = 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
