using UnityEngine;

public class BounceBullet : MonoBehaviour
{
    public float speed = 6f;      // 이동 속도
    public int maxBounce = 3;     // 최대 반사 횟수
    public float damage = 15f;    // 데미지

    private Vector2 dir;         // 이동 방향
    private int bounceCount;     // 반사 횟수

    
    // 초기화 (보스가 호출)
    
    public void Init(Vector2 direction)
    {
        dir = direction.normalized;
        bounceCount = 0;
    }

    
    // 이동
    
    void Update()
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);
    }

    
    // 충돌 처리
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 플레이어 맞음
        if (collision.collider.CompareTag("Player"))
        {
            PlayerBase player = collision.collider.GetComponent<PlayerBase>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            ReturnToPool();
            return;
        }

        // 벽 반사
        if (collision.collider.CompareTag("Wall"))
        {
            Vector2 normal = collision.contacts[0].normal;
            dir = Vector2.Reflect(dir, normal);

            bounceCount++;

            if (bounceCount >= maxBounce)
            {
                ReturnToPool();
            }
        }
    }

    
    // 풀 반환
    
    void ReturnToPool()
    {
        PoolManager.Instance.ReturnBounceBullet(gameObject);
    }
}
