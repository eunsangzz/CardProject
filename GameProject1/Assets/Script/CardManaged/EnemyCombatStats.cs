using UnityEngine;

public class EnemyCombatStats : MonoBehaviour
{
    [SerializeField] private int attackPower = 1;
    [SerializeField] private int maxHealth = 6;
    [SerializeField] private int currentHealth = 6;

    public int AttackPower => attackPower;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    private void Awake()
    {
        if (maxHealth <= 0)
            maxHealth = 6;

        if (currentHealth <= 0)
            currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead) return;
        currentHealth = Mathf.Max(0, currentHealth - damage);
    }
}
