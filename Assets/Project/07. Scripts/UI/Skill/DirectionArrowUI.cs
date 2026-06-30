using UnityEngine;

// 플레이어 머리 위에 고정된 화살표 UI.
// 보상상자나 포탈 위치를 안내할 때 사용

public class DirectionArrowUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private RectTransform arrowRect;
    [SerializeField] private Canvas canvas;

    [Header("카메라")]
    [SerializeField] private Camera targetCamera;

    [Header("기준 대상")]
    [SerializeField] private Transform player;

    [Header("플레이어 머리 위 위치")]
    [SerializeField] private Vector2 playerScreenOffset = new Vector2(0f, 150f);

    [Header("화살표 회전 보정")]
    [SerializeField] private float rotationOffset = -90f;

    private Transform target;

    private void Awake()
    {
        // 카메라가 연결되지 않았으면 Main Camera를 자동으로 찾는다.
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        // 시작할 때는 화살표 이미지만 숨긴다.
        if (arrowRect != null)
        {
            arrowRect.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        // 플레이어나 목표가 없으면 화살표를 숨긴다.
        if (player == null || target == null)
        {
            if (arrowRect != null)
            {
                arrowRect.gameObject.SetActive(false);
            }

            return;
        }

        UpdateArrow();
    }

    // 화살표가 가리킬 목표를 설정한다.
    // 보상상자 생성 시에는 보상상자 Transform,
    // 보상상자 획득 후에는 포탈 Transform이 들어온다.
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (arrowRect != null)
        {
            arrowRect.gameObject.SetActive(true);
        }

        Debug.Log($"DirectionArrowUI Target 설정: {newTarget.name}", newTarget.gameObject);
    }

    // 화살표를 숨긴다.
    public void Hide()
    {
        target = null;

        if (arrowRect != null)
        {
            arrowRect.gameObject.SetActive(false);
        }
    }

    private void UpdateArrow()
    {
        if (arrowRect == null || canvas == null || targetCamera == null)
        {
            return;
        }

        // 플레이어 위치를 화면 좌표로 변환한다.
        Vector3 playerScreenPosition =
            targetCamera.WorldToScreenPoint(player.position);

        // 목표 위치를 화면 좌표로 변환한다.
        Vector3 targetScreenPosition =
            targetCamera.WorldToScreenPoint(target.position);

        // 카메라 뒤쪽에 있으면 정상적으로 표시하기 어렵기 때문에 숨긴다.
        if (playerScreenPosition.z < 0f || targetScreenPosition.z < 0f)
        {
            arrowRect.gameObject.SetActive(false);
            return;
        }

        arrowRect.gameObject.SetActive(true);

        // 플레이어 머리 위 위치를 화면 좌표 기준으로 계산한다.
        Vector2 arrowScreenPosition =
            new Vector2(playerScreenPosition.x, playerScreenPosition.y)
            + playerScreenOffset;

        // 화면 좌표를 Canvas 로컬 좌표로 변환한다.
        RectTransform canvasRect =
            canvas.transform as RectTransform;

        Camera canvasCamera = null;

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = canvas.worldCamera;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            arrowScreenPosition,
            canvasCamera,
            out Vector2 localPosition
        );

        // 화살표 UI 위치를 플레이어 머리 위로 고정한다.
        arrowRect.anchoredPosition = localPosition;

        // 플레이어에서 목표를 향하는 화면상 방향을 구한다.
        Vector2 direction =
            (Vector2)targetScreenPosition - (Vector2)playerScreenPosition;

        if (direction.sqrMagnitude <= 0.01f)
        {
            return;
        }

        direction.Normalize();

        // 방향 벡터를 각도로 변환한다.
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 화살표 이미지의 기본 방향에 맞게 보정한다.
        arrowRect.rotation =
            Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

}
