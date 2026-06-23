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
            Debug.LogError("SwordWavePool이 연결되지 않았습니다.", gameObject);
            return;
        }

        if (firePoint == null)
        {
            Debug.LogError("FirePoint가 연결되지 않았습니다.", gameObject);
            return;
        }

        if (attackStats == null)
        {
            Debug.LogError("AttackStats가 없습니다.", gameObject);
            return;
        }

        if (testDirection.sqrMagnitude <= 0f)
        {
            Debug.LogError("발사 방향이 잘못되었습니다.", gameObject);
            return;
        }

        DamageInfo1 damageInfo =
            attackStats.CreateDamageInfo(gameObject);

        swordWavePool.Spawn(
            firePoint.position,
            testDirection.normalized,
            damageInfo
        );

        Debug.Log("검기 테스트 발사");
    }
}
