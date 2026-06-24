//[경험치 자석]
using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    [Header("자석 능력치")]
    [SerializeField] private float magnetSpeed = 8.0f;    //경험치 빨려 들어오는 속도
    [SerializeField] private float baseRadius = 3.0f;     //자석 범위

    private CircleCollider2D magnetCollider;

    private void Awake()
    {
        magnetCollider = GetComponent<CircleCollider2D>();

        //게임 시작 시 기본 세팅한 반지름 크기로 설정
        if (magnetCollider != null )
        {
            magnetCollider.radius = baseRadius;
        }
    }

    //향후 자석 범위 업그레이드용
    public void UpgradeRadius(float scanRangeMultiplier)
    {
        if ( magnetCollider != null )
        {
            magnetCollider.radius = baseRadius * scanRangeMultiplier;
            Debug.Log("자석 업그레이드 완료");
        }
    }

    //영역 안에 무언가 들어와서 머무는 동안 매 프레임 체크
    private void OnTriggerStay2D(Collider2D other)
    {
        //태그가 Exp면
        if (other.CompareTag("Exp"))
        {
            ExpOrb expOrb = other.GetComponent<ExpOrb>();

            if (expOrb != null)
            {
                //최상위 부모(Player)로 이동
                expOrb.MoveToPlayer(transform.parent, magnetSpeed);
            }
        }
    }

    //자석 범위 기즈모
    private void OnDrawGizmos()
    {
        //실행중이 아닐땐 하늘색으로 범위 시각화
        Gizmos.color = new Color(0f, 0.7f, 1.0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, baseRadius);

        //게임이 실행 중이고 콜라이더가 제대로 작동 중이라면 연두색으로
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        if (col != null && Application.isPlaying)
        {
            Gizmos.color = new Color(0f, 1.0f, 0.5f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }
    }
}
