using UnityEngine;

public class ChaseArrowController : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private SkillManager skillManager;
    [SerializeField] private SkillData chaseArrowSkillData;
    [SerializeField] private SkillProjectilePool chaseArrowPool;
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private Transform firePoint;

    [Header("궁수 스킬 기본 설정")]
    [SerializeField] private float baseCooldown = 2f;
    [SerializeField] private float chaseArrowRange = 7f;
    [SerializeField] private float damageMultiplier = 1f;

    [Header("레벨 설정")]
    [SerializeField] private int fourWayUnlockLevel = 2;

    private float cooldownTimer;
    private void Awake()
    {
        CacheReferentces();
    }

    private void Update()
    {
        // 게임이 일시정지 상태라면 쿨타임과 발사를 처리하지 않는다.
        if (Time.timeScale <= 0f)
        {
            return;
        }

        // 궁수 스킬이 아직 배우지 않은 상태면 작동하지 않는다.
        int chaseArrowLevel = GetChaseArrowLevel();

        if(chaseArrowLevel <= 0)
        {
            return;
        }

        cooldownTimer -= Time.deltaTime;

        if(cooldownTimer > 0f)
        {
            return;
        }

        TryFireChaseArrowSkill(chaseArrowLevel);
    }

    private void CacheReferentces()
    {
        // Inspector에서 연결하지 않았을 때 같은 오브젝트에서 자동으로 찾는다.
        if (skillManager == null)
        {
            skillManager = GetComponent<SkillManager>();
        }

        if (targetDetector == null)
        {
            targetDetector = GetComponent<TargetDetector>();
        }

        if (attackStats == null)
        {
            attackStats = GetComponent<AttackStats>();
        }
    }

    private int GetChaseArrowLevel()
    {
        if(skillManager == null)
        {
            return 0;
        }

        if (chaseArrowSkillData == null)
        {
            return 0;
        }

        return skillManager.GetCurrentLevel(chaseArrowSkillData);
    }

    private void TryFireChaseArrowSkill(int chaseArrowLevel)
    {
        if (chaseArrowPool == null)
        {
            return;
        }

        if (firePoint == null)
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

        Transform target =
            targetDetector.FindNearestTarget(chaseArrowRange);

        if (target == null)
        {
            return;
        }
        Vector2 direction =
            (target.position - firePoint.position).normalized;

        DamageInfo1 damageInfo =
            CreateChaseAorrwDamageInfo();

        if (chaseArrowLevel >= fourWayUnlockLevel)
        {
            FireSingleArrow(
                direction,
                damageInfo,
                chaseArrowLevel
            );

            FireFourWayArrows(
                damageInfo,
                chaseArrowLevel
            );
        }
        else
        {
            FireSingleArrow(
                direction,
                damageInfo,
                chaseArrowLevel
            );
        }
        cooldownTimer = GetFinalCooldown();
    }
    private void FireSingleArrow(
       Vector2 direction,
       DamageInfo1 damageInfo,
       int chaseArrowLevel)
    {
        SpawnArrow(
            direction,
            damageInfo,
            chaseArrowLevel
        );
    }
    private void FireFourWayArrows(
        DamageInfo1 damageInfo,
        int chaseArrowLevel)
    {
        SpawnArrow(Vector2.up, damageInfo, chaseArrowLevel);
        SpawnArrow(Vector2.down, damageInfo, chaseArrowLevel);
        SpawnArrow(Vector2.left, damageInfo, chaseArrowLevel);
        SpawnArrow(Vector2.right, damageInfo, chaseArrowLevel);
    }

    private void SpawnArrow(
        Vector2 direction,
        DamageInfo1 damageInfo,
        int chaseArrowLevel)
    {
        if(direction.sqrMagnitude <= 0f)
        {
            return;
        }

        SkillProjectileBase projectile =
            chaseArrowPool.Spawn(
                firePoint.position,
                direction.normalized,
                damageInfo);

        ChaseArrowProjectile chaseArrow = projectile as ChaseArrowProjectile;

        if(chaseArrow == null)
        {
            return;
        }

        chaseArrow.SetupArcherArrow(chaseArrowLevel);
    }

    private DamageInfo1 CreateChaseAorrwDamageInfo()
    {
        DamageInfo1 baseDamageInfo = attackStats.CreateDamageInfo(gameObject);

        float finalDamage = baseDamageInfo.Damage * damageMultiplier;

        return new DamageInfo1(
            finalDamage,
            baseDamageInfo.IsCritical,
            baseDamageInfo.Attacker
            );
    }

    private float GetFinalCooldown()
    {
        if(attackStats == null)
        {
            return baseCooldown;
        }

        // 파이어볼처럼 AttackStats의 전체 스킬 쿨타임 감소 효과를 적용한다.
        return attackStats.GetFinalSkillCooldown(baseCooldown);
    }

    private void OnDrawGizmos()
    {
        // 추적 화살이 적을 탐색하는 공격 사거리
        // 화살 자체 유도 범위와는 별개
        Gizmos.color = Color.yellow;

        Vector3 gizmoCenter = transform.position;

        if(targetDetector != null)
        {
            gizmoCenter = targetDetector.transform.position;
        }

        Gizmos.DrawWireSphere(gizmoCenter,chaseArrowRange);
    }
}
