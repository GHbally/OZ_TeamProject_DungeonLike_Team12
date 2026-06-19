using UnityEngine;

public class StageManager : MonoBehaviour
{
    public int chapter = 1; //현재 챕터
    public int stage = 1; //현재 스테이지

    public WaveManager waveManager;

    private void Start()
    {
        waveManager.StartStage(chapter, stage);
    }
    public void ClearStage()
    {
        stage++;

        if (stage > 5)
        {
            chapter++;
            stage = 1;
        }

        waveManager.StartStage(chapter, stage);
    }
}
