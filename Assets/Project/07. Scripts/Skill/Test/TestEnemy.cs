using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable1
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageInfo1 damageInfo)
    {
        if (isDead)
        {
            return;
        }
        currentHealth -= damageInfo.Damage;

        Debug.Log(
            $"{name} 피해: {damageInfo.Damage}, " +
            $"치명타: {damageInfo.IsCritical}, " +
            $"남은 체력: {currentHealth}"
        );

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    { 
        if (isDead)
        {
            return;
        }
        isDead = true;
        Debug.Log($"{name} 사망");
        Destroy(gameObject);
    }
}