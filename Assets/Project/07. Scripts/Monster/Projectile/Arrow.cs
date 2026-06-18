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
        Invoke(nameof(ReturnPool), lifeTime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        Debug.Log("플레이어 피격");
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
