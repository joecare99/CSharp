using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using SharpHack.Engine;
using SharpHack.Engine.Pathfinding;

namespace SharpHack.ViewModel;

public enum LayeredEntityKind
{
    Player,
    Creature,
    Item
}

public sealed record LayeredEntity(LayeredEntityKind Kind, DisplayTile Tile, string? name);

public sealed record LayeredCell(DisplayTile Structure, IReadOnlyList<LayeredEntity> Entities);

public enum LayeredTileHoverAction
{
    None,
    GoTo,
    Pickup,
    ToggleDoor
}

public partial class LayeredGameViewModel : ObservableObject
{
    private readonly GameSession _session;
    private LayeredCell[] _cells;
    private CancellationTokenSource? _goCts;

    private int _viewOffsetX;
    private int _viewOffsetY;

    [ObservableProperty]
    private string _playerName;

    [ObservableProperty]
    private int _hP;

    [ObservableProperty]
    private int _maxHP;

    [ObservableProperty]
    private int _level;

    [ObservableProperty]
    private bool _autoPickup = true;

    [ObservableProperty]
    private bool _autoEquip = true;

    [ObservableProperty]
    private bool _autoDoorOpen = true;

    public ObservableCollection<string> Messages { get; } = new();
    public ObservableCollection<IItem> Inventory { get; } = new();

    public IMap Map => _session.Map;
    public byte[] MiniMap => _session.MiniMap;

    public ICreature Player => _session.Player;
    public IList<ICreature> Enemies => _session.Enemies;

    public int ViewWidth { get; private set; }
    public int ViewHeight { get; private set; }

    private byte[]? _lastMiniMap;

    public int ViewOffsetX
    {
        get => _viewOffsetX;
        private set => SetProperty(ref _viewOffsetX, value);
    }

    public int ViewOffsetY
    {
        get => _viewOffsetY;
        private set => SetProperty(ref _viewOffsetY, value);
    }

    public void SetViewSize(int width, int height)
    {
        if (ViewWidth == width && ViewHeight == height)
        {
            return;
        }

        ViewWidth = width;
        ViewHeight = height;
        _cells = new LayeredCell[ViewWidth * ViewHeight];
        UpdateCells();

        OnPropertyChanged(nameof(ViewWidth));
        OnPropertyChanged(nameof(ViewHeight));
        OnPropertyChanged(nameof(Cells));
    }

    public IReadOnlyList<LayeredCell> Cells => _cells;

    public LayeredGameViewModel(GameSession session, int viewWidth = 40, int viewHeight = 25)
    {
        _session = session;
        _session.OnMessage += OnGameMessage;

        ViewWidth = viewWidth;
        ViewHeight = viewHeight;
        _cells = new LayeredCell[ViewWidth * ViewHeight];

        PlayerName = _session.Player.Name;
        UpdateStats();
        UpdateInventory();
        UpdateCells();
    }

    private void OnGameMessage(string message)
    {
        Messages.Add(message);
        if (Messages.Count > 5)
        {
            Messages.RemoveAt(0);
        }
    }

    private void UpdateStats()
    {
        HP = _session.Player.HP;
        MaxHP = _session.Player.MaxHP;
        Level = _session.Level;
    }

    private void UpdateInventory()
    {
        Inventory.Clear();
        foreach (var item in _session.Player.Inventory)
        {
            Inventory.Add(item);
        }
    }

    private void UpdateCells()
    {
        var map = Map;
        var player = Player;
        var playerPos = player.Position;
        var cells = new LayeredCell[ViewWidth * ViewHeight];

        int maxOffsetX = Math.Max(0, map.Width - ViewWidth);
        int maxOffsetY = Math.Max(0, map.Height - ViewHeight);
        int offsetX = Math.Clamp(playerPos.X - ViewWidth / 2, 0, maxOffsetX);
        int offsetY = Math.Clamp(playerPos.Y - ViewHeight / 2, 0, maxOffsetY);

        ViewOffsetX = offsetX;
        ViewOffsetY = offsetY;

        int index = 0;
        for (int y = 0; y < ViewHeight; y++)
        {
            int mapY = y + offsetY;
            bool rowInBounds = mapY >= 0 && mapY < map.Height;

            for (int x = 0; x < ViewWidth; x++)
            {
                int mapX = x + offsetX;

                if (!rowInBounds || mapX < 0 || mapX >= map.Width)
                {
                    cells[index] = new LayeredCell(DisplayTile.Empty, Array.Empty<LayeredEntity>());
                    index++;
                    continue;
                }

                var tile = map[mapX, mapY];
                if (!tile.IsExplored)
                {
                    cells[index] = new LayeredCell(DisplayTile.Empty, Array.Empty<LayeredEntity>());
                    index++;
                    continue;
                }

                var surWall = 0;
                if (tile.Type == TileType.Wall || tile.Type == TileType.DoorClosed || tile.Type == TileType.DoorOpen)
                {
                    foreach (var d in new List<(byte bit, int dx, int dy)>() { (1, 1, 0), (2, 0, -1), (4, -1, 0), (8, 0, 1) })
                    {
                        if (map.IsValid(mapX + d.dx, mapY + d.dy)
                            && map[mapX + d.dx, mapY + d.dy].IsExplored
                            && map[mapX + d.dx, mapY + d.dy].Type is TileType.Wall or TileType.DoorClosed or TileType.DoorOpen)
                        {
                            surWall |= d.bit;
                        }
                    }
                }

                var structure = CreateStructureTile(tile, surWall);
                var entities = CreateEntities(tile, isPlayer: mapX == playerPos.X && mapY == playerPos.Y);

                cells[index] = new LayeredCell(structure, entities);
                index++;
            }
        }

        _cells = cells;
        OnPropertyChanged(nameof(PrimaryActionHint));
    }

