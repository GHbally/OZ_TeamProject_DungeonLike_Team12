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
        // 1. 디버그 로그 추가 (버튼 클릭 확인용)
        Debug.Log("리롤 시작!");

        // 2. 사용 가능한 스킬 리스트 복사본 생성 (원본 유지)
        List<SkillData> pool = new List<SkillData>(allAvailableSkills);

        // 3. 버튼 개수만큼 랜덤하게 뽑기
        for (int i = 0; i < skillButtons.Count; i++)
        {
            if (pool.Count == 0) break; // 스킬이 부족하면 중단

            int randomIndex = Random.Range(0, pool.Count);
            SkillData selected = pool[randomIndex];

            // 4. 버튼의 텍스트(TMP) 변경
            // 주의: 버튼 하위에 있는 Text (TMP) 컴포넌트를 찾습니다.
            TextMeshProUGUI buttonText = skillButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = selected.SkillName;
            }

            // 5. 뽑은 스킬은 풀에서 제거 (중복 방지)
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
