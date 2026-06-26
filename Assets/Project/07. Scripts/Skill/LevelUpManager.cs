using System.Collections.Generic;
using UnityEngine;
using TMPro; // TMP를 사용하므로 필수!

public class LevelUpManager : MonoBehaviour
{
    [Header("UI 구성요소")]
    [SerializeField] private GameObject levelUpPanel;

    // 카드 버튼 UI들.
    // Button 컴포넌트가 아니라 TestSkillButtonUI 컴포넌트를 넣어야 한다.
    [SerializeField] private List<SkillButtonUI> skillButtons = new();

    [Header("데이터")]
    [SerializeField] private SkillManager skillManager;

    [Header("설정")]
    [SerializeField, Min(1)] private int displayCardCount = 3;

    private readonly List<SkillCardInfo> currentDisplayedCards = new();

    
    /////////////////////////////클리어 보상 추가////////////////////////////
    [Header("스테이지 클리어 보상")]
    [SerializeField] private GameObject nextStagePortal; // 스킬 선택 후 나타날 포털

    private void Start()
    {
        // 시작할 때 패널이 켜져 있다면 꺼줌
        if (levelUpPanel != null && levelUpPanel.activeSelf)
        {
            levelUpPanel.SetActive(false);
        }

    }

    public void OpenLevelUpUI()
    {
        if (levelUpPanel == null)
        {
            Debug.LogError("LevelUpPanel이 연결되지 않았습니다.", gameObject);
            return;
        }

        levelUpPanel.SetActive(true);

        // 카드 선택 중에는 게임을 멈춘다.
        Time.timeScale = 0f;

        RerollSkills();
    }

    public void RerollSkills()
    {
        Debug.Log("리롤 시작!");

        if (skillManager == null)
        {
            Debug.LogError("SkillManager가 연결되지 않았습니다.", gameObject);
            return;
        }

        if (skillButtons == null || skillButtons.Count == 0)
        {
            Debug.LogError("SkillButtonUI 리스트가 비어 있습니다.", gameObject);
            return;
        }

        // 현재 보이던 카드를 저장해서 리롤 시 가능하면 제외한다.
        List<SkillData> previousSkills = new();

        for (int i = 0; i < currentDisplayedCards.Count; i++)
        {
            previousSkills.Add(currentDisplayedCards[i].SkillData);
        }

        currentDisplayedCards.Clear();

        int requestCount = Mathf.Min(
            displayCardCount,
            skillButtons.Count
        );

        List<SkillCardInfo> cards =
            skillManager.GetRandomSkillCardsExcept(
                requestCount,
                previousSkills
            );

        Debug.Log($"새로 받은 카드 수: {cards.Count}");

        currentDisplayedCards.AddRange(cards);

        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillButtonUI buttonUI = skillButtons[i];

            if (buttonUI == null)
            {
                Debug.LogWarning($"{i}번 SkillButtonUI가 비어 있습니다.");
                continue;
            }

            if (i >= currentDisplayedCards.Count)
            {
                buttonUI.gameObject.SetActive(false);
                continue;
            }

            SkillCardInfo cardInfo = currentDisplayedCards[i];

            // 버튼 UI에 카드 정보를 넣고 클릭 콜백을 연결한다.
            buttonUI.Setup(
                cardInfo,
                OnSkillCardSelected
            );

            Debug.Log($"{i}번 카드 표시: {cardInfo.Name}");
        }
    }
    private void OnSkillCardSelected(SkillCardInfo selectedCard)
    {
        if (selectedCard == null)
        {
            Debug.LogWarning("선택된 카드가 null입니다.");
            return;
        }

        Debug.Log($"{selectedCard.Name} 선택됨!");

        // 실제 스킬 적용은 SkillManager가 담당한다.
        skillManager.ApplySkillChoice(selectedCard.SkillData);

        CloseLevelUpUI();

        if (nextStagePortal != null) // 포털이 연결되어 있다면
        {
            nextStagePortal.SetActive(true); // 포털 보이기
        }
    }

    public void CloseLevelUpUI()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }
}
