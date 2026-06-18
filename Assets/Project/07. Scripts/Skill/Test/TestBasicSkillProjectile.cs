using UnityEngine;

public class TestBasicSkillProjectile : SkillProjectileBase
{
    protected override void OnInitialized()
    {
        base.OnInitialized();

        // 발사 이펙트나 사운드가 필요하면 이곳에 추가
    }
    protected override void OnHitTarget(
        IDamageable1 damageable,
        Collider2D hitCollider)
    {
        base.OnHitTarget(damageable, hitCollider);

        Debug.Log($"{name}: {hitCollider.name} 명중",this);
    }

    protected override void OnBeforeRelease()
    {
        base.OnBeforeRelease();

        // TrailRenderer 등을 사용하는 경우 이곳에서 초기화
    }
}
