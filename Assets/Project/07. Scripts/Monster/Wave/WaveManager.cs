using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public MonsterSpawner spawner;
    private int currentWave; // 현재 웨이브 번호
    private int aliveMonsters; //살아있는 몬스터 수

    public WaveData[] waves;

    public void StartStage(int chapter, int stage)
    {
        currentWave = 0;
        CreateStageData(chapter, stage);
        StartCoroutine(StartWave());
    }
    IEnumerator StartWave()
    {
        WaveData data = waves[currentWave];

        aliveMonster = data.warriorCount + data.archerCount;

        spawner.SpawnWave(data);
        yield return null;

    }
    public void MonsterDead()
    {
        aliveMonsters--;

        if (aliveMonsters <= 0)
        {
            Debug.Log("스테이지 클리어");
            FindFirstObjectByType<StageManager>().ClearStage();
        }
        else
        {
            StartCoroutine(StartWave());
        }
    }
}
