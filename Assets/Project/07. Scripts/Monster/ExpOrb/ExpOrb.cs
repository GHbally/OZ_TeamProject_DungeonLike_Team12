using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [Header("경험치 설정")]
    public int expAmount = 10;

    private PlayerLevelSystem playerLevelSystem;

    private void Start()
    {
        PlayerBase player = FindFirstObjectByType<PlayerBase>();

        if (player != null)
        {
            playerLevelSystem = player.GetComponent<PlayerLevelSystem>();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어만 먹기 가능
        if (!other.CompareTag("Player")) return;

        //경험치 획득
        if (playerLevelSystem != null)
        {
            playerLevelSystem.EarnExp(expAmount);
            Debug.Log($"경험치 {expAmount} 획득");
        }

        ReturnPool();
    }

    void ReturnPool()
    {
        PoolManager.Instance.ReturnExpOrb(gameObject);
    }
}
