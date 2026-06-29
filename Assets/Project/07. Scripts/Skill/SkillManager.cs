using System.Collections.Generic;
using UnityEngine;


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
        //CacheReferences();
        //InitializeSkillRuntimes();
    }

    void Start()
    {
        CacheReferences();
        InitializeSkillRuntimes();

        attackStats = GetComponent<AttackStats>();
        if (attackStats == null) attackStats = FindFirstObjectByType<AttackStats>();

        //전사 베기가 5번칸에 있으므로 5로 설정
        if (allSkills != null && allSkills.Count > 5 && allSkills[5] != null)
        {
            // 전사 베기 스킬 데이터 원본을 런타임 주머니에 1레벨로 강제 생성
            if (skillRuntimes.TryGetValue(allSkills[5], out SkillRuntime warriorRuntime))
            {
                warriorRuntime.LevelUp(); //1레벨 시작
                if (HUDController.Instance != null)
                {
                    HUDController.Instance.UpdateSkillHUD(allSkills[5], warriorRuntime.CurrentLevel);
                }
            }
        }
    }

    // 비어있는 컴퍼넌트 자동 붙이기
    private void CacheReferences()
    {
        // SkillManager가 Player에 붙어 있으면 GetComponent로 찾을 수 있다.
        if (attackStats == null)
        {
            attackStats = GetComponent<AttackStats>();
        }

        // SkillManager가 GameManager 같은 별도 오브젝트에 있으면
        // Player 쪽 컴포넌트를 자동으로 찾아준다.
        if (attackStats == null)
        {
            attackStats = FindFirstObjectByType<AttackStats>();
        }
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

        //[추가] 실시간 HUD 동기화
        //스킬 레벨 오를때마다 HUD 갱신
        if (HUDController.Instance != null)
        {
            //리스트에 안전하게 들어있는지 확인하고, 오직 5, 6, 7번 칸의 스킬일 때만 HUD를 켜기
            if (allSkills.Count > 7 &&
               (selectedSkill == allSkills[5] || selectedSkill == allSkills[6] || selectedSkill == allSkills[7]))
            {
                HUDController.Instance.UpdateSkillHUD(selectedSkill, runtime.CurrentLevel);
            }
            else
            {
                Debug.Log($"[패시브 패스] {selectedSkill.SkillName}은 패시브이므로 HUD에 등록하지 않음");
            }
        }
    }

    private void ApplySkillEffect(
        SkillData skillData,
        int currentLevel)
    {
        if (skillData == null)
        {
            return;
        }

        SkillEffectType effectType = skillData.EffectType;

        float value = skillData.EffectValuePerLevel;

        // EffectType이 None이면 레벨만 올리고 실제 스탯은 건드리지 않는다.
        // 전사 스킬, 파이어볼 같은 액티브 스킬은 각 스크립트가 현재 레벨을 확인해서 동작한다.
        if (effectType == SkillEffectType.None)
        {
            Debug.Log(
                $"{skillData.SkillName}: 별도 스탯 적용 없음 / Lv.{currentLevel}",
                this
            );

            return;
        }

        if (attackStats == null)
        {
            return;
        }

        switch (effectType)
        {
            case SkillEffectType.AttackDamageFlat:
                // 공격력을 고정 수치만큼 증가시킨다.
                attackStats.IncreaseAttackDamage(value);
                break;

            case SkillEffectType.AttackDamagePercent:
                // 공격력을 퍼센트로 증가시킨다.
                // 예: value가 10이면 공격력 10% 증가.
                attackStats.IncreaseAttackDamagePercent(value);
                break;

            case SkillEffectType.AttackSpeedFlat:
                // 공격속도를 고정 수치만큼 증가시킨다.
                attackStats.IncreaseAttackSpeed(value);
                break;

            case SkillEffectType.AttackRangeFlat:
                // 공격 사거리를 고정 수치만큼 증가시킨다.
                attackStats.IncreaseAttackRange(value);
                break;

            case SkillEffectType.CriticalChanceFlat:
                // 치명타 확률을 증가시킨다.
                // AttackStats 내부에서 0~1 사이로 제한한다.
                attackStats.IncreaseCriticalChance(value);
                break;

            case SkillEffectType.CriticalMultiplierFlat:
                // 치명타 배율을 증가시킨다.
                attackStats.IncreaseCriticalMultiplier(value);
                break;

            case SkillEffectType.SkillCooldownReductionPercent:
                // 전체 스킬 쿨타임 감소 패시브.
                // 파이어볼 같은 액티브 스킬의 최종 쿨타임 계산에 사용된다.
                attackStats.ReduceSkillCooldownPercent(value);
                break;
            case SkillEffectType.MovingSpeedFlat:
                // 이동속도 증가
                attackStats.IncreaseMovingSpeed(value);
                break;
        }

        Debug.Log(
        $"{skillData.SkillName} 효과 적용 완료 / 타입: {effectType}, 값: {value}, 현재 레벨: {currentLevel}",
        this
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