    private static DisplayTile CreateStructureTile(ITile tile, int surWall)
    {
        if (!tile.IsExplored)
        {
            return DisplayTile.Empty;
        }

        if (!tile.IsVisible && tile.Type == TileType.Room)
        {
            return DisplayTile.Floor_Lit;
        }

        return tile.Type switch
        {
            TileType.Wall => surWall switch
            {
                0 => DisplayTile.Wall_NS,
                1 => DisplayTile.Wall_EW,
                3 => DisplayTile.Wall_ES,
                4 => DisplayTile.Wall_EW,
                5 => DisplayTile.Wall_EW,
                6 => DisplayTile.Wall_WS,
                7 => DisplayTile.Wall_ENW,
                8 => DisplayTile.Wall_NS,
                9 => DisplayTile.Wall_EN,
                10 => DisplayTile.Wall_NS,
                11 => DisplayTile.Wall_ENS,
                12 => DisplayTile.Wall_NW,
                13 => DisplayTile.Wall_EWS,
                14 => DisplayTile.Wall_NWS,
                _ => DisplayTile.Wall_ENWS
            },
            TileType.Floor => DisplayTile.Floor_Dark,
            TileType.Room => DisplayTile.Floor_Lit,
            TileType.DoorClosed => surWall switch
            {
                2 => DisplayTile.Door_Closed_NS,
                8 => DisplayTile.Door_Closed_NS,
                10 => DisplayTile.Door_Closed_NS,
                _ => DisplayTile.Door_Closed_EW
            },
            TileType.DoorOpen => surWall switch
            {
                2 => DisplayTile.Door_Open_NS,
                8 => DisplayTile.Door_Open_NS,
                10 => DisplayTile.Door_Open_NS,
                _ => DisplayTile.Door_Open_EW
            },
            TileType.StairsDown => DisplayTile.Stairs_Down,
            TileType.StairsUp => DisplayTile.Stairs_Up,
            _ => DisplayTile.Empty
        };
    }

    private static IReadOnlyList<LayeredEntity> CreateEntities(ITile tile, bool isPlayer)
    {
        // Rules:
        // - Only show creatures if visible (same as minimap)
        // - Items show if explored (same as old VM)
        // - Player entity always on its tile
        var list = new List<LayeredEntity>(capacity: 3);

        if (isPlayer)
        {
            list.Add(new LayeredEntity(LayeredEntityKind.Player, DisplayTile.Archaeologist, "Player"));
        }

        if (tile.IsVisible && tile.Creature is Creature creature)
        {
            var dt = creature.Name switch
            {
                "Goblin" => DisplayTile.Goblin,
                _ => DisplayTile.Goblin
            };
            list.Add(new LayeredEntity(LayeredEntityKind.Creature, dt, creature.Name));
        }

        if (tile.Items.Count > 0)
        {
            foreach (var item in tile.Items)
            {
                var dt = item.Symbol switch
                {
                    '[' => DisplayTile.Armor,
                    _ => DisplayTile.Sword
                };
                list.Add(new LayeredEntity(LayeredEntityKind.Item, dt, item.Name));
            }
        }

        return list;
    }

    private bool IsEnemyNearPlayer()
    {
        var pp = Player.Position;
        return Enemies.Any(e =>
        {
            int dx = Math.Abs(e.Position.X - pp.X);
            int dy = Math.Abs(e.Position.Y - pp.Y);
            return dx <= 1 && dy <= 1;
        });
    }

    private static Direction? TryGetDirection(Point from, Point to)
    {
        int dx = Math.Clamp(to.X - from.X, -1, 1);
        int dy = Math.Clamp(to.Y - from.Y, -1, 1);

        return (dx, dy) switch
        {
            (0, -1) => Direction.North,
            (0, 1) => Direction.South,
            (-1, 0) => Direction.West,
            (1, 0) => Direction.East,
            (-1, -1) => Direction.NorthWest,
            (1, -1) => Direction.NorthEast,
            (-1, 1) => Direction.SouthWest,
            (1, 1) => Direction.SouthEast,
            _ => null
        };
    }

    private void NotifyMiniMapIfChanged()
    {
        var current = _session.MiniMap;
        if (!ReferenceEquals(_lastMiniMap, current))
        {
            _lastMiniMap = current;
            OnPropertyChanged(nameof(MiniMap));
        }
    }

