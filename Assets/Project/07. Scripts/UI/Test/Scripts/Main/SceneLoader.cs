using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    public CanvasGroup fadeGroup;
    public float fadeDuration = 1.0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 파괴되지 않음
        }
        else
        {
            Destroy(gameObject); // 만약 이미 존재한다면 중복 생성 방지
        }
    }

    // 통합 함수: 이 함수 하나만 사용하세요!
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            // 중요: deltaTime이 아닌 unscaledDeltaTime을 써야 게임이 멈춰도 연출이 진행됨
            timer += Time.unscaledDeltaTime;
            fadeGroup.alpha = timer / fadeDuration;
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}