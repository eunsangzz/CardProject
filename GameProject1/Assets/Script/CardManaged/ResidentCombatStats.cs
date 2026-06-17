using UnityEngine;

public class ResidentCombatStats : MonoBehaviour
{
    public const int DefaultAttackPower = 2;
    public const int DefaultMaxHealth = 10;

    private CardIdentity _identity;

    private CardCombatData CombatData => _identity.Data.combat;

    public int AttackPower => CombatData.attackPower;
    public int MaxHealth => CombatData.maxHealth;
    public int CurrentHealth => CombatData.currentHealth;
    public bool IsDead => CurrentHealth <= 0;

    public void Initialize(CardIdentity identity)
    {
        Bind(identity);

        CombatData.enabled = true;
        CombatData.attackPower = DefaultAttackPower;
        CombatData.maxHealth = DefaultMaxHealth;
        CombatData.currentHealth = DefaultMaxHealth;
    }

    public void Bind(CardIdentity identity)
    {
        _identity = identity;

        if (_identity.Data.combat == null)
            _identity.Data.combat = new CardCombatData();
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || IsDead) return;
        CombatData.currentHealth = Mathf.Max(0, CurrentHealth - damage);
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;
        CombatData.currentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
    }
}
