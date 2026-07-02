using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // TMP를 사용하므로 필수!

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

    [Header("리롤 버튼")]
    [SerializeField] private Button rerollButton;          // 리롤 버튼
    [SerializeField] private TMP_Text rerollButtonText;    // 리롤 버튼 안의 텍스트
    [SerializeField] private CanvasGroup rerollButtonCanvasGroup;
    [SerializeField] private int maxRerollCount = 1;       // 스킬 선택창마다 가능한 리롤 횟수

    private int currentRerollCount;                        // 현재 남은 리롤 횟수


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

        // 시작할 때 리롤 버튼 UI도 기본 상태로 맞춰둔다.
        currentRerollCount = maxRerollCount;
        UpdateRerollButtonUI();

    }

    public void OpenLevelUpUI()
    {
        if (levelUpPanel == null)
        {
            Debug.LogError("LevelUpPanel이 연결되지 않았습니다.", gameObject);
            return;
        }

        levelUpPanel.SetActive(true);

        GameManager.Instance.ChangeState(GameManager.GameState.Menu);

        // 카드 선택 중에는 게임을 멈춘다.
        Time.timeScale = 0f;

        // 스킬 선택창이 새로 열릴 때마다 리롤 횟수를 초기화한다.
        currentRerollCount = maxRerollCount;

        // 버튼 텍스트를 갱신한다.
        UpdateRerollButtonUI();

        // 처음 스킬 선택창이 열릴 때 카드들을 뽑는다.
        GenerateSkillCards();
    }

    public void RerollSkills()
    {
        if (currentRerollCount <= 0)
        {
            UpdateRerollButtonUI();
            return;
        }

        // 리롤 횟수를 1 줄인다.
        currentRerollCount--;

        // 스킬 카드들을 다시 뽑는다.
        GenerateSkillCards();

        // 버튼 텍스트를 "다시 뽑기 (0)"으로 바꾸고 버튼을 비활성화한다.
        UpdateRerollButtonUI();

        // 기존 코드를 UpdateRerollButtonUI() 함수로 이동
        //if (skillManager == null)
        //{
        //    Debug.LogError("SkillManager가 연결되지 않았습니다.", gameObject);
        //    return;
        //}

        //if (skillButtons == null || skillButtons.Count == 0)
        //{
        //    Debug.LogError("SkillButtonUI 리스트가 비어 있습니다.", gameObject);
        //    return;
        //}

        //// 현재 보이던 카드를 저장해서 리롤 시 가능하면 제외한다.
        //List<SkillData> previousSkills = new();

        //for (int i = 0; i < currentDisplayedCards.Count; i++)
        //{
        //    previousSkills.Add(currentDisplayedCards[i].SkillData);
        //}

        //currentDisplayedCards.Clear();

        //int requestCount = Mathf.Min(
        //    displayCardCount,
        //    skillButtons.Count
        //);

        //List<SkillCardInfo> cards =
        //    skillManager.GetRandomSkillCardsExcept(
        //        requestCount,
        //        previousSkills
        //    );

        //Debug.Log($"새로 받은 카드 수: {cards.Count}");

        //currentDisplayedCards.AddRange(cards);

        //for (int i = 0; i < skillButtons.Count; i++)
        //{
        //    SkillButtonUI buttonUI = skillButtons[i];

        //    if (buttonUI == null)
        //    {
        //        Debug.LogWarning($"{i}번 SkillButtonUI가 비어 있습니다.");
        //        continue;
        //    }

        //    if (i >= currentDisplayedCards.Count)
        //    {
        //        buttonUI.gameObject.SetActive(false);
        //        continue;
        //    }

        //    SkillCardInfo cardInfo = currentDisplayedCards[i];

        //    // 버튼 UI에 카드 정보를 넣고 클릭 콜백을 연결한다.
        //    buttonUI.Setup(
        //        cardInfo,
        //        OnSkillCardSelected
        //    );

        //    Debug.Log($"{i}번 카드 표시: {cardInfo.Name}");
        //}

        // 실제로 스킬 카드들을 랜덤으로 뽑아서 UI에 표시하는 함수.
        // 기존 RerollSkills 안에 있던 카드 뽑기 코드를 이 함수로 옮겼다.
    }

    private void GenerateSkillCards()
    {
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

        currentDisplayedCards.AddRange(cards);

        for (int i = 0; i < skillButtons.Count; i++)
        {
            SkillButtonUI buttonUI = skillButtons[i];

            if (buttonUI == null)
            {
                continue;
            }

            if (i >= currentDisplayedCards.Count)
            {
                buttonUI.gameObject.SetActive(false);
                continue;
            }

            SkillCardInfo cardInfo = currentDisplayedCards[i];

            // 혹시 이전에 꺼졌던 카드가 있다면 다시 켠다.
            buttonUI.gameObject.SetActive(true);

            // 버튼 UI에 카드 정보를 넣고 클릭 콜백을 연결한다.
            buttonUI.Setup(
                cardInfo,
                OnSkillCardSelected
            );
        }
    }
    private void UpdateRerollButtonUI()
    {
        // 리롤 가능 여부를 먼저 계산한다.
        bool canReroll = currentRerollCount > 0;

        // 버튼 안의 숫자 텍스트를 갱신한다.
        if (rerollButtonText != null)
        {
            rerollButtonText.text = $"Reroll [ {currentRerollCount} ]";
        }

        // 실제 버튼 클릭 가능 여부를 바꾼다.
        if (rerollButton != null)
        {
            rerollButton.interactable = canReroll;
        }

        // CanvasGroup으로 버튼 안의 이미지, 텍스트를 전부 한 번에 흐리게 만든다.
        if (rerollButtonCanvasGroup != null)
        {
            rerollButtonCanvasGroup.alpha = canReroll ? 1f : 0.35f;
        }
    }

    private void OnSkillCardSelected(SkillCardInfo selectedCard)
    {
        if (selectedCard == null)
        {
            Debug.LogWarning("선택된 카드가 null입니다.");
            return;
        }

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
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
        Time.timeScale = 1f;
    }
}
