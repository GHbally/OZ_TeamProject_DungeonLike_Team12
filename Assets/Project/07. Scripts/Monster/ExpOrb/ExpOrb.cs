using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [Header("경험치 설정")]
    public int expAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어만 먹기 가능
        if (!other.CompareTag("Player"))
            return;

        Debug.Log($"경험치 {expAmount} 획득");

        // 나중에 경험치 시스템 연결
        // other.GetComponent<PlayerExp>()
        //      .AddExp(expAmount);

        ReturnPool();
    }

    void ReturnPool()
    {
        PoolManager.Instance.ReturnExpOrb(gameObject);
    }
}
