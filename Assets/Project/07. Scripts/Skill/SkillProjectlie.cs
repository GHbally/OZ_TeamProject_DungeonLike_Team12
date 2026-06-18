using UnityEngine;

public class Projectlie : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float lifeTime = 3f;

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
        if(!collision.TryGetComponent<IDamageable1>(out var damageable))
        {
            return;
        }
        damageable.TakeDamage(damageInfo);

        Destroy(gameObject);
    }
}
