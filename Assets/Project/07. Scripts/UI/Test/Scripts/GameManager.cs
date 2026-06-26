using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject deathMenuUI; // 인스펙터에서 연결

    public enum GameState { Playing, Paused, GameOver, Won }
    public GameState currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                if (deathMenuUI != null) deathMenuUI.SetActive(false);
                break;

            case GameState.GameOver:
                // 게임 일시정지
                Time.timeScale = 0f;

                // 사망 UI 활성화
                if (deathMenuUI != null) deathMenuUI.SetActive(true);

                // 필요하다면 마우스 커서도 보이게 처리
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 재시작
    }
}