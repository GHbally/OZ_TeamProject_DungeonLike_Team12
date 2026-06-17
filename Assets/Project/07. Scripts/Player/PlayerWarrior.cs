//[전사]
//나중에 자동스킬 쪽 Attack과 병합 2026.06.17
using UnityEngine;

public class PlayerWarrior : PlayerBase
{
    [Header("전사 스탯")]
    [SerializeField] private float swordDamage = 30.0f;     //전사 전용 자동스킬 데미지 수정 부탁드립니다.
    [SerializeField] private float attackCooldown = 0.5f;   //쿨타임

    protected override void Start()
    {
        //부모 클래스(PlayerBase)기능 먼저 실행
        base.Start();
    }
}
