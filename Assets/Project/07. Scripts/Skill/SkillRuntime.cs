using UnityEngine;

// 게임 플레이 중 스킬의 현재 상태를 저장하는 클래스
// SkillData는 원본 데이터이므로 현재 레벨을 직접 저장 X
public class SkillRuntime
{
    public SkillData Data { get; }
    public int CurrentLevel { get; private set; }

    // 현재 레벨이 최대 레벨 이상이면 true
    // 최대 레벨 스킬은 카드 후보에서 제외할 때 사용
    public bool IsMaxLevel => CurrentLevel >= Data.MaxLevel;

    public SkillRuntime(SkillData data)
    {
        Data = data;
        CurrentLevel = 0;
    }
    public void LevelUp()
    {
        if (IsMaxLevel)
        {
            return;
        }
        CurrentLevel++;
    }
}
