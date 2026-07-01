using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private GameObject deathMenuUI;
    [SerializeField] private GameObject winMenuUI; // 승리 UI 추가

    public enum GameState { Playing, Paused, GameOver, Won, Menu }
    public GameState currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        ChangeState(GameState.Playing); // 게임 시작 시 Playing 상태로 초기화
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                if (deathMenuUI) deathMenuUI.SetActive(false);
                if (winMenuUI) winMenuUI.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                break;

            case GameState.Paused:
            case GameState.Menu: // UI 조작 중일 때 마우스 사용 가능
                Time.timeScale = 0f; // 필요에 따라 멈춤
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                if (deathMenuUI) deathMenuUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                //BGM변경
                if (SoundManager.Instance != null)
                {
                    //루프는 1번만 재생 되도록 패배 Lose BGM 재생
                    SoundManager.Instance.PlayBGM("Lose vol. 1", 0.5f, false);
                }
                break;

            case GameState.Won: // 승리 상태 로직 추가
                Time.timeScale = 0f;
                if (winMenuUI) winMenuUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                //BGM변경
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayBGM("Win vol. 1", 0.8f, false);
                }
                break;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}