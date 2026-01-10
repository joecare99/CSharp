using System.Linq;
using SharpHack.Base.Model;
using SharpHack.LevelGen;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Data;

namespace SharpHack.Engine;

public class GameSession
{
    public IMap Map { get; private set; }
    public ICreature Player { get; private set; }
    public IList<ICreature> Enemies { get; private set; } = [];
    public bool IsRunning { get; private set; } = true;
    public int Level { get; private set; } = 1; // Add Level property

#if _DEBUG
    private const int Width = 30;
    private const int Height = 20;
#else
    private const int Width = 80;
    private const int Height = 50;
#endif
    private readonly IMapGenerator _mapGenerator;
    private readonly IRandom _random;
    private readonly ICombatSystem _combatSystem;
    private readonly IEnemyAI _enemyAI; // Add field
    private readonly FieldOfView _fov;
    private readonly IGamePersist _persistence;

    public byte[] MiniMap {
        get
        {
            // Simple minimap representation: 1 byte per tile
            var miniMap = new byte[Map.Width * Map.Height];
            for (int x = 0; x < Map.Width; x++)
            {
                for (int y = 0; y < Map.Height; y++)
                {
                    var tile = Map[x, y];
                    byte value = !tile.IsExplored? (byte)0 : (byte)(tile.Type switch
                    {
                        TileType.Wall => 8,
                        TileType.Floor => 1,
                        TileType.StairsUp => 4,
                        TileType.StairsDown => 4,
                        TileType.DoorClosed => 4,
                        TileType.DoorOpen => 4,
                        _ => 0
                    });
                    if (tile.IsVisible && tile.Creature != null)
                    {
                       value |= 0x40; // Enemy
                    }
                    if (tile.IsExplored && tile.Items.Count > 0)
                    {
                       value |= 0x2; // Items
                    }
                    if (Player.Position.X == x && Player.Position.Y == y)
                    {
                        value |= 0x80; // Player
                    }
                    miniMap[y * Map.Width + x] = value;
                }
            }
            return miniMap;
        }
    }

    public event Action<string>? OnMessage;

    public GameSession(IMapGenerator mapGenerator,IGamePersist gamePersist, IRandom random, ICombatSystem combatSystem, IEnemyAI enemyAI) // Update constructor
    {
        _mapGenerator = mapGenerator;
        _random = random;
        _combatSystem = combatSystem;
        _enemyAI = enemyAI; // Assign field
        _persistence = gamePersist;
        Initialize();
        _fov = new FieldOfView(Map);
        UpdateFov();
    }

    private static IEnumerable<Point> GetAdjacent8(Point p)
    {
        yield return new Point(p.X - 1, p.Y - 1);
        yield return new Point(p.X, p.Y - 1);
        yield return new Point(p.X + 1, p.Y - 1);
        yield return new Point(p.X - 1, p.Y);
        yield return new Point(p.X + 1, p.Y);
        yield return new Point(p.X - 1, p.Y + 1);
        yield return new Point(p.X, p.Y + 1);
        yield return new Point(p.X + 1, p.Y + 1);
    }

    private void EnsureEntryAreaWalkable(Point entry)
    {
        if (Map == null)
        {
            return;
        }

        if (!Map.IsValid(entry))
        {
            return;
        }

        var entryTile = Map[entry];
        if (entryTile == null)
        {
            return;
        }

        if (entryTile.Type == TileType.Wall)
        {
            entryTile.Type = TileType.Floor;
        }

        bool hasExit = GetAdjacent8(entry).Any(p =>
        {
            if (!Map.IsValid(p))
            {
                return false;
            }

            var t = Map[p];
            return t != null && t.IsWalkable;
        });

        if (hasExit)
        {
            return;
        }

        foreach (var p in GetAdjacent8(entry))
        {
            if (!Map.IsValid(p))
            {
                continue;
            }

            var t = Map[p];
            if (t == null)
            {
                continue;
            }

            if (t.Type == TileType.Wall)
            {
                t.Type = TileType.Floor;
                break;
            }
        }
    }

    private void Initialize(Point? startPosition = null)
    {
        Map = _mapGenerator.Generate(Width, Height, startPosition);

        if (Player == null)
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

        if (startPosition.HasValue)
        {
            Player.Position = startPosition.Value;
            EnsureEntryAreaWalkable(Player.Position);

            var st = Map[Player.Position];
            if (st != null)
            {
                st.Type = TileType.StairsUp;
            }
        }
        else
        {
            // Ensure player is on a valid tile (initial spawn)
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

    private void NextLevel(Point entryPosition)
    {
        _persistence.SaveLevel(Level, Map, Player, Enemies);
        Level++;
        if (_persistence.LoadLevel(Level, out var _Map, out var _Enemies))
        {
            Map = _Map;
            Enemies = _Enemies;
        }
        else
        {
            Enemies.Clear();
            Initialize(entryPosition);
        }
        _fov.Map = Map;
        UpdateFov();
        Log($"You descend to level {Level}.");
    }
    private void PrevLevel(Point entryPosition)
    {
        _persistence.SaveLevel(Level, Map, Player, Enemies);
        Level--;
        if (_persistence.LoadLevel(Level, out var _Map, out var _Enemies))
        {
            Map = _Map;
            Enemies = _Enemies;
        }
        else
        {
            Enemies.Clear();
            Initialize(entryPosition);
        }
        _fov.Map = Map;
        UpdateFov();
        Log($"You ascend to level {Level}.");
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

    public void MovePlayer(Direction direction, bool autoPickup = true, bool autoEquip = true)
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
            if (Map[newPos].Creature == null)
            {
                Player.Position = newPos;

                if (Map[newPos].Items.Count > 0)
                {
                    foreach (var item in Map[newPos].Items.ToList())
                    {
                        Log($"You see a {item.Name}.");
                        if (autoPickup)
                        {
                            PickUpItem(Player, item, autoEquip);
                        }
                    }
                }

                if (Map[newPos].Type == TileType.StairsDown)
                {
                    NextLevel(newPos);
                    return;
                }

                if (Map[newPos].Type == TileType.StairsUp)
                {
                    PrevLevel(newPos);
                    return;
                }

                UpdateFov();
                Update();
            }
            else
            {
                Attack(Player, Map[newPos].Creature!);
                Update();
            }
        }
    }

    public void PickUpItem(ICreature creature, Item item, bool autoEquip = true)
    {
        if (Map[item.Position].Items.Contains(item))
        {
            Map[item.Position].Items.Remove(item);
            creature.Inventory.Add(item);
            Log($"{creature.Name} picks up {item.Name}.");

            if (!autoEquip)
            {
                return;
            }

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

    private void Attack(ICreature attacker, ICreature defender)
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

    public bool ToggleDoorAt(Point position)
    {
        if (!Map.IsValid(position))
        {
            return false;
        }

        // Only allow if adjacent (8-neighborhood)
        if (Math.Abs(position.X - Player.Position.X) > 1 || Math.Abs(position.Y - Player.Position.Y) > 1)
        {
            return false;
        }

        var tile = Map[position];
        if (tile == null)
        {
            return false;
        }

        if (!tile.IsExplored)
        {
            return false;
        }

        if (tile.Type == TileType.DoorClosed)
        {
            tile.Type = TileType.DoorOpen;
            UpdateFov();
            Log("You open the door.");
            return true;
        }

        if (tile.Type == TileType.DoorOpen)
        {
            // Don't close on creatures; basic safety
            if (tile.Creature != null)
            {
                return false;
            }

            tile.Type = TileType.DoorClosed;
            UpdateFov();
            Log("You close the door.");
            return true;
        }

        return false;
    }
}
