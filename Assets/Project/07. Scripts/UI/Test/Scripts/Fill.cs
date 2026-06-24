using UnityEngine;

public class ForceAnchorFix : MonoBehaviour
{
    public RectTransform fillRect; // Fill 오브젝트를 여기에 드래그

    void Start()
    {
        // 강제로 앵커를 고정 (왼쪽 정렬 예시)
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.pivot = new Vector2(0, 0.5f);
        fillRect.anchoredPosition = Vector2.zero;
    }
}
