using UnityEngine;

public class WarriorMonster : MonsterBase
{
    [Header("근접 공격")]
    public float damage = 10.0f;        //공격력
    public float damageInterval = 1.0f; //1초마다 데미지 들어감
    private float damageTimer;          //공격 타이머

    //오브젝트 풀에서 몬스터가 스폰될때 발동
    protected override void OnEnable()
    {
        //부모의 체력, 플레이어 추적 받아오기
        base.OnEnable();
        //비비는 데미지값 받아오기
        damageTimer = damageInterval;
    }

    //캐릭터 거리 두고 상태 전환
    protected override void UpdateState()
    {
        //워리어는 비벼서 공격하므로 비워둠
    }

    //매 프레임마다 원거리 공격 시간
    protected override void AttackLogic()
    {
        //워리어는 필요없음
    }

    //비벼지는 동안
    private void OnTriggerStay2D(Collider2D other)
    {
        //대상이 플레이어가 아니면 무시
        if (!other.CompareTag("Player")) return;

        damageTimer += Time.deltaTime;

        //1초마다 데미지
        if (damageTimer >= damageInterval)
        {
            //타이머 0으로 리셋
            damageTimer = 0f;
            
            //PlayerBase.cs 불러오기 
            PlayerBase playerCore = other.GetComponent<PlayerBase>();

            if (playerCore != null)
            {
                //플레이어의 피격 함수에 전달
                playerCore.TakeDamage(damage);
            }         
        }
    }

    //플레이어가 몬스터 비비기 영역 탈출시
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        damageTimer = damageInterval; //타이머 충전

    }
}
