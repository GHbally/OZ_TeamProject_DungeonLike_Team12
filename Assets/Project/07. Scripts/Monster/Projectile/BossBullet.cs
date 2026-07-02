using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 5f; // 탄속
    public float damage = 20f;// 데미지
    public float lifeTime = 5f; // 5초 뒤 자동 반환
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
        
        CancelInvoke(); // 이전 자동 반환 예약 취소

        //SFX
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("SFX_FireSpellv3");
        }


        Invoke(nameof(ReturnToPool), lifeTime); // lifeTime 후 풀로 반환
    }

    private void Update()
    {
        transform.position += (Vector3)(moveDir * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        //플레이어 컴포넌트 줍줍
        PlayerMovement movement = other.GetComponent<PlayerMovement>();

        //만약 플레이어가 대쉬 중(무적 상태)이라면 데미지를 주지 않고 그냥 통과
        if (movement != null && movement.IsInvincible)
        {
            return; //무적이면 데미지 처리를 하지 않고 리턴
        }

        PlayerBase player = other.GetComponent<PlayerBase>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        ReturnToPool(); // 맞으면 풀로 반환
    }
    private void ReturnToPool()
    {
        CancelInvoke(); // 중복 반환 방지

        PoolManager.Instance.ReturnBossBullet(gameObject); // 풀로 반환
    }

    private void OnDisable()
    {
        CancelInvoke(); // 비활성화될 때 예약 제거
    }
}