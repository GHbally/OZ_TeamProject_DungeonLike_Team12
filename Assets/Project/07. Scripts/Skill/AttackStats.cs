using UnityEngine;

/*
[0617] 수정
 - DamageInfo1 수정 (몬스터와 맞춰서 넣어야 함)
 */

public class AttackStats : MonoBehaviour
{
    [Header("Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float attackRange = 2f;

    [Header("Critical")]
    [SerializeField] private float criticalChance = 0.05f;
    [SerializeField] private float criticalMultiplier = 2f;

    [Header("CoolTime")]
    [SerializeField] float skillCoolMultiplier = 1f;

    public float AttackDamage => attackDamage;
    public float AttackSpeed => attackSpeed;
    public float AttackRange => attackRange;
    public float CriticalChance => criticalChance;
    public float CriticalMultiplier => criticalMultiplier;
    
    // 스킬 쿨
    // 1.0 = 감소 없음
    // 0.8 = 모든 스킬 쿨 20% 감소
    public float SkillCooldownMultiplier => skillCoolMultiplier;

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
    public void IncreaseAttackDamage(float amount)
    {
        attackDamage = Mathf.Max(0f, attackDamage +  amount);
        Debug.Log($"공격력 증가: {amount}, 현재 공격력: {attackDamage}", this);
    }

    public void IncreaseAttackDamagePercent(float amount)
    {
        float multiplier = 1f + amount / 100f;
        attackDamage = Mathf.Max(0f, attackDamage * multiplier);
        Debug.Log($"공격력 퍼센트 증가: {amount}%, 현재 공격력: {attackDamage}", this);
    }
    
    public void IncreaseAttackSpeed(float amount)
    {
        attackSpeed = Mathf.Max(0.01f, attackSpeed + amount);
        Debug.Log($"공격 속도 증가: {amount}, 현재 공격속도: {attackSpeed}", this);
    }
    public void IncreaseAttackRange(float amount)
    {
        attackRange = Mathf.Max(0f, attackRange + amount);
        Debug.Log($"공격 사거리 증가: {amount}, 현재 공격사거리: {attackRange}", this);
    }
    public void IncreaseCriticalChance(float amount)
    {
        criticalChance = Mathf.Clamp01(criticalChance + amount);
        Debug.Log($"치명타 확률 증가: {amount}, 현재 치명타 확률: {criticalChance}", this);
    }
    public void IncreaseCriticalMultiplier(float amount)
    {
        criticalMultiplier = Mathf.Max(1f,criticalMultiplier + amount);
        Debug.Log($"치명타 배율 증가: {amount}, 현재 치명타 배율: {criticalMultiplier}", this);
    }

    public float GetFinalSkillCooldown(float baseCooldown)
    {
        // 쿨타임이 0 이하가 되는 것을 막기 위해 최소값을 보정한다.
        float safeBaseCooldown = Mathf.Max(0.01f, baseCooldown);

        // 전체 쿨타임 감소 배율을 적용한다.
        return safeBaseCooldown * skillCoolMultiplier;
    }

    // 전체 스킬 쿨타임 감소 패시브를 적용할 때 사용할 함수.
    // 예: percent = 10이면 전체 스킬 쿨타임 10% 감소.
    public void ReduceAllSkillCooldownPercent(float percent)
    {
        // 10% 감소라면 multiplier는 0.9가 되어야 한다.
        float reduceValue = percent / 100f;

        skillCoolMultiplier -= reduceValue;

        // 너무 낮아지면 밸런스가 무너질 수 있으므로 최소 0.2까지 제한한다.
        skillCoolMultiplier = Mathf.Clamp(skillCoolMultiplier, 0.2f, 1f);

        Debug.Log(
            $"전체 스킬 쿨타임 감소 적용: {percent}% / 현재 배율: {skillCoolMultiplier}",
            this
        );
    }
}
