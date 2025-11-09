using DetectiveGame.Engine.Cards;

namespace DetectiveGame.Engine.Game;

public class Player
{
    public int Id { get; }
    public string Name { get; }
    public List<Card> Hand { get; } = new();
    public HashSet<Card> SeenCards { get; } = new();
    public bool Active { get; internal set; } = true;

    public Player(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => $"#{Id} {Name} (Hand {Hand.Count})";
}