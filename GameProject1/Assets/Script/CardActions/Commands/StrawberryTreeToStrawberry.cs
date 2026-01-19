using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrawberryTreeToStrawberryCommand : ICardCommand
{
    public int WorkerCost => 1;
    public bool CanExecute(GameData gd) => gd.Woker >= WorkerCost;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        cm.RemoveCardByIndex(7); // StrawBerryTree Á¦°Å
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(6); gd.addCardCount(6);
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(6); gd.addCardCount(6);
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(6); gd.addCardCount(6);
    }
}
