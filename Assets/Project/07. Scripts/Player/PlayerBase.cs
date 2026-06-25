//[플레이어 코어 부모 클래스]
using UnityEngine;
using UnityEngine.UI; // UI 사용을 위해 추가(김영웅 수정)

public class PlayerBase : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private Slider hpSlider; // 인스펙터에서 드래그할 슬라이더(김영웅 수정)

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

    private SpriteRenderer[] childRenderers; //스켈레탈 파츠 자식들을 다 담아줄 배열

    //상속받을 자식(직업)들이 Override할 수 있게 protected
    protected virtual void Start()
    {
        currentHp = maxHp;                          //게임시작 후 현재체력 최대로

        if (hpSlider != null)//슬라이더 초기 값 설정(김영웅 수정)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }

        movement = GetComponent<PlayerMovement>();  //PlayerMovement 스크립트 연결

        Transform visual = transform.Find("Visual");    //자식 Visual 찾기
        if (visual != null)
        {
            animator = visual.GetComponentInChildren<Animator>(); ;         //애니메이터 빼옴

            childRenderers = visual.GetComponentsInChildren<SpriteRenderer>();
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

        Debug.Log("플레이어 피격됨");
        //위가 아니면 데미지만큼 현재 체력 감소
        currentHp -= damage;

        //체력 계산 도중 현재HP 범위 제한(0 ~ maxHp)
        //Clamp(현재 HP, 최소값0, 최대값 최대HP)
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        // [추가] 슬라이더 값 갱신(김영웅 수정)
        if (hpSlider != null)
        {
            hpSlider.value = currentHp;
        }

        //묶어 놓은 자식 스프라이트들이 존재하면 피격 코루틴
        if (childRenderers != null && childRenderers.Length > 0)
        {
            //중복 피격 시 색상 꼬임 방지
            StopAllCoroutines();
            //피격시 빨갛게 되는 효과
            StartCoroutine(HurtFlashCo());
        }

        if (currentHp <= 0)
        {
            //체력 0이하 사망
            Death();
        }
    }

    
    ///////////////////////// 체력 회복 추가////////////////////////////////
   
    public void Heal(float healAmount)
    {
        // 죽은 상태면 회복 불가
        if (IsDead)
            return;

        // 체력 회복
        currentHp += healAmount;

        // 최대 체력을 넘지 않도록 제한
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        // HP UI 갱신
        if (hpSlider != null)
        {
            hpSlider.value = currentHp;
        }

        Debug.Log($"체력 {healAmount} 회복 / 현재 HP : {currentHp}");
    }

    private System.Collections.IEnumerator HurtFlashCo()
    {
        //쪼개져있는 스프라이트들 전부 빨간색으로
        foreach (SpriteRenderer sr in childRenderers)
        {
            if (sr != null)
            {
                string objName = sr.gameObject.name.ToLower();
                //그림자와 눈은 색상에서 제외
                if (objName.Contains("shadow")) continue;

                if (objName.Contains("eye") || IsParentContainsName(sr.transform, "eye")) continue;
                sr.color = Color.red;
            }   
        }
 
        yield return new WaitForSeconds(0.1f);  //0.1초 동안 유지

        //원래색상 복귀
        foreach (SpriteRenderer sr in childRenderers)
        {
            if (sr != null) 
            {
                string objName = sr.gameObject.name.ToLower();
                if (objName.Contains("shadow")) continue;

                if (objName.Contains("eye") || IsParentContainsName(sr.transform, "eye")) continue;
            }
            sr.color = Color.white;
        }
    }

    //[사망 메서드]
    protected virtual void Death()
    {
        if (IsDead) return; //중복 사망 방지
        IsDead = true;

        Debug.Log($"캐릭터 사망");

        //사망하면 빨갛게 변했던 색을 원래대로 돌린 후 애니메이션 재생
        if (childRenderers != null)
        {
            foreach (SpriteRenderer sr in childRenderers)
            {
                if (sr != null)
                {
                    string objName = sr.gameObject.name.ToLower();
                    if (objName.Contains("shadow")) continue;

                    if (objName.Contains("eye") || IsParentContainsName(sr.transform, "eye")) continue;
                    sr.color = Color.white;
                }
            }
        }

        if (movement != null)
        {
            movement.StopAllCoroutines();
            movement.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("1_Move", false);
            //사망 애니메이션 재생
            animator.SetTrigger("4_Death");
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
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        Collider2D col = GetComponent<Collider2D>();
        {
            if (col != null)
            {
                col.enabled = false;
                Debug.Log("시체 충돌 비활성화");
            }
        }


        AutoAttackController autoAttack = GetComponent<AutoAttackController>();
        if (autoAttack != null)
        {
            //사망 시 자동 공격 정지
            autoAttack.StopAttack();
        }
    }

    //내 상위 부모들을 타고 올라가며 이름에 eye가 있는 애들 찾아줄 메서드
    private bool IsParentContainsName(Transform current, string targetName)
    {
        Transform parent = current.parent;

        //Visual까지 타고 올라가면서
        while(parent != null && parent.name != "Visual" && parent.name != gameObject.name)
        {
            //지정해둔 이름의 찾음(eye찾을 것들)
            if (parent.name.ToLower().Contains(targetName))
            {
                return true;
            }
            //한 단계 위쪽 부모 주소로 이동
            parent = parent.parent;
        }
        //끝까지 올라갔는데도 없으면 해당 파츠가 아니므로 false 반환
        return false;
    }

    [ContextMenu("강제 사망 테스트")]
    public void TestDeathButton()
    {
        // 내 체력을 0으로 만들고 사망 메서드를 직접 실행
        TakeDamage(maxHp);
    }
}
