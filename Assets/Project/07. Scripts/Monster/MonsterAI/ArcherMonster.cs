using UnityEngine;


//플레이어 발견 - 사거리 밖 - 추적
//사거리 진입 - 정지 - 공격
//플레이어 멀어짐 - 다시 추적

public class ArcherMonster : MonsterBase
{
    [Header("원거리 몬스터 설정")]
    public float attackRange = 7f; //사거리
    public float attackCooldown = 2f;   //공격주기
    private float attackTimer;  //연사 속도 타이머

    public float attackPreDelay = 0.5f; //선공격 딜레이

    protected override void OnEnable()
    {
        base.OnEnable();    //부모의 체력, 플레이어 추적 받아오기
        attackTimer = 0f;   //화살 타이머 0으로 시작하게 세팅
    }


    //사거리별 상태변화 로직
    protected override void UpdateState()
    {
        // 플레이어가 없으면 종료
        if (player == null) return;

        // 메인 카메라 가져오기
        Camera cam = Camera.main;

        // 카메라가 없으면 추적 상태
        if (cam == null)
        {
            currentState = MonsterState.Chase;
            return;
        }

        // 플레이어와 몬스터를 화면 좌표(Viewport)로 변환
        Vector3 playerViewportPos = cam.WorldToViewportPoint(player.position);
        Vector3 monsterViewportPos = cam.WorldToViewportPoint(transform.position);

        // 플레이어 판정
        // X는 화면 안(0~1)
        // Y는 화면 아래(-)만 막고 위쪽은 제한하지 않는다.
        bool isPlayerInRange =
            playerViewportPos.x >= 0f && playerViewportPos.x <= 1f &&
            playerViewportPos.y >= 0f &&
            playerViewportPos.z > 0f;

        // 몬스터는 반드시 화면 안에 있어야 함
        bool isMonsterInCamera =
            monsterViewportPos.x >= 0f && monsterViewportPos.x <= 1f &&
            monsterViewportPos.y >= 0f && monsterViewportPos.y <= 1f &&
            monsterViewportPos.z > 0f;

        // 몬스터가 화면 안에 있고,
        // 플레이어가 화면 위쪽이나 화면 안에 있으면 공격
        if (isMonsterInCamera && isPlayerInRange)
        {
            currentState = MonsterState.Attack;
        }
        else
        {
            currentState = MonsterState.Chase;
        }
    }

    //원거리 공격
    protected override void AttackLogic()
    {
        attackTimer += Time.deltaTime;  //화살 쿨타임 타이머에 시간을 계속 더해줌

        //쿨타임 마다
        if (attackTimer >= attackCooldown)
        {
            attackTimer = 0f;       //타이머 0으로 초기화하고
            StartAttackMotion();    //공격 모션
        }
    }

    void StartAttackMotion()
    {
        if (animator != null)
        {
            animator.SetTrigger("6_Other");

            Invoke(nameof(Shoot), attackPreDelay);
        }

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySFX("SFX_StoneThrowerv1releasewav");
        }

    }

    //투사체 발사
    void Shoot()
    {
        //오브젝트 풀링으로 화살 가져오기
        GameObject arrow = PoolManager.Instance.GetArrow();

        if (arrow == null) return;  //화살이 있을때만 실행

        //화살 생성 위치
        arrow.transform.position = transform.position + new Vector3(0, 0.5f, 0);

        Vector3 targetPosition = player.position + new Vector3(0, 0.6f, 0);

        //플레이어 방향 계산
        Vector2 dir = ((Vector2)player.position - (Vector2)transform.position).normalized;

        arrow.GetComponent<Arrow>().Initialized(dir);

        //Debug.Log("원거리 몬스터 발사");
    }
}
