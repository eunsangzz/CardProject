using System.Collections;
using System.Collections.Generic;

public class HouseSpawnPlayerCommand : ICardCommand
{
    public int WorkerCost => 2;
    public float Duration => 60f;
    public IReadOnlyList<CardRequirement> Requirements =>
        System.Array.Empty<CardRequirement>();
    public bool ConsumeTarget => false;

    public bool CanExecute(GameData gameData) =>
        gameData.Woker >= WorkerCost &&
        gameData.PlayerCount >= 2 &&
        gameData.gold > 15;

    public IEnumerator Execute(CardManager cardManager, GameData gameData)
    {
        gameData.AddGold(-15);
        yield return new UnityEngine.WaitForSeconds(Duration);
        cardManager.SpawnPlayerCard();
        gameData.AddPlayer(1);
        gameData.AddWorker(1);
    }
}
