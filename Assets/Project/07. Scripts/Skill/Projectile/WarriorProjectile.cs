using UnityEngine;

public class WarriorProjectile : SkillProjectileBase
{
    [Header("전사 베기 설정")]
    [SerializeField] private float slashOffset = 2f;

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

        //SFX
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("SFX_SwordSlidev2wav");
        }
    }

    protected override void UpdateMovement()
    {
        // 전사 베기는 이동 X
        // 투사체 수명 처리만 사용
    }
}
