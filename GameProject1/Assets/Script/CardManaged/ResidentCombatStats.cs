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
    public ArmorType ArmorType => CombatData.armorType;
    public int ArmorDurability => CombatData.armorDurability;
    public bool IsDead => CurrentHealth <= 0;
    public bool IsInitialized => _identity != null && _identity.Data.combat != null;

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

        if (CombatData.armorType != ArmorType.None && CombatData.armorDurability > 0)
        {
            int absorbed = Mathf.Min(damage, CombatData.armorDurability);
            CombatData.armorDurability -= absorbed;
            damage -= absorbed;

            if (CombatData.armorDurability <= 0)
            {
                CombatData.armorType = ArmorType.None;
                CombatData.armorDurability = 0;
            }
        }

        if (damage <= 0)
            return;

        CombatData.currentHealth = Mathf.Max(0, CurrentHealth - damage);
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;
        CombatData.currentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
    }

    public void EquipArmor(ArmorType armorType)
    {
        CombatData.armorType = armorType;
        CombatData.armorDurability = GetArmorMaxDurability(armorType);
    }

    public void ResetArmorForBattle()
    {
        if (CombatData.armorType == ArmorType.None)
            return;

        CombatData.armorDurability = GetArmorMaxDurability(CombatData.armorType);
    }

    public static int GetArmorMaxDurability(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.Wood:
                return 5;
            case ArmorType.Iron:
                return 10;
            default:
                return 0;
        }
    }
}
