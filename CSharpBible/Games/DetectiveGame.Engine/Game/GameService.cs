using DetectiveGame.Engine.Cards;
using DetectiveGame.Engine.Game.Interfaces;

namespace DetectiveGame.Engine.Game;

public class GameService : IGameSetup, IGameService
{
    public GameState CreateNew(IReadOnlyList<string> playerNames, int? seed = null)
    {
        if (playerNames.Count < 2) throw new ArgumentException("Need at least 2 players");
        var rnd = seed.HasValue ? new Random(seed.Value) : Random.Shared;
        var state = new GameState();
        for (int i = 0; i < playerNames.Count; i++) state.PlayersInternal().Add(new Player(i, playerNames[i]));

        // choose solution
        var person = GameData.Persons[rnd.Next(GameData.Persons.Count)];
        var weapon = GameData.Weapons[rnd.Next(GameData.Weapons.Count)];
        var room = GameData.Rooms[rnd.Next(GameData.Rooms.Count)];
        state.Solution = new CaseSolution(person, weapon, room);

        // remaining deck
        var deck = GameData.All.Where(c => !state.Solution.All.Contains(c)).OrderBy(_ => rnd.Next()).ToList();
        int pIndex = 0;
        foreach (var card in deck)
        {
            state.PlayersInternal()[pIndex].Hand.Add(card);
            pIndex = (pIndex + 1) % state.Players.Count;
        }
        return state;
    }

    public Suggestion MakeSuggestion(GameState state, int askingPlayerId, Card person, Card weapon, Card room)
    {
        if (state.Finished) throw new InvalidOperationException("Game finished");
        var sug = new Suggestion(askingPlayerId, person, weapon, room);
        var players = state.Players;
        for (int step = 1; step < players.Count; step++)
        {
            var p = players[(state.CurrentPlayerIndex + step) % players.Count];
            if (!p.Active) continue;
            Card? reveal = p.Hand.FirstOrDefault(c => c == person || c == weapon || c == room);
            if (reveal != null)
            {
                p.SeenCards.Add(reveal); // actually asking player sees it; modeling simple global knowledge for now
                sug = sug with { RefutingPlayerId = p.Id, RevealedCard = reveal };
                break;
            }
        }
        state.History.Add(sug);
        return sug;
    }

    public bool MakeAccusation(GameState state, int playerId, Card person, Card weapon, Card room)
    {
        if (state.Finished) throw new InvalidOperationException("Game finished");
        var correct = state.Solution.Person == person && state.Solution.Weapon == weapon && state.Solution.Room == room;
        if (correct)
        {
            state.Finished = true;
            state.WinnerPlayerId = playerId;
            return true;
        }
        var player = state.Players.First(p => p.Id == playerId);
        player.Active = false;
        // if only one active left they win automatically
        if (state.Players.Count(p => p.Active) == 1)
        {
            state.Finished = true;
            state.WinnerPlayerId = state.Players.First(p => p.Active).Id;
        }
        return false;
    }

    public void NextTurn(GameState state)
    {
        if (state.Finished) return;
        var count = state.Players.Count;
        for (int i = 0; i < count; i++)
        {
            state.CurrentPlayerIndex = (state.CurrentPlayerIndex + 1) % count;
            if (state.Players[state.CurrentPlayerIndex].Active) break;
        }
    }
}

internal static class GameStateExtensions
{
    public static List<Player> PlayersInternal(this GameState state) => (List<Player>)state.GetType()
        .GetField("_players", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
        .GetValue(state)!;
}
