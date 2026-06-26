using UnityEngine;

public class RewardBox : MonoBehaviour
{
    private LevelUpManager levelUpManager; // 씬에서 자동으로 찾을 레벨업 매니저

    private bool isOpened = false; // 이미 열렸는지 확인

    private void Start()
    {
        // 씬 안에 있는 LevelUpManager를 자동으로 찾음
        levelUpManager = FindFirstObjectByType<LevelUpManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어가 아니면 실행하지 않음
        if (!other.CompareTag("Player"))
            return;

        // 이미 열렸으면 다시 실행하지 않음
        if (isOpened)
            return;

        // LevelUpManager를 못 찾았으면 오류 출력
        if (levelUpManager == null)
        {
            Debug.LogError("LevelUpManager를 찾지 못했습니다.");
            return;
        }

        isOpened = true; // 상자 열림 처리

        levelUpManager.OpenLevelUpUI(); // 스킬 선택창 열기

        gameObject.SetActive(false); // 상자 숨기기
    }
}
