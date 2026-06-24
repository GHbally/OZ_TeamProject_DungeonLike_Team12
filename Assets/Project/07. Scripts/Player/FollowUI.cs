using UnityEngine;

public class FollowUI : MonoBehaviour
{
    [Header("따라다닐 대상")]
    [SerializeField] private Transform target; // 플레이어의 Transform을 드래그하세요

    [Header("위치 보정")]
    [SerializeField] private Vector2 offset = new Vector2(0, -0.5f); // 발 아래 오프셋 값

    private Canvas parentCanvas;

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null || target == null)
        {
            Debug.LogError("FollowUI: Canvas나 Target이 할당되지 않았습니다.");
            enabled = false;
        }
    }

    void LateUpdate() // Update 대신 LateUpdate를 사용해 끊김 방지
    {
        if (target != null)
        {
            // 타겟의 월드 좌표를 부모 Canvas의 스크린 좌표로 변환
            Vector2 positionOnScreen = Camera.main.WorldToScreenPoint(target.position);

            // 오프셋 적용
            positionOnScreen += offset;

            // 최종적으로 UI의 position에 대입
            transform.position = positionOnScreen;
        }
    }
}
