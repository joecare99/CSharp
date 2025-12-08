using DetectiveGame.Engine.Cards;

namespace DetectiveGame.Engine.Game;

public sealed record Suggestion(int AskingPlayerId, Card Person, Card Weapon, Card Room)
{
    public int? RefutingPlayerId { get; init; }
    public Card? RevealedCard { get; init; }
}