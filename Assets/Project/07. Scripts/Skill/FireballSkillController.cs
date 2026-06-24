using UnityEngine;

// 파이어볼 스킬 발사를 담당하는 컨트롤러.
// Player 오브젝트에 붙여서 사용한다.
// 실제 파이어볼 오브젝트는 FireballProjectile이 담당하고,
// 이 스크립트는 "언제 발사할지"만 관리한다.

public class FireballSkillController : MonoBehaviour
{
    [Header("필수 참조")]
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private Transform firePoint;

    [Header("파이어볼 스킬")]
    [SerializeField] private SkillData fireballSkillData;
    [SerializeField] private SkillProjectilePool fireballPool;

    // 파이어볼 탐색 범위
    [SerializeField] private float fireballRange = 6f;
    [SerializeField] private float baseCooldown = 3f;

    // 파이어볼 데미지 배율
    [SerializeField] private float fireballDamageMultiplier = 1.2f;

    // 마스터 레벨
    private int masterLevel = 5;

    [Header("마스터 불길 Pool")]
    [SerializeField] private SkillProjectilePool fireTrailPool;

    [Header("크기 배율")]
    [SerializeField] private float baseSizeMultiplier = 1f;
    [SerializeField] private float sizeIncreasePerLevel = 0.2f;
    [SerializeField] private float maxSizeMultiplier = 2f;

    private float cooldownTimer;

    private void Awake()
    {
        CacheReferences();

        // 시작하자마자 바로 발사되지 않게 약간의 대기 시간을 준다.
        cooldownTimer = 0.2f;
    }

    private void Update()
    {
        int fireballLevel = GetFireballLevel();

        // 파이어볼을 아직 배우지 않았다면 발사하지 않음
        if(fireballLevel <= 0)
        {
            return;
        }

        cooldownTimer -= Time.deltaTime;

        // 쿨타임이 남아 있으면 아직 발사하지 않는다.
        if (cooldownTimer > 0f)
        {
            return;
        }

        TryCastFireball(fireballLevel);

        // 파이어볼 레벨과 상관없이 기본 쿨타임을 사용한다.
        // 단, 나중에 전체 스킬 쿨타임 감소 패시브가 있으면 그 배율만 적용된다.
        cooldownTimer = GetFinalCooldown();
    }

    private void CacheReferences()
    {
        // Inspector 연결을 깜빡했을 때 자동으로 찾아주는 안전장치다.
        if (skillManager == null)
        {
            skillManager = FindFirstObjectByType<SkillManager>();
        }

        if (attackStats == null)
        {
            attackStats = GetComponent<AttackStats>();
        }

        if (targetDetector == null)
        {
            targetDetector = GetComponent<TargetDetector>();
        }

        if (attackStats == null)
        {
            attackStats = GetComponent<AttackStats>();
        }

        if (firePoint == null)
        {
            firePoint = transform;
        }
    }

    private int GetFireballLevel()
    {
        if (skillManager == null)
        {
            return 0;
        }

        if (fireballSkillData == null)
        {
            return 0;
        }

        // SkillData 에셋 자체로 현재 레벨을 확인한다.
        return skillManager.GetCurrentLevel(fireballSkillData);
    }

    private float GetFinalCooldown()
    {
        // 전체 스킬 쿨타임 감소 스탯이 없다면 기본 쿨타임 그대로 사용한다.
        if (attackStats == null)
        {
            return baseCooldown;
        }

        // 전체 스킬 쿨타임 감소 배율을 적용한다.
        return attackStats.GetFinalSkillCooldown(baseCooldown);
    }
    
    private void TryCastFireball(int fireballLevel)
    {
        if(fireballPool == null)
        {
            return;
        }

        if(targetDetector == null)
        {
            return;
        }
        if(attackStats == null)
        {
            return;
        }

        // 파이어볼 전용 사거리 안에서 가장 가까운 적을 찾는다.
        Transform target = targetDetector.FindNearestTarget(fireballRange);

        if(target == null)
        {
            return;
        }

        Vector2 direction = target.position - firePoint.position;

        if(direction.sqrMagnitude <= 0f)
        {
            return;
        }

        DamageInfo1 damageInfo = CreateFireballDamageInfo();

        // Pool에서 파이어볼 투사체를 꺼내 발사한다.
        SkillProjectileBase projectile = fireballPool.Spawn(firePoint.position, direction.normalized, damageInfo);

        FireballProjectile fireball = projectile as FireballProjectile;

        bool isMaster =
            fireballLevel >= masterLevel;

        // 파이어볼 레벨에 따라 크기와 마스터 여부만 전달한다.
        fireball.SetupFireball(fireballLevel, GetSizeByLevel(fireballLevel), isMaster, fireTrailPool );

        Debug.Log($"파이어볼 발사 / Lv.{fireballLevel}");
    }

    private DamageInfo1 CreateFireballDamageInfo()
    {
        // AttackStats의 치명타 계산을 그대로 사용한다.
        DamageInfo1 baseDamageInfo =
            attackStats.CreateDamageInfo(gameObject);

        // 파이어볼 전용 데미지 배율을 적용한다.
        float finalDamage =
            baseDamageInfo.Damage * fireballDamageMultiplier;

        return new DamageInfo1(
            finalDamage,
            baseDamageInfo.IsCritical,
            baseDamageInfo.Attacker
        );
    }
    private float GetSizeByLevel(int level)
    {
        // 레벨이 1보다 낮게 들어오면 계산이 꼬일 수 있으므로 최소 1로 보정한다.
        int safeLevel = Mathf.Max(1, level);

        // 1레벨은 기본 크기만 사용한다.
        // 2레벨부터 레벨당 증가량이 적용된다.
        float calculatedSize =
            baseSizeMultiplier +
            ((safeLevel - 1) * sizeIncreasePerLevel);

        // 파이어볼이 너무 커지는 것을 막기 위해 최대 크기로 제한한다.
        return Mathf.Min(
            maxSizeMultiplier,
            calculatedSize
        );
    }
    private void OnDrawGizmos()
    {
        // 파이어볼이 적을 감지하는 범위를 Scene 뷰에 표시한다.
        // 실제 파이어볼 타겟 탐색도 fireballRange를 사용한다.
        Gizmos.color = Color.green;

        Vector3 gizmoCenter = transform.position;

        // TargetDetector가 연결되어 있으면 TargetDetector 위치 기준으로 그린다.
        // 실제 탐색도 targetDetector.FindNearestTarget(fireballRange)를 사용하기 때문이다.
        if (targetDetector != null)
        {
            gizmoCenter = targetDetector.transform.position;
        }

        Gizmos.DrawWireSphere(gizmoCenter, fireballRange );
    }
}
