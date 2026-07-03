using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    [Header("UI")]
    [SerializeField] public Image iconImage;
    [SerializeField] public TMP_Text nameText;
    [SerializeField] public TMP_Text descText;

    [Header("마우스 오버 표시 UI")]
    [SerializeField] private GameObject hoverFrame;

    [Header("카드 사운드")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip selectSound;

    private Button button;
    private SkillCardInfo currentCardInfo;
    private Action<SkillCardInfo> onClicked;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogError($"{name}: Button 컴포넌트가 없습니다.", gameObject);
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickButton);

        if (nameText != null)
        {
            nameText.raycastTarget = false;
        }

        if (descText != null)
        {
            descText.raycastTarget = false;
        }

        if (iconImage != null)
        {
            iconImage.raycastTarget = false;
        }

        HideHoverFrame();
    }

    public void Setup(
        SkillCardInfo cardInfo,
        Action<SkillCardInfo> clickCallback)
    {
        currentCardInfo = cardInfo;
        onClicked = clickCallback;

        HideHoverFrame();

        if (cardInfo == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        if (iconImage != null)
        {
            iconImage.sprite = cardInfo.Icon;
            iconImage.enabled = cardInfo.Icon != null;
            iconImage.raycastTarget = false;
        }

        if (nameText != null)
        {
            nameText.text = cardInfo.Name;
        }

        if (descText != null)
        {
            descText.text =
                $"Lv.{cardInfo.CurrentLevel} → Lv.{cardInfo.NextLevel}\n" +
                cardInfo.Description;
        }
    }

    private void OnClickButton()
    {
        if (currentCardInfo == null)
        {
            Debug.LogWarning($"{name}: 선택할 카드 정보가 없습니다.", gameObject);
            return;
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(selectSound);
        }

        onClicked?.Invoke(currentCardInfo);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스로 올린 카드도 현재 선택된 카드로 만든다.
        // 이렇게 해야 마우스와 키보드 선택 위치가 서로 맞는다.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
        else
        {
            ShowHoverFrame();
            PlayHoverSound();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 마우스가 나가도 키보드 선택 상태라면 Hover UI를 유지한다.
        if (EventSystem.current != null &&
            EventSystem.current.currentSelectedGameObject == gameObject)
        {
            return;
        }

        HideHoverFrame();
    }

    public void OnSelect(BaseEventData eventData)
    {
        // 키보드 방향키/WASD로 선택됐을 때 Hover UI 표시
        ShowHoverFrame();
        PlayHoverSound();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        HideHoverFrame();
    }

    private void PlayHoverSound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(hoverSound);
        }
    }

    private void ShowHoverFrame()
    {
        if (hoverFrame == null)
        {
            return;
        }

        hoverFrame.SetActive(true);
    }

    private void HideHoverFrame()
    {
        if (hoverFrame == null)
        {
            return;
        }

        hoverFrame.SetActive(false);
    }
}