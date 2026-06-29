using UnityEngine;

// 스킬 선택 시 실제로 어떤 능력치를 올릴지 구분하는 타입.
// 전사 스킬, 파이어볼처럼 별도 스크립트가 레벨을 확인해서 동작하는 스킬은 None을 사용한다.
public enum SkillEffectType
{
    None,

    AttackDamageFlat,
    AttackDamagePercent,
    AttackSpeedFlat,
    AttackRangeFlat,

    CriticalChanceFlat,
    CriticalMultiplierFlat,

    MaxHpFlat,

    SkillCooldownReductionPercent,

    MovingSpeedFlat
}

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill/Skill Data")]

// 스킬 이름, 설명, 아이콘, 최대 레벨, 스킬 레벨업 시 설명
public class SkillData : ScriptableObject
{
    [Header("기본 정보")]
    // [SerializeField]를 사용하여 인스펙터에서 수정 가능하게 함
    [SerializeField] private string skillName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon; // 아이콘 변수 추가
    [SerializeField] private int maxLevel = 5;
    [SerializeField] private string[] levelUpDescriptions;

    [Header("실제 적용 효과")]
    [SerializeField] private SkillEffectType effectType = SkillEffectType.None;

    // 스킬을 한 번 선택할 때마다 적용될 수치.
    [SerializeField] private float effectValuePerLevel = 0f;

    // 외부에서 읽을 수 있도록 public 프로퍼티 생성
    public string SkillName => skillName;
    public string Description => description;
    public Sprite Icon => icon; // 대문자 I로 시작
    public int MaxLevel => maxLevel;

    public SkillEffectType EffectType => effectType;
    public float EffectValuePerLevel => effectValuePerLevel;

    public bool isActiveSkill;
    public int SkillID;


    // 다음 레벨 설명 반환
    public string GetLevelUpDescription(int nextLevel)
    {
        // 레벨 설명이 비어있으면 기본 설명으로 대신 사용
        if(levelUpDescriptions == null||levelUpDescriptions.Length == 0)
        {
            return description;
        }
        // nextLevel은 1부터 시작하지만 배열 인덱스는 0부터 시작하므로 -1
        int index = Mathf.Clamp(nextLevel - 1, 0, levelUpDescriptions.Length - 1);

        return levelUpDescriptions[index];
    }
}