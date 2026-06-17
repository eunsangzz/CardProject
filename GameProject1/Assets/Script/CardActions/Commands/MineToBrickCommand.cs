using System.Collections;
using System.Collections.Generic;

public class MineToBrickCommand : ICardCommand
{
    public int WorkerCost => 1;
    public float Duration => 5f;
    public IReadOnlyList<CardRequirement> Requirements { get; } =
        new[] { new CardRequirement(1, 2) };
    public bool ConsumeTarget => false;

    public bool CanExecute(GameData gameData) =>
        gameData.Woker >= WorkerCost &&
        gameData.StoneCard >= 2 &&
        gameData.gold >= 1;

    public IEnumerator Execute(CardManager cardManager, GameData gameData)
    {
        if (gameData.QusetNum == 4)
            gameData.AddQuest(1);

        yield return new UnityEngine.WaitForSeconds(Duration);
        cardManager.CreateIntermediateCard(1);
        gameData.addCardCount(13);
    }
}
