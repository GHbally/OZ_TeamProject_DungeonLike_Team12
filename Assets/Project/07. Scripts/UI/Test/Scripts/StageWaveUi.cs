using TMPro;
using UnityEngine;

public class StageWaveUI : MonoBehaviour
{
    [Header("UI 텍스트")]
    public TMP_Text stageText;      // 현재 스테이지 표시
    public TMP_Text waveText;       // 현재 웨이브 표시
    public TMP_Text monsterText;    // 남은 몬스터 수 표시

    // 스테이지 표시 갱신
    public void UpdateStage(int chapter, int stage)
    {
        stageText.text = $"{chapter}-{stage}";
    }

    // 웨이브 표시 갱신
    public void UpdateWave(int currentWave, int totalWave)
    {
        waveText.text = $"{currentWave} / {totalWave}";
    }

    // 남은 몬스터 수 표시 갱신
    public void UpdateMonsterCount(int aliveMonster)
    {
        monsterText.text = $"{aliveMonster}";
    }
}
