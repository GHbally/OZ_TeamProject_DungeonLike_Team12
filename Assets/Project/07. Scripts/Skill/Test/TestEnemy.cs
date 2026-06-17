using UnityEngine;

public class DummyEnemy : MonoBehaviour, IDamageable1
{
    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(DamageInfo1 damageInfo)
    {
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
        Debug.Log($"{name} 사망");
        Destroy(gameObject);
    }
}