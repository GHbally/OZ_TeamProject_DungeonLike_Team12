using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private int spawnWarriorNumber = 120;
    [SerializeField] private int spawnArcherNumber = 30;

    public WaveData[] waves; //웨이브 데이터 배열

    [Header("맵 안 스폰 범위")]
    public Vector2 spawnMin = new Vector2(-12f, -22f); // 잔디 영역 왼쪽 아래
    public Vector2 spawnMax = new Vector2(7.7f, -1.7f);   // 잔디 영역 오른쪽 위
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

    [Header("스테이지 안내 화살표UI")]
    
    [SerializeField] private StageGuideController stageGuideController;

    [Header("클리어 UI")]
    public WaveClaerUI wavestageClearUI;

    // 클리어 UI가 화면에 머무는 시간
    [SerializeField] private float waveClearImageDuration = 3f;
    [SerializeField] private float nextWaveDelay = 3f;
    [SerializeField] private float stageClearImageDuration = 3f;

    [Header("보상 상자 Prefab")]
    public GameObject rewardBoxPrefab; // 보상 상자 프리팹

    [Header("보스 소환 이펙트")]
    public GameObject bossSpawnEffectPrefab;

    [Header("몬스터 스폰 시간")]
    [SerializeField] private float spawnDelay = 0.05f;

    // 현재 스테이지 클리어 후 생성된 보상 상자를 저장한다.
    // 다음 스테이지로 넘어갈 때 이 오브젝트를 제거하기 위해 사용한다.
    private GameObject spawnedRewardBox;

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
                    warriorCount = spawnWarriorNumber, archerCount = spawnArcherNumber - 30,
                    hpMultiplier = 1f, speedMultiplier = 1f
                },
                new WaveData()
                {
                    warriorCount = spawnWarriorNumber, archerCount = spawnArcherNumber,
                    hpMultiplier= 1.2f, speedMultiplier = 1.05f
                },
                new WaveData()
                {
                    warriorCount = spawnWarriorNumber, archerCount = spawnArcherNumber,
                    hpMultiplier = 1.3f, speedMultiplier= 1.1f
                }
            };
        }
        else if (chapter == 1 && stage == 2)
        {
            waves = new WaveData[]
            {
                new WaveData()
                {
                    warriorCount = spawnWarriorNumber + 10, archerCount = spawnArcherNumber + 10,
                    hpMultiplier = 1.5f, speedMultiplier = 1.1f
                },

                new WaveData()
                {
                    warriorCount = spawnWarriorNumber + 10,archerCount = spawnArcherNumber + 10,
                    hpMultiplier = 1.7f,speedMultiplier = 1.15f
                },

                new WaveData()
                {
                    warriorCount = spawnWarriorNumber + 10, archerCount = spawnArcherNumber + 10,
                    hpMultiplier = 1.9f, speedMultiplier = 1.2f
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
            StartCoroutine(WaitBossInCamera());
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

        // 스폰할 몬스터 목록 생성
        List<MonsterType> spawnList = new List<MonsterType>();

        // 전사 몬스터 추가
        for (int i = 0; i < data.warriorCount; i++)
        {
            spawnList.Add(MonsterType.Warrior);
        }

        // 궁수 몬스터 추가
        for (int i = 0; i < data.archerCount; i++)
        {
            spawnList.Add(MonsterType.Archer);
        }

        // 랜덤 섞기
        for (int i = 0; i < spawnList.Count; i++)
        {
            int randomIndex = Random.Range(i, spawnList.Count);

            MonsterType temp = spawnList[i];
            spawnList[i] = spawnList[randomIndex];
            spawnList[randomIndex] = temp;
        }

        // 랜덤 순서대로 생성
        foreach (MonsterType monsterType in spawnList)
        {
            if (monsterType == MonsterType.Warrior)
            {
                SpawnWarrior(data);
            }
            else
            {
                SpawnArcher(data);
            }

            yield return new WaitForSeconds(spawnDelay);
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

        // 보스 중복 생성 방지
        isLastBossSpawned = true;

        // 보스가 생성될 위치 저장
        Vector3 bossSpawnPos = new Vector3(-7f, -2f, 0f);

        // 이펙트 재생 시작
        StartCoroutine(BossSpawnEffectCo(bossSpawnPos));
    }
    

    /////////////////////////////보스 소환 이펙트//////////////////////////
    private IEnumerator BossSpawnEffectCo(Vector3 spawnPos)
    {
        // 이펙트 프리팹이 연결되어 있으면
        if (bossSpawnEffectPrefab != null)
        {
            // 이펙트 생성
            GameObject effect = Instantiate(bossSpawnEffectPrefab, spawnPos, Quaternion.identity);

            // 생성된 위치를 보스 위치로 맞춤
            effect.transform.position = spawnPos;

            // 크기를 2배로 설정
            effect.transform.localScale = Vector3.one * 5f;

            // 이펙트 안의 모든 SpriteRenderer 가져오기
            SpriteRenderer[] effectRenderers =  effect.GetComponentsInChildren<SpriteRenderer>();

            // 모든 SpriteRenderer 설정
            foreach (SpriteRenderer renderer in effectRenderers)
            {
                if (renderer != null)
                {
                    // 다른 오브젝트보다 앞에 보이게 설정
                    renderer.sortingOrder = 100;

                    // 투명하지 않게 설정
                    renderer.color = Color.white;
                }
            }

            // 테스트용으로 3초 동안 보이게 함
            yield return new WaitForSeconds(3f);

            // 이펙트 삭제
            Destroy(effect);
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

        // 이펙트가 끝난 뒤 보스 생성
        GameObject boss = Instantiate(lastBossPrefab,spawnPos,Quaternion.identity);

        // 보스가 완전히 나타난 뒤 움직이고 공격하게 함
        StartCoroutine(FadeInBossCo(boss));

    }


    /////////////////보스가 서서히 나타나게 하는 코드///////////////
    private IEnumerator FadeInBossCo(GameObject boss)
    {
        // 보스가 없으면 종료
        if (boss == null)
        {
            yield break;
        }

        // 보스 스프라이트 전부 가져오기
        SpriteRenderer[] renderers = boss.GetComponentsInChildren<SpriteRenderer>();

        // 보스 콜라이더 전부 가져오기
        Collider2D[] colliders = boss.GetComponentsInChildren<Collider2D>();

        // 보스 Rigidbody 가져오기
        Rigidbody2D rb = boss.GetComponent<Rigidbody2D>();

        // 원래 Rigidbody 제약 저장
        RigidbodyConstraints2D originalConstraints = RigidbodyConstraints2D.None;

        // Rigidbody가 있으면 등장 중 움직임 고정
        if (rb != null)
        {
            originalConstraints = rb.constraints;
            rb.linearVelocity = Vector2.zero;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        // 등장 중에는 충돌/접촉 데미지 방지
        foreach (Collider2D col in colliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        // 처음에는 완전히 투명하게 설정
        foreach (SpriteRenderer sr in renderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 0f;
                sr.color = color;
            }
        }

        // 페이드 인 시간
        float fadeTime = 1.5f;

        // 흐른 시간
        float elapsed = 0f;

        // 1.5초 동안 서서히 나타나기
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;

            float alpha = elapsed / fadeTime;

            foreach (SpriteRenderer sr in renderers)
            {
                if (sr != null)
                {
                    Color color = sr.color;
                    color.a = alpha;
                    sr.color = color;
                }
            }

            yield return null;
        }

        // 완전히 보이게 고정
        foreach (SpriteRenderer sr in renderers)
        {
            if (sr != null)
            {
                Color color = sr.color;
                color.a = 1f;
                sr.color = color;
            }
        }

        // Rigidbody 원래대로 복구
        if (rb != null)
        {
            rb.constraints = originalConstraints;
        }

        // 콜라이더 다시 켜기
        foreach (Collider2D col in colliders)
        {
            if (col != null)
            {
                col.enabled = true;
            }
        }

        // LastBossMonster 가져오기
        LastBossMonster bossMonster = boss.GetComponent<LastBossMonster>();

        // 페이드가 끝난 뒤에만 보스 패턴 시작
        if (bossMonster != null)
        {
            bossMonster.StartBossPattern();
        }
    }

    // 보스가 카메라에 들어올 때까지 기다리는 코루틴
    private IEnumerator WaitBossInCamera()
    {
        // 보스가 생성될 위치
        Vector3 bossPos = new Vector3(-7f, -2f, 0f);

        // 메인 카메라 가져오기
        Camera cam = Camera.main;

        // 계속 검사
        while (true)
        {
            // 월드 좌표를 화면 좌표로 변환
            Vector3 viewPos = cam.WorldToViewportPoint(bossPos);

            // 화면 안에 있는지 검사
            bool isVisible =
                viewPos.z > 0 &&  // 카메라 앞에 있는가
                viewPos.x >= 0f &&  // 화면 왼쪽보다 오른쪽인가
                viewPos.x <= 1f &&  // 화면 오른쪽보다 왼쪽인가
                viewPos.y >= 0f &&  // 화면 아래보다 위인가
                viewPos.y <= 1f;  // 화면 위보다 아래인가

            // 화면 안에 들어왔다면
            if (isVisible)
            {
                SpawnLastBoss();// 보스 생성

                yield break;// 더 이상 검사하지 않고 종료
            }

            // 다음 프레임까지 기다림
            yield return null;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        Camera cam = Camera.main;

        for (int i = 0; i < 100; i++)
        {
            float randomX = Random.Range(spawnMin.x, spawnMax.x);
            float randomY = Random.Range(spawnMin.y, spawnMax.y);
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

        // 아직 몬스터 남아있음
        if (aliveMonster > 0)
            return;

        // 다음 웨이브
        currentWave++;

        // 모든 웨이브 클리어
        if (currentWave >= waves.Length)
        {
            // 클리어 코드
            // 여기 있던 코드를 함수로 옮겨놨습니다
            StartCoroutine(StageClearFlow(deadPosition));

            return;
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

    public void RemoveSpawnedRewardBox()
    {
        // 저장된 보상 상자가 없으면 제거할 것이 없으므로 종료한다.
        if (spawnedRewardBox == null)
        {
            return;
        }

        // Instantiate로 만든 보상 상자이므로 Destroy로 제거한다.
        Destroy(spawnedRewardBox);

        // 이미 제거한 오브젝트를 다시 참조하지 않도록 null로 초기화한다.
        spawnedRewardBox = null;
    }

    private IEnumerator StageClearFlow(Vector3 deadPosition)
    {
        // 스테이지 클리어 UI를 켠다.
        if (wavestageClearUI != null)
        {
            wavestageClearUI.ShowStageClear();
        }

        // 스테이지 클리어 UI를 n초 동안 보여준다.
        yield return new WaitForSeconds(stageClearImageDuration);

        if (wavestageClearUI != null)
        {
            wavestageClearUI.HideStageClear();
        }

        if (rewardBoxPrefab != null)
        {
            spawnedRewardBox = Instantiate(rewardBoxPrefab, deadPosition, Quaternion.identity);

            // 스테이지 클리어 후에는 안내 화살표가 보상상자를 가리키게 한다.
            if (stageGuideController != null)
            {
                stageGuideController.ShowRewardBox(spawnedRewardBox.transform);
            }
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
    }
}

