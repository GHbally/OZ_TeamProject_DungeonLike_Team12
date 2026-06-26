//HUD 제어 스크립트
using UnityEngine;
using UnityEngine.UI;
using TMPro;            //텍스트 바꾸기용

public class HUDController : MonoBehaviour
{
    public static HUDController Instance;           //HUD 부르기 버튼

    [Header("Level Settings")]
    [SerializeField] private TMP_Text levelText;    //레벨 텍스트

    [Header("HP Bar Settings")]
    [SerializeField] private Slider hpSlider;       //체력바 슬라이더 오브젝트 담을 애

    [Header("EXP Bar Settings")]
    [SerializeField] private Slider expSlider;      //경험치바 슬라이더 오브젝트 담을 애

    private void Awake()
    {
        Instance = this;
    }

    //레벨 업데이트용
    public void UpdateLevel(int currentLevel)
    {
        //레벨 글자 갈아끼울거
        levelText.text = $"{currentLevel}";
    }

    //HP업데이트 해줄 친구
    public void UpdateHP(float currentHp, float maxHp)
    {
        //최대피만큼 설정
        hpSlider.maxValue = maxHp;

        //현재 피만큼 채워줄거
        hpSlider.value = currentHp;
    }

    //EXP업데이트 해줄 친구
    public void UpdateEXP(float currentExp, float maxExp)
    {
        //경험치 요구량만큼 설정
        expSlider.maxValue = maxExp;

        //현재 쌓인 경험치 수치에 맞춰 채울거
        expSlider.value = currentExp;
    }
}
