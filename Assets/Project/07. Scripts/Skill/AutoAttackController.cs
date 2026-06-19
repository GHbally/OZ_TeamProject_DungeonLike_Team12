using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField] private SkillProjectilePool projectilePool;
    //[SerializeField] private SkillProjectileBase projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float attackTimer;
    private bool isDead;
    private bool isAttackEnabled = true;

    private void Awake()
    {
        CacheReferences();
    }
    private void Update()
    {
        if (!isAttackEnabled || isDead)
        {
            return;
        }
        attackTimer -= Time.deltaTime;

        if(attackTimer > 0f)
        {
            return;
        }
        TryAttack();
    }

    // 컴포넌트 자동 참조
    private void CacheReferences()
    {
        if (attackStats == null)
        {
            attackStats = GetComponent<AttackStats>();
        }

        if (targetDetector == null)
        {
            targetDetector = GetComponent<TargetDetector>();
        }
    }

    //공격
    private void TryAttack()
    {
        Transform target = targetDetector.FindNearestTarget(attackStats.AttackRange);
        if(target == null)
        {
            return;
        }
        FireProjectile(target);

        attackTimer = attackStats.GetAttackInterval();
    }

    //발사 메서드(파이어포인트에서)
    private void FireProjectile(Transform target)
    {
        if (target == null)
        {
            return;
        }

        //(적 위치 좌표 - 내 총구 위치 좌표) 계산해서 발사 방향 계산
        Vector2 direction = (target.position - firePoint.position).normalized;

        Debug.Log(
        $"[AutoAttack] 사용하는 풀: " +
        $"{(projectilePool == null ? "NULL" : projectilePool.gameObject.name)}",
        gameObject
        );

        DamageInfo1 damageInfo = attackStats.CreateDamageInfo(gameObject);

        projectilePool.Spawn(
            firePoint.position,
            direction.normalized,
            damageInfo
        );
    }
    public void SetAttackEnabled(bool enabled)
    {
        isAttackEnabled = enabled;

        // 다시 활성화되면 바로 공격할 수 있도록 타이머 초기화
        if (enabled)
        {
            attackTimer = 0f;
        }
    }

    //공격 멈추기
    public void StopAttack()
    {
        SetAttackEnabled(false);
    }

    //다시 공격
    public void ResumeAttack()
    {
        SetAttackEnabled(true);
    }

    public void SetDead(bool dead)
    {
        isDead = dead;
    }   
}
