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
    [SerializeField] private float baseColldown = 2f;
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

        //TryFireChaseArrowSkill(chaseArrowLevel);
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

        if (chaseArrowSkillData)
        {
            return 0;
        }

        return skillManager.GetCurrentLevel(chaseArrowSkillData);
    }

    private void TryFireChaseArrowSkill()
    {

    }
}
