using System.Collections;
using System.Collections.Generic;

public class StrawberryTreeToStrawberryCommand : ICardCommand
{
    public int WorkerCost => 1;
    public float Duration => 6f;
    public IReadOnlyList<CardRequirement> Requirements =>
        System.Array.Empty<CardRequirement>();
    public bool ConsumeTarget => true;
    public bool CanExecute(GameData gameData) => gameData.Woker >= WorkerCost;

    public IEnumerator Execute(CardManager cardManager, GameData gameData)
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new UnityEngine.WaitForSeconds(2f);
            cardManager.CreateBasicCard(6);
            gameData.addCardCount(6);
        }
    }
}
