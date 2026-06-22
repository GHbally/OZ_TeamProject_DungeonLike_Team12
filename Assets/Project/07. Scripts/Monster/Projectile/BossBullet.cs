using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public float speed = 5f;      // Åº¼Ó
    public float damage = 20f;    // µ¥¹ÌÁö

    private Vector2 moveDir;

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
        if (!other.CompareTag("Player"))
            return;

        PlayerBase player = other.GetComponent<PlayerBase>();

        if (player != null)
        {
            player.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}