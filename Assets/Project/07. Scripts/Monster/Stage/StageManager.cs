using System.Collections;
using System.Collections.Generic;
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

    [Header("맵 전환 Obejct")]
    public StageMapSwitcher stageMapSwitcher;

    private int currentEnemyCount = 0;//김영웅 수정
    private bool isBossDead = false;//김영웅 수정

    // 적이 생성될 때마다 호출 (보스, 잡몹 등) 김영웅 수정
    public void RegisterEnemy() => currentEnemyCount++;

    [Header("시작/미션 UI")]
    public GameStartManager gameStartManager;

    [Header("미션 스테이지 UI")]
    [SerializeField] private StageWaveUI missionStageWaveUI;

    [Header("스테이지 안내 화살표UI")]
    [SerializeField] private StageGuideController stageGuideController;

    [Header("플레이어 위치 이동")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] stageStartPoints;

    // 게임이 시작될 때 한 번만 실행
    private void Start()
    {
        // 처음에는 포탈을 숨겨둠
        nextStagePortal.SetActive(false);

        // 현재 스테이지에 맞는 맵을 켜고 플레이어를 시작 위치로 이동시킨다.
        if (stageMapSwitcher != null)
        {
            stageMapSwitcher.ChangeMap(stage);
        }

        // 현재 챕터와 스테이지의 첫 번째 웨이브 시작
        waveManager.StartStage(chapter, stage);
    }

    [Header("포탈 생성 위치")]
    public Vector3 portalSpawnPosition = new Vector3(-2.5f, -11.54f, 0f); // 포탈이 생성될 위치
    // 모든 웨이브를 클리어했을 때 호출
    public void ClearStage()
    {
        nextStagePortal.transform.position = new Vector3(-2.5f, -11.54f, 0f); // 포탈 위치를 0,0,0으로 이동

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
        // 기존 함수를 UI를 띄우기 위해 코루틴으로 옮겨놨습니다.

        StartCoroutine(NextStageFlow());
    }

    private IEnumerator NextStageFlow()
    {
        // 다음 스테이지로 넘어가기 시작하면 이전 스테이지 안내 화살표를 숨긴다.
        if (stageGuideController != null)
        {
            stageGuideController.Hide();
        }

        nextStagePortal.SetActive(false);

        // 다음 스테이지로 넘어가는 즉시 이전 스테이지 보상상자를 제거한다.
        // Mission UI가 떠 있는 3초 동안 보상상자가 남아 보이지 않게 하기 위해서다.
        if (waveManager != null)
        {
            waveManager.RemoveSpawnedRewardBox();
        }

        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.ReturnAllOrbsInScene();
        }

        if (chapter == 1 && stage == 3)
        {
            Debug.Log("게임 클리어");
            yield break;
        }

        stage++;

        if (missionStageWaveUI != null)
        {
            missionStageWaveUI.UpdateStage(chapter, stage);
        }

        if (stageMapSwitcher != null)
        {
            stageMapSwitcher.ChangeMap(stage);
        }

        // 맵이 바뀐 뒤 플레이어를 해당 스테이지 시작 위치로 이동시킨다.
        MovePlayerToStageStartPoint();

        // 카메라가 플레이어 위치를 따라갈 시간을 조금 준다.
        // 아직 Time.timeScale을 0으로 만들기 전이라 카메라 이동이 가능하다.
        //yield return new WaitForSecondsRealtime(0.1f);
        yield return new WaitForSeconds(1.5f);

        if (gameStartManager != null)
        {
            yield return StartCoroutine(gameStartManager.ShowMissionForNextStage());
        }

        waveManager.StartStage(chapter, stage);
    }

    private void MovePlayerToStageStartPoint()
    {
        // 플레이어가 연결되지 않았으면 이동할 수 없으므로 종료한다.
        if (player == null)
        {
            Debug.LogWarning("StageManager: Player가 연결되지 않았습니다.", gameObject);
            return;
        }

        // stage는 1부터 시작하고 배열은 0부터 시작하므로 -1을 해준다.
        int targetIndex = stage - 1;

        // 잘못된 스테이지 번호이거나 시작 위치가 부족하면 종료한다.
        if (targetIndex < 0 || targetIndex >= stageStartPoints.Length)
        {
            Debug.LogWarning($"StageManager: Stage {stage}에 해당하는 시작 위치가 없습니다.", gameObject);
            return;
        }

        // 해당 스테이지의 시작 위치가 비어 있으면 종료한다.
        if (stageStartPoints[targetIndex] == null)
        {
            Debug.LogWarning($"StageManager: Stage {stage} 시작 위치가 연결되지 않았습니다.", gameObject);
            return;
        }

        // 플레이어를 현재 스테이지 시작 위치로 이동시킨다.
        player.position = stageStartPoints[targetIndex].position;
        //스테이지 별로 포탈 이동을 제어 할때 여기 배열 추가
    }
}
