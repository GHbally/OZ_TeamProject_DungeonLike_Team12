using UnityEngine;
using System.Collections;

/************************
 중간보스 클래스
플레이어 추적 -> 돌진 -> 대기 반복

 **************************/
public class MidBossMonster : MonsterBase
{
    [Header("보스 설정")]

    //플레이어가 범위 안으로 들어오면 돌진 시작
    public float detectRange = 8f;
    //돌진 속도
    public float dashSpeed = 10f;
    //돌진 지속시간
    public float dashDuration = 0.5f;
    //돌진 후 정지 시간
    public float waitTime = 2f;
    //돌진 데미지
    public float damage = 30f;
    //현재 돌진중인지 확인
    private bool isDashing;
    //현재 대기 중인지 확인
    private bool isWaiting;

    [Header("접촉 데미지")]
    public float touchDamage = 15f;
    public float damageInterval = 1f;

    private float damageTimer;

    protected override void OnEnable()
    {
        base.OnEnable();

        damageTimer = damageInterval;
    }



    protected override void UpdateState()
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

    protected override void AttackLogic()
    {
        // 돌진 또는 대기 중이 아닐 때만 실행
        if (!isDashing && !isWaiting)
        {
            StartCoroutine(DashPattern());
        }
    }

    IEnumerator DashPattern()
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

    protected override void Death()
    {
        Debug.Log("중간보스 처치");

        // 부모 클래스 사망 처리
        base.Death();
    }
}
