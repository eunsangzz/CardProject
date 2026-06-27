using System.Collections;
using System.Collections.Generic;

public class WoodArmorCommand : IResidentUpgradeCommand
{
    public int WorkerCost => 1;
    public float Duration => 5f;
    public IReadOnlyList<CardRequirement> Requirements { get; } =
        new[]
        {
            new CardRequirement(0, 2),
            new CardRequirement(10, 1),
        };
    public bool ConsumeTarget => false;

    public bool CanExecute(GameData gameData) =>
        gameData.Woker >= WorkerCost &&
        gameData.WoodCard >= 2 &&
        gameData.BranchCard >= 1;

    public IEnumerator Execute(CardManager cardManager, GameData gameData)
    {
        yield return new UnityEngine.WaitForSeconds(Duration);
    }

    public void ApplyToResident(ResidentCombatStats resident)
    {
        if (resident != null)
            resident.EquipArmor(ArmorType.Wood);
    }
}
