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

    private int currentEnemyCount = 0;//김영웅 수정
    private bool isBossDead = false;//김영웅 수정

    // 적이 생성될 때마다 호출 (보스, 잡몹 등) 김영웅 수정
    public void RegisterEnemy() => currentEnemyCount++;

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
        nextStagePortal.transform.position = new Vector3(0f, 0f, 0f); // 포탈 위치를 0,0,0으로 이동

        nextStagePortal.SetActive(true); // 포탈 오브젝트 활성화

        SpriteRenderer sr = nextStagePortal.GetComponent<SpriteRenderer>(); // 포탈 오브젝트의 SpriteRenderer 가져오기

        if (sr != null) // SpriteRenderer가 있으면 실행
        {
            sr.enabled = true; // SpriteRenderer를 켜서 화면에 보이게 함

            sr.color = Color.white; // 색을 흰색으로 바꿔서 잘 보이게 함

            sr.sortingLayerName = "Default"; // 기본 정렬 레이어 사용

            sr.sortingOrder = 999; // 다른 배경보다 훨씬 앞에 보이게 함
        }

        nextStagePortal.transform.localScale = new Vector3(3f, 3f, 1f); // 포탈 크기를 크게 키워서 테스트

        Debug.Log("포탈 활성화 완료 / 위치: " + nextStagePortal.transform.position); // 포탈 위치 확인
    }

    public void UnregisterEnemy(bool isBoss)// 김영웅 수정
    {
        currentEnemyCount--;
        if (isBoss) isBossDead = true;

        // 보스가 죽었고, 잡몹이 모두 제거되었을 때만 클리어
        if (isBossDead && currentEnemyCount <= 0)
        {
            ClearStage();
        }
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
