using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SpawnCardCommand : ICommand
{
    public string DebugName => "SpawnCard";

    private readonly System.Func<GameObject> spawnFunc;
    private GameObject spawned;

    public SpawnCardCommand(System.Func<GameObject> spawnFunc)
    {
        this.spawnFunc = spawnFunc;
    }

    public void Execute()
    {
        spawned = spawnFunc?.Invoke();
    }

    public void Undo()
    {
        if (spawned == null) return;
        spawned.SetActive(false);
    }
}
