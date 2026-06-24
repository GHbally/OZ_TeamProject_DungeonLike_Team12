using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    public Slider hpSlider;
    public float maxHp = 100f;
    private float currentHp;

    void Start()
    {
        currentHp = maxHp;
        UpdateHpBar();
    }

    // 데미지를 입었을 때 호출하는 함수
    public void TakeDamage(float amount)
    {
        currentHp -= amount;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp); // 0~maxHp 사이 유지
        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        hpSlider.value = currentHp / maxHp; // 0~1 사이 값으로 정규화
    }
}
