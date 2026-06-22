using UnityEngine;

public class AoEDamage : MonoBehaviour
{
    public float damage = 25f;

    private void OnTriggerStay2D(Collider2D other)
    {
        //플레이어가 아니면 무시
        if (!other.CompareTag("Player")) return;

        //안전지대 체크
        if (other.GetComponent<SafeZone>() != null)
        {
            return; // 안전지대 안이면 데미지 안 받음
        }

        PlayerBase player = other.GetComponent<PlayerBase>();
        if (player != null)
        {
            player.TakeDamage(damage); //실제 데미지 처리
        }
    }
}
