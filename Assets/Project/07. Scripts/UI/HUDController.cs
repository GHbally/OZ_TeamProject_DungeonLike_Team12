//HUD 제어 스크립트
using System.Collections.Generic;
using TMPro;            //텍스트 바꾸기용
using UnityEngine;
using UnityEngine.UI;

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
        [HideInInspector] public SkillData assignedSkill;   //이 슬롯이 쥐고 있는 스킬 원본 데이터 기억용
    }

    [Header("Skill HUD Settings")]
    [SerializeField] private List<SkillSlotUI> skillSlots = new List<SkillSlotUI>();

    //스킬 레벨별 색
    [Header("Level Color Settings")]
    [SerializeField] private Color normalColor = Color.white;               //1~2레벨 기본 색상
    [SerializeField] private Color cyanColor = new Color(0f, 1f, 1f);       //3~4레벨 시안색
    [SerializeField] private Color orangeColor = new Color(1f, 0.5f, 0f);   //5레벨 주황색

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

    public void UpdateSkillHUD(SkillData skillData, int currentLevel)
    {
        if (skillData == null) return;

        //이미 HUD 슬롯에 등록된 스킬인지 체크 (레벨업)
        for (int i = 0; i < skillSlots.Count; i++)
        {
            if (skillSlots[i].assignedSkill == skillData)
            {
                RefreshSlotVisual(skillSlots[i], currentLevel);
                return;
            }
        }

        //새로운 액티브 스킬 등록 -> 빈 슬롯 찾기
        for (int i = 0; i < skillSlots.Count; i++)
        {
            if (skillSlots[i].assignedSkill == null)
            {
                skillSlots[i].assignedSkill = skillData;

                //하위 자식 아이콘 오브젝트 및 컴포넌트 강제 활성화
                if (skillSlots[i].iconImage != null)
                {
                    skillSlots[i].iconImage.gameObject.SetActive(true);
                    skillSlots[i].iconImage.enabled = true;
                    skillSlots[i].iconImage.sprite = skillData.Icon;
                    skillSlots[i].iconImage.color = Color.white; // 원래 도트 색 유지
                }

                // 하위 자식 레벨 텍스트 오브젝트 강제 활성화
                if (skillSlots[i].levelText != null)
                {
                    skillSlots[i].levelText.gameObject.SetActive(true);
                    skillSlots[i].levelText.enabled = true;
                }

                RefreshSlotVisual(skillSlots[i], currentLevel);
                Canvas.ForceUpdateCanvases(); // UI 즉시 강제 리프레시

                Debug.Log($"[HUD 등록 성공] {skillSlots[i].slotObject.name} -> {skillData.SkillName} 장착 완료!");
                return; // 딱 한 칸만 채우고 종료
            }
        }
    }

    private void RefreshSlotVisual(SkillSlotUI slot, int level)
    {
        if (slot.levelText == null) return;

        //레벨 텍스트 변경 (1~5 표기)
        slot.levelText.text = level.ToString();

        //레벨별 조건에 맞춰 텍스트와 아이콘 색상 변경
        Color targetColor = normalColor;

        if (level >= 5)
        {
            targetColor = orangeColor;  //5레벨 마스터: 주황색
        }
        else if (level >= 3)
        {
            targetColor = cyanColor;    //3레벨 이상: 시안색
        }

        slot.levelText.color = targetColor;

        //아이콘에도 약간의 오라 광채를 주고 싶다면 색상 살짝 믹싱
        if (slot.iconImage != null)
        {
            slot.iconImage.color = targetColor;
        }
    }
}
