using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private readonly Stack<ICommand> undoStack = new();
    private readonly Stack<ICommand> redoStack = new();

    public bool CanUndo => undoStack.Count > 0;
    public bool CanRedo => redoStack.Count > 0;

    public void Do(ICommand command)
    {
        command.Execute();
        undoStack.Push(command);
        redoStack.Clear();
    }

    public void Undo()
    {
        if (!CanUndo) return;

        var cmd = undoStack.Pop();
        cmd.Undo();
        redoStack.Push(cmd);
    }

    public void Redo()
    {
        if (!CanRedo) return;

        var cmd = redoStack.Pop();
        cmd.Execute();
        undoStack.Push(cmd);
    }

    public void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
    }
}