using System;
using System.Collections.Generic;
using UnityEngine;

// 이 스크립트가 붙은 Gameobj에 Collider2D와 Rigidbody2D가 반드시 존재하도록 함.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public abstract class SkillProjectileBase : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 10f;
    protected bool rotateToDirection = true;

    [SerializeField] protected float lifeTime = 1f;
    protected float remainingLifeTime;

    [SerializeField] private LayerMask enemyLayer;

    protected Vector2 moveDirection;

    protected DamageInfo1 damageInfo;
    
    protected GameObject owner;     // 투사체를 발사한 대상

    protected int maxHitCount = 100;  // 최대 타격할 수 있는 적의 수
    protected int currentHitCount;  // 타격한 적의 수

    protected bool isInitialized;

    protected bool isReleased;      // 이미 제거가 되었는 지 여부

    private Action<SkillProjectileBase> releaseAction;          // 풀로 변환하는 함수

    private readonly HashSet<IDamageable1> hitTargets = new();  // 중복 피해를 받지 않게 함

    public virtual void Initialize(
        Vector2 direction, 
        DamageInfo1 newDamageInfo, 
        Action<SkillProjectileBase> newReleaseAction)
    {

        moveDirection = direction.normalized;

        damageInfo = newDamageInfo;
        owner = newDamageInfo.Attacker;
        releaseAction = newReleaseAction;

        remainingLifeTime = lifeTime;
        currentHitCount = 0;

        isInitialized = true;
        isReleased = false;

        hitTargets.Clear();

        // 풀에 있던 오브젝트 활성화
        gameObject.SetActive(true);

        if (rotateToDirection)
        {
            RotateToMoveDirection();
        }

        // 자식 투사체 추가 초기화
        OnInitialized();

    }

    protected virtual void Update()
    {
        if (!isInitialized || isReleased)
        {
            return;
        }

        UpdateLifetime();

        // 수명 처리 과정에서 반환됐을 수도 있으므로 다시 검사
        if (isReleased)
        {
            return;
        }

        UpdateMovement();
    }
    

    // 투사체의 수명을 감소시키는 함수
    protected virtual void UpdateLifetime()
    {
        remainingLifeTime -= Time.deltaTime;

        if (remainingLifeTime <= 0f)
        {
            ReleaseProjectile();
        }
    }

    // 투사체를 현재 이동 방향으로 이동시키는 함수
    protected virtual void UpdateMovement()
    {
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
    }

    protected void RotateToMoveDirection()
    {
        if (moveDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        float angle =Mathf.Atan2(moveDirection.y, moveDirection.x)* Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInitialized || isReleased)
        {
            return;
        }
        if (!IsInEnemyLayer(collision.gameObject.layer))
        {
            return;
        }

        IDamageable1 damageable = collision.GetComponentInParent<IDamageable1>();

        // 데미지 받지 않은 오브젝트는 무시
        if (damageable == null)
        {
            return;
        }

        // 이미 피해를 받은 적은 중복 피해를 주지 않음
        if (hitTargets.Contains(damageable))
        {
            return;
        }

        ProcessHit(damageable, collision);
    }

    // 적중한 적에게 피해를 주고 관통 횟수 계산
    protected virtual void ProcessHit(IDamageable1 damageable, Collider2D hitCollider)
    {
        hitTargets.Add(damageable);

        damageable.TakeDamage(damageInfo);

        currentHitCount++;

        // 자식 클래스의 적중 효과 실행
        OnHitTarget(damageable, hitCollider);

        // 최대 적중 횟수에 도달하면 풀로 반환한다.
        if (currentHitCount >= maxHitCount)
        {
            ReleaseProjectile();
        }
    }

    // 투사체를 Destroy 하지 않고 생성된 풀에 반환
    protected virtual void ReleaseProjectile()
    {
        if (isReleased)
        {
            return;
        }
        isReleased = true;
        isInitialized = false;

        // 자식 클래스가 반환 직전 정리할 내용 실행
        OnBeforeRelease();

        // 반환 함수가 있다면 풀에 반환
        if (releaseAction != null)
        {
            releaseAction.Invoke(this);
            return;
        }
        // 비활성화
        gameObject.SetActive(false);
    }

    // 전달받은 레이어가 enemyLayer에 포함되어 있는지 확인
    protected bool IsInEnemyLayer(int layer)
    {
        return (enemyLayer.value & (1 << layer)) != 0;
    }

    // 투사체의 이동 방향을 변경할 수 있는 함수 (유도나 도탄)
    protected void SetMoveDirection(Vector2 newDirection)
    {
        if (newDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        moveDirection = newDirection.normalized;

        if (rotateToDirection)
        {
            RotateToMoveDirection();
        }
    }

    // 투사체의 이동 속도를 변경할 수 있는 함수
    public void SetMoveSpeed(float newMoveSpeed)
    {
        moveSpeed = Mathf.Max(0f, newMoveSpeed);
    }

    // 최대 적중 횟수를 변경한다.
    public void SetMaxHitCount(int newMaxHitCount)
    {
        maxHitCount = Mathf.Max(1, newMaxHitCount);
    }
    
    // 자식 클래스에서 재정의 (초기화한 직후 호출)
    protected virtual void OnInitialized()
    {
    }

    // 자식 클래스에서 재정의 (투사체가 적에게 명중한 직후 호출 - 도탄, 폭발, 상태이상 등)
    protected virtual void OnHitTarget(IDamageable1 damageable,Collider2D hitCollider)
    {
    }

    // 자식 클래스에서 재정의 (반환되기 직전 호출)
    protected virtual void OnBeforeRelease()
    {
    }

    // 오브젝트가 비활성화 될 때 이전 사용 상태를 정리
    protected virtual void OnDisable()
    {
        isInitialized = false;
        isReleased = true;

        currentHitCount = 0;
        remainingLifeTime = 0f;

        owner = null;
        releaseAction = null;

        hitTargets.Clear();
    }
}
