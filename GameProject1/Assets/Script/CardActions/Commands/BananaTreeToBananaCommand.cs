using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaTreeToBananaCommand : ICardCommand
{
    public int WorkerCost => 1;
    public bool CanExecute(GameData gd) => gd.Woker >= WorkerCost;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        cm.RemoveCardByIndex(4); // BananaTree 제거
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(5); gd.addCardCount(5);
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(5); gd.addCardCount(5);
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(5); gd.addCardCount(5);

        if (gd.QusetNum == 2) gd.AddQuest(1); // 기존 직접 증가 제거
    }
}