using System;
using UnityEngine;

[Serializable]
public class CardInstanceData
{
    public string uniqueId;
    public CardId cardId;

    public float positionX;
    public float positionY;
    public float positionZ;

    public CardCombatData combat = new CardCombatData();

    public static CardInstanceData CreateNew(CardId id)
    {
        return new CardInstanceData
        {
            uniqueId = Guid.NewGuid().ToString("N"),
            cardId = id
        };
    }

    public CardInstanceData Clone()
    {
        return new CardInstanceData
        {
            uniqueId = uniqueId,
            cardId = cardId,
            positionX = positionX,
            positionY = positionY,
            positionZ = positionZ,
            combat = combat != null ? combat.Clone() : new CardCombatData()
        };
    }

    public void CapturePosition(Vector3 position)
    {
        positionX = position.x;
        positionY = position.y;
        positionZ = position.z;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(positionX, positionY, positionZ);
    }
}

[Serializable]
public class CardCombatData
{
    public bool enabled;
    public int attackPower;
    public int maxHealth;
    public int currentHealth;
    public ArmorType armorType;
    public int armorDurability;

    public CardCombatData Clone()
    {
        return new CardCombatData
        {
            enabled = enabled,
            attackPower = attackPower,
            maxHealth = maxHealth,
            currentHealth = currentHealth,
            armorType = armorType,
            armorDurability = armorDurability
        };
    }
}

public enum ArmorType
{
    None = 0,
    Wood = 1,
    Iron = 2
}
