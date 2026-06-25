using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // 인스펙터에서 PauseMenu 오브젝트를 드래그해서 넣어주세요
    private bool isPaused = false;

    void Update()
    {
        // ESC 키 입력 감지
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // UI 숨기기
        Time.timeScale = 1f;          // 게임 시간 정상화
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);  // UI 보여주기
        Time.timeScale = 0f;          // 게임 시간 멈춤
        isPaused = true;
    }

    public void OpenSettings()
    {
        Debug.Log("설정창 열기 기능 구현");
    }
}
