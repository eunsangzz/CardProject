using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockToStoneCommand : ICardCommand
{
    public int WorkerCost => 1;
    public bool CanExecute(GameData gd) => gd.Woker >= WorkerCost;

    public IEnumerator Execute(CardManager cm, GameData gd)
    {
        cm.RemoveCardByIndex(3); // Rock Á¦°Å
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(1); gd.addCardCount(1);
        yield return new UnityEngine.WaitForSeconds(2f);

        cm.CreateBasicCard(1); gd.addCardCount(1);
    }
}
