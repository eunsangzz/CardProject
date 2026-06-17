using System.Collections;
using System.Collections.Generic;

public class ForgeIronCommand : ICardCommand
{
    public int WorkerCost => 1;
    public float Duration => 5f;
    public IReadOnlyList<CardRequirement> Requirements { get; } =
        new[]
        {
            new CardRequirement(0, 2),
            new CardRequirement(10, 2),
            new CardRequirement(8, 1),
        };
    public bool ConsumeTarget => false;

    public bool CanExecute(GameData gameData) =>
        gameData.Woker >= WorkerCost &&
        gameData.WoodCard >= 2 &&
        gameData.IronCard >= 1 &&
        gameData.BranchCard >= 2;

    public IEnumerator Execute(CardManager cardManager, GameData gameData)
    {
        yield return new UnityEngine.WaitForSeconds(Duration);
        cardManager.CreateIntermediateCard(2);
        gameData.addCardCount(11);
    }
}
