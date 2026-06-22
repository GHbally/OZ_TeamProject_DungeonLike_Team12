using UnityEngine;
using System.Collections;

public class LastBossMonster : MonsterBase
{
    [Header("보스 설정")]
    public float patternDelay = 2f;     // 패턴 종료 후 다음 패턴까지 대기 시간

    [Header("장판")]
    public GameObject warningPrefab;    // 경고 원 프리팹
    public GameObject explosionPrefab;  // 폭발 이펙트 프리팹

    [Header("궁극기")]
    public GameObject safeZonePrefab;       // 안전지대 프리팹
    public GameObject mapExplosionPrefab;   // 맵 전체 폭발 프리팹

    private BossPhase currentPhase;     // 현재 보스 페이즈 저장

    private Coroutine patternRoutine;

    protected override void OnEnable()
    {
        base.OnEnable();
        currentPhase = BossPhase.Phase1;

        // 기존 코루틴 정리
        if (patternRoutine != null) StopCoroutine(patternRoutine);

        patternRoutine = StartCoroutine(PatternLoop());
    }

    protected override void Update()
    {
        base.Update(); // 부모 Update 실행

        CheckPhase(); // 체력에 따라 페이즈 변경
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
                    yield return StartCoroutine(AllDirectionPattern()); //전방위 탄환
                    yield return StartCoroutine(UltimatePattern()); //궁극기
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

            if (bullet == null) yield break; // 풀 비었으면 종료

            bullet.transform.position = transform.position; // 생성 위치

            // 탄환 컴포넌트 가져오기
            BossBullet bossBullet = bullet.GetComponent<BossBullet>();

            if (bossBullet != null)
            {
                bossBullet.Init(dir); // 방향 전달
            }

            yield return new WaitForSeconds(0.3f); // 발사 간격
        }
    }


    /////////////////////장판 패턴/////////////////////////

    IEnumerator AoEAttackPattern()
    {
        if (player == null) yield break;

        Vector3 targetPos = player.position; // 현재 플레이어 위치 저장

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

        yield return new WaitForSeconds(2f);
    }

    ///////////////////// 전방위 발사 탄환 ////////////////////////////

    IEnumerator AllDirectionPattern()
    {
        int count = 24; // 24 방향 공격

        for (int i = 0; i < count; i++)
        {
            GameObject bullet = PoolManager.Instance.GetBossBullet();

            if (bullet == null) yield break;

            bullet.transform.position = transform.position;

            float angle = i * (360f / count); // 각도 계산

            Vector2 dir =new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad));

            BossBullet bossBullet = bullet.GetComponent<BossBullet>();

            if (bossBullet != null)
            {
                bossBullet.Init(dir);
            }
        }

        yield return new WaitForSeconds(2f);
    }


    ///////////////////// 궁극기 ////////////////////////////
    //캐스팅 -> 안전지대 생성 -> 3초대기 -> 맵 전체 폭발
    IEnumerator UltimatePattern()
    {
        if (player == null) yield break;

        Debug.Log("궁극기 시전");

        Vector2 safePos = Random.insideUnitCircle * 5f; // 안전지대 위치

        Instantiate(safeZonePrefab, safePos, Quaternion.identity); // 안전지대 생성

        yield return new WaitForSeconds(3f); // 회피 시간

        Instantiate(mapExplosionPrefab, Vector3.zero, Quaternion.identity); // 맵 폭발
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
