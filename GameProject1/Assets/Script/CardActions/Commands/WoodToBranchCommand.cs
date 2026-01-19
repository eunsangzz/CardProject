using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodToBranchCommand : ICardCommand
{
    public int WorkerCost => 1;
    public bool CanExecute(GameData gd) => gd.Woker >= WorkerCost;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        cm.RemoveCardByIndex(0); // Wood Á¦°Å
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateIntermediateCard(4); gd.addCardCount(10); // Branch
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateIntermediateCard(4); gd.addCardCount(10);
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateIntermediateCard(4); gd.addCardCount(10);

        if (gd.QusetNum == 1) gd.AddQuest(1);
    }
}