    public void RefreshMiniMap()
    {
        _lastMiniMap = null;
        OnPropertyChanged(nameof(MiniMap));
    }

    public LayeredTileHoverAction GetHoverActionForView(Point targetView)
    {
        var world = new Point(targetView.X + ViewOffsetX, targetView.Y + ViewOffsetY);
        if (!Map.IsValid(world))
        {
            return LayeredTileHoverAction.None;
        }

        var tile = Map[world];
        if (!tile.IsExplored)
        {
            return LayeredTileHoverAction.None;
        }

        var playerPosition = Player.Position;
        bool adjacent = Math.Abs(world.X - playerPosition.X) <= 1 && Math.Abs(world.Y - playerPosition.Y) <= 1;

        if (adjacent && (tile.Type == TileType.DoorClosed || tile.Type == TileType.DoorOpen))
        {
            return LayeredTileHoverAction.ToggleDoor;
        }

        if (adjacent && tile.Items.Count == 1)
        {
            var item = tile.Items[0];
            if (item is not IContainerItem)
            {
                return LayeredTileHoverAction.Pickup;
            }
        }

        if (tile.IsWalkable)
        {
            return LayeredTileHoverAction.GoTo;
        }

        return LayeredTileHoverAction.None;
    }

    [RelayCommand]
    public void ToggleDoorAtView(Point targetView)
    {
        if (GetHoverActionForView(targetView) != LayeredTileHoverAction.ToggleDoor)
        {
            return;
        }

        var world = new Point(targetView.X + ViewOffsetX, targetView.Y + ViewOffsetY);
        if (_session.ToggleDoorAt(world))
        {
            UpdateCells();
            UpdateStats();
            OnPropertyChanged(nameof(Cells));
            NotifyMiniMapIfChanged();
        }
    }

    [RelayCommand]
    public void PickupAtView(Point targetView)
    {
        if (GetHoverActionForView(targetView) != LayeredTileHoverAction.Pickup)
        {
            return;
        }

        var world = new Point(targetView.X + ViewOffsetX, targetView.Y + ViewOffsetY);
        var tile = Map[world];
        if (tile.Items.Count != 1)
        {
            return;
        }

        _session.PickUpItem(Player, tile.Items[0], autoEquip: AutoEquip);
        UpdateInventory();
        UpdateCells();
        UpdateStats();
        OnPropertyChanged(nameof(Cells));
        NotifyMiniMapIfChanged();
    }

    public bool CanClickGoToView(Point targetView)
    {
        return GetHoverActionForView(targetView) == LayeredTileHoverAction.GoTo;
    }

    [RelayCommand]
    public async Task GoToViewAsync(Point targetView)
    {
        if (!CanClickGoToView(targetView))
        {
            return;
        }

        var targetWorld = new Point(targetView.X + ViewOffsetX, targetView.Y + ViewOffsetY);
        await GoToWorldAsync(targetWorld);
    }

    [RelayCommand]
    public async Task GoToWorldAsync(Point targetWorld)
    {
        if (!Map.IsValid(targetWorld))
        {
            return;
        }

        _goCts?.Cancel();
        _goCts = new CancellationTokenSource();
        var token = _goCts.Token;

        while (!token.IsCancellationRequested)
        {
            if (Player.Position == targetWorld)
            {
                break;
            }

            if (IsEnemyNearPlayer())
            {
                break;
            }

            if (!Map.IsValid(targetWorld) || !Map[targetWorld].IsExplored || !Map[targetWorld].IsWalkable)
            {
                break;
            }

            var start = Player.Position;
            var path = AStarPathfinder.FindPath(
                Map,
                start,
                targetWorld,
                canEnter: p => Map[p].IsExplored && Map[p].IsWalkable);

            if (path is null || path.Count < 2)
            {
                break;
            }

            var next = path[1];
            var direction = TryGetDirection(start, next);
            if (direction is null)
            {
                break;
            }

            Move(direction.Value);

            await Task.Delay(25, token);
        }
    }

    [RelayCommand]
    public void Move(Direction direction)
    {
        _session.MovePlayer(direction, autoPickup: AutoPickup, autoEquip: AutoEquip, autoDoorOpen: AutoDoorOpen);
        UpdateStats();
        UpdateInventory();
        UpdateCells();
        OnPropertyChanged(nameof(Cells));
        NotifyMiniMapIfChanged();
    }

    public string? PrimaryActionHint
    {
        get
        {
            var a = _session.GetPrimaryAction();
            return string.IsNullOrWhiteSpace(a.Message) ? null : a.Message;
        }
    }

    [RelayCommand]
    public void ExecutePrimaryAction()
    {
        if (_session.ExecutePrimaryAction())
        {
            UpdateStats();
            UpdateInventory();
            UpdateCells();
            OnPropertyChanged(nameof(Cells));
            NotifyMiniMapIfChanged();
            OnPropertyChanged(nameof(PrimaryActionHint));
        }
    }
}
