using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

// UI를 만들 때 요기서 함수를 호출해서 카드 후보를 받고,
// 카드 선택 후 다시 선택된 SkillData를 이 클래스에 넘겨주면 된다.

// 전체 스킬 목록관리, 현재 스킬 레벨 관리, 레벨업 시 랜덤 카드 후보 생성, 선택된 스킬의 실제 효과 적용

public class SkillManager : MonoBehaviour
{
    [Header("전체 스킬 목록")]
    [SerializeField] private List<SkillData> allSkills = new();

    // Player 오브젝트에 AttackStats가 붙어 있으면 자동 찾기용
    private AttackStats attackStats;

    // 스킬별 현재 레벨을 저장하는 Dictionary
    private readonly Dictionary<SkillData, SkillRuntime> skillRuntimes = new();

    private void Awake()
    {
        InitializeSkillRuntimes();
    }

    // allSkills에 등록된 모든 스킬의 런타임 정보를 미리 만든다.
    // 처음에는 CurrentLevel이 0인 상태로 시작
    private void InitializeSkillRuntimes()
    {
        skillRuntimes.Clear();

        // allSkills 기준으로 런타임 데이터를 만든다.
        for (int i = 0; i < allSkills.Count; i++)
        {
            SkillData skillData = allSkills[i];

            if (skillData == null)
            {
                continue;
            }

            if (skillRuntimes.ContainsKey(skillData))
            {
                continue;
            }

            skillRuntimes.Add(
                skillData,
                new SkillRuntime(skillData)
            );
        }
    }

    public List<SkillCardInfo> GetRandomSkillCards(int count)
    {
        List<SkillData> availableSkills = GetAvailableSkills();

        Shuffle(availableSkills);

        List<SkillCardInfo> result = new();

        int cardCount = Mathf.Min(
            count,
            availableSkills.Count
        );

        for (int i = 0; i < cardCount; i++)
        {
            result.Add(
                CreateSkillCardInfo(availableSkills[i])
            );
        }

        return result;
    }

    // 리롤 시 이전에 나온 카드는 가능하면 제외하고 새 후보를 뽑는다.
    public List<SkillCardInfo> GetRandomSkillCardsExcept(
        int count,
        List<SkillData> exceptSkills)
    {
        List<SkillData> availableSkills = GetAvailableSkills();

        for (int i = availableSkills.Count - 1; i >= 0; i--)
        {
            SkillData skillData = availableSkills[i];

            if (exceptSkills.Contains(skillData))
            {
                availableSkills.RemoveAt(i);
            }
        }

        // 제외 후 후보가 부족하면 전체 후보에서 다시 뽑는다.
        if (availableSkills.Count < count)
        {
            availableSkills = GetAvailableSkills();
        }

        Shuffle(availableSkills);

        List<SkillCardInfo> result = new();

        int cardCount = Mathf.Min(
            count,
            availableSkills.Count
        );

        for (int i = 0; i < cardCount; i++)
        {
            SkillData skillData = availableSkills[i];

            result.Add(
                CreateSkillCardInfo(skillData)
            );
        }

        return result;
    }

    public void ApplySkillChoice(SkillData selectedSkill)
    {
        if (selectedSkill == null)
        {
            return;
        }

        if (!skillRuntimes.TryGetValue(
                selectedSkill,
                out SkillRuntime runtime))
        {
            runtime = new SkillRuntime(selectedSkill);
            skillRuntimes.Add(selectedSkill, runtime);
        }

        if (runtime.IsMaxLevel)
        {
            return;
        }

        runtime.LevelUp();

        Debug.Log(
            $"{selectedSkill.SkillName} 적용됨 / Lv.{runtime.CurrentLevel}"
        );

        ApplySkillEffect(selectedSkill, runtime.CurrentLevel);
    }

    private void ApplySkillEffect(
        SkillData skillData,
        int currentLevel)
    {
        Debug.Log(
            $"스킬 효과 적용 위치: {skillData.SkillName} / Lv.{currentLevel}"
        );

        // TODO: 실제 효과 연결
        // 예: 공격력 증가, 공격속도 증가, 투사체 개수 증가
    }

    private List<SkillData> GetAvailableSkills()
    {
        List<SkillData> result = new();

        for (int i = 0; i < allSkills.Count; i++)
        {
            SkillData skillData = allSkills[i];

            if (skillData == null)
            {
                continue;
            }

            if (!skillRuntimes.TryGetValue(
                    skillData,
                    out SkillRuntime runtime))
            {
                result.Add(skillData);
                continue;
            }

            if (!runtime.IsMaxLevel)
            {
                result.Add(skillData);
            }
        }

        return result;
    }

    private SkillCardInfo CreateSkillCardInfo(SkillData skillData)
    {
        int currentLevel = GetCurrentLevel(skillData);

        int nextLevel = Mathf.Clamp(
            currentLevel + 1,
            1,
            skillData.MaxLevel
        );

        string description =
            skillData.GetLevelUpDescription(nextLevel);

        return new SkillCardInfo(
            skillData,
            skillData.SkillName,
            description,
            skillData.Icon,
            currentLevel,
            nextLevel,
            skillData.MaxLevel
        );
    }

    public int GetCurrentLevel(SkillData skillData)
    {
        if (skillData == null)
        {
            return 0;
        }

        if (!skillRuntimes.TryGetValue(
                skillData,
                out SkillRuntime runtime))
        {
            return 0;
        }

        return runtime.CurrentLevel;
    }

    private void Shuffle(List<SkillData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            SkillData temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}


