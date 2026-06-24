using UnityEngine;
using System.Collections;

public class LastBossMonster : MonsterBase
{
    [Header("보스 설정")]
    public float patternDelay = 1f;     // 패턴 종료 후 다음 패턴까지 대기 시간

    [Header("장판")]
    public GameObject warningPrefab;    // 경고 원 프리팹
    public GameObject explosionPrefab;  // 폭발 이펙트 프리팹

    [Header("궁극기")]
    public GameObject safeZonePrefab;       // 안전지대 프리팹
    public GameObject mapExplosionPrefab;   // 맵 전체 폭발 프리팹
    public float ultimateCastTime = 5f; //궁극기 시전시간


    [Header("접촉 데미지")] // 플레이어에게 줄 접촉 데미지
    public float touchDamage = 10f;
    public float touchDamageInterval = 1f;// 몇 초마다 데미지를 줄지
    private float touchDamageTimer;// 데미지 시간을 계산할 타이머

    private BossPhase currentPhase;     // 현재 보스 페이즈 저장

    private Coroutine patternRoutine;

    protected override void OnEnable()
    {
        base.OnEnable();

        // 맵 중앙에 보스 배치
        transform.position = Vector3.zero;
        currentState = MonsterState.Attack;
        // Rigidbody가 존재하면
        if (rb != null)
        {
            // 이동 속도 제거
            rb.linearVelocity = Vector2.zero;

            // 회전 속도 제거
            rb.angularVelocity = 0f;

            // 물리 힘을 받지 않도록
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 바로 데미지를 줄 수 있도록 타이머 초기화
        touchDamageTimer = touchDamageInterval;
        StartCoroutine(InitBoss());
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 플레이어가 아니면 종료
        if (!other.CompareTag("Player"))
            return;

        // 시간을 계속 누적
        touchDamageTimer += Time.deltaTime;

        // 설정한 시간이 지나면
        if (touchDamageTimer >= touchDamageInterval)
        {
            //0초부터 시작
            touchDamageTimer = 0f;

            // PlayerBase 가져오기
            PlayerBase playerBase = other.GetComponent<PlayerBase>();

            if (playerBase != null)// 플레이어가 존재하면
            {
                // 플레이어에게 데미지 주기
                playerBase.TakeDamage(touchDamage);

                Debug.Log("최종보스 접촉 데미지");
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        // 플레이어가 아니면 종료
        if (!other.CompareTag("Player"))
            return;

        // 타이머 초기화
        touchDamageTimer = touchDamageInterval;
    }

    IEnumerator InitBoss()
    {
        while (PoolManager.Instance == null)
            yield return null;

        currentPhase = BossPhase.Phase1;

        if (patternRoutine != null)
            StopCoroutine(patternRoutine);

        patternRoutine = StartCoroutine(PatternLoop());
    }

    protected override void Update()
    {
        base.Update(); // 부모 Update 실행

        CheckPhase(); // 체력에 따라 페이즈 변경
    }

    // 최종보스는 움직이지 않게 FixedUpdate를 막음
    protected override void FixedUpdate()
    {
       
    }
    protected override void UpdateState() { }

    protected override void AttackLogic() { }

    private void CheckPhase()
    {
        float hpPercent = currentHp / maxHp; // 현재 체력 비율 계산

        if (hpPercent <= 0.4f) // 체력 40% 이하
        {
            currentPhase = BossPhase.Phase3; // Phase3 진입
        }
        else if (hpPercent <= 0.7f) // 체력 70% 이하
        {
            currentPhase = BossPhase.Phase2; // Phase2 진입
        }
        else
        {
            currentPhase = BossPhase.Phase1; // 그 외는 Phase1
        }
    }

    //현재 페이즈 확인 -> 패턴 실행 -> 2초 휴식 -> 반복
    IEnumerator PatternLoop()
    {
        while (!isDead) //보스가 살아있는동안 반복
        {
            switch (currentPhase)
            {
                case BossPhase.Phase1:
                    yield return StartCoroutine(ShootPattern()); //슈팅
                    yield return StartCoroutine(AoEAttackPattern()); //장판
                    break;
                case BossPhase.Phase2:
                    yield return StartCoroutine(SummonPattern()); //소환
                    yield return StartCoroutine(BounceBulletPattern()); //튕기는 탄환
                    break;
                case BossPhase.Phase3:
                    yield return StartCoroutine(Phase3Pattern());
                    break;
            }
            yield return new WaitForSeconds(patternDelay); //패턴사이휴식
        }
    }


    ///////////////슈팅////////////////////
    IEnumerator ShootPattern()
    {
        for (int i = 0; i < 5; i++)
        {
            if (player == null) yield break; // 플레이어 없으면 종료

            // 플레이어 방향 계산
            Vector2 dir =
                ((Vector2)player.position - (Vector2)transform.position).normalized;

            // 풀에서 탄환 가져오기
            GameObject bullet = PoolManager.Instance.GetBossBullet();

            if (bullet == null)
            {
                Debug.LogError("BossBullet 생성 실패 (Pool or Prefab 문제)");
                yield break;
            }

            bullet.transform.position = transform.position; // 생성 위치

            // 탄환 컴포넌트 가져오기
            BossBullet bossBullet = bullet.GetComponent<BossBullet>();

            if (bossBullet != null)
            {
                bossBullet.Init(dir); // 방향 전달
            }

            yield return new WaitForSeconds(0.15f); // 발사 간격
        }
    }


    /////////////////////장판 패턴/////////////////////////

    IEnumerator AoEAttackPattern()
    {
        if (player == null) yield break;

        Vector3 targetPos = player.position; // 현재 플레이어 위치 저장

        GameObject warning =
        Instantiate(warningPrefab, targetPos, Quaternion.identity); // 경고 생성

        yield return new WaitForSeconds(2f); // 피할 시간 제공
        

        Instantiate(explosionPrefab, targetPos, Quaternion.identity); // 폭발
    }


    ///////////////////// 소환 패턴 ////////////////////////////

    IEnumerator SummonPattern()
    {
        // 3~5마리 랜덤 소환
        int count = Random.Range(3, 6);

        for (int i = 0; i < count; i++)
        {
            // 전사 몬스터를 풀에서 가져옴
            GameObject monster = PoolManager.Instance.GetWarriorMonster();

            // 풀에 없으면 스킵
            if (monster == null) continue;

            // 보스 기준 주변 랜덤 위치 생성
            Vector2 randomOffset = Random.insideUnitCircle * 3f;

            // 최종 스폰 위치 = 보스 위치 + 랜덤 오프셋
            Vector2 spawnPos = (Vector2)transform.position + randomOffset;

            // 위치 세팅
            monster.transform.position = spawnPos;
        }

        // 한 프레임 쉬기
        yield return new WaitForSeconds(0.5f);
    }

    ///////////////////// 벽 반사 탄환 ////////////////////////////

    IEnumerator BounceBulletPattern()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject bullet = PoolManager.Instance.GetBounceBullet();

            if (bullet == null) yield break;

            bullet.transform.position = transform.position;

            // 랜덤 방향 생성
            Vector2 dir = new Vector2(
                    Mathf.Cos(Random.Range(0f, 360f) * Mathf.Deg2Rad),
                    Mathf.Sin(Random.Range(0f, 360f) * Mathf.Deg2Rad));

            BounceBullet bounce = bullet.GetComponent<BounceBullet>();

            if (bounce != null)
            {
                bounce.Init(dir); // 방향 전달
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    ///////////////////// 전방위 발사 탄환 ////////////////////////////

    IEnumerator AllDirectionPattern()
    {
        int count = 24;

        for (int i = 0; i < count; i++)
        {
            GameObject bullet = PoolManager.Instance.GetBossBullet();

            if (bullet == null)
            {
                Debug.LogError("보스 탄환 풀이 비었습니다!");
                yield break;
            }

            bullet.transform.position = transform.position;

            float angle = i * (360f / count);

            Vector2 dir = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad));

            BossBullet bossBullet = bullet.GetComponent<BossBullet>();

            if (bossBullet != null)
            {
                bossBullet.Init(dir);
            }
        }

        yield break;
    }

    IEnumerator Phase3Pattern()
    {
        Debug.Log("궁극기 시전"); // 궁극기 시작 로그 출력

        // 맵 안의 랜덤 위치를 안전지대로 선택
        Vector2 safePos = Random.insideUnitCircle * 5f;

        // 선택된 위치에 안전지대 생성
        GameObject safeZone =
            Instantiate(safeZonePrefab, safePos, Quaternion.identity);

        float timer = 0f;              // 궁극기 진행 시간 측정
        float shootInterval = 1f;      // 탄환 발사 간격(1초)

        // 궁극기 시전 시간이 끝날 때까지 반복
        while (timer < ultimateCastTime)
        {
            // 전방위 탄환 발사
            yield return StartCoroutine(AllDirectionPattern());

            // 다음 탄환 발사까지 1초 대기
            yield return new WaitForSeconds(shootInterval);

            // 경과 시간 누적
            timer += shootInterval;
        }

        ///////////////////// 궁극기 ////////////////////////////
        //캐스팅 -> 안전지대 생성 -> 3초대기 -> 맵 전체 폭발

        // 궁극기 시전이 끝나면 맵 전체 폭발 생성
        GameObject explosion =
            Instantiate(mapExplosionPrefab, Vector3.zero, Quaternion.identity);

        // 폭발 이펙트가 보이도록 1초 대기
        yield return new WaitForSeconds(1f);

        // 안전지대 제거
        Destroy(safeZone);

        // 폭발 이펙트 제거
        Destroy(explosion);
    }
    
    //////////////////보스 사망/////////////////////

    protected override void Death()
    {
        isDead = true;                     // 사망 상태

        currentState = MonsterState.Dead; // 상태 변경

        StopAllCoroutines();              // 진행 중인 패턴 종료

        Debug.Log("최종보스 처치");

        StageManager stageManager = FindFirstObjectByType<StageManager>();

        if (stageManager != null)
        {
            stageManager.ClearStage();    // 스테이지 클리어
        }

        Destroy(gameObject);              // 보스 제거
    }

}
