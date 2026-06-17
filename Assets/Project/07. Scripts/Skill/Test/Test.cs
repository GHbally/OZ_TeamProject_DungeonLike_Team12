using UnityEngine;

public interface IDamageable1
{
    void TakeDamage(DamageInfo1 damageInfo);
}

public struct DamageInfo1
{
    public float Damage;
    public bool IsCritical;
    public GameObject Attacker;

    public DamageInfo1(float damage, bool isCritical, GameObject attacker)
    {
        Damage = damage;
        IsCritical = isCritical;
        Attacker = attacker;
    }
}
