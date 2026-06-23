using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField]public Image iconImage;
    [SerializeField] public TMP_Text nameText;
    [SerializeField] public TMP_Text descText;

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
    }

    // LevelUpManager가 리롤할 때 카드 정보를 넣어준다.
    public void Setup(
        SkillCardInfo cardInfo,
        Action<SkillCardInfo> clickCallback)
    {
        currentCardInfo = cardInfo;
        onClicked = clickCallback;

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

        Debug.Log($"카드 버튼 클릭됨: {currentCardInfo.Name}");

        // 선택된 카드 정보를 LevelUpManager에게 전달한다.
        onClicked?.Invoke(currentCardInfo);
    }
}
