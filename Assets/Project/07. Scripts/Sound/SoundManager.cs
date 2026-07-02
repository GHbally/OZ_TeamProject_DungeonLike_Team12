//대망의 사운드 매니저
using System.Collections.Generic; //제네릭제네릭
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("오디오 소스 설정")]
    [SerializeField] private AudioSource bgmSource;     //배경음 소스
    [SerializeField] private AudioSource sfxSource;     //효과음 소스

    [Header("사운드 파일 등록")]
    [SerializeField] private List<AudioClip> bgmClips;  //배경음 클립
    [SerializeField] private List<AudioClip> sfxClips;  //효과음 클립

    //이름으로 사운드 파일 빠르게 찾기 위한 사전
    private readonly Dictionary<string, AudioClip> bgmDictionary = new();
    private readonly Dictionary<string, AudioClip> sfxDictionary = new();

    //각 SFX가 마지막으로 재생된 시간 기록(중복 효과음 방지)
    private readonly Dictionary<string, float> sfxLastPlayTimes = new();

    //최소 재생 간격 설정
    [Header("오디오 최적화")]
    [SerializeField] private float minPlayInterval = 0.08f;

    private void Awake()
    {
        //싱글톤 세팅
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  //씬이 바뀌어도 음악이 끊기지 않게 보존
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //리스트에 넣은 클립들을 이름 기반 딕셔너리로 자동 정렬
        foreach (var clip in bgmClips)
        {
            if (clip != null) bgmDictionary[clip.name] = clip;
        }
        foreach (var clip in sfxClips)
        {
            if (clip != null) sfxDictionary[clip.name] = clip;
        }
    }

    //BGM(배경음) 재생 시스템
    public void PlayBGM(string bgmName, float volum = 0.15f, bool loop = true)
    {
        //BGM없으면 리턴
        if (!bgmDictionary.TryGetValue(bgmName, out AudioClip clip))
        {
            return;
        }

        //이미 같은 배경음 나오고 있다면 중복 재생 방지
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.volume = volum;
        bgmSource.loop = loop;      //루프 설정
        bgmSource.Play();           //재생
    }

    //BGM(배경음) 멈추기
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    //SFX(효과음) 재생 시스템
    public void PlaySFX(string sfxName, float volum = 0.4f)
    {
        //SFX없으면 리턴
        if (!sfxDictionary.TryGetValue(sfxName, out AudioClip clip))
        {
            return;
        }

        if (sfxLastPlayTimes.TryGetValue(sfxName, out float lastTime))
        {
            if (Time.unscaledTime - lastTime < minPlayInterval)
            {
                return;
            }
        }

        //재생이 통과 됐으면 현재 시간 최신화
        sfxLastPlayTimes[sfxName] = Time.unscaledTime;

        //PlayOneShot: 하나의 오디오 소스에서 여러 효과음이 겹쳐서 나게 해줌
        sfxSource.PlayOneShot(clip, volum);
    }
}
