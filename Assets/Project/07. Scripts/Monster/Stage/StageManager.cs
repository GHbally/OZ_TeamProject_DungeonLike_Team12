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
        if (chapter == 1 && stage == 3)
        {
            Debug.Log("게임 클리어");
            return;
        }
        stage++;
        waveManager.StartStage(chapter, stage);
    }
}
