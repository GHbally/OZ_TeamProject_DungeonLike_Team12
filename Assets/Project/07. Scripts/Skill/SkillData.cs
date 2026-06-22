using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill/Skill Data")]

// 스킬 이름, 설명, 아이콘, 최대 레벨, 스킬 레벨업 시 설명
public class SkillData : ScriptableObject
{
    // [SerializeField]를 사용하여 인스펙터에서 수정 가능하게 함
    [SerializeField] private string skillName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon; // 아이콘 변수 추가
    [SerializeField] private int maxLevel = 5;
    [SerializeField] private string[] levelUpDescriptions;

    // 외부에서 읽을 수 있도록 public 프로퍼티 생성
    public string SkillName => skillName;
    public string Description => description;
    public Sprite Icon => icon; // 대문자 I로 시작
    public int MaxLevel => maxLevel;


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