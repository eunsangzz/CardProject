using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MineToBrickCommand : ICardCommand
{
    public int WorkerCost => 1;

    public bool CanExecute(GameData gd)
        => gd.Woker >= WorkerCost
        && gd.StoneCard >= 2
        && gd.gold >= 1;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        if (gd.QusetNum == 4) gd.AddQuest(1);

        cm.RemoveCardByIndex(1);
        cm.RemoveCardByIndex(1);

        yield return new UnityEngine.WaitForSeconds(5f);

        cm.CreateIntermediateCard(1);
        gd.addCardCount(13);
    }
}
