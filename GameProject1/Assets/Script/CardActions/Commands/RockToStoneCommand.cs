using System.Collections;
using System.Collections.Generic;

public class RockToStoneCommand : ICardCommand
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
        cardManager.CreateBasicCard(1);
        gameData.addCardCount(1);

        yield return new UnityEngine.WaitForSeconds(2f);
        cardManager.CreateBasicCard(1);
        gameData.addCardCount(1);
    }
}
