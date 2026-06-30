using UnityEngine;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject startPanel;

    [Header("다음 스테이지 Mission UI 대기시간")]
    [SerializeField] private float nextStageMissionDuration = 3f;

    private Coroutine currentSequence;

    private void Start()
    {
        currentSequence = StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
        //시간 멈추기 전 카메라 플레이어 위치로 고정^^
        CameraFollow cam = FindFirstObjectByType<CameraFollow>();
        if (cam != null)
        {
            cam.ResetCameraPosition();
        }

        // 1. 게임 시간 멈춤
        Time.timeScale = 0f;

        missionPanel.SetActive(true);
        startPanel.SetActive(false);

        // 중요: unscaledDeltaTime을 사용하지 않아도 
        // yield return new WaitForSecondsRealtime을 쓰면 
        // timeScale이 0이어도 시간이 흐릅니다.
        yield return new WaitForSecondsRealtime(2.0f);

        missionPanel.SetActive(false);
        startPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(1.0f);

        startPanel.SetActive(false);

        // 2. 게임 시간 다시 정상으로 복구
        Time.timeScale = 1f;
    }

    // 다음 스테이지로 넘어갈 때 Mission UI만 3초 동안 보여주는 함수.
    // StageManager에서 이 함수를 코루틴으로 호출하면 됩니다.
    public IEnumerator ShowMissionForNextStage()
    {
        // 이미 다른 시작 연출이 실행 중이면 중복 실행을 막는다.
        if (currentSequence != null)
        {
            yield return currentSequence;
        }

        // 게임 시간을 멈춘다.
        Time.timeScale = 0f;

        if (missionPanel != null)
        {
            missionPanel.SetActive(true);
        }

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        // Time.timeScale이 0이어도 3초 대기하기 위해 Realtime을 사용한다.
        yield return new WaitForSecondsRealtime(nextStageMissionDuration);

        if (missionPanel != null)
        {
            missionPanel.SetActive(false);
        }

        // 게임 시간을 다시 정상으로 돌린다.
        Time.timeScale = 1f;
    }
}