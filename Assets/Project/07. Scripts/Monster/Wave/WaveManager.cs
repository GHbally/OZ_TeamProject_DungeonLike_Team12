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
    private bool isLastBossSpawned = false; // true면 보스를 다시 생성하지 않음

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
    public GameObject lastBossPrefab;

    ////////스테이지 UI추가/////////////
    [Header("UI")]
    public StageWaveUI stageWaveUI;


    [Header("클리어 UI")]
    public WaveClaerUI wavestageClearUI;

    // 클리어 UI가 화면에 머무는 시간
    [SerializeField] private float waveClearImageDuration = 3f;
    [SerializeField] private float nextWaveDelay = 3f;
    [SerializeField] private float stageClearImageDuration = 3f;


    // 스테이지별 웨이브 데이터 생성
    void CreateStageData(int chapter, int stage)
    {
        // 1-1
        if (chapter == 1 && stage == 1)
        {
            waves = new WaveData[]
            {
                new WaveData()
                {
                    warriorCount = 65, archerCount = 15,
                    hpMultiplier = 1f, speedMultiplier = 1f
                },
                new WaveData()
                {
                    warriorCount = 65, archerCount = 15,
                    hpMultiplier= 1.1f, speedMultiplier = 1.05f
                },
                new WaveData()
                {
                    warriorCount = 65, archerCount = 15,
                    hpMultiplier = 1.2f, speedMultiplier= 1.1f
                }
            };
        }
        else if (chapter == 1 && stage == 2)
        {
            waves = new WaveData[]
            {
                new WaveData()
                {
                    warriorCount = 75, archerCount = 25,
                    hpMultiplier = 1.2f, speedMultiplier = 1.1f
                },

                new WaveData()
                {
                    warriorCount = 75,archerCount = 25,
                    hpMultiplier = 1.3f,speedMultiplier = 1.15f
                },

                new WaveData()
                {
                    warriorCount = 75, archerCount = 25,
                    hpMultiplier = 1.4f, speedMultiplier = 1.2f
                }
            };
        }

        // 1-3 최종보스
        else if (chapter == 1 && stage == 3)
        {
            waves = new WaveData[]
            {
                new WaveData(){ warriorCount = 0, archerCount = 0,}
            };
        }
    }



    // 스테이지 시작
    public void StartStage(int chapter, int stage)
    {
        Debug.Log($"[STAGE START] {chapter}-{stage}");

        ////////스테이지 UI추가/////////////
        if (stageWaveUI != null)
        {
            stageWaveUI.UpdateStage(chapter, stage);
        }

        if (player == null)
        {
            //플레이어 위치를 변수에 저장
            player = GameObject.FindGameObjectWithTag("Player") .transform;
        }
        //이전 코루틴 종료
        StopAllCoroutines();

        // 첫 웨이브 부터 시작
        currentWave = 0;

        // 새 스테이지가 시작될 때 최종보스 생성 여부 초기화
        isLastBossSpawned = false;

        // 웨이브 데이터 생성
        CreateStageData(chapter, stage);

        // 1-3 최종보스
        if (chapter == 1 && stage == 3)
        {
            aliveMonster = 1;

            /////////스테이지 UI추가//////////
            if (stageWaveUI != null)
            {
                stageWaveUI.UpdateWave(1, 1);
                stageWaveUI.UpdateMonsterCount(aliveMonster);
                stageWaveUI.UpdateMonsterCount(aliveMonster);
            }
            SpawnLastBoss();
            return;
        }
        //웨이브 시작
        StartCoroutine(StartWave());

    }


    /********************
    1-1 ~ 1-2 -> 일반 웨이브 진행
    1-3 -> 최종보스 생성
    보스 처치 -> 게임 클리어
    **********************/
    //웨이브 시작
    IEnumerator StartWave()
    {
        // 현재 웨이브의 정보를 가져오기
        WaveData data = waves[currentWave];
        // 살아있는 몬스터의 수를 계산
        aliveMonster = data.warriorCount + data.archerCount;

        ////////스테이지 UI추가/////////////
        if (stageWaveUI != null)
        {
            stageWaveUI.UpdateWave(currentWave + 1, waves.Length);
            stageWaveUI.UpdateMonsterCount(aliveMonster);
        }

        //전사 몬스터 생성
        for (int i = 0; i < data.warriorCount; i++)
        {
            SpawnWarrior(data);

            yield return new WaitForSeconds(0.2f);
        }


        // 궁수 몬스터 생성
        for (int i = 0; i < data.archerCount; i++)
        {
            SpawnArcher(data);

            yield return new WaitForSeconds(0.2f);
        }
    }

    //전사 생성
    void SpawnWarrior(WaveData data)
    {
        GameObject monster = PoolManager.Instance.GetWarriorMonster();

        if (monster == null)
            return;

        monster.transform.position = GetRandomSpawnPosition();

        MonsterBase monsterBase = monster.GetComponent<MonsterBase>();

        if (monsterBase != null)
        {
            monsterBase.ApplyStageMultiplier
            (
                data.hpMultiplier,
                data.speedMultiplier
            );
        }
    }


    // 궁수 생성
    void SpawnArcher(WaveData data)
    {
        GameObject monster = PoolManager.Instance.GetArcherMonster();

        if (monster == null)
            return;

        monster.transform.position = GetRandomSpawnPosition();

        MonsterBase monsterBase =
        monster.GetComponent<MonsterBase>();

        if (monsterBase != null)
        {
            monsterBase.ApplyStageMultiplier
            (
                data.hpMultiplier,
                data.speedMultiplier
            );
        }
    }


    // 최종보스 생성 함수
    public void SpawnLastBoss()
    {
        // 이미 최종보스가 생성된 상태라면 다시 생성하지 않음
        if (isLastBossSpawned)
        {
            return;
        }

        // 최종보스 프리팹이 연결되어 있지 않으면 생성하지 않음
        if (lastBossPrefab == null)
        {
            Debug.LogError("Last Boss Prefab이 연결되지 않았습니다.");
            return;
        }

        // 최종보스 생성 완료 표시
        isLastBossSpawned = true;

        // 최종보스를 지정한 위치에 1마리만 생성
        Instantiate(lastBossPrefab, new Vector3(-4.4f, -9.7f, 0f), Quaternion.identity);

        // 콘솔 확인용 로그
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


    public GameObject rewardBoxPrefab; // 보상 상자 프리팹


    /******************
    몬스터 사망 -> aliveMonster 감소
    0이 되면 -> 다음 웨이브 
    마지막 웨이브면 -> 스테이지 클리어
     *******************/
    //몬스터 사망시 호출
    public void MonsterDead(Vector3 deadPosition)
    {
        aliveMonster--;

        ///// 스테이지 UI추가/////////
        if (stageWaveUI != null)
        {
            stageWaveUI.UpdateMonsterCount(aliveMonster);
        }

        Debug.Log("몬스터 죽음 / 남은 수: " + aliveMonster);
        // 아직 몬스터 남아있음
        if (aliveMonster > 0)
            return;

        // 다음 웨이브
        currentWave++;

        // 모든 웨이브 클리어
        if (currentWave >= waves.Length)
        {
            // 스테이지 클리어 UI를 켠다.
            if (wavestageClearUI != null)
            {
                wavestageClearUI.ShowStageClear();
            }

            // 스테이지 클리어 UI를 n초 동안 보여준다.
            new WaitForSeconds(stageClearImageDuration);

            if (wavestageClearUI != null)
            {
                wavestageClearUI.HideStageClear();
            }

            Debug.Log("스테이지 클리어");
            Debug.Log("상자 생성!");
            Debug.Log(deadPosition); //마지막 몬스터가 죽은 위치 확인용

            if (rewardBoxPrefab != null)
            {
                Instantiate(rewardBoxPrefab, deadPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogError("RewardBoxPrefab이 WaveManager에 연결되지 않았습니다.");
            }


            // 추가 : 씬에서 StageManager 찾기
            StageManager stageManager = FindFirstObjectByType<StageManager>();

            // 추가 :  StageManager가 있으면 ClearStage 실행
            if (stageManager != null)
            {
                stageManager.ClearStage(); // 포탈 활성화
            }
            else
            {
                Debug.LogError("StageManager를 찾지 못했습니다."); // StageManager 미존재 오류
            }

            return; // 스테이지 클리어 처리가 끝났으므로 종료
        }
        StartCoroutine(WaveClearFlow());
    }

    private IEnumerator WaveClearFlow()
    {
        // 웨이브 클리어 이미지를 켠다.
        if (wavestageClearUI != null)
        {
            wavestageClearUI.ShowWaveClear();
        }

        // 웨이브 클리어 이미지를 n초 동안 보여준다.
        yield return new WaitForSeconds(waveClearImageDuration);

        // 웨이브 클리어 이미지를 끈다.
        if (wavestageClearUI != null)
        {
            wavestageClearUI.HideWaveClear();
        }

        // 이미지가 사라진 뒤 n초 기다린다.
        yield return new WaitForSeconds(nextWaveDelay);

        // 다음 웨이브를 시작한다.
        StartCoroutine(StartWave());
    }
}

