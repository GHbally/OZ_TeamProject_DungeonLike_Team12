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
        if(attackStats == null)
        {
            attackStats = GetComponent<AttackStats>();
        }
        InitializeSkillRuntimes();
    }

    // allSkills에 등록된 모든 스킬의 런타임 정보를 미리 만든다.
    // 처음에는 CurrentLevel이 0인 상태로 시작
    private void InitializeSkillRuntimes()
    {
        skillRuntimes.Clear();

        for(int i= 0; i< skillRuntimes.Count; i++)
        {
            SkillData skillData = allSkills[i];
            if(skillData == null)
            {
                continue;
            }

            // 같은 skillData가 중복 등록되어 있으면 한 번만 추가.
            if (skillRuntimes.ContainsKey(skillData))
            {
                continue;
            }

            skillRuntimes.Add(skillData,new SkillRuntime(skillData));
        }
    }

    

    public void OnSkillSelected(string skillName)
    {
        Debug.Log(skillName + " 선택됨!");
        // 여기에 스킬 적용 로직을 작성
    }

    // UI 담당자분 께서 UI를 열 때 호출할 함수
    // count 개수만큼 랜덤 스킬 카드 후보를 만들어 반환
    // 예시 List<SkillCardInfo> cards = skillManager.GetRandomSkillCards(3);
    public List<SkillCardInfo> GetRandomSkillCards(int count)
    {
        // 최대 레벨이 아닌 스킬만 후보로 가져온다.
        List<SkillData> availableSkills = GetAvailableSkills();

        // 후보 목록을 무작위로 섞는다.
        Shuffle(availableSkills);

        List<SkillCardInfo> result = new();

        // 요청한 개수보다 후보가 적으면 작은 값을 사용한다.
        int cardCount = Mathf.Min(count,availableSkills.Count);

        for(int i =0; i<cardCount; i++)
        {
            SkillData skillData = availableSkills[i];

            SkillCardInfo cardInfo = CreateSkillCardInfo(skillData);

            result.Add(cardInfo);
        }

        return result;
    }

    public void ApplySkillChoice(SkillData selectedSkill)
    {
        if(selectedSkill == null)
        {
            Debug.LogWarning("선택된 스킬이 null 입니다.");
            return;
        }

        // 등록되지 않은 스킬이 들어와도 런타임 정보를 새로 만들어 처리
        if(!skillRuntimes.TryGetValue(selectedSkill, out SkillRuntime runtime))
        {
            runtime = new SkillRuntime(selectedSkill);
            skillRuntimes.Add(selectedSkill,runtime);
        }

        // 최대 레벨이면 더 이상 적용 X
        if (runtime.IsMaxLevel)
        {
            Debug.Log($"{selectedSkill.SkillName}은 이미 최대 레벨입니다.");
            return;
        }

        runtime.LevelUp();

        Debug.Log($"{selectedSkill.SkillName} 선택됨 / Lv.{runtime.CurrentLevel}");
    }

    // 현재 카드 후보로 나올 수 있는 스킬 목록을 만든다.
    // 최대 레벨이 된 스킬은 후보에서 제외.
    private List<SkillData> GetAvailableSkills()
    {
        List<SkillData> result = new();

        for(int i=0;i<allSkills.Count;i++)
        {
            SkillData skilldata = allSkills[i];

            // 만약 런타임 정보가 없다면 후보에 포함
            if(skilldata == null)
            {
                continue;
            }
            if(!skillRuntimes.TryGetValue(skilldata, out SkillRuntime runtime))
            {
                result.Add(skilldata);
                continue;
            }

            if(!runtime.IsMaxLevel)
            {
                result.Add(skilldata);
            }
        }
        return result;
    }

    // SkillData를 UI 표시 용 SkillCardInfo로 변환
    private SkillCardInfo CreateSkillCardInfo(SkillData skillData)
    {
        int currentLevel = GetCurrentLevel(skillData);

        // 현재 레벨이 0이면 다음 레벨은 1.
        // 이미 보유 중이면 현재 레벨 + 1.
        int nextLevel = Mathf.Clamp(currentLevel + 1, 1, skillData.MaxLevel);

        // 다음 레벨에 해당하는 설명을 가져온다.
        string description = skillData.GetLevelUpDescription(nextLevel);

        return new SkillCardInfo(
            skillData,
            skillData.SkillName,
            description,
            skillData.Icon,
            currentLevel,
            nextLevel,
            skillData.MaxLevel);
    }

    // 특정 스킬의 현재 레벨을 반환한다.
    public int GetCurrentLevel(SkillData skillData)
    {
        if(skillData == null)
        {
            return 0;
        }
        if(!skillRuntimes.TryGetValue(skillData, out SkillRuntime runtime)){
            return 0;
        }
        return runtime.CurrentLevel;
    }

    // 무작위로 섞는 함수
    private void Shuffle(List<SkillData> list)
    {
        for(int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            SkillData temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }


}