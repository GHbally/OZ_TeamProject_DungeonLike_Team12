using UnityEngine;

// 스테이지 클리어 후 안내 화살표의 목표를 관리하는 스크립트.
// 처음에는 보상상자를 가리키고, 보상상자를 먹으면 포탈을 가리킴

public class StageGuideController : MonoBehaviour
{
    [Header("화살표 UI")]
    [SerializeField] private DirectionArrowUI directionArrowUI;

    [Header("포탈 Target")]
    [SerializeField] private Transform portalTarget;
    public void ShowRewardBox(Transform rewardBoxTarget)
    {
        if (directionArrowUI == null)
        {
            Debug.LogWarning("StageGuideController: DirectionArrowUI가 연결되지 않았습니다.", gameObject);
            return;
        }

        if (rewardBoxTarget == null)
        {
            Debug.LogWarning("StageGuideController: RewardBox Target이 없습니다.", gameObject);
            return;
        }

        directionArrowUI.SetTarget(rewardBoxTarget);
    }

    // 포탈을 가리키게 한다.
    public void ShowPortal()
    {
        if (directionArrowUI == null)
        {
            Debug.LogWarning("StageGuideController: DirectionArrowUI가 연결되지 않았습니다.", gameObject);
            return;
        }

        if (portalTarget == null)
        {
            Debug.LogWarning("StageGuideController: Portal Target이 연결되지 않았습니다.", gameObject);
            return;
        }

        directionArrowUI.SetTarget(portalTarget);
    }

    // 안내 화살표를 숨긴다.
    public void Hide()
    {
        if (directionArrowUI == null)
        {
            return;
        }

        directionArrowUI.Hide();
    }
}
