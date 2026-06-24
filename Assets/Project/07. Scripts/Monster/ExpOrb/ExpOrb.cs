using UnityEngine;

public class ExpOrb : MonoBehaviour
{
    [Header("경험치 설정")]
    public int expAmount = 10;

    private PlayerLevelSystem playerLevelSystem;

    //자석용 변수
    private Transform targetPlayer;     //날아갈 목표(플레이어)
    private float moveSpeed;            //자석으로 당겨지는 속도
    private bool isPulled = false;      //자석에 당겨지는 중인지 체크하는 스위치
    private bool isEaten = false;       //중복 흡수 및 풀링 반환 버그 방지용 스위치


    private void OnEnable()
    {
        //자석 상태 정보 청소
        isPulled = false;
        isEaten = false;
        targetPlayer = null;

        //풀에서 나올 때마다 하이어라키에서 살아있는 플레이어의 레벨 시스템 주소 새로 획득
        PlayerBase player = FindFirstObjectByType<PlayerBase>();
        if (player != null)
        {
            playerLevelSystem = player.GetComponent<PlayerLevelSystem>();
        }
    }

    //자석 기능용 메서드
    public void MoveToPlayer(Transform playerTarget, float speed)
    {
        if (isPulled) return;   //이미 날아가는 중이면 중복 연산 방지

        targetPlayer = playerTarget;
        moveSpeed = speed;
        isPulled = true;
    }

    private void Update()
    {
        //자석 범위 안에 걸리면 매 프레임 플레이어에게 돌진
        if (isPulled && targetPlayer != null)
        {
            //MoveTowards로 부드럽게 날아가기
            transform.position = Vector2.MoveTowards(
                transform.position, 
                targetPlayer.position, 
                moveSpeed * Time.deltaTime
                );

            //플레이어 중심점과 보석 사이 실제 거리 계산
            float distance = Vector2.Distance(transform.position, targetPlayer.position);

            if (distance <= 0.5f)
            {
                EatExpOrb();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //이미 먹힌 상태면 패스
        if (isEaten) return;
        //플레이어만 먹기 가능
        if (!other.CompareTag("Player")) return;

        EatExpOrb();
    }

    private void EatExpOrb()
    {
        if (isEaten) return;
        isEaten = true;

        if (playerLevelSystem != null)
        {
            playerLevelSystem.EarnExp(expAmount);
            Debug.Log($"경험치 {expAmount} 획득 (자석 흡수)");
        }
        else
        {
            Debug.LogError("경험치 못먹음");
        }
        ReturnPool();
    }
    void ReturnPool()
    {
        PoolManager.Instance.ReturnExpOrb(gameObject);
    }
}
