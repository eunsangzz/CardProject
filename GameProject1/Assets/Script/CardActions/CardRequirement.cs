public readonly struct CardRequirement
{
    public int CardIndex { get; }
    public int Count { get; }

    public CardRequirement(int cardIndex, int count)
    {
        CardIndex = cardIndex;
        Count = count;
    }
}
