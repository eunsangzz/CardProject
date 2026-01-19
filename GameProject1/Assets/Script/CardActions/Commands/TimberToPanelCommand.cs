using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimberToPanelCommand : ICardCommand
{
    public int WorkerCost => 1;

    public bool CanExecute(GameData gd)
        => gd.Woker >= WorkerCost
        && gd.WoodCard >= 2
        && gd.BranchCard >= 1
        && gd.gold >= 1;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        if (gd.QusetNum == 4) gd.AddQuest(1);

        cm.RemoveCardByIndex(0);
        cm.RemoveCardByIndex(0);
        cm.RemoveCardByIndex(10);

        if (gd.QusetNum == 6) gd.AddQuest(1);

        yield return new UnityEngine.WaitForSeconds(5f);

        cm.CreateIntermediateCard(0);
        gd.addCardCount(14);
    }
}
