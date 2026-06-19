using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public void OnSkillSelected(string skillName)
    {
        Debug.Log(skillName + " 선택됨!");
        // 여기에 스킬 적용 로직을 작성
    }
}