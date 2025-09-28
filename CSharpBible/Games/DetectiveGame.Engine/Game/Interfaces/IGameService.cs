using DetectiveGame.Engine.Cards;

namespace DetectiveGame.Engine.Game.Interfaces;

public interface IGameService
{
    Suggestion MakeSuggestion(GameState state, int askingPlayerId, Card person, Card weapon, Card room);
    bool MakeAccusation(GameState state, int playerId, Card person, Card weapon, Card room);
    void NextTurn(GameState state);
}