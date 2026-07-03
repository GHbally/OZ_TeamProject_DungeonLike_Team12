using DG.Tweening;
using System.Collections;
using UnityEngine;

public class LastBossMonster : MonsterBase
{
    [Header("보스 설정")]
    public float patternDelay = 1f;     // 패턴 종료 후 다음 패턴까지 대기 시간

    [Header("장판")]
    public GameObject warningPrefab;    // 경고 원 프리팹
    public GameObject explosionPrefab;  // 폭발 이펙트 프리팹

    public GameObject aoeEffectPrefab; // 장판 이펙트 프리팹

    [Header("궁극기")]
    public GameObject safeZonePrefab;       // 안전지대 프리팹
    public GameObject mapExplosionPrefab;   // 맵 전체 폭발 프리팹
    public float ultimateCastTime = 5f; //궁극기 시전시간


    [Header("접촉 데미지")] // 플레이어에게 줄 접촉 데미지
    public float touchDamage = 10f;
    public float touchDamageInterval = 1f;// 몇 초마다 데미지를 줄지
    private float touchDamageTimer;// 데미지 시간을 계산할 타이머

    private BossPhase currentPhase;     // 현재 보스 페이즈 저장

    [Header("페이즈 시각 효과")]
    [SerializeField] private Color phase1Color = Color.white;
    [SerializeField] private Color phase2Color = Color.yellow;
    [SerializeField] private Color phase3Color = Color.red;

   

    private Coroutine patternRoutine;

    // false면 공격, 탄환, 접촉 데미지 전부 막음
    private bool canAct = false;

    // 보스가 공격을 시작할 수 있는지 여부
    private bool canStartPattern = false;

    // 보스 죽음 연출이 이미 시작됐는지 확인하는 변수
    private bool isBossDeathStarted = false;

    // 보스가 죽었을 때 이 변수를 이용해서 안전장판을 삭제할 수 있음
    private GameObject currentSafeZone;

    // 보스가 죽었을 때 폭발 이펙트도 같이 정리하기 위해 사용
    private GameObject currentMapExplosion;


    protected override void OnEnable()
    {
        base.OnEnable();

        // 보스 안에 있는 모든 SpriteRenderer를 다시 찾는다
        monsterRenderers.Clear();

        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null) continue;

