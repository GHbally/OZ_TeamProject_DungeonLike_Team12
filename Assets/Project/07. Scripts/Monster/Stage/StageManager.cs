using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int chapter = 1; // 현재 챕터
    public int stage = 1;   // 현재 스테이지

    public WaveManager waveManager; // 웨이브 매니저 연결

    public GameObject rewardBox; // 보상 상자 오브젝트

    public GameObject nextStagePortal; // 다음 스테이지 포털 오브젝트

    private void Start()
    {
        rewardBox.SetActive(false); // 게임 시작 시 보상 상자 숨기기

        nextStagePortal.SetActive(false); // 게임 시작 시 포털 숨기기

        waveManager.StartStage(chapter, stage); // 첫 스테이지 시작
    }

    public void ClearStage()
    {
        rewardBox.SetActive(true); // 스테이지 클리어 시 보상 상자 보이기
    }

    public void NextStage()
    {
        nextStagePortal.SetActive(false); // 포털 숨기기

        if (chapter == 1 && stage == 3) // 마지막 스테이지라면
        {
            Debug.Log("게임 클리어"); // 게임 클리어 로그 출력
            return;
        }

        stage++; // 다음 스테이지 번호 증가

        waveManager.StartStage(chapter, stage); // 다음 스테이지 시작
    }
}
