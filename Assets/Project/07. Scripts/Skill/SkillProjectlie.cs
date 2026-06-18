using UnityEngine;

// 이 스크립트가 붙은 Gameobj에 Collider2D와 Rigidbody2D가 반드시 존재하도록 함.
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class SkillProjectlie : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 1f;
    [SerializeField] private LayerMask enemyLayer;

    private Vector2 moveDirection;
    private DamageInfo1 damageInfo;
    private float remainingLifeTime;
    private bool isInitialized;

    public void Initialize(Vector2 direction, DamageInfo1 newDamageInfo)
    {
        moveDirection = direction.normalized;
        damageInfo = newDamageInfo;
        remainingLifeTime = lifeTime;
        isInitialized = true;
        RotateToDirection();
    }

    private void Update()
    {
        if(!isInitialized)
        {
            return;
        }
        transform.position += (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        remainingLifeTime -= Time.deltaTime;

        if(remainingLifeTime < 0f)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInitialized)
        {
            return;
        }
        if (!IsInEnemyLayer(collision.gameObject.layer))
        {
            return;
        }

        IDamageable1 damageable = collision.GetComponentInParent<IDamageable1>();
        if (damageable == null)
        {
            return;
        }

        damageable.TakeDamage(damageInfo);
    }

    private void RotateToDirection()
    {
        if (moveDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        float angle = Mathf.Atan2(moveDirection.y,moveDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // 전달받은 레이어가 enemyLayer에 포함되어 있는지 확인
    private bool IsInEnemyLayer(int layer)
    {
        return (enemyLayer.value & (1 << layer)) != 0;
    }
}
