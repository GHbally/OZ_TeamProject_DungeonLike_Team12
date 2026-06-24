using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private RectTransform fill; // Fill 오브젝트의 RectTransform
    [SerializeField] private Slider hpSlider;    // Slider 컴포넌트 (Fill Rect는 None으로)

    void Update()
    {
        // Slider 값(0~1)에 따라 Fill의 앵커 Max X만 조정
        float ratio = hpSlider.value / hpSlider.maxValue;
        fill.anchorMax = new Vector2(ratio, fill.anchorMax.y);
    }
}
