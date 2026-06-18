using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    [SerializeField]private AttackStats attackstats;
    [SerializeField] private LayerMask enemyLayer;
    private int maximumDetectionCount = 32;
    private Collider2D[] results;

    private ContactFilter2D enemyFilter;

    private void Awake()
    {
        CacheReferences();
        InitializeResults();
        SetupContactFilter();
    }
    
    private void CacheReferences()
    {
        if(attackstats == null)
        {
            attackstats = GetComponent<AttackStats>();
        }
    }
    private void InitializeResults()
    {
        results = new Collider2D[Mathf.Max(1, maximumDetectionCount)];
    }

    private void SetupContactFilter()
    {
        enemyFilter = new ContactFilter2D();
        enemyFilter.SetLayerMask(enemyLayer);
        enemyFilter.useTriggers = true;
    }
    public Transform FindNearertTarget(float range)
    {
        int count = Physics2D.OverlapCircle(transform.position, range, enemyFilter, results);
        Transform nearestTarget = null;
        float nearestDistanceSqr = float.MaxValue;

        for(int i = 0; i < count; i++)
        {
            Collider2D targetCollider = results[i];
            if(targetCollider == null)
            {
                continue;
            }

            if (!targetCollider.TryGetComponent<IDamageable1>(out IDamageable1 damageable))
            {
                continue;
            }
            Vector2 direction = targetCollider.transform.position - transform.position;

            float distanceSqr = direction.sqrMagnitude;

            if(distanceSqr < nearestDistanceSqr)
            {
                nearestDistanceSqr= distanceSqr;
                nearestTarget = targetCollider.transform;
            }
        }
        return nearestTarget;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackstats.AttackRange);
    }
}
