using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeGoldCommand : ICardCommand
{
    public int WorkerCost => 1;

    public bool CanExecute(GameData gd)
        => gd.Woker >= WorkerCost
        && gd.WoodCard >= 2
        && gd.GoldCard >= 1
        && gd.BranchCard >= 1;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        cm.RemoveCardByIndex(0);
        cm.RemoveCardByIndex(0);
        cm.RemoveCardByIndex(10);
        cm.RemoveCardByIndex(9);

        yield return new UnityEngine.WaitForSeconds(5f);

        cm.CreateIntermediateCard(3); // GoldIngot 프리팹 인덱스(네 기존과 동일 가정)
        gd.addCardCount(12);
    }
}