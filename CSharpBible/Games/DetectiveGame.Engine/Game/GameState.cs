using DetectiveGame.Engine.Cards;

namespace DetectiveGame.Engine.Game;

public class GameState
{
    public Guid Id { get; } = Guid.NewGuid();
    public IReadOnlyList<Player> Players => _players;
    private readonly List<Player> _players = new();
    public int CurrentPlayerIndex { get; internal set; }
    public CaseSolution Solution { get; internal set; } = null!; // set in setup
    public List<Suggestion> History { get; } = new();
    public bool Finished { get; internal set; }
    public int? WinnerPlayerId { get; internal set; }
}
