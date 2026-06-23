using UnityEngine;

public class WarriorProjectile : SkillProjectileBase
{
    [Header("전사 베기 설정")]
    [SerializeField] private float slashOffset = 1f;
    [SerializeField] private float slashAngleOffset = 90f;

    protected override void OnInitialized()
    {
        // 전사 베기는 앞으로 날아가지 않고 제자리 판정만 사용
        SetMoveSpeed(0f);

        // 여러 적을 동시에 벨 수 있게 한다.
        SetMaxHitCount(100);

        // 공격자가 있으면 공격자 앞쪽에 베기 판정을 배치
        if(owner != null)
        {
            transform.position = owner.transform.position + (Vector3)(moveDirection.normalized * slashOffset);
        }

        //RotateSlashToDirection();
    }

    protected override void UpdateMovement()
    {
        // 전사 베기는 이동 X
        // 투사체 수명 처리만 사용
    }

    private void RotateSlashToDirection()
    {
        if(moveDirection.sqrMagnitude <= 0f)
        {
            return;
        }

        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x * Mathf.Rad2Deg);

        transform.rotation = Quaternion.Euler(0f, 0f, angle + slashAngleOffset);
    }

    protected override void OnHitTarget(IDamageable1 damageable, Collider2D hitCollider)
    {
        Debug.Log($"전사 베기 적중: {hitCollider.name}");
    }

}
