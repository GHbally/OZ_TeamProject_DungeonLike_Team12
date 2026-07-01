using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 인스펙터에서 직접 드래그해서 연결할 패널
    public GameObject SkillUI;

    // "내 정보" 버튼에 연결할 함수
    public void OpenUserInfo()
    {
        if (SkillUI != null)
        {
            SkillUI.SetActive(true);
        }
    }

    // "닫기" 버튼에 연결할 함수
    public void CloseUserInfo()
    {
        if (SkillUI != null)
        {
            SkillUI.SetActive(false);
        }
    }
}
