using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelUpManager : MonoBehaviour
{
    public GameObject levelUpPanel;
    public GameObject skillButtonPrefab;
    public Transform skillContainer;
    public List<SkillData> skillDatabase;

    public void OpenLevelUp()
    {
        if (levelUpPanel == null || skillContainer == null) return;

        Time.timeScale = 0f; // 게임 멈춤
        levelUpPanel.SetActive(true);

        // 자식 오브젝트 정리 (즉시 삭제)
        foreach (Transform child in skillContainer)
        {
            Destroy(child.gameObject);
        }

        // 스킬이 3개 미만일 경우 에러 방지
        int count = Mathf.Min(3, skillDatabase.Count);
        List<SkillData> randomSkills = skillDatabase.OrderBy(x => Random.value).Take(count).ToList();

        foreach (SkillData skill in randomSkills)
        {
            GameObject newButton = Instantiate(skillButtonPrefab, skillContainer);
            var ui = newButton.GetComponent<SkillButtonUI>();

            if (ui != null) ui.Setup(skill);

            var btn = newButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners(); // 기존 리스너 초기화
                btn.onClick.AddListener(() => SelectSkill(skill));
            }
        }
    }

    public void SelectSkill(SkillData skill)
    {
        // 여기서 캐릭터 로직 수행
        Debug.Log($"{skill.skillName} 선택됨!");

        CloseLevelUp();
    }

    public void CloseLevelUp()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 재개
    }
}
