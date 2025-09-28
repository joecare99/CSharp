namespace DetectiveGame.Engine.Game.Interfaces;

public interface IGameSetup
{
    GameState CreateNew(IReadOnlyList<string> playerNames, int? seed = null);
}