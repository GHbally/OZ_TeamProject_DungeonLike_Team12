using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField] private SkillProjectilePool projectilePool;
    //[SerializeField] private SkillProjectileBase projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("전사 5레벨 검기")]
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private SkillProjectilePool swordWavePool;
    // 문자열 ID 대신 WarriorSlash SkillData 에셋을 직접 연결한다.
    [SerializeField] private SkillData warriorSlashSkillData;
    [SerializeField] private int swordWaveUnlockLevel = 5;

    private float attackTimer;
    private bool isDead;
    private bool isAttackEnabled = true;

    private void Awake()
    {
        CacheReferences();

        if (skillManager == null)
        {
            skillManager = FindFirstObjectByType<SkillManager>();
        }

        if (attackStats != null)
        {
            attackTimer = attackStats.GetAttackInterval();
        }
    }
    private void Update()
    {
        if (!isAttackEnabled || isDead)
        {
            return;
        }
        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
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
        if (target == null)
        {
            return;
        }
        float distanceSqr = (target.position - transform.position).sqrMagnitude;

        // 공격 범위의 제곱
        float attackRangeSqr = attackStats.AttackRange * attackStats.AttackRange;
        if (distanceSqr > attackRangeSqr)
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

        if (direction.sqrMagnitude <= 0f)
        {
            return;
        }

        // 전사 검기를 위한 위치
        Vector2 normalizedDirection = direction.normalized;

        DamageInfo1 damageInfo = attackStats.CreateDamageInfo(gameObject);

        projectilePool.Spawn(
            firePoint.position,
            direction.normalized,
            damageInfo
        );

        TryFireSwordWave(
        normalizedDirection,
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
    private void TryFireSwordWave(
    Vector2 direction,
    DamageInfo1 damageInfo)
    {
        if (skillManager == null)
        {
            return;
        }

        if (swordWavePool == null)
        {
            return;
        }

        int warriorSlashLevel = skillManager.GetCurrentLevel(warriorSlashSkillData);


        if (warriorSlashLevel < swordWaveUnlockLevel)
        {
            return;
        }

        Debug.Log("전사 5레벨 검기 발사");

        swordWavePool.Spawn(
            firePoint.position,
            direction,
            damageInfo
        );
    }
}
