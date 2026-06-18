using System.Xml.Serialization;
using UnityEngine;

public class HealOrb : MonoBehaviour
{
    [Header("회복량 설정")]
    public int healAmount = 20;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어만 획득
        if (!other.CompareTag("Player")) return;

        Debug.Log($"체력{healAmount} 회복");
        // 나중에 플레이어 체력 시스템 연결
        // other.GetComponent<PlayerHP>()   .Heal(healAmount);

        ReturnPool();
    }

    void ReturnPool()
    {
        PoolManager.Instance.ReturnHealOrb(gameObject);
    }
}
