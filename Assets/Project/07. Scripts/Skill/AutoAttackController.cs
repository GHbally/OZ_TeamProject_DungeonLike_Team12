using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField]private TestSkillProjectilePool projectilePool;
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
    private void FireProjectile(Transform target)
    {
        if (target == null)
        {
            return;
        }
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
    public void StopAttack()
    {
        SetAttackEnabled(false);
    }
    public void ResumeAttack()
    {
        SetAttackEnabled(true);
    }

    public void SetDead(bool dead)
    {
        isDead = dead;
    }   
}
