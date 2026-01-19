using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class MoveCardCommand : ICommand
{
    private readonly Transform cardTransform;
    private readonly Vector3 fromPos;
    private readonly Vector3 toPos;

    public string DebugName => "MoveCard";
    
    public MoveCardCommand(Transform cardTransform, Vector3 formPos, Vector3 toPos)
    {
        this.cardTransform = cardTransform;
        this.fromPos = fromPos;
        this.toPos = toPos;
    }

    public void Execute()
    {
        if (cardTransform == null) return;
        cardTransform.position = toPos;
    }

    public void Undo()
    {
        if (cardTransform == null) return;
        cardTransform.position = fromPos;
    }
}
