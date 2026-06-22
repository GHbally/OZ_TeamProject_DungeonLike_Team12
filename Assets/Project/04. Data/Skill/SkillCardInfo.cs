using JetBrains.Annotations;
using UnityEngine;

public class SkillCardInfo
{
    public SkillData SkillData { get; }
    public string Name { get; }
    public string Description { get; }
    public Sprite Icon { get; }
    public int CurrentLevel { get; }
    public int MaxLevel { get; }

    public SkillCardInfo(
        SkillData skilldata,
        string name,
        string description,
        Sprite icon,
        int currentLevel,
        int maxLevel
        )
    {
        SkillData = skilldata;
        Name = name;
        Description = description;
        icon = Icon;
        CurrentLevel = currentLevel;
        MaxLevel = maxLevel;
    }
}
