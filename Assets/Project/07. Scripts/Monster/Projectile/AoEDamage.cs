using UnityEngine;

public class AoEDamage : MonoBehaviour
{
    public float damage = 25f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        //플레이어가 아니면 무시
        if (!other.CompareTag("Player")) return;

        Collider2D safeZone = Physics2D.OverlapPoint(other.transform.position,
            LayerMask.GetMask("SafeZone"));
        //안전지대 체크
        if (safeZone != null)
        {
            return; // 안전지대 안이면 데미지 안 받음
        }

        //맵익스플로전에만 효과음 사용
        if (gameObject.name.Contains("MapExplosion"))
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX("SFX_SummonThunderCreatureTwov1");
            }
        }

        PlayerBase player = other.GetComponent<PlayerBase>();
        if (player != null)
        {
            player.TakeDamage(damage); //실제 데미지 처리
        }
    }
}
