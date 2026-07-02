using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    [SerializeField]public Image iconImage;
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

    //[SerializeField] private int buttonIndex;
    //[SerializeField] private LevelUpManager manager;
    //private SkillData mySkill;

    private void Awake()
    {
        button = GetComponent<Button>();

        // 버튼 클릭은 Inspector가 아니라 이 스크립트에서 관리한다.
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickButton);

        // 텍스트가 버튼 클릭을 막지 않게 한다.
        if (nameText != null)
        {
            nameText.raycastTarget = false;
        }

        if (descText != null)
        {
            descText.raycastTarget = false;
        }

        // 시작할 때 마우스 오버 표시 UI는 꺼둔다.
        HideHoverFrame();
    }

    // LevelUpManager가 리롤할 때 카드 정보를 넣어준다.
    public void Setup(
        SkillCardInfo cardInfo,
        Action<SkillCardInfo> clickCallback)
    {
        currentCardInfo = cardInfo;
        onClicked = clickCallback;

        // 카드가 재사용될 수 있으므로 Setup할 때마다 Hover 표시를 꺼준다.
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

            // 아이콘 이미지가 클릭을 막지 않게 한다.
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
        else
        {
            Debug.LogWarning("SoundManager.Instance가 없습니다.");
        }
        // 선택된 카드 정보를 LevelUpManager에게 전달한다.
        onClicked?.Invoke(currentCardInfo);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 마우스가 카드 위에 올라오면 모서리 UI를 켠다.
        ShowHoverFrame();

        // 카드에 마우스를 올렸을 때 Hover 사운드를 1번 재생한다.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX(hoverSound);
        }
        else
        {
            Debug.LogWarning("SoundManager.Instance가 없습니다.");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideHoverFrame();
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
