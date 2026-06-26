using UnityEngine;
public class StageManager : MonoBehaviour
{
    // 현재 챕터 번호
    public int chapter = 1;

    // 현재 스테이지 번호
    public int stage = 1;

    // 웨이브를 관리하는 WaveManager 연결
    public WaveManager waveManager;

    // 다음 스테이지로 이동하는 포탈 오브젝트
    public GameObject nextStagePortal;

    // 게임이 시작될 때 한 번만 실행
    private void Start()
    {
        // 처음에는 포탈을 숨겨둠
        nextStagePortal.SetActive(false);

        // 현재 챕터와 스테이지의 첫 번째 웨이브 시작
        waveManager.StartStage(chapter, stage);
    }

    public Vector3 portalSpawnPosition = new Vector3(0f, 0f, 0f); // 포탈이 생성될 위치
    // 모든 웨이브를 클리어했을 때 호출
    public void ClearStage()
    {
        // 포탈 위치를 (0, 0, 0)으로 이동
        nextStagePortal.transform.position = portalSpawnPosition;

        // 다음 스테이지로 갈 수 있도록 포탈 활성화
        nextStagePortal.SetActive(true);
    }

    // 플레이어가 포탈에 들어왔을 때 호출
    public void NextStage()
    {
        // 포탈은 다음 스테이지가 시작되면 다시 숨김
        nextStagePortal.SetActive(false);

        // 마지막 스테이지라면 게임 종료
        if (chapter == 1 && stage == 3)
        {
            Debug.Log("게임 클리어"); // 콘솔에 게임 클리어 출력
            return; // 더 이상 실행하지 않음
        }

        // 다음 스테이지 번호 증가
        stage++;

        // 다음 스테이지의 웨이브 시작
        waveManager.StartStage(chapter, stage);
    }
}
