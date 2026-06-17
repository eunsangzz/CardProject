using System.Collections.Generic;
using UnityEngine;

public sealed class MoveCardCommand : ICommand
{
    private readonly List<Transform> cardTransforms;
    private readonly List<Vector3> fromPositions;
    private readonly List<Vector3> toPositions;

    public string DebugName => "MoveCard";

    public MoveCardCommand(Transform cardTransform, Vector3 fromPos, Vector3 toPos)
        : this(
            new List<Transform> { cardTransform },
            new List<Vector3> { fromPos },
            new List<Vector3> { toPos })
    {
    }

    public MoveCardCommand(
        IList<Transform> transforms,
        IList<Vector3> fromPositions,
        IList<Vector3> toPositions)
    {
        cardTransforms = new List<Transform>(transforms);
        this.fromPositions = new List<Vector3>(fromPositions);
        this.toPositions = new List<Vector3>(toPositions);
    }

    public void Execute()
    {
        MoveTo(toPositions);
    }

    public void Undo()
    {
        MoveTo(fromPositions);
    }

    private void MoveTo(IList<Vector3> positions)
    {
        int count = Mathf.Min(cardTransforms.Count, positions.Count);
        for (int i = 0; i < count; i++)
        {
            if (cardTransforms[i] != null)
                cardTransforms[i].position = positions[i];
        }
    }
}
