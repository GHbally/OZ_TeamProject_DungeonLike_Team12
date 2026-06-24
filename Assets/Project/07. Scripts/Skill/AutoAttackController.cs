using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField] private SkillProjectilePool projectilePool;
    //[SerializeField] private SkillProjectileBase projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SkillManager skillManager;

    [Header("전사 5레벨 검기")]
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
        if (!isAttackEnabled)
        {
            return;
        }

        if (attackStats == null)
        {
            return;
        }

        if (targetDetector == null)
        {
            return;
        }
        Transform target = targetDetector.FindNearestTarget(attackStats.AttackRange);

        if (target == null)
        {
            return;
        }

        bool isTargetInRange = targetDetector.IsTargetInRange(target, attackStats.AttackRange);

        if (!isTargetInRange)
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

        // 기본 전사 공격 : 근접 베기
        projectilePool.Spawn(
            firePoint.position,
            direction.normalized,
            damageInfo
        );

        // 전사 검기 공격 : 기본 전사 공격스킬이 5레벨 이상이면 검기 추가 발사
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
        if (warriorSlashSkillData == null)
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