            monsterRenderers.Add(sr);
        }

        // 메인 SpriteRenderer 지정
        if (monsterRenderers.Count > 0)
        {
            mainSpriteRenderer = monsterRenderers[0];
        }

        /////////////보스 애니메이터 수안 추가 
        animator = GetComponentInChildren<Animator>();

        // 맵 중앙에 보스 배치
        transform.position = new Vector3(-7f, -2f, 0f);
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

        isBossDeathStarted = false;

        // 바로 데미지를 줄 수 있도록 타이머 초기화
        touchDamageTimer = touchDamageInterval;

        // 처음에는 공격하지 않음
        canStartPattern = false;

        canAct = false;

        // 시작은 1페이즈
        currentPhase = BossPhase.Phase1;

        // 1페이즈 색상 적용
        ApplyPhaseColor(currentPhase);

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 등장 연출이 끝나기 전이면 접촉 데미지 금지
        if (!canAct)
        {
            return;
        }

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

        while (!canStartPattern)
        {
            yield return null;
        }

        if (patternRoutine != null)
        {
            StopCoroutine(patternRoutine);
        }

        patternRoutine = StartCoroutine(PatternLoop());
    }

    protected override void Update()
    {
        // 등장 연출이 끝나기 전이면 보스 로직 실행 안 함
        if (!canAct)
        {
            return;
        }

        // 부모 Update 실행
        base.Update();

        // 체력에 따라 페이즈 변경
        CheckPhase();

        
    }

    // 최종보스는 움직이지 않게 FixedUpdate를 막음
    protected override void FixedUpdate()
    {
       
    }
    protected override void UpdateState() { }

    protected override void AttackLogic() { }

    private void CheckPhase()
    {
        // 현재 체력 비율 계산
        float hpPercent = currentHp / maxHp;

        // 변경될 페이즈 저장
        BossPhase newPhase;

        // 체력 40% 이하 → 3페이즈
        if (hpPercent <= 0.4f)
        {
            newPhase = BossPhase.Phase3;
        }
        // 체력 70% 이하 → 2페이즈
        else if (hpPercent <= 0.7f)
        {
            newPhase = BossPhase.Phase2;
        }
        // 그 외 → 1페이즈
        else
        {
            newPhase = BossPhase.Phase1;
        }

        // 페이즈가 바뀌었을 때만 실행
        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            Debug.Log("현재 페이즈 : " + currentPhase);
            // 페이즈에 맞는 색상 적용
            ApplyPhaseColor(currentPhase);
        }
    }

    /// ////////////////보스 페이즈 별 색상 변경 함수///////////////////////////
    private void ApplyPhaseColor(BossPhase phase)
    {
        Color targetColor = phase1Color;

        if (phase == BossPhase.Phase2)
        {
            targetColor = phase2Color;
        }
        else if (phase == BossPhase.Phase3)
        {
            targetColor = phase3Color;
        }

        // SpriteRenderer 전부 다시 찾기
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (SpriteRenderer sr in renderers)
        {
            if (sr == null) continue;

            sr.color = targetColor;
        }
    }

    public override void TakeDamage(DamageInfo1 damageInfo)
    {
        base.TakeDamage(damageInfo);
        // 부모 MonsterBase의 데미지 처리 실행

        StartCoroutine(ResetBossPhaseColorAfterHit());
        // 피격 효과가 끝난 뒤 보스 페이즈 색을 다시 적용
    }

    private IEnumerator ResetBossPhaseColorAfterHit()
    {
        yield return new WaitForSeconds(0.12f);
        // 살짝 더 기다린 뒤 색을 다시 적용

        if (!isDead)
        {
            ApplyPhaseColor(currentPhase);
            // 현재 페이즈 색을 다시 적용
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
        // 탄환 발사 패턴이 시작될 때 공격 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

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
        // 장판 공격 패턴이 시작될 때 공격 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        if (player == null) yield break;

        Vector3 targetPos = player.position; // 현재 플레이어 위치 저장

        GameObject warning =
        Instantiate(warningPrefab, targetPos, Quaternion.identity); // 경고 생성

        yield return new WaitForSeconds(2f); // 피할 시간 제공
        
        Destroy(warning); // 경고 삭제

        GameObject aoe = 
            Instantiate(aoeEffectPrefab, targetPos, Quaternion.identity); // 장판 생성

        Destroy(aoe, 1.5f); // 장판 1.5초 후 삭제
    }


    ///////////////////// 소환 패턴 ////////////////////////////

    IEnumerator SummonPattern()
    {
        // 몬스터 소환은 마법 시전 느낌이므로 Cast 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("Cast");
        }

        // 3~5마리 랜덤 소환
        int count = Random.Range(3, 6);

        for (int i = 0; i < count; i++)
        {
            // 전사 몬스터를 풀에서 가져옴
            GameObject monster = PoolManager.Instance.GetWarriorMonster();

            // 풀에 없으면 스킵
            if (monster == null) continue;

            // 소환된 몬스터의 MonsterBase를 가져옴
            MonsterBase monsterBase = monster.GetComponent<MonsterBase>();
            
            // 나중에 죽을 때 보상상자를 드랍하지 않게 하기 위해 사용
            if (monsterBase != null)
            {
                monsterBase.isBossSummonedMonster = true;
            }

            // 풀에 없으면 스킵
          

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
        // 튕기는 탄환 발사 시 공격 애니메이션 실행
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

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

    // ====================== 3페이즈 궁극기 패턴 ======================
    IEnumerator Phase3Pattern()
    {
        // Animator 컴포넌트가 존재하는지 확인
        if (animator != null)
        {
            // Cast 트리거를 실행하여 캐스팅(시전) 애니메이션 재생
            animator.SetTrigger("Cast");
        }

        // 콘솔에 궁극기 시작 로그 출력(디버깅용)
        Debug.Log("궁극기 시전");

        // 현재 메인 카메라 가져오기
        Camera cam = Camera.main;

        // 화면(Viewport) 안에서 랜덤한 위치를 선택
        // Viewport 좌표는 (0,0)이 왼쪽 아래, (1,1)이 오른쪽 위
        Vector3 viewportPos = new Vector3(

            // 화면 가로 15% ~ 85% 사이에서 랜덤
            // 가장자리에 생성되지 않도록 여유를 둠
            Random.Range(0.15f, 0.85f),

            // 화면 세로 15% ~ 85% 사이에서 랜덤
            Random.Range(0.15f, 0.85f),

            // 카메라와의 거리(Z축)
            // ViewportToWorldPoint()에서 반드시 필요
            Mathf.Abs(cam.transform.position.z)
        );

        // 화면 좌표(Viewport)를 실제 게임 월드 좌표로 변환
        Vector3 safePos = cam.ViewportToWorldPoint(viewportPos);

        // 2D 게임이므로 Z축은 항상 0으로 고정
        safePos.z = 0;

        // 안전지대 생성
        // 플레이어가 이동해야 하는 위치
        currentSafeZone = Instantiate(safeZonePrefab, safePos, Quaternion.identity);

        // 궁극기 캐스팅 시간 계산용 변수

        // 현재 캐스팅이 얼마나 진행되었는지 저장
        float timer = 0f;

        // 탄환을 몇 초마다 발사할지 설정
        float shootInterval = 1f;

        // 궁극기 캐스팅 시간 동안 반복
        while (timer < ultimateCastTime)
        {
            // -----------------------------
            // 전방위 탄환 발사 패턴 실행
            // -----------------------------
            yield return StartCoroutine(AllDirectionPattern());

            // 다음 탄환 발사까지 1초 대기
            yield return new WaitForSeconds(shootInterval);

            // 경과 시간 누적
            timer += shootInterval;
        }

        //////////////////// 궁극기 발동////////////////////

        // 맵 전체 폭발 생성
        currentMapExplosion = Instantiate(mapExplosionPrefab, Vector3.zero, Quaternion.identity);

        // 폭발 이펙트가 보이도록 잠시 대기
        yield return new WaitForSeconds(0.5f);

        // 안전지대 삭제

        // 안전지대가 아직 존재하면 삭제
        if (currentSafeZone != null)
        {
            Destroy(currentSafeZone);

            // 삭제 후 참조 제거
            currentSafeZone = null;
        }

        // 폭발 이펙트 삭제

        // 폭발 이펙트가 아직 존재하면 삭제
        if (currentMapExplosion != null)
        {
            Destroy(currentMapExplosion);

            // 삭제 후 참조 제거
            currentMapExplosion = null;
        }

        // 코루틴 종료
        // PatternLoop()로 돌아가 다음 패턴을 진행
    }

    //////////////////보스 사망/////////////////////

    protected override void Death()
    {
        // 죽음 연출이 이미 시작됐다면 다시 실행하지 않음
        if (isBossDeathStarted) return;

        // 죽음 연출 시작 처리
        isBossDeathStarted = true;

        // 보스 사망 상태로 변경
        isDead = true;

        // 현재 상태를 Dead로 변경
        currentState = MonsterState.Dead;

        GameObject hpUI = GameObject.Find("Canvas/HpUI");

        if (hpUI != null)
        {
            hpUI.SetActive(false);
        }

        // 보스 패턴 코루틴 정지
        if (patternRoutine != null)
        {
            StopCoroutine(patternRoutine);
            patternRoutine = null;
        }

        // 충돌을 꺼서 죽은 뒤 추가 피격 방지
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 죽는 애니메이션 실행
        if (animator != null)
        {
            // 다른 트리거 초기화
            animator.ResetTrigger("Attack");
            animator.ResetTrigger("Cast");

            // 이동 애니메이션 끄기
            animator.SetBool("1_Move", false);

            // 죽는 애니메이션 실행
            animator.SetTrigger("4_Death");
        }

        // 죽는 연출 후 삭제
        StartCoroutine(BossDieSequence());
    }

    private IEnumerator BossDieSequence()
    {
        // --- 1. 타격감 및 슬로우 모션 ---
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.2f); // 히트 스톱

        Time.timeScale = 0.5f; // 슬로우 모션 시작

        // 카메라 연출 (DOTween)
        Sequence camSeq = DOTween.Sequence().SetUpdate(true);
        camSeq.Join(Camera.main.transform.DOMove(new Vector3(transform.position.x, transform.position.y, -10f), 3.0f));
        camSeq.Join(Camera.main.DOOrthoSize(3f, 3.0f));

        yield return new WaitForSecondsRealtime(3.0f);

        // --- 2. UI 등장 연출 ---
        GameObject uiRoot = GameObject.Find("Victory");
        if (uiRoot != null)
        {
            uiRoot.SetActive(true);

            // UI 애니메이터 실행 (트리거 이름이 "Play"라고 가정)
            Animator uiAnim = uiRoot.GetComponent<Animator>();
            if (uiAnim != null)
            {
                uiAnim.updateMode = AnimatorUpdateMode.UnscaledTime; // 게임 일시정지 무시하고 재생
                uiAnim.SetTrigger("Play");
            }

            // CanvasGroup 설정 및 애니메이션
            CanvasGroup cg = uiRoot.GetComponent<CanvasGroup>() ?? uiRoot.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            RectTransform rect = uiRoot.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, 1000);

            Sequence s = DOTween.Sequence().SetUpdate(true);
            s.Join(rect.DOAnchorPos(Vector2.zero, 0.6f).SetEase(Ease.OutBack));
            s.Join(cg.DOFade(1, 0.3f));
        }

        // --- 3. 최종 상태 처리 ---
        Time.timeScale = 1f;

        // 게임 상태 변경 및 보스 삭제
        StageManager stageManager = FindFirstObjectByType<StageManager>();
        if (stageManager != null) stageManager.UnregisterEnemy(true);

        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Won);

        Destroy(gameObject);
    }

    // 보스가 완전히 나타난 뒤 이 함수가 실행됨
    public void StartBossPattern()
    {
        // 이제부터 공격, 접촉 데미지, 패턴 실행 가능
        canAct = true;

        // 보스 페이즈를 1페이즈로 시작
        currentPhase = BossPhase.Phase1;

        // 기존 패턴 코루틴이 있으면 중복 방지를 위해 정지
        if (patternRoutine != null)
        {
            StopCoroutine(patternRoutine);
            patternRoutine = null;
        }

        // 이제부터 패턴 시작
        patternRoutine = StartCoroutine(PatternLoop());
    }
}
