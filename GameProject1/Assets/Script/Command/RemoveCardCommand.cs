using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RemoveCardCommand : ICommand
{
    private readonly GameObject cardObj;
    private readonly Vector3 prevPos;
    private readonly Transform prevParents;
    private readonly bool prevActive;

    public string DebugName => "RemoveCard";

    public RemoveCardCommand(GameObject cardObj)
    {
        this.cardObj = cardObj;
        prevPos = cardObj != null ? cardObj.transform.position : Vector3.zero;
        prevParents = cardObj != null ? cardObj.transform.parent : null;
        prevActive = cardObj != null && cardObj.activeSelf;
    }

    public void Execute()
    {
        if (cardObj == null) return;
        cardObj.SetActive(false);
    }

    public void Undo()
    {
        if (cardObj == null) return;

        cardObj.transform.SetParent(prevParents);
        cardObj.transform.position = prevPos;
        cardObj.SetActive(prevActive);
    }

}
