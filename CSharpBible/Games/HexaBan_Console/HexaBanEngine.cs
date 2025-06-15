using HexaBan.Models.Interfaces;
using System.Net.Security;

public class HexaBanEngine2 : IHexaBanEngine
{
    public int Width => _playfield.Width;
    public int Height => _playfield.Height;
    public TileType[,] Board
    {
        get
        {
            var board = new TileType[Width, Height];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    board[x, y] = _playfield.Tiles[y, x];

            var (px, py) = _playfield.PlayerPosition;
            if (board[px, py] == TileType.Goal)
                board[px, py] = TileType.PlayerOnGoal;
            else
                board[px, py] = TileType.Player;
            return board;
        }
    }

    public (int X, int Y) PlayerPosition => _playfield.PlayerPosition;

    public bool LevelBeendet { get; private set; }

    public int Level => _level;

    public string Preview => _levelRepo.GetLevelAsciiArt(_level+1);

    private IHexaBanLevelRepository _levelRepo;
    private int _level;
    private Playfield _playfield;

    public HexaBanEngine2(IHexaBanLevelRepository hexaBanLevel)
    {
        _levelRepo = hexaBanLevel;
        _level = 1; // Beispiel für Level-Initialisierung
        _playfield = new Playfield();
        Reset();
    }

    public void StartNextLevel()
    {
        LevelBeendet = false;
        _playfield.LoadLevel(_levelRepo.GetLevel(++_level));
    }

    public void Reset()
    {
        LevelBeendet = false;
        _playfield.LoadLevel(_levelRepo.GetLevel(_level));
    }

    public void Move(HexDirection dir)
    {
        int dx = 0, dy = 0;
        var pos = _playfield.PlayerPosition;
        switch (dir)
        {
            case HexDirection.Left: dx = -1; break;
            case HexDirection.Right: dx = 1; break;
            case HexDirection.UpLeft: dx = (pos.Y % 2 == 0) ? -1 : 0; dy = -1; break;
            case HexDirection.UpRight: dx = (pos.Y % 2 == 0) ? 0 : 1; dy = -1; break;
            case HexDirection.DownLeft: dx = (pos.Y % 2 == 0) ? -1 : 0; dy = 1; break;
            case HexDirection.DownRight: dx = (pos.Y % 2 == 0) ? 0 : 1; dy = 1; break;
        }
        int nx = pos.X + dx;
        int ny = pos.Y + dy;
        if (nx < 0 || nx >= _playfield.Width || ny < 0 || ny >= _playfield.Height)
            return;
        var target = _playfield.Tiles[ny, nx];

        if (target == TileType.Wall)
            return;

        if (target == TileType.Box || target == TileType.BoxOnGoal)
        {
            // Box muss im selben Zick-Zack-Muster wie der Spieler geschoben werden
            int boxDx = 0, boxDy = 0;
            switch (dir)
            {
                case HexDirection.Left: boxDx = -1; break;
                case HexDirection.Right: boxDx = 1; break;
                case HexDirection.UpLeft: boxDx = (ny % 2 == 0) ? -1 : 0; boxDy = -1; break;
                case HexDirection.UpRight: boxDx = (ny % 2 == 0) ? 0 : 1; boxDy = -1; break;
                case HexDirection.DownLeft: boxDx = (ny % 2 == 0) ? -1 : 0; boxDy = 1; break;
                case HexDirection.DownRight: boxDx = (ny % 2 == 0) ? 0 : 1; boxDy = 1; break;
            }
            int bx = nx + boxDx, by = ny + boxDy;
            if (bx < 0 || bx >= _playfield.Width || by < 0 || by >= _playfield.Height)
                return;
            var boxTarget = _playfield.Tiles[by, bx];
            if (boxTarget != TileType.Floor && boxTarget != TileType.Goal)
                return;
            // Box bewegen
            _playfield.Tiles[by, bx] = (boxTarget == TileType.Goal) ? TileType.BoxOnGoal : TileType.Box;
            _playfield.Tiles[ny, nx] = (target == TileType.BoxOnGoal) ? TileType.Goal : TileType.Floor;
        }
        // Spieler bewegen
        _playfield.PlayerPosition = (nx, ny);

        ///Erkenne, daß das Level beendet wurde.
        // Prüfe, ob das Level gelöst ist (alle Ziele mit Boxen belegt)
        bool levelBeendet = true;
        for (int y = 0; y < _playfield.Height; y++)
        {
            for (int x = 0; x < _playfield.Width; x++)
            {
                if (_playfield.Tiles[y, x] == TileType.Goal)
                {
                    levelBeendet = false;
                    break;
                }
            }
            if (!levelBeendet)
                break;
        }
        LevelBeendet = levelBeendet;
        
        
    }

}
