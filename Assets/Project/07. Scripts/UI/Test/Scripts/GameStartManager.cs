using UnityEngine;
using System.Collections;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] private GameObject missionPanel;
    [SerializeField] private GameObject startPanel;

    private void Start()
    {
        StartCoroutine(GameStartSequence());
    }

    private IEnumerator GameStartSequence()
    {
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
}