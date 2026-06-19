using UnityEngine;
using System.Collections;

/************************************************
 * MidBossMonster
 *
 * 플레이어를 추적하다가 공격 범위에 진입하면
 * 돌진 공격을 수행하는 중간보스
 *
 * 기능
 * - 플레이어 추적
 * - 돌진 공격
 * - 접촉 데미지
 * - 대기 패턴
 * - 사망 처리
 ************************************************/


public class MidBossMonster : MonsterBase
{
    [Header("보스 설정")]

    public float detectRange = 8f; //공격 시작 거리
    public float dashSpeed = 10f; //돌진 속도
    public float dashDuration = 0.5f; //돌진 지속시간
    public float waitTime = 2f; //돌진 후 대기 시간
    public float damage = 30f; //돌진 데미지
    private bool isDashing; //현재 돌진중인지 확인
    private bool isWaiting; //현재 대기 중인지 확인

    [Header("접촉 데미지")]
    public float touchDamage = 20f; //접촉하고 있을대 데미지
    public float damageInterval = 1f; // 1초마다 데미지

    private float damageTimer;


    ///////////////////////몬스터 활성화//////////////////////
    protected override void OnEnable()
    {
        base.OnEnable(); //부모 클래스 초기화

        damageTimer = damageInterval; //접촉 데미지 타이머 초기화
    }


    /*******************
    플레이어 존재 확인 -> 돌진?대기? -> (예스 -> 종료)아니요-> 거리 계산 -> detectRange안
    -> Attack -> detectRange 밖 -> Chase
     ********************/
    protected override void UpdateState() //추적 상태인지 공격상태인지
    {
        //플레이어가 없으면 종료
        if(player == null)return;

        //플레이어와 보스 사이의 거리 계산
        float distance = Vector2.Distance(transform.position, player.position);

        //돌진중도 아니고 대기중도 아닐때
        if (!isDashing && !isWaiting)
        {
            //플레이어가 감지 범위 안에 있으면 공격상태
            if (distance <= detectRange)
            {
                currentState = MonsterState.Attack;
            }
        }

        //아니면 추적상태
        else 
        {
            currentState = MonsterState.Chase;
        }
    }
   
    protected override void AttackLogic() // 공격 상태
    {
        // 돌진 또는 대기 중이 아닐 때만 실행
        if (!isDashing && !isWaiting)
        {
            StartCoroutine(DashPattern());
        }
    }

    IEnumerator DashPattern() //돌진패턴
    {
        // 돌진 시작
        isDashing = true;

        // 돌진 시작 시점 플레이어 방향 저장
        Vector2 dir =((Vector2)player.position - (Vector2)transform.position).normalized;

        float timer = 0f;

        // 설정된 시간 동안 돌진
        while (timer < dashDuration)
        {
            rb.MovePosition(rb.position +dir * dashSpeed * Time.fixedDeltaTime);

            timer += Time.deltaTime;

            yield return null;
        }

        // 돌진 종료
        isDashing = false;

        // 대기 시작
        isWaiting = true;

        yield return new WaitForSeconds(waitTime);

        // 대기 종료
        isWaiting = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 돌진 중일 때만 공격 판정
        if (!isDashing) return;

        // 플레이어가 아니면 무시
        if (!other.CompareTag("Player")) return;

        // PlayerBase 가져오기
        PlayerBase playerBase = other.GetComponent<PlayerBase>();

        if (playerBase != null)
        {
            // 플레이어에게 데미지 전달
            playerBase.TakeDamage(damage);

            Debug.Log("중간보스 돌진 공격 적중");
        }
    }

    //플레이어와 접촉중
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        damageTimer += Time.deltaTime;

        //일정 시간마다 데미지
        if (damageTimer >= damageInterval)
        {
            damageTimer = 0f;

            PlayerBase playerbase = other.GetComponent<PlayerBase>();

            if (playerbase != null)
            {
                playerbase.TakeDamage(touchDamage);
                Debug.Log("중간보스 접촉 데미지");
            }
        }
    }

    //플레이어가 벗어나면 타이머 초기하ㅓ
    private void OnTriggerExit2D(Collider2D other)
    {
       if(!other.CompareTag("Player")) return;
        damageTimer = damageInterval;
    }


    /***********************
    상태를 Dead로 변경 -> 돌진 코루틴 정지 -> 경험치 드랍
    -> WaveManager에 사망 알림 -> 오브젝트 제거
     ************************/
    protected override void Death()
    {
        currentState = MonsterState.Dead;

        // 돌진 코루틴 정지
        StopAllCoroutines();

        Debug.Log("중간보스 처치");

        // 경험치 드랍
        if (DropManager.Instance != null)
        {
            DropManager.Instance.DropExp(transform.position);
        }

        // WaveManager에 보스 사망 알림
        FindFirstObjectByType<WaveManager>() ?.MonsterDead();

        // 보스 제거
        Destroy(gameObject);
    }
}
