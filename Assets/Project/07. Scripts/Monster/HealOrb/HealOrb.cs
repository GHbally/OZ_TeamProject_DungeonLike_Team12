using System.Xml.Serialization;
using UnityEngine;

public class HealOrb : MonoBehaviour
{
    [Header("회복량 설정")]
    public int healAmount = 20;

    
    ////////////////////힐 실제 추가/////////////////////
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어만 획득
        if (!other.CompareTag("Player"))
            return;

        // PlayerBase 가져오기
        PlayerBase player = other.GetComponent<PlayerBase>();

        // PlayerBase가 있으면 체력 회복
        if (player != null)
        {
            player.Heal(healAmount);

            Debug.Log($"체력 {healAmount} 회복");
        }

        // 회복 구슬 풀로 반환
        ReturnPool();
    }

    void ReturnPool()
    {
        PoolManager.Instance.ReturnHealOrb(gameObject);
    }
}
