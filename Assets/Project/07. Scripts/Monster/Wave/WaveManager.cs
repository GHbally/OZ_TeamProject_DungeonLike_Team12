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

    [Header("맵 안 스폰 범위")]
    public Vector2 spawnMin = new Vector2(-18f, -24f); // 잔디 영역 왼쪽 아래
    public Vector2 spawnMax = new Vector2(10f, 3f);   // 잔디 영역 오른쪽 위
    public Transform player;        // 플레이어 위치

    //[Header("랜덤 스폰")]
    //public Transform player;        // 플레이어 위치

    //public float minSpawnRadius = 12f; // 최소 생성 거리
    //public float maxSpawnRadius = 18f; // 최대 생성 거리

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

        // 2-1
        if (chapter == 2 && stage == 1)
        {
            waves = new WaveData[]
            {
               new WaveData(){ warriorCount = 3, archerCount = 0 },
                new WaveData(){ warriorCount = 5, archerCount = 1 },
                new WaveData(){ warriorCount = 7, archerCount = 2 }
            };
        }

        // 2-2
        else if (chapter == 2 && stage == 2)
        {
            waves = new WaveData[]
            {
                new WaveData(){ warriorCount = 5, archerCount = 1 },
                new WaveData(){ warriorCount = 7, archerCount = 2 },
                new WaveData(){ warriorCount = 10, archerCount = 3 }
            };
        }

        // 2-3
        else if (chapter == 2 && stage == 3)
        {
            waves = new WaveData[]
            {
                new WaveData(){ warriorCount = 8, archerCount = 2 },
                new WaveData(){ warriorCount = 10, archerCount = 3 },
                new WaveData(){ warriorCount = 12, archerCount = 4 }
            };
        }

        // 2-4
        else if (chapter == 2 && stage == 4)
        {
            waves = new WaveData[]
            {
                new WaveData(){ warriorCount = 10, archerCount = 3 },
                new WaveData(){ warriorCount = 12, archerCount = 4 },
                new WaveData(){ warriorCount = 15, archerCount = 5 }
            };
        }

        //2-5 최종 보스
        else if (chapter == 2 && stage == 5)
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
        Debug.Log($"[STAGE START] {chapter}-{stage}");
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

        // 1-5 중간보스
        if (chapter == 1 && stage == 5)
        {
            aliveMonster = 1;
            SpawnMidBoss();
            return;
        }

        // 2-5 최종보스
        if (chapter == 2 && stage == 5)
        {
            aliveMonster = 1;
            SpawnLastBoss();
            return;
        }
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
    public void SpawnMidBoss()
    {
        if (midBossPrefab == null) return;

        Instantiate(midBossPrefab,GetRandomSpawnPosition(), Quaternion.identity);

        Debug.Log("중간보스 등장");
    }

    //최종 보스 생성
    public void SpawnLastBoss()
    {
        if(lastBossPrefab == null) return;

        Instantiate(lastBossPrefab, GetRandomSpawnPosition(), Quaternion.identity);

        Debug.Log("최종보스 등장");
    }

    Vector3 GetRandomSpawnPosition()
    {
        Debug.Log("Player Pos : " + player.position);
        Camera cam = Camera.main;

        for (int i = 0; i < 100; i++)
        {
            float randomX = Random.Range(spawnMin.x, spawnMax.x);
            float randomY = Random.Range(spawnMin.y, spawnMax.y);
            Debug.Log($"Spawn Test : {randomX}, {randomY}");
            Vector2 spawnPos = new Vector2(randomX, randomY);

            if (cam != null)
            {
                Vector3 viewPos = cam.WorldToViewportPoint(spawnPos);

                bool isInCamera =
                    viewPos.x >= 0f && viewPos.x <= 1f &&
                    viewPos.y >= 0f && viewPos.y <= 1f &&
                    viewPos.z > 0f;

                if (isInCamera)
                    continue;
            }

            bool blocked = Physics2D.OverlapCircle(
                spawnPos,
                checkRadius,
                spawnBlockLayer
            );

            if (!blocked)
            {
                return spawnPos;
            }
        }

        Debug.LogWarning("카메라 밖 스폰 위치를 찾지 못했습니다.");

        return player.position + Vector3.right * 10f;
    }

    // 플레이어 주변 랜덤 위치 생성
    //Vector3 GetRandomSpawnPosition()
    //{
    //    // 최대 20번 시도
    //    for (int i = 0; i < 20; i++)
    //    {
    //        // 랜덤 방향
    //        Vector2 randomDir = Random.insideUnitCircle.normalized;

    //        // 랜덤 거리
    //        float distance =
    //            Random.Range(minSpawnRadius,maxSpawnRadius);

    //        // 최종 생성 위치
    //        Vector2 spawnPos =(Vector2)player.position +randomDir * distance;

    //        // 장애물 또는 몬스터 검사
    //        bool blocked =Physics2D.OverlapCircle(
    //                spawnPos,
    //                checkRadius,
    //                spawnBlockLayer
    //            );

    //        // 비어있는 위치 발견
    //        if (!blocked)
    //        {
    //            return spawnPos;
    //        }
    //    }

    //    // 실패 시 플레이어 근처 반환
    //    return player.position;
    //}



    /******************
    몬스터 사망 -> aliveMonster 감소
    0이 되면 -> 다음 웨이브 
    마지막 웨이브면 -> 스테이지 클리어
     *******************/
    //몬스터 사망시 호출
    public void MonsterDead()
    {
        aliveMonster--;
        Debug.Log("몬스터 죽음 / 남은 수: " + aliveMonster);
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

