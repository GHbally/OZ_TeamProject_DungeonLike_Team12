using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위해 반드시 추가

public class UIStageManager : MonoBehaviour
{
    // 버튼에 연결할 함수
    public void LoadNextStage()
    {
        // 현재 활성화된 씬의 인덱스를 가져와서 1을 더해 다음 씬으로 이동
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // 마지막 씬인지 확인 (씬 목록을 넘어가면 오류 방지)
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("마지막 스테이지입니다! 메인 메뉴로 돌아갑니다.");
            SceneManager.LoadScene(0); // 메인 메뉴 씬(인덱스 0)으로 이동
        }
    }
}