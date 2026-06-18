using UnityEngine;
using System.Collections.Generic;
using System.Linq; // 랜덤 추출을 위해 추가

public class LevelUpManager : MonoBehaviour
{
    public GameObject levelUpPanel;
    public GameObject skillButtonPrefab;
    public Transform skillContainer;
    public List<SkillData> skillDatabase; // 인스펙터에서 스킬 데이터들을 드래그해서 넣으세요

    public void OpenLevelUp()
    {
        Time.timeScale = 0f; // 게임 멈춤
        levelUpPanel.SetActive(true);

        // 기존에 생성된 버튼 삭제
        foreach (Transform child in skillContainer) Destroy(child.gameObject);

        // 랜덤하게 3개 추출 (중복 방지)
        List<SkillData> randomSkills = skillDatabase.OrderBy(x => Random.value).Take(3).ToList();

        // 버튼 생성 및 데이터 전달
        foreach (SkillData skill in randomSkills)
        {
            GameObject newButton = Instantiate(skillButtonPrefab, skillContainer);
            newButton.GetComponent<SkillButtonUI>().Setup(skill);

            // 버튼 클릭 시 수행할 동작 연결
            newButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SelectSkill(skill));
        }
    }

    public void SelectSkill(SkillData skill)
    {
        Debug.Log(skill.skillName + " 선택됨!");
        // 여기서 실제 캐릭터 스킬 강화 로직 호출
        CloseLevelUp();
    }

    public void CloseLevelUp()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 재개
    }
}
