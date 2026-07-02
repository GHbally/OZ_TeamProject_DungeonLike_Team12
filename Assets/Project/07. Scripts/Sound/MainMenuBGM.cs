using UnityEngine;

public class MainMenuBGM : MonoBehaviour
{
    [Header("메인메뉴 BGM 설정")]
    [SerializeField] private string bgmName = "Loading... - Ambient"; // 음악 파일 이름 고정
    [SerializeField] private float volume = 0.15f;                    // 볼륨 크기 (0~1 사이)

    void Start()
    {
        // 사운드 매니저 싱글톤 인스턴스가 안전하게 생성된 시점(Start)에 BGM 재생 명령을 내립니다.
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgmName, volume);
            Debug.Log($"메인메뉴 BGM 재생 시도: {bgmName}");
        }
    }
}
