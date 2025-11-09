namespace DetectiveGame.Engine.Cards;

public sealed record Card(string Name, CardCategory Category)
{
    public override string ToString() => $"{Name} ({Category})";
}