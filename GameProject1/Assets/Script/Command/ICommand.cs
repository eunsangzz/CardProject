public interface ICommand
{
    void Execute();
    void Undo();
    string DebugName { get; }
}