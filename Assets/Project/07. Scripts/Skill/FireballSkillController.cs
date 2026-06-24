using UnityEngine;

// 파이어볼 스킬 발사를 담당하는 컨트롤러.
// Player 오브젝트에 붙여서 사용한다.
// 실제 파이어볼 오브젝트는 FireballProjectile이 담당하고,
// 이 스크립트는 "언제 발사할지"만 관리한다.

public class FireballSkillController : MonoBehaviour
{
    [Header("필수 참조")]
    [SerializeField] private SkillManager skillManager;
}
