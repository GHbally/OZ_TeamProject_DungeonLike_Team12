using UnityEngine;

public class AutoAttackController : MonoBehaviour
{
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private TargetDetector targetDetector;
    [SerializeField] private Projectlie projectilePrefab;
    [SerializeField] private Transform firePoint;

    private float attackTimer;
    private bool isDead;


    private void Awake()
    {
        if (attackStats == null)
        {
            attackStats = GetComponent<AttackStats>();
        }
        if(targetDetector == null)
        {
            targetDetector = GetComponent<TargetDetector>();
        }
    }
    private void Update()
    {
        if (isDead)
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
    private void TryAttack()
    {
        Transform target = targetDetector.FindNearertTarget(attackStats.AttackRange);
        if(target == null)
        {
            return;
        }
        FireProjectile(target);

        attackTimer = attackStats.GetAttackInterval();
    }
    private void FireProjectile(Transform target)
    {
        Vector2 direction = (target.position - firePoint.position).normalized;

        Projectlie projectlie = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        DamageInfo1 damageInfo = attackStats.CreateDamageInfo(gameObject);

        projectlie.Initialize(direction, damageInfo);
    }
    public void SetDead(bool dead)
    {
        isDead = dead;
    }   

}
