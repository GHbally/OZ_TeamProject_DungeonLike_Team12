using Unity.VisualScripting;
using UnityEngine;

public class StageMapSwitcher : MonoBehaviour
{
    [Header("스테이지 맵 목록")]
    [SerializeField] private GameObject[] stageMaps;

    private void Start()
    {
        // 게임 시작 시 1번 스테이지 맵만 켠다.
        ChangeMap(1);
    }

    public void ChangeMap(int stage)
    {
        // 스테이지 번호는 1부터 시작하지만,
        // 배열 인덱스는 0부터 시작하므로 -1을 해준다.
        int targetIndex = stage - 1;

        // 잘못된 스테이지 번호가 들어오면 실행하지 않는다.
        if (targetIndex < 0 || targetIndex >= stageMaps.Length)
        {
            Debug.LogError(
                $"StageMapSwitcher: stage {stage}에 해당하는 맵이 없습니다.",
                gameObject
            );

            return;
        }

        // 모든 맵을 검사한다.
        for (int i = 0; i < stageMaps.Length; i++)
        {
            // 배열에 빈 칸이 있으면 건너뛴다.
            if (stageMaps[i] == null)
            {
                continue;
            }

            // 현재 스테이지에 해당하는 맵만 켜고,
            // 나머지 맵은 전부 끈다.
            stageMaps[i].SetActive(i == targetIndex);
        }

        Debug.Log($"현재 맵 변경 완료: Stage {stage}", gameObject);

        //BGM
        if (SoundManager.Instance != null)
        {
            switch (stage)
            {
                case 1: //1 스테이지일 때
                    SoundManager.Instance.PlayBGM("A Great Journey - Overworld", 0.5f);
                    break;

                case 2: //2 스테이지일 때
                    SoundManager.Instance.PlayBGM("Gearing Up - Battle", 0.5f);
                    break;

                case 3: //3 스테이지(예: 보스 방)일 때
                    SoundManager.Instance.PlayBGM("The Beast's Lair - Boss Fight", 0.6f);
                    break;

                default: //지정되지 않은 번호는 안전하게 기본 맵 음악으로 연주
                    SoundManager.Instance.PlayBGM("A Great Journey - Overworld", 0.5f);
                    break;
            }
        }
    }
}
