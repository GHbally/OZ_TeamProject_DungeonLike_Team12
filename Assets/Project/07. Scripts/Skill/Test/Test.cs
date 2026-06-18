using UnityEngine;

public interface IDamageable1
{
    void TakeDamage(DamageInfo1 damageInfo);
}

public readonly struct DamageInfo1
{
    public float Damage { get; }
    public bool IsCritical { get; }
    public GameObject Attacker { get; }

    public DamageInfo1(float damage, bool isCritical, GameObject attacker)
    {
        Damage = damage;
        IsCritical = isCritical;
        Attacker = attacker;
    }
}
