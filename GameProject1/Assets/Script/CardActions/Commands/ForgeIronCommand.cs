using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeIronCommand : ICardCommand
{
    public int WorkerCost => 1;

    public bool CanExecute(GameData gd)
        => gd.Woker >= WorkerCost
        && gd.WoodCard >= 2
        && gd.IronCard >= 1
        && gd.BranchCard >= 2;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        cm.RemoveCardByIndex(0);
        cm.RemoveCardByIndex(0);
        cm.RemoveCardByIndex(10);
        cm.RemoveCardByIndex(8);

        yield return new UnityEngine.WaitForSeconds(5f);

        cm.CreateIntermediateCard(2); // IronIngot 프리팹 인덱스(네 기존과 동일 가정)
        gd.addCardCount(11);
    }
}
