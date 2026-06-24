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

        //TryCastFireball(fireballLevel);

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
}
