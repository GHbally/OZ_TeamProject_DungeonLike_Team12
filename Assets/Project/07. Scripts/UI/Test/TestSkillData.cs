using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skill/Skill Data")]
public class SkillData : ScriptableObject
{
    // [SerializeField]를 사용하여 인스펙터에서 수정 가능하게 함
    [SerializeField] private string skillName;
    [SerializeField] private string description;
    [SerializeField] private Sprite icon; // 아이콘 변수 추가

    // 외부에서 읽을 수 있도록 public 프로퍼티 생성
    public string SkillName => skillName;
    public string Description => description;
    public Sprite Icon => icon; // 대문자 I로 시작
}