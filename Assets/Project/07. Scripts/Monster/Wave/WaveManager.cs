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

    [Header("랜덤 스폰")]
    public Transform player;        // 플레이어 위치

    public float minSpawnRadius = 12f; // 최소 생성 거리
    public float maxSpawnRadius = 18f; // 최대 생성 거리

    [Header("생성 제한")]
    public LayerMask spawnBlockLayer; // 장애물 + 몬스터

    public float checkRadius = 0.7f;  // 생성 가능 여부 검사 반경

    [Header("보스 프리팹")]
    public GameObject midBossPrefab;
    public GameObject lastBossPrefab;

    // 스테이지별 웨이브 데이터 생성
    void CreateStageData(int chapter, int stage)
    {
        // 1-1
        if (chapter == 1 && stage == 1)
        {
            waves = new WaveData[]
            {
            new WaveData(){ warriorCount = 3, archerCount = 0 },
            new WaveData(){ warriorCount = 5, archerCount = 1 },
            new WaveData(){ warriorCount = 7, archerCount = 2 }
            };
        }

        // 1-2
        else if (chapter == 1 && stage == 2)
        {
            waves = new WaveData[]
            {
            new WaveData(){ warriorCount = 5, archerCount = 1 },
            new WaveData(){ warriorCount = 7, archerCount = 2 },
            new WaveData(){ warriorCount = 10, archerCount = 3 }
            };
        }

        // 1-3
        else if (chapter == 1 && stage == 3)
        {
            waves = new WaveData[]
            {
            new WaveData(){ warriorCount = 8, archerCount = 2 },
            new WaveData(){ warriorCount = 10, archerCount = 3 },
            new WaveData(){ warriorCount = 12, archerCount = 4 }
            };
        }

        // 1-4
        else if (chapter == 1 && stage == 4)
        {
            waves = new WaveData[]
            {
            new WaveData(){ warriorCount = 10, archerCount = 3 },
            new WaveData(){ warriorCount = 12, archerCount = 4 },
            new WaveData(){ warriorCount = 15, archerCount = 5 }
            };
        }

        // 1-5 (중간 보스)
        else if (chapter == 1 && stage == 5)
        {
            waves = new WaveData[]
            {
            new WaveData()
            {
                warriorCount = 0,
                archerCount = 0
            }
            };
        }
    }



    // 스테이지 시작
    public void StartStage(int chapter, int stage)
    {
        if (player == null)
        {
            //플레이어 위치를 변수에 저장
            player = GameObject.FindGameObjectWithTag("Player") .transform;
        }
        //이전 코루틴 종료
        StopAllCoroutines();
        // 첫 웨이브 부터 시작
        currentWave = 0;
        // 웨이브 데이터 생성
        CreateStageData(chapter, stage);
        //웨이브 시작
        StartCoroutine(StartWave());
    }


    /********************
    1-1 ~ 1-4 -> 일반 웨이브 진행
    1-5 -> 중간보스 생성
    aliveMonster = 1
    보스 처치 -> aliveMonster = 0 -> 스테이지 클리어
     **********************/
    //웨이브 시작
    IEnumerator StartWave()
    {
        StageManager stageManager = FindFirstObjectByType<StageManager>();

        // 1-5 보스 스테이지
        if (stageManager.chapter == 1 && stageManager.stage == 5)
        {
            SpawnMidBoss();

            // 보스 1마리 살아있음
            aliveMonster = 1;

            yield break;
        }

        // 현재 웨이브의 정보를 가져오기
        WaveData data = waves[currentWave];
        // 살아있는 몬스터의 수를 계산
        aliveMonster = data.warriorCount + data.archerCount;

        //전사 몬스터 생성
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

    //중간 보스 생성
    void SpawnMidBoss()
    {
        if (midBossPrefab == null) return;

        Instantiate(midBossPrefab,GetRandomSpawnPosition(), Quaternion.identity);

        Debug.Log("중간보스 등장");
    }

    // 플레이어 주변 랜덤 위치 생성
    Vector3 GetRandomSpawnPosition()
    {
        // 최대 20번 시도
        for (int i = 0; i < 20; i++)
        {
            // 랜덤 방향
            Vector2 randomDir = Random.insideUnitCircle.normalized;

            // 랜덤 거리
            float distance =
                Random.Range(minSpawnRadius,maxSpawnRadius);

            // 최종 생성 위치
            Vector2 spawnPos =(Vector2)player.position +randomDir * distance;

            // 장애물 또는 몬스터 검사
            bool blocked =Physics2D.OverlapCircle(
                    spawnPos,
                    checkRadius,
                    spawnBlockLayer
                );

            // 비어있는 위치 발견
            if (!blocked)
            {
                return spawnPos;
            }
        }

        // 실패 시 플레이어 근처 반환
        return player.position;
    }



    /******************
    몬스터 사망 -> aliveMonster 감소
    0이 되면 -> 다음 웨이브 
    마지막 웨이브면 -> 스테이지 클리어
     *******************/
    //몬스터 사망시 호출
    public void MonsterDead()
    {
        aliveMonster--;

        // 아직 몬스터 남아있음
        if (aliveMonster > 0)
            return;

        // 다음 웨이브
        currentWave++;

        // 모든 웨이브 클리어
        if (currentWave >= waves.Length)
        {
            Debug.Log("스테이지 클리어");
            FindFirstObjectByType<StageManager>().ClearStage();
            return;
        }

        StartCoroutine(StartWave());
    }
}

