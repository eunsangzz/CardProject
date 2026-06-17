using System.Collections;
using System.Collections.Generic;

public interface ICardCommand
{
    int WorkerCost { get; }
    float Duration { get; }
    IReadOnlyList<CardRequirement> Requirements { get; }
    bool ConsumeTarget { get; }
    bool CanExecute(GameData gameData);
    IEnumerator Execute(CardManager cardManager, GameData gameData);
}
