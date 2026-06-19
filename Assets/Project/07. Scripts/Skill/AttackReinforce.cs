using UnityEngine;

public class AttackReinforce : MonoBehaviour
{
    [SerializeField]
    private AttackStats attackStats;
    private AutoAttackController controller;

    private void Awake()
    {
        // 같은 GameObject에 있는 AttackStats 자동 검색
        if (attackStats == null)
        {
            attackStats = GetComponent<AttackStats>();
        }

        if (attackStats == null)
        {
            Debug.LogError($"{name}: AttackStats를 찾을 수 없습니다.",this);
        }
    }
    private void Update()
    {
        if (attackStats == null)
        {
            return;
        }

        // 숫자 1: 공격력 10 증가
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            attackStats.IncreaseAttackDamage(10f);
        }

        // 숫자 2: 공격 속도 0.5 증가
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            attackStats.IncreaseAttackSpeed(0.5f);
        }

        // 숫자 3: 공격 범위 1 증가
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            attackStats.IncreaseAttackRange(1f);
        }

        // 숫자 4: 치명타 확률 10% 증가
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            attackStats.IncreaseCriticalChance(0.1f);
        }

        // 숫자 5: 치명타 피해 배율 0.5 증가
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            attackStats.IncreaseCriticalMultiplier(0.15f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            controller.StopAttack();
        }
    }
}
