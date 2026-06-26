using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("화살 설정")]
    public float speed = 8f; // 화살속도
    public int damage = 10; // 공격력
    public float lifeTime = 3f; //제거 시간
    private Vector2 direction; //이동방향


    //발사 할때 방향
    public void Initialized(Vector2 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        Invoke(nameof(ReturnPool), lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        //이동컴포넌트 가져와서
        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        //플레이어 대쉬(무적)상태일 경우
        if (playerMovement != null && playerMovement.IsInvincible)
        {
            return; //아무것도 안하고 빠져나가기
        }

        Debug.Log("플레이어 피격");
        PlayerBase player = other.GetComponent<PlayerBase>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
        ReturnPool();
    }
    void ReturnPool()
    {
        PoolManager.Instance.ReturnArrow(gameObject);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
