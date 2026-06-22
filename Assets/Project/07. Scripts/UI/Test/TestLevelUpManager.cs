using System.Collections.Generic;
using UnityEngine;
using TMPro; // TMP를 사용하므로 필수!

public class LevelUpManager : MonoBehaviour
{
    [Header("UI 구성요소")]
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private List<GameObject> skillButtons; // 버튼 오브젝트들을 담는 리스트

    [Header("데이터")]
    [SerializeField] private List<SkillData> allAvailableSkills; // 등록할 스킬 데이터 파일들

    [SerializeField] private List<SkillData> currentDisplayedSkills; // 현재 화면에 출력된 스킬들

    private void Start()
    {
        // 시작할 때 패널이 켜져 있다면 꺼줌
        if (levelUpPanel.activeSelf)
        {
            levelUpPanel.SetActive(false);
        }
    }

    public void OpenLevelUpUI()
    {
        levelUpPanel.SetActive(true);
        Time.timeScale = 0f; // 게임 일시정지
        RerollSkills(); // 창을 열 때 자동으로 스킬 생성
    }

    public void RerollSkills()
    {
        Debug.Log("리롤 시작!");

        // 1. 현재 출력할 리스트 초기화
        currentDisplayedSkills.Clear();

        List<SkillData> pool = new List<SkillData>(allAvailableSkills);

        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (pool.Count == 0) break;

            int randomIndex = Random.Range(0, pool.Count);
            SkillData selected = pool[randomIndex];

            // 2. 중요: 현재 화면에 표시된 데이터 리스트에 저장
            currentDisplayedSkills.Add(selected);

            // 3. UI 업데이트 (기존 코드 유지)
            TextMeshProUGUI buttonText = skillButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = selected.SkillName;
            }

            pool.RemoveAt(randomIndex);
        }
    }
    public void OnSkillSelected(int index)
    {
        SkillData selected = currentDisplayedSkills[index];
        Debug.Log(selected.SkillName + " 선택됨!");

        // 여기에 실제로 스탯을 올리는 로직을 넣으세요.
        CloseLevelUpUI();
    }

    public void CloseLevelUpUI()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // 게임 다시 시작
    }
}
