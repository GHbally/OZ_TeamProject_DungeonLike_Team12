//[전사]
//나중에 자동스킬 쪽 Attack과 병합 2026.06.17  (O)
//나중에 전사 전용 스킬 추가 2026.06.18
using UnityEngine;

public class PlayerWarrior : PlayerBase
{
    [Header("전사 스킬 레벨")]
    [Range(1,5)]
    public int skillLevel = 1;  //전사 베기 스킬 기본 1부터 시작

    [Header("전사 스킬 프리팹")]
    [SerializeField] private GameObject slashPrefab;        //전방 베기
    [SerializeField] private GameObject swordWavePrefab;    //5레벨 마스터시 사용할 검기

    //AutoAttackController.cs 및 AttackStats.cs 가져오기
    private AutoAttackController autoAttack;
    private AttackStats attackStats;

    protected override void Start()
    {
        //부모 클래스(PlayerBase)기능 먼저 실행
        base.Start();
        //자동공격 + 스탯 연결
        autoAttack = GetComponent<AutoAttackController>();
        attackStats = GetComponent<AttackStats>();
        //현재 레벨 스탯 가져오기
        ApplyStatsBasedOnLevel();
    }

    //레벨업 메서드
    public void LevelUp()
    {
        //스킬레벨 1을 더하고 1~5범위를 벗어나지 못하게
        skillLevel = Mathf.Clamp(skillLevel + 1, 1, 5);
        //변경된 레벨 스탯 가져오기
        ApplyStatsBasedOnLevel();
    }

    private void ApplyStatsBasedOnLevel()
    {
        if (attackStats == null) return;

        //레벨별 추가 데미지
        float additionalDamage = (skillLevel - 1) * 10.0f;
        attackStats.IncreaseAttackDamage(additionalDamage);

        //레벨별 추가 사거리
        float additionalRange = (skillLevel - 1) * 0.2f;
        attackStats.IncreaseAttackRange(additionalRange);

        //레벨별 추가 공격속도
        float additionalSpeed = (skillLevel - 1) * 0.1f;
        attackStats.IncreaseAttackSpeed(additionalSpeed);
    }
}
