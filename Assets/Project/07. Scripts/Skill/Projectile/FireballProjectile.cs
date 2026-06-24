using UnityEngine;

public class FireballProjectile : SkillProjectileBase
{
    private int hitCount = 100;

    // 마스터 시 불길 생성 offset
    private float fireTrailSpawnOffset = 0f;

    private float trailSpawnDistance = 0.5f;
    private Vector3 lastTrailSpawnPosition;

    private int fireballLevel;
    private float sizeMultiplier = 1f;
    private bool isMaster;

    // 마스터 시 생성할 불길 장판 pool
    private SkillProjectilePool fireTrailPool;
    private Vector3 originalScale;

    private void Awake()
    {
        // 풀링 오브젝트는 계속 재사용되므로 원래 크기를 저장해둔다.
        // 그래야 레벨별 크기를 적용한 뒤 풀로 돌아갈 때 원상복구할 수 있다.
        originalScale = transform.localScale;
    }

    protected override void OnInitialized()
    {
        SetMaxHitCount(hitCount);

        // 풀에서 다시 꺼낼 때마다 마지막 불길 생성 위치를 현재 위치로 초기화한다.
        lastTrailSpawnPosition = transform.position;

        // 기본 크기로 pool 회수 발사 직후에 현재 레벨 크기로 다시 갱신
        ApplySize();
    }

    public void SetupFireball(
        int newFireballLevel, 
        float newsizeMultiplier, 
        bool newIsmater, 
        SkillProjectilePool newFireTrailPool)
    {
        // FireballSkillController 에서 현재 파이어볼 레벨 정보를 넣어줌
        fireballLevel = newFireballLevel;
        sizeMultiplier = Mathf.Max(0.1f, newsizeMultiplier);
        isMaster = newIsmater;
        fireTrailPool = newFireTrailPool;

        ApplySize();

        if (isMaster)
        {
            SpawnFireTrail();
            lastTrailSpawnPosition = transform.position;
        }

    }

    private void ApplySize()
    {
        // 파이어볼 크기 증가
        transform.localScale = originalScale * sizeMultiplier;
    }

    protected override void UpdateMovement()
    {
        // 파이어볼은 일반 투사체처럼 앞으로 이동해야 하므로
        // 부모의 기본 이동 로직을 실행한다.
        base.UpdateMovement();

        // 마스터 상태일 때만 지나가는 위치마다 불길을 생성한다.
        TrySpawnTrailByDistance();
    }

    private void TrySpawnTrailByDistance()
    {
        if (!isMaster)
        {
            return;
        }

        if (fireTrailPool == null)
        {
            return;
        }

        float distanceFromLastTrail =
            Vector3.Distance(transform.position, lastTrailSpawnPosition);

        // 마지막 불길 위치에서 일정 거리 이상 이동했을 때만 새 불길을 생성한다.
        // 이 값이 너무 작으면 불길이 너무 많이 생기고,
        // 너무 크면 경로가 듬성듬성 보인다.
        if (distanceFromLastTrail < trailSpawnDistance)
        {
            return;
        }

        SpawnFireTrail();

        // 방금 불길을 생성한 위치를 저장한다.
        lastTrailSpawnPosition = transform.position;
    }

    private void SpawnFireTrail()
    {
        if (fireTrailPool == null)
        {
            return;
        }

        // 파이어볼 현재 위치에 불길 장판을 생성한다.
        // 불길은 움직이지 않는 장판이므로 방향은 Vector2.zero로 넘긴다.
        fireTrailPool.Spawn(transform.position, Vector2.zero, damageInfo);
    }

    protected override void OnHitTarget(IDamageable1 damageable, Collider2D hitCollider)
    {
        // 불길은 적중 시 생성하지 않음
    }

    protected override void OnBeforeRelease()
    {
        // 풀로 돌아가기 전에 이전 발사 상태를 초기화
        // 이걸 안하면 다음에 낮은 레벨 파이어볼이 나가도 크기가 남을 수 있음
        transform.localScale = originalScale;

        fireballLevel = 0;
        sizeMultiplier = 1f;
        isMaster = false;
        fireTrailPool = null;
        lastTrailSpawnPosition = transform.position;
    }
    
}
