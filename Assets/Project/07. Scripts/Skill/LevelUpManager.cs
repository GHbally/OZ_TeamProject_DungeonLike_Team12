using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // TMP를 사용하므로 필수!
using DG.Tweening;    //두트윈재밌겠당히히
using UnityEngine.EventSystems;

public class LevelUpManager : MonoBehaviour
{
    [Header("UI 구성요소")]
    [SerializeField] private GameObject levelUpPanel;
    //두트윈 패널 크기 조절용
    [SerializeField] private RectTransform panelRect;

    // 카드 버튼 UI들.
    // Button 컴포넌트가 아니라 TestSkillButtonUI 컴포넌트를 넣어야 한다.
    [SerializeField] private List<SkillButtonUI> skillButtons = new();

    [Header("데이터")]
    [SerializeField] private SkillManager skillManager;

    [Header("설정")]
    [SerializeField, Min(1)] private int displayCardCount = 3;
    [SerializeField] private float panelOpenDuration = 0.4f;    //패널이 열리는 시간
    [SerializeField] private float cardOpenDuration = 0.35f;    //카드가 커지는 시간
    [SerializeField] private float delayBetweenCards = 0.12f;   //카드 간 등장 간격

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

    private List<CanvasGroup> buttonCanvasGroups = new List<CanvasGroup>();

    private Coroutine showCardsCoroutine;

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

