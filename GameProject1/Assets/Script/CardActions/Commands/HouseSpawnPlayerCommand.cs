using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSpawnPlayerCommand : ICardCommand
{
    public int WorkerCost => 2;

    public bool CanExecute(GameData gd)
        => gd.Woker >= WorkerCost
        && gd.PlayerCount >= 2
        && gd.gold > 15;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        gd.AddGold(-15);

        yield return new UnityEngine.WaitForSeconds(60f);

        cm.SpawnPlayerCard();
        gd.AddPlayer(1);
        gd.AddWorker(1);
    }
}
