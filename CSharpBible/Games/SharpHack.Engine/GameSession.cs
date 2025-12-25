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
    public int Level { get; private set; } = 1; // Add Level property

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
        if (Player == null) // Only create player if not exists (preserve stats between levels)
        {
            Player = new Creature
            {
                Name = "Hero",
                Symbol = '@',
                Color = System.ConsoleColor.Yellow,
                HP = 100,
                MaxHP = 100,
                BaseAttack = 10,
                BaseDefense = 2,
                Position = new Point(1, 1)
            };
        }
        
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
        SpawnItems();
        SpawnStairs(); // Add call
    }

    private void SpawnItems()
    {
        int itemCount = 5;
        for (int i = 0; i < itemCount; i++)
        {
            int x, y;
            int attempts = 0;
            do
            {
                x = _random.Next(Map.Width);
                y = _random.Next(Map.Height);
                attempts++;
            } while ((!Map[x, y].IsWalkable || Map[x, y].Items.Count > 0) && attempts < 100);

            if (Map[x, y].IsWalkable && Map[x, y].Items.Count == 0)
            {
                var itemType = _random.Next(2);
                Item item;
                if (itemType == 0)
                {
                    item = new Weapon { Name = "Sword", Symbol = '/', Color = System.ConsoleColor.Cyan, AttackBonus = 5 };
                }
                else
                {
                    item = new Armor { Name = "Leather Armor", Symbol = '[', Color = System.ConsoleColor.Cyan, DefenseBonus = 2 };
                }
                item.Position = new Point(x, y);
                Map[x, y].Items.Add(item);
            }
        }
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
                    BaseAttack = 5, // Changed from Attack
                    BaseDefense = 1, // Changed from Defense
                    Position = new Point(x, y)
                };
                Enemies.Add(enemy);
                Map[x, y].Creature = enemy;
            }
        }
    }

    private void SpawnStairs()
    {
        int x, y;
        int attempts = 0;
        do
        {
            x = _random.Next(Map.Width);
            y = _random.Next(Map.Height);
            attempts++;
        } while ((!Map[x, y].IsWalkable || Map[x, y].Items.Count > 0 || (x == Player.Position.X && y == Player.Position.Y)) && attempts < 100);

        if (Map[x, y].IsWalkable)
        {
            Map[x, y].Type = TileType.StairsDown;
        }
    }

    private void NextLevel()
    {
        Level++;
        Enemies.Clear();
        Initialize();
        _fov.Map = Map; // Update FOV map reference
        UpdateFov();
        Log($"You descend to level {Level}.");
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
                
                // Check for items
                if (Map[newPos].Items.Count > 0)
                {
                    foreach (var item in Map[newPos].Items.ToList())
                    {
                        Log($"You see a {item.Name}.");
                        // Auto-pickup for now
                        PickUpItem(Player, item);
                    }
                }

                // Check for stairs
                if (Map[newPos].Type == TileType.StairsDown)
                {
                    NextLevel();
                    return; // Skip update after level change to avoid immediate enemy move on new level
                }

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

    public void PickUpItem(Creature creature, Item item)
    {
        if (Map[item.Position].Items.Contains(item))
        {
            Map[item.Position].Items.Remove(item);
            creature.Inventory.Add(item);
            Log($"{creature.Name} picks up {item.Name}.");

            // Auto-equip if slot is empty
            if (item is Weapon w && creature.MainHand == null)
            {
                creature.MainHand = w;
                Log($"{creature.Name} equips {w.Name}.");
            }
            else if (item is Armor a && creature.Body == null)
            {
                creature.Body = a;
                Log($"{creature.Name} equips {a.Name}.");
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
