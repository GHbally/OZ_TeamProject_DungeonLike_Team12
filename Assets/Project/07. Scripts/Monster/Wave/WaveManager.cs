using System.Collections;
using UnityEngine;


/****************************
게임 시작
↓
웨이브1 생성
↓
몬스터 전부 처치
↓
웨이브2 생성
↓
몬스터 전부 처치
↓
웨이브3 생성
↓
몬스터 전부 처치
↓
스테이지 클리어
↓
다음 스테이지 시작
 *****************************/
public class WaveManager : MonoBehaviour
{

    private int currentWave; // 현재 웨이브 번호
    private int aliveMonster; //살아있는 몬스터 수

    public WaveData[] waves; //웨이브 데이터 배열

    [Header("스폰 위치")]
    public Transform[] spawnPoints;

    // 스테이지별 웨이브 데이터 생성
    void CreateStageData(int chapter, int stage)
    {
        // 챕터1 스테이지 1
        if (chapter == 1 && stage == 1)
        {
            waves = new WaveData[]
            {
            new WaveData()
            {
                warriorCount = 3,
                archerCount = 0
            },

            new WaveData()
            {
                warriorCount = 5,
                archerCount = 1
            },

            new WaveData()
            {
                warriorCount = 7,
                archerCount = 2
            }
            };
        }
    }


    
    // 스테이지 시작
    public void StartStage(int chapter, int stage)
    {
        //이전 코루틴 종료
        StopAllCoroutines();
        // 첫 웨이브 부터 시작
        currentWave = 0;
        // 웨이브 데이터 생성
        CreateStageData(chapter, stage);
        //웨이브 시작
        StartCoroutine(StartWave());
    }

    //웨이브 시작
    IEnumerator StartWave()
    {
        // 현재 웨이브의 정보를 가져오기
        WaveData data = waves[currentWave];
        // 살아있는 몬스터의 수를 계산
        aliveMonster = data.warriorCount + data.archerCount;

        for (int i = 0; i < data.warriorCount; i++)
        {
            SpawnWarrior();

            yield return new WaitForSeconds(0.2f);
        }


        // 궁수 몬스터 생성
        for (int i = 0; i < data.archerCount; i++)
        {
            SpawnArcher();

            yield return new WaitForSeconds(0.2f);
        }
    }

    //전사 생성
    void SpawnWarrior()
    {
        GameObject monster = PoolManager.Instance.GetWarriorMonster();

        if (monster == null)
            return;

        monster.transform.position = GetRandomSpawnPosition();
    }


    // 궁수 생성
    void SpawnArcher()
    {
        GameObject monster = PoolManager.Instance.GetArcherMonster();

        if (monster == null)
            return;

        monster.transform.position = GetRandomSpawnPosition();
    }


    // 랜덤 스폰 위치 반환
    Vector3 GetRandomSpawnPosition()
    {
        int index = Random.Range(0, spawnPoints.Length);

        return spawnPoints[index].position;
    }

    //몬스터 사망시 호출
    public void MonsterDead()
    {
        //몬스터 감소
        aliveMonster--;

        if (aliveMonster <= 0)
        {
            currentWave++;

            // 아직 몬스터가 남아 있다면 리턴
            if (aliveMonster > 0)
                return;

            // 다음 웨이브 이동
            currentWave++;

            // 모든 웨이브 클리어
            if (currentWave >= waves.Length)
            {
                Debug.Log("스테이지 클리어");

                FindFirstObjectByType<StageManager>()
                    .ClearStage();

                return;
            }

            // 다음 웨이브 시작
            StartCoroutine(StartWave());
        }
    }
}
