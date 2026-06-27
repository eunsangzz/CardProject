using System.Collections;
using System.Collections.Generic;

public class IronArmorCommand : IResidentUpgradeCommand
{
    public int WorkerCost => 1;
    public float Duration => 8f;
    public IReadOnlyList<CardRequirement> Requirements { get; } =
        new[]
        {
            new CardRequirement(11, 2),
            new CardRequirement(14, 1),
        };
    public bool ConsumeTarget => false;

    public bool CanExecute(GameData gameData) =>
        gameData.Woker >= WorkerCost &&
        gameData.IronIngotCard >= 2 &&
        gameData.PanelCard >= 1;

    public IEnumerator Execute(CardManager cardManager, GameData gameData)
    {
        yield return new UnityEngine.WaitForSeconds(Duration);
    }

    public void ApplyToResident(ResidentCombatStats resident)
    {
        if (resident != null)
            resident.EquipArmor(ArmorType.Iron);
    }
}
