using UnityEngine;

public class RewardBox : MonoBehaviour
{
    private LevelUpManager levelUpManager; // 씬에서 자동으로 찾을 레벨업 매니저

    private StageGuideController stageGuideController; // 보상상자 / 포탈 안내 화살표 관리자

    private bool isOpened = false; // 이미 열렸는지 확인

    private void Start()
    {
        // 씬 안에 있는 LevelUpManager를 자동으로 찾음
        levelUpManager = FindFirstObjectByType<LevelUpManager>();

        // 씬 안에 있는 StageGuideController를 자동으로 찾음
        // 보상상자를 먹으면 화살표가 포탈을 가리키게 하기 위해 사용한다.
        stageGuideController = FindFirstObjectByType<StageGuideController>();
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

        // 보상상자를 먹었으므로 이제 안내 화살표가 포탈을 가리키게 한다.
        if (stageGuideController != null)
        {
            stageGuideController.ShowPortal();
        }

        gameObject.SetActive(false); // 상자 숨기기
    }
}
