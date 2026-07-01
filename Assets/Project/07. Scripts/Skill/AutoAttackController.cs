using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField] private SkillProjectilePool projectilePool;
    //[SerializeField] private SkillProjectileBase projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private SkillManager skillManager;

    [Header("전사 스킬 설정")]
    // 전사 스킬 레벨마다 증가할 공격력
    [SerializeField] private float warriorDamageIncreasePerLevel = 5f;
    [Header("전사 5레벨 검기")]
    [SerializeField] private SkillProjectilePool swordWavePool;
    // 문자열 ID 대신 WarriorSlash SkillData 에셋을 직접 연결한다.
    [SerializeField] private SkillData warriorSlashSkillData;
    [SerializeField] private int swordWaveUnlockLevel = 5;

    private float attackTimer;
    private bool isDead;
    private bool isAttackEnabled = true;

    [Header("공격 탐색 설정")]
    [SerializeField] private float noTargetRetryDelay = 0.1f;

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
            // 적이 없을 때 매 프레임 공격 시도하지 않도록 짧게 대기한다.
            attackTimer = noTargetRetryDelay;
            return;
        }

        bool isTargetInRange = targetDetector.IsTargetInRange(target, attackStats.AttackRange);

        if (!isTargetInRange)
        {
            attackTimer = noTargetRetryDelay;
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

        DamageInfo1 damageInfo = CreateWarriorDamageInfo();

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
    private DamageInfo1 CreateWarriorDamageInfo()
    {
        // AttackStats에서 기본 데미지 정보를 만든다.
        DamageInfo1 baseDamageInfo = attackStats.CreateDamageInfo(gameObject);

        int warriorSlashLevel = GetWarriorSlashLevel();

        // 전사 스킬 레벨마다 공격력을 5씩 증가시킨다.
        // 예: Lv.1 = +5, Lv.2 = +10, Lv.5 = +25
        float bonusDamage = warriorSlashLevel * warriorDamageIncreasePerLevel;

        float finalDamage = baseDamageInfo.Damage + bonusDamage;

        return new DamageInfo1(
            finalDamage,
            baseDamageInfo.IsCritical,
            baseDamageInfo.Attacker
        );
    }
    private int GetWarriorSlashLevel()
    {
        if (skillManager == null)
        {
            return 0;
        }

        if (warriorSlashSkillData == null)
        {
            return 0;
        }

        return skillManager.GetCurrentLevel(warriorSlashSkillData);
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

        int warriorSlashLevel = GetWarriorSlashLevel();

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
