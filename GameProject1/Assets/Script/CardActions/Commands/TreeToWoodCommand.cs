using System.Collections;
using System.Collections.Generic;

public class TreeToWoodCommand : ICardCommand
{
    public int WorkerCost => 1;
    public float Duration => 4f;
    public IReadOnlyList<CardRequirement> Requirements =>
        System.Array.Empty<CardRequirement>();
    public bool ConsumeTarget => true;
    public bool CanExecute(GameData gameData) => gameData.Woker >= WorkerCost;

    public IEnumerator Execute(CardManager cardManager, GameData gameData)
    {
        yield return new UnityEngine.WaitForSeconds(2f);
        cardManager.CreateBasicCard(0);
        gameData.addCardCount(0);

        yield return new UnityEngine.WaitForSeconds(2f);
        cardManager.CreateBasicCard(0);
        gameData.addCardCount(0);
    }
}
