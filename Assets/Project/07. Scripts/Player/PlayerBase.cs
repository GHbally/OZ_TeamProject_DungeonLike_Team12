//[플레이어 코어 부모 클래스]
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [Header("플레이어 스탯")]
    [SerializeField] private float maxHp = 100.0f;  //최대 HP
    private float currentHp;                        //현재 체력

    //[읽기 전용] 최대체력, 현재체력 다른 스크립트에서 쓰세용 (람다식 프로퍼티)
    //피통은 다른쪽에서 건드리면 위험해서 오직 여기서만 처리
    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    //캐릭터 사망 상태 스위치
    public bool IsDead { get; private set; } = false;

    //플레이어 움직임이랑 애니메이터 조작을 위해 가져올 컴포넌트 변수들
    private PlayerMovement movement;
    private Animator animator;

    //상속받을 자식(직업)들이 Override할 수 있게 protected
    protected virtual void Start()
    {
        currentHp = maxHp;                          //게임시작 후 현재체력 최대로

        movement = GetComponent<PlayerMovement>();  //PlayerMovement 스크립트 연결

        Transform visual = transform.Find("Visual");    //자식 Visual 찾기
        if (visual != null)
        {
            animator = visual.GetComponent<Animator>(); //Visual 있으면 애니메이터 컴포넌트 빼옴
        }
    }

    //[피격 메서드]
    //virtual 붙여서 자식이 오버라이딩 할 수 있게 해줌
    public virtual void TakeDamage(float damage)
    {
        if (IsDead) return; //이미 죽었으면 피격계산이 필요 없으므로 메서드 종료

        //PlayerMovement가 작동 중이고 대쉬 상태일때
        if (movement != null && movement.IsInvincible)
        {
            return; //데미지 계산X -> 무적
        }
        //위가 아니면 데미지만큼 현재 체력 감소
        currentHp -= damage;

        //체력 계산 도중 현재HP 범위 제한(0 ~ maxHp)
        //Clamp(현재 HP, 최소값0, 최대값 최대HP)
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        if (animator != null)
        {
            //피격 애니메이션 재생용
            animator.SetTrigger("Hurt");
        }

        if (currentHp <= 0)
        {
            //체력 0이하 사망
            Die();
        }
    }

    //[사망 메서드]
    protected virtual void Die()
    {
        if (IsDead) return; //중복 사망 방지

        IsDead = true;

        if (animator != null)
        {
            //사망 애니메이션 재생용
            animator.SetTrigger("Die");
        }

        if(movement != null)
        {
            //사망 후 이동 스크립트 Off
            movement.enabled = false;

            //물리 힘으로 미끄러지는 버그 방지용으로 리지드바디 가져온 후
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                //물리적 이동 즉시 0으로 처리해서 제자리 고정
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
