//[주변 몬스터 감지]
using UnityEngine;

public class TargetDetector : MonoBehaviour
{
    [SerializeField] private AttackStats attackstats;   //AttackStats 컴포넌트 가져오기
    [SerializeField] private LayerMask enemyLayer;      //대상 레이어(ex: Enemy)
    private int maximumDetectionCount = 32;             //한번에 감지할 최대 적 숫자
    private Collider2D[] results;                       //감지된 콜라이더 담는 배열

    private ContactFilter2D enemyFilter;                //충돌 연산 시 적 레이어만 걸러낼 필터

    private void Awake()
    {
        CacheReferences();
        InitializeResults();
        SetupContactFilter();
    }
    
    //스탯 컴포넌트 연결
    private void CacheReferences()
    {
        if(attackstats == null)
        {
            attackstats = GetComponent<AttackStats>();
        }
    }

    //감지할 적 담는 바구니
    private void InitializeResults()
    {
        //바구니가 0이하가 되지 않도록
        results = new Collider2D[Mathf.Max(1, maximumDetectionCount)];
    }

    //적 레이어 및 트리거 감지 설정
    private void SetupContactFilter()
    {
        enemyFilter = new ContactFilter2D();    //
        enemyFilter.SetLayerMask(enemyLayer);   //몬스터 레이어인 애들
        enemyFilter.useTriggers = true;         //Is Trigger 켜진 애들도 감지 대상으로
    }

    //가까운 적 탐지
    public Transform FindNearestTarget(float range)
    {
        //원을 그려 몬스터를 담고
        int count = Physics2D.OverlapCircle(transform.position, range, enemyFilter, results);
        Transform nearestTarget = null;
        float nearestDistanceSqr = float.MaxValue;

        //담긴 애들로 최단거리 경쟁 시작
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
        return nearestTarget;   //최종적으로 남은 적 반환
    }

    // 특정 타겟이 공격 범위 안에 있는지 확인한다.
    // 적 중심점이 아니라 Collider의 가장 가까운 지점을 기준으로 검사한다.
    // 그래서 기즈모 원에 적 Collider가 닿으면 공격 가능한 상태로 인정된다.
    public bool IsTargetInRange(Transform target, float range)
    {
        if (target == null)
        {
            return false;
        }

        Collider2D targetCollider = target.GetComponent<Collider2D>();

        if (targetCollider == null)
        {
            return false;
        }

        Vector2 detectorPosition = transform.position;

        // 타겟 Collider에서 플레이어에게 가장 가까운 지점을 가져온다.
        Vector2 closestPoint =targetCollider.ClosestPoint(detectorPosition);

        float distanceSqr = (closestPoint - detectorPosition).sqrMagnitude;

        float rangeSqr = range * range;

        return distanceSqr <= rangeSqr;
    }

    //감지 범위 기즈모
    private void OnDrawGizmos()
    {
        if (attackstats == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackstats.AttackRange);
    }
}
