using UnityEngine;

// 전사 스킬이 5레벨일 때 앞으로 날아가는 검기 투사체.
// 벽이나 오브젝트에 막히지 않고 지나가며,
// 적은 한 번씩만 타격한다.
public class WarriorSwordWaveProjectile : SkillProjectileBase
{
    [Header("검기 설정")]
    [SerializeField] private int maxPierceCount = 999;

    protected override void OnInitialized()
    {
        // 검기는 여러 적을 관통해야 하므로 적중 가능 횟수를 크게 설정한다.
        SetMaxHitCount(maxPierceCount);
    }

    protected override void OnHitTarget(
        IDamageable1 damageable,
        Collider2D hitCollider)
    {
        Debug.Log($"전사 검기 적중: {hitCollider.name}");
    }
}
