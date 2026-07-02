using UnityEngine;

public class LobbyBGM : MonoBehaviour
{
    [Header("로비 BGM 설정")]
    [SerializeField] private string bgmName = "Adventure's Spirit - Title Theme"; // 로비 브금 파일명 고정
    [SerializeField] private float volume = 0.15f;                             // 볼륨 크기 (0~1 사이)

    void Start()
    {
        // 씬이 로딩되고 오브젝트들이 정돈된 시점(Start)에 사운드 매니저를 호출합니다.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgmName, volume);
            Debug.Log($"로비 BGM 재생 시도: {bgmName}");
        }
    }
}
