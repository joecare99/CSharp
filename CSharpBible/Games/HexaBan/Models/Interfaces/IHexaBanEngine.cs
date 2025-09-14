namespace HexaBan.Models.Interfaces;

public interface IHexaBanEngine
{
    int Width { get; }
    int Height { get; }
    TileType[,] Board { get; }
    (int X, int Y) PlayerPosition { get; }
    bool LevelBeendet { get; }
    int Level { get; }
    string Preview { get; }

    void Move(HexDirection dir);
    void Reset();
    void StartNextLevel();
}
