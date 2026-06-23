using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 5f;      // 탄속
    public float damage = 20f;    // 데미지

    private Vector2 moveDir; // 탄환 이동 방향
    private Rigidbody2D rb; // Rigidbody2D 저장

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // Rigidbody2D 가져오기

        if (rb != null) // Rigidbody2D가 있다면
        {
            rb.gravityScale = 0f; // 중력 제거
            rb.bodyType = RigidbodyType2D.Kinematic; // 물리 충돌로 밀리지 않게 설정
        }
    }

    public void Init(Vector2 dir)
    {
        moveDir = dir.normalized;
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("보스 탄환 충돌 감지: " + other.name); // 충돌 확인용 로그

        if (!other.CompareTag("Player")) return;

        PlayerBase player = other.GetComponent<PlayerBase>();

        if (player != null)
        {
            player.TakeDamage(damage);
            Debug.Log("보스 탄환 데미지 적용"); // 데미지 확인용 로그
        }

        Destroy(gameObject);
    }
}