using UnityEngine;

/*
[0617] 수정
 - DamageInfo1 수정 (몬스터와 맞춰서 넣어야 함)
 */

public class AttackStats : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackDamage = 50f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 6f;

    [Header("Critical")]
    [Range(0f, 1f)]
    [SerializeField] private float criticalChance = 0.05f;
    [SerializeField] private float criticalMultiplier = 2f;

    public float AttackDamage => attackDamage;
    public float AttackSpeed => attackSpeed;
    public float AttackRange => attackRange;
    public float CriticalChance => criticalChance;
    public float CriticalMultiplier => criticalMultiplier;

    public float GetAttackInterval()
    {
        return 1f / Mathf.Max(attackSpeed, 0.01f);
    }

    public DamageInfo1 CreateDamageInfo(GameObject attacker)
    {
        bool isCritical = Random.value < criticalChance;

        float finalDamage = isCritical
            ? attackDamage * criticalMultiplier
            : attackDamage;

        return new DamageInfo1(finalDamage, isCritical, attacker);
    }
}
