using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public CanvasGroup fadeGroup; // 인스펙터에서 검은색 패널(CanvasGroup)을 드래그해 넣으세요.
    public float fadeDuration = 1.0f; // 페이드 시간

    public void OnClickStart()
    {
        StartCoroutine(FadeAndLoad());
    }

    IEnumerator FadeAndLoad()
    {
        // 1. 서서히 어둡게 (Alpha 0 -> 1)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = timer / fadeDuration;
            yield return null;
        }

        // 2. 씬 전환
        SceneManager.LoadScene("LobbyScene");
    }
    public void LoadNextScene()
    {
        Debug.Log("클릭 이벤트가 감지되었습니다!"); // 이 로그가 찍히는지 확인하세요.
        SceneManager.LoadScene("LobbyScene");
    }
}