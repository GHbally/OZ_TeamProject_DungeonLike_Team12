using JetBrains.Annotations;
using UnityEngine;

public class SkillCardInfo
{
    public SkillData SkillData { get; }

    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }

    public int CurrentLevel { get; }
    public int NextLevel { get; }
    public int MaxLevel { get; }

    public SkillCardInfo(
        SkillData skillData,
        string name,
        string description,
        Sprite icon,
        int currentLevel,
        int nextLevel,
        int maxLevel)
    {
        SkillData = skillData;
        Name = name;
        Description = description;
        Icon = icon;
        CurrentLevel = currentLevel;
        NextLevel = nextLevel;
        MaxLevel = maxLevel;
    }
}