        //두트윈 페이드 효과용
        UpdateCanvasGroupCache();

    }

    private void Update()
    {
        // 레벨업 패널이 꺼져 있으면 입력을 받지 않는다.
        if (levelUpPanel == null || !levelUpPanel.activeSelf)
        {
            return;
        }

        // 스페이스 바를 누르면 현재 선택된 UI를 클릭한다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SelectCurrentCard();
        }
    }

    private void UpdateCanvasGroupCache()
    {
        buttonCanvasGroups.Clear();
        foreach (var btn in skillButtons)
        {
            if (btn != null)
            {
                CanvasGroup cg = btn.GetComponent<CanvasGroup>() ?? btn.gameObject.AddComponent<CanvasGroup>();
                buttonCanvasGroups.Add(cg);
            }
        }
    }

    public void OpenLevelUpUI()
    {
        if (levelUpPanel == null || panelRect == null) return;

        levelUpPanel.SetActive(true);

        GameManager.Instance.ChangeState(GameManager.GameState.Menu);

        // 카드 선택 중에는 게임을 멈춘다. 두트윈은 돌아가게 할거
        Time.timeScale = 0f;

        // 스킬 선택창이 새로 열릴 때마다 리롤 횟수를 초기화한다.
        currentRerollCount = maxRerollCount;

        // 버튼 텍스트를 갱신한다.
        UpdateRerollButtonUI();

        // 처음 스킬 선택창이 열릴 때 카드들을 뽑는다.
        GenerateSkillCards();
        UpdateCanvasGroupCache();   //혹시 모를 리스트 바뀔시에 갱신

        //기존 트윈청소
        panelRect.localScale = Vector3.zero;
        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (skillButtons[i] != null)
            {
                skillButtons[i].transform.localScale = Vector3.zero;
                if (i < buttonCanvasGroups.Count) buttonCanvasGroups[i].alpha = 0f;
            }
        }

        panelRect.DOKill();
        foreach (var btn in skillButtons) { if (btn != null) btn.transform.DOKill(); }
        foreach (var cg in buttonCanvasGroups) { if (cg != null) cg.DOKill(); }

        //배경 패널 꿀렁 연출 (시퀀스)
        Sequence slimeSeq = DOTween.Sequence();
        slimeSeq.Append(panelRect.DOScale(new Vector3(1.2f, 0.75f, 1f), panelOpenDuration * 0.4f).SetEase(Ease.OutQuad))
                .Append(panelRect.DOScale(new Vector3(0.9f, 1.1f, 1f), panelOpenDuration * 0.3f).SetEase(Ease.InOutQuad))
                .Append(panelRect.DOScale(Vector3.one, panelOpenDuration * 0.3f).SetEase(Ease.OutQuad))
                .SetUpdate(true); //시간 정지 무시 옵션

        //꿀렁임이 끝나면 카드들이 순서대로 뿅뿅 등장
        slimeSeq.OnComplete(() =>
        {
            showCardsCoroutine = StartCoroutine(ShowCardsCoroutine());
        });
    }

    private IEnumerator ShowCardsCoroutine()
    {
        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (skillButtons[i] == null || !skillButtons[i].gameObject.activeSelf) continue;

            //페이드 인
            if (i < buttonCanvasGroups.Count)
            {
                buttonCanvasGroups[i].DOFade(1f, cardOpenDuration).SetUpdate(true);
            }

            if (i < buttonCanvasGroups.Count) buttonCanvasGroups[i].DOFade(1f, cardOpenDuration).SetUpdate(true);
            skillButtons[i].transform.DOScale(Vector3.one, cardOpenDuration).SetEase(Ease.OutBack).SetUpdate(true);

            //카드 등장할 때 사운드 매니저가 있다면 효과음 재생
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX("SFX_UIHoverPaperDash01");
            }

            //실시간 정지 상태이므로 WaitForSeconds 대신 WaitForSecondsRealtime을 써야 대기 창이 먹힙니다!
            yield return new WaitForSecondsRealtime(delayBetweenCards);
        }
        showCardsCoroutine = null;
    }


    public void RerollSkills()
    {
        if (currentRerollCount <= 0)
        {
            UpdateRerollButtonUI();
            return;
        }

        //리롤에도 SFX적용
        if (showCardsCoroutine != null)
        {
            StopCoroutine(showCardsCoroutine);
            showCardsCoroutine = null;
        }
        foreach (var btn in skillButtons) { if (btn != null) btn.transform.DOKill(); }
        foreach (var cg in buttonCanvasGroups) { if (cg != null) cg.DOKill(); }

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

        //리롤 후에도 뿅뿅 연출하려면 코루틴 재실행
        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (skillButtons[i] != null)
            {
                skillButtons[i].transform.localScale = Vector3.zero;
                if (i < buttonCanvasGroups.Count) buttonCanvasGroups[i].alpha = 0f;
            }
        }
        StartCoroutine(RerollAnimationDelayCo());
    }

    private IEnumerator RerollAnimationDelayCo()
    {
        yield return new WaitForEndOfFrame(); //한 프레임 대기
        showCardsCoroutine = StartCoroutine(ShowCardsCoroutine()); //연출 및 사운드
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

        // 카드 생성 후 첫 번째 카드를 키보드 선택 상태로 만든다.
        StartCoroutine(DelaySelectFirstSkillCard());
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
        //닫힐 때 잔여 트윈 정지
        panelRect.DOKill();

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
        GameManager.Instance.ChangeState(GameManager.GameState.Playing);
        Time.timeScale = 1f;
    }

    private void SelectSecondSkillCard()
    {
        if (EventSystem.current == null)
        {
            Debug.LogWarning("EventSystem이 씬에 없습니다.");
            return;
        }

        if (skillButtons.Count <= 1)
        {
            return;
        }

        if (skillButtons[1] == null)
        {
            return;
        }

        if (!skillButtons[1].gameObject.activeInHierarchy)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(skillButtons[1].gameObject);
    }
    

    private void SelectCurrentCard()
    {
        if (EventSystem.current == null)
        {
            Debug.LogWarning("EventSystem이 없습니다.");
            return;
        }

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
        {
            return;
        }

        Button selectedButton = selectedObject.GetComponent<Button>();

        if (selectedButton == null)
        {
            return;
        }

        if (!selectedButton.interactable)
        {
            return;
        }

        selectedButton.onClick.Invoke();
    }

    private IEnumerator DelaySelectFirstSkillCard()
    {
        yield return new WaitForSecondsRealtime(0.7f);

        SelectSecondSkillCard();
    }
}
