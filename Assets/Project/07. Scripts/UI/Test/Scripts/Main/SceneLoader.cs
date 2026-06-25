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
        // 1. 페이드 아웃 (검은색으로 변함)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeGroup.alpha = timer / fadeDuration;
            yield return null;
        }

        // 2. 씬 전환 (이름을 인자로 받음)
        SceneManager.LoadScene(sceneName);
    }
}