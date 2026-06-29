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

    [Header("Dash Bar Settings")]
    [SerializeField] private Slider dashSlider;     //대쉬 게이지 슬라이더 오브젝트를 담을 애

    [Header("Kill Count Settings")]
    [SerializeField] private TMP_Text killCountText; //킬 카운트 텍스트 수정용

    //HUD 슬롯 제어
    [System.Serializable]
    public class SkillSlotUI
    {
        public GameObject slotObject;   //슬롯 부모 오브젝트 (ex: SkillSlot1)
        public Image iconImage;         //스킬 아이콘 이미지 컴포넌트
        public TMP_Text levelText;      //스킬 레벨 텍스트
    }

    private int currentKillCount = 0;   //킬카운트

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

    //Dash업데이트 해줄 친구
    public void UpdateDash(float currentDash, float maxDash)
    {
        if (dashSlider != null)
        {
            //대쉬 최대치
            dashSlider.maxValue = maxDash;
            //대쉬 쿨
            dashSlider.value = currentDash;
        }
    }

    //KillCount업데이트 해줄 친구
    public void UpdateKillCount()
    {
        currentKillCount++; //몬스터 죽을때마다 +1

        if (killCountText != null)
        {
            //텍스트 갱신
            killCountText.text = currentKillCount.ToString();
        }
    }
}
