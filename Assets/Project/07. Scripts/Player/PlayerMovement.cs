//[플레이어 이동 관리 클래스]
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("이동속도")]
    [SerializeField] private float moveSpeed = 5.0f; //이동 속도

    //[읽기, 쓰기] 외부에서 내 MoveSpeed를 수정할 수 있게 열기
    //이동 속도는 외부적인 스펙업을 통해 바뀔 여지가 있으므로 읽기+쓰기
    public float MoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    [Header("대쉬")]
    [SerializeField] private float dashSpeed = 15.0f;   //대쉬 속도
    [SerializeField] private float dashDuration = 0.2f; //대쉬 지속 시간
    [SerializeField] private float dashCooldown = 1.0f; //대쉬 쿨

    private Rigidbody2D rb;                 //리지드바디
    private Vector2 moveVec;                //입력받은 X, Y축 이동방향 값 저장할 벡터 변수
    private Transform visualTransform;      //캐릭터 스프라이트를 담고 있는 자식 위치 저장용 변수
    private Animator animator;              //애니메이션 제어용 변수

    private bool isDashing = false;         //현재 대쉬중인지
    private bool canDash = true;            //지금 대쉬 사용할 수 있는 상태인지
    private Vector2 dashDirection;          //대쉬 시작한 시점의 이동 방향 저장

    //현재 캐릭터가 움직이고 있는지 확인하기 위해 이동 벡터 길이를 제곱한 값을 넘겨주는 프로퍼티
    //멈추면 0, 움직이면 0보다 큼
    public float CurrentSpeed => moveVec.sqrMagnitude;

    public bool IsInvincible => isDashing;  //대쉬중 무적 판정 해줄 프로퍼티

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();               //리지드바디 할당
        visualTransform = transform.Find("Visual");     //자식 중 Visual인 오브젝트 위치 정보 할당
        //만약 Visual을 찾았으면 실행
        if (visualTransform != null)
        {
            //Visual이 가지고 있는 Animator컴포넌트를 가져와 animator에 할당
            animator = visualTransform.GetComponentInChildren<Animator>();
        }
    }

    void Update()
    {
        //대쉬중엔 조작 무시
        if (isDashing) return;

        float x = Input.GetAxisRaw("Horizontal");   //AD 좌우 이동
        float y = Input.GetAxisRaw("Vertical");     //WS 상하 이동

        moveVec = new Vector2(x, y).normalized;     //대각선이동 1로 고정시키기

        //D 누를때
        if (x > 0)
        {
            //오른쪽 보게 만들기
            visualTransform.localScale = new Vector3(-1, 1, 1);
        }
        //A 누를때
        else if (x < 0)
        {
            //왼쪽 보게 만들기
            visualTransform.localScale = new Vector3(1, 1, 1);
        }

        //애니메이터 컴포넌트가 잘 있다면
        if (animator != null)
        {
            if (moveVec != Vector2.zero)
            {
                //움직이고 있다면 에셋 내부 변수인 "1_Move" 켜기
                animator.SetBool("1_Move", true);
            }
            else
            {
                //멈췄다면 "1_Move"를 끄기
                animator.SetBool("1_Move", false);
            }
        }

        //대쉬키(스페이스바)를 누르고, 현재 멈춰있지 않고 움직이는 상태일 때 실행
        if (Input.GetKeyDown(KeyCode.Space) && canDash && moveVec != Vector2.zero)
        {
            //대쉬 코루틴 실행
            StartCoroutine(DashCo());
        }
    }
    private void FixedUpdate()
    {
        //대쉬중이면
        if (isDashing)
        {
            //대쉬 방향이랑 속도로 이동
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else
        {
            //아니면 그냥 일반 이동
            rb.linearVelocity = moveVec * moveSpeed;
        }
    }

    //[대쉬 코루틴]
    private System.Collections.IEnumerator DashCo()
    {
        canDash = false;    //대쉬 썼으니 쿨타임 상태로
        isDashing = true;   //대쉬 중 켜기

        //대쉬 도중 방향 고정
        dashDirection = moveVec;

        //dashDuration 대쉬 지속시간 동안 잠깐 대기 시키며 대쉬 속도 유지
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;  //대쉬 중 끄기

        //대쉬가 끝났을때 물리속도를 순간적으로 0으로 만들어 미끄러짐을 방지해줌
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        //대쉬가 끝난 순간 키보드 뗐을 경우를 대비해 애니메이션도 Idle(0)상태로 강제 전환
        if (animator != null)
        {
            animator.Play("IDLE");
        }

        //대쉬 쿨타임
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;     //대쉬 사용 가능
    }
}
