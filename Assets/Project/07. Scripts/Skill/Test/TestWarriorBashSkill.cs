using UnityEngine;

public class TestWarriorBashSkill : MonoBehaviour
{
    [Header("검기 풀")]
    [SerializeField] private SkillProjectilePool swordWavePool;

    [Header("발사 위치")]
    [SerializeField] private Transform firePoint;

    [Header("발사 방향")]
    [SerializeField] private Vector2 testDirection = Vector2.right;

    private AttackStats attackStats;

    private void Awake()
    {
        attackStats = GetComponent<AttackStats>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireSwordWave();
        }
    }

    private void FireSwordWave()
    {
        if (swordWavePool == null)
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

        if (testDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        DamageInfo1 damageInfo =
            attackStats.CreateDamageInfo(gameObject);

        swordWavePool.Spawn(
            firePoint.position,
            testDirection.normalized,
            damageInfo
        );
    }
}
