using System.Collections;
using System.Collections.Generic;

public class TimberToPanelCommand : ICardCommand
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
        gameData.BranchCard >= 1 &&
        gameData.gold >= 1;

    public IEnumerator Execute(CardManager cardManager, GameData gameData)
    {
        if (gameData.QusetNum == 4)
            gameData.AddQuest(1);
        if (gameData.QusetNum == 6)
            gameData.AddQuest(1);

        yield return new UnityEngine.WaitForSeconds(Duration);
        cardManager.CreateIntermediateCard(0);
        gameData.addCardCount(14);
    }
}
