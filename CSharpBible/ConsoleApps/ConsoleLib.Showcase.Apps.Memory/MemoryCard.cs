namespace ConsoleLib.Showcase.Apps.Memory;

public sealed class MemoryCard
{
    internal MemoryCard(int index, string value)
    {
        Index = index;
        Value = value;
    }

    public int Index { get; }
    public string Value { get; }
    public bool IsFaceUp { get; internal set; }
    public bool IsMatched { get; internal set; }
}
