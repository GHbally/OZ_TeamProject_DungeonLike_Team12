using UnityEngine;

public class WarriorMonster : MonsterBase
{
    [Header("근접 공격")]
    public int damage = 10; //공격력
    public float damageInterval = 1f; //1초마다 데미지 들어감

    private float damageTimer;

    protected override void Chase()
    {
        if (player == null)
            return;
        //플레이어 방향 계산
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        //플레이어 추적
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);
    }
    protected override void Attack()
    {

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;

        damageTimer += Time.deltaTime;

        //1초마다 데미지
        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;
            Debug.Log("플레이어 피격");

            
            //.TakeDamage(damage);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        damageTimer = 0f;

    }
}
