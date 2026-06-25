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
                deathMenuUI.SetActive(false); // 게임 중엔 숨김
                break;
            case GameState.GameOver:
                Time.timeScale = 0f;
                deathMenuUI.SetActive(true);  // 사망 시 표시
                break;
                // ... 나머지 상태 ...
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 재시작
    }
}