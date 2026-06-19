using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButtonUI : MonoBehaviour
{
    [SerializeField] private int buttonIndex;
    [SerializeField] private LevelUpManager manager;
    public Image iconImage;
    public TMP_Text nameText;
    public TMP_Text descText;
    private SkillData mySkill;

    public void Setup(SkillData data)
    {
        mySkill = data;
        iconImage.sprite = data.Icon;
        nameText.text = data.SkillName;
        descText.text = data.Description;
    }

    public void OnClickButton()
    {
        manager.OnSkillSelected(buttonIndex);
    }
}
