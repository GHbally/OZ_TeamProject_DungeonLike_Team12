using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillButtonUI : MonoBehaviour
{
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
        Debug.Log(mySkill.SkillName + " º±≈√µ !");
    }
}
