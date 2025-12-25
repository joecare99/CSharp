using SharpHack.Base.Model;
using SharpHack.LevelGen;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Interfaces; // Add using

namespace SharpHack.Engine;

public class GameSession
{
    public Map Map { get; private set; }
    public Creature Player { get; private set; }
    public List<Creature> Enemies { get; private set; } = new();
    public bool IsRunning { get; private set; } = true;

    private readonly IMapGenerator _mapGenerator;
    private readonly IRandom _random;
    private readonly ICombatSystem _combatSystem;
    private readonly IEnemyAI _enemyAI; // Add field
    private readonly FieldOfView _fov;

    public event Action<string>? OnMessage;

    public GameSession(IMapGenerator mapGenerator, IRandom random, ICombatSystem combatSystem, IEnemyAI enemyAI) // Update constructor
    {
        _mapGenerator = mapGenerator;
        _random = random;
        _combatSystem = combatSystem;
        _enemyAI = enemyAI; // Assign field
        Initialize();
        _fov = new FieldOfView(Map);
        UpdateFov();
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
            Attack = 10, // Set default attack
            Defense = 2, // Set default defense
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

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        // Simple spawn logic for now
        // In a real game, this would be more sophisticated (e.g. based on level depth)
        int enemyCount = 5;

        for (int i = 0; i < enemyCount; i++)
        {
            int x, y;
            int attempts = 0;
            do
            {
                x = _random.Next(Map.Width); // Use _random
                y = _random.Next(Map.Height); // Use _random
                attempts++;
            } while ((!Map[x, y].IsWalkable || (x == Player.Position.X && y == Player.Position.Y)) && attempts < 100);

            if (Map[x, y].IsWalkable && (x != Player.Position.X || y != Player.Position.Y))
            {
                var enemy = new Creature
                {
                    Name = "Goblin",
                    Symbol = 'g',
                    Color = System.ConsoleColor.Green,
                    HP = 20,
                    MaxHP = 20,
                    Attack = 5,
                    Defense = 1,
                    Position = new Point(x, y)
                };
                Enemies.Add(enemy);
                Map[x, y].Creature = enemy;
            }
        }
    }

    private void UpdateFov()
    {
        _fov.Compute(Player.Position, 10); // 10 is the view radius
    }

    private void Log(string message)
    {
        OnMessage?.Invoke(message);
    }

    public void Update()
    {
        // Game logic update (turn processing)
        foreach (var enemy in Enemies.ToList()) // ToList to allow modification of collection if needed (though we don't remove here)
        {
            var nextPos = _enemyAI.GetNextMove(enemy, Player, Map);
            
            if (nextPos.X == Player.Position.X && nextPos.Y == Player.Position.Y)
            {
                Attack(enemy, Player);
            }
            else if (nextPos != enemy.Position)
            {
                // Move enemy
                Map[enemy.Position].Creature = null;
                enemy.Position = nextPos;
                Map[enemy.Position].Creature = enemy;
            }
        }
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
            // Check for creature
            if (Map[newPos].Creature == null)
            {
                Player.Position = newPos;
                UpdateFov();
                Update(); // Enemy turn after player moves
            }
            else
            {
                Attack(Player, Map[newPos].Creature!);
                Update(); // Enemy turn after player attacks
            }
        }
    }

    private void Attack(Creature attacker, Creature defender)
    {
        _combatSystem.Attack(attacker, defender, Log); // Delegate to combat system

        if (defender.HP <= 0)
        {
            // Death handling remains here or could be part of a result object from combat system
            // For now, we check HP after attack
            Enemies.Remove(defender);
            Map[defender.Position].Creature = null;
        }
    }
}
