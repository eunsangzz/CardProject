using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeToWoodCommand : ICardCommand
{
    public int WorkerCost => 1;

    public bool CanExecute(GameData gd) => gd.Woker >= WorkerCost;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        cm.RemoveCardByIndex(2);
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(0); gd.addCardCount(0);
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(0); gd.addCardCount(0);
    }
}

