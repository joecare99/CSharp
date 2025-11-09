using DetectiveGame.Engine.Cards;

namespace DetectiveGame.Engine.Game;

public sealed record CaseSolution(Card Person, Card Weapon, Card Room)
{
    public IEnumerable<Card> All => new[] { Person, Weapon, Room };
}