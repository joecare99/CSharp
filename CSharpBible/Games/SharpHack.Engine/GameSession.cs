using SharpHack.Base.Model;
using SharpHack.LevelGen;

namespace SharpHack.Engine;

public class GameSession
{
    public Map Map { get; private set; }
    public Creature Player { get; private set; }
    public bool IsRunning { get; private set; } = true;

    private readonly IMapGenerator _mapGenerator;

    public GameSession(IMapGenerator mapGenerator)
    {
        _mapGenerator = mapGenerator;
        Initialize();
    }

    private void Initialize()
    {
        Map = _mapGenerator.Generate(80, 25);
        Player = new Creature
        {
            Name = "Hero",
            Symbol = '@',
            Color = System.ConsoleColor.Yellow,
            HP = 100,
            MaxHP = 100,
            Position = new Point(1, 1) // TODO: Find valid start position
        };
        
        // Ensure player is on a valid tile
        if (!Map.IsValid(Player.Position) || !Map[Player.Position].IsWalkable)
        {
             // Simple fallback search for a walkable tile
             for(int x=0; x<Map.Width; x++)
                 for(int y=0; y<Map.Height; y++)
                     if(Map[x,y].IsWalkable)
                     {
                         Player.Position = new Point(x,y);
                         goto Found;
                     }
             Found:;
        }
    }

    public void Update()
    {
        // Game logic update (turn processing)
    }

    public void MovePlayer(Direction direction)
    {
        var newPos = Player.Position;
        switch (direction)
        {
            case Direction.North: newPos.Y--; break;
            case Direction.South: newPos.Y++; break;
            case Direction.West: newPos.X--; break;
            case Direction.East: newPos.X++; break;
            case Direction.NorthWest: newPos.X--; newPos.Y--; break;
            case Direction.NorthEast: newPos.X++; newPos.Y--; break;
            case Direction.SouthWest: newPos.X--; newPos.Y++; break;
            case Direction.SouthEast: newPos.X++; newPos.Y++; break;
        }

        if (Map.IsValid(newPos) && Map[newPos].IsWalkable)
        {
            Player.Position = newPos;
        }
    }
}
