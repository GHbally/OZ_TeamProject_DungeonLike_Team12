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
        iconImage.sprite = data.icon;
        nameText.text = data.skillName;
        descText.text = data.description;
    }

    public void OnClickButton()
    {
        Debug.Log(mySkill.skillName + " º±≈√µ !");
    }
}
