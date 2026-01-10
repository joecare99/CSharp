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

public partial class GameViewModel : ObservableObject
{
    private readonly GameSession _session;
    private DisplayTile[] _displayTiles;

    private int _viewOffsetX;
    private int _viewOffsetY;

    private CancellationTokenSource? _goCts;

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

    public ObservableCollection<string> Messages { get; } = new();
    public ObservableCollection<Item> Inventory { get; } = new();

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
        _displayTiles = new DisplayTile[ViewWidth * ViewHeight];
        UpdateDisplayBuffer();

        OnPropertyChanged(nameof(ViewWidth));
        OnPropertyChanged(nameof(ViewHeight));
        OnPropertyChanged(nameof(DisplayTiles));
    }

    public IReadOnlyList<DisplayTile> DisplayTiles => _displayTiles;

    public GameViewModel(GameSession session, int viewWidth = 40, int viewHeight = 25)
    {
        _session = session;
        _session.OnMessage += OnGameMessage;

        ViewWidth = viewWidth;
        ViewHeight = viewHeight;
        _displayTiles = new DisplayTile[ViewWidth * ViewHeight];

        PlayerName = _session.Player.Name;
        UpdateStats();
        UpdateInventory();
        UpdateDisplayBuffer();
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

    private void UpdateDisplayBuffer()
    {
        var map = Map;
        var player = Player;
        var playerPos = player.Position;

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
                DisplayTile displayTile;

                if (!rowInBounds || mapX < 0 || mapX >= map.Width)
                {
                    displayTile = DisplayTile.Empty;
                }
                else
                {
                    var tile = map[mapX, mapY];
                    var surWall = 0;
                    if (tile.Type == TileType.Wall)
                    {
                        foreach (var d in new List<(byte, int, int)>() { (1, 1, 0), (2, 0, -1), (4, -1, 0), (8, 0, 1) })
                            if (map.IsValid(mapX + d.Item2, mapY + d.Item3) && map[mapX + d.Item2, mapY + d.Item3].IsExplored && map[mapX + d.Item2, mapY + d.Item3].Type == TileType.Wall)
                                surWall |= d.Item1;
                    }
                    bool isPlayer = mapX == playerPos.X && mapY == playerPos.Y;
                    displayTile = CreateDisplayTile(tile, surWall, player, isPlayer);
                }

                _displayTiles[index] = displayTile;
                index++;
            }
        }
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
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        dx = Math.Clamp(dx, -1, 1);
        dy = Math.Clamp(dy, -1, 1);

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

    private static DisplayTile CreateDisplayTile(ITile tile,int surWall, ICreature player, bool isPlayer)
    {
        if (!tile.IsExplored)
        {
            return DisplayTile.Empty;
        }

        if (!tile.IsVisible && tile.Type == TileType.Floor) 
        {
            return tile.IsExplored ? DisplayTile.Floor_Lit : DisplayTile.Floor_Dark;
        }

        if (isPlayer)
        {
            return DisplayTile.Archaeologist;
        }

        if (tile.Creature is Creature creature)
        {
            return creature.Name switch
            {
                "Goblin" => DisplayTile.Goblin,
                _ => DisplayTile.Goblin
            };
        }

        if (tile.Items.Count > 0)
        {
            var item = tile.Items[0];
            return item.Symbol switch
            {
                '[' => DisplayTile.Armor,
                _ => DisplayTile.Sword
            };
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
                9=> DisplayTile.Wall_EN,
                10 => DisplayTile.Wall_NS,
                11 => DisplayTile.Wall_ENS,
                12 => DisplayTile.Wall_NW,
                13 => DisplayTile.Wall_EWS,
                14 => DisplayTile.Wall_NWS,
                _ => DisplayTile.Wall_ENWS               
            },
            TileType.Floor => DisplayTile.Floor_Lit,
            TileType.DoorClosed => DisplayTile.Door_Closed,
            TileType.DoorOpen => DisplayTile.Door_Open,
            TileType.StairsDown => DisplayTile.Stairs_Down,
            TileType.StairsUp => DisplayTile.Stairs_Up,
            _ => DisplayTile.Empty
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

    public enum TileHoverAction
    {
        None,
        GoTo,
        Pickup,
        ToggleDoor
    }

    public TileHoverAction GetHoverActionForView(Point targetView)
    {
        var world = new Point(targetView.X + ViewOffsetX, targetView.Y + ViewOffsetY);
        if (!Map.IsValid(world))
        {
            return TileHoverAction.None;
        }

        var tile = Map[world];
        if (!tile.IsExplored)
        {
            return TileHoverAction.None;
        }

        var pp = Player.Position;
        bool adjacent = Math.Abs(world.X - pp.X) <= 1 && Math.Abs(world.Y - pp.Y) <= 1;

        if (adjacent && (tile.Type == TileType.DoorClosed || tile.Type == TileType.DoorOpen))
        {
            return TileHoverAction.ToggleDoor;
        }

        if (adjacent && tile.Items.Count == 1)
        {
            var item = tile.Items[0];
            if (item is not SharpHack.Base.Interfaces.IContainerItem)
            {
                return TileHoverAction.Pickup;
            }
        }

        if (tile.IsWalkable)
        {
            return TileHoverAction.GoTo;
        }

        return TileHoverAction.None;
    }

    [RelayCommand]
    public void ToggleDoorAtView(Point targetView)
    {
        var action = GetHoverActionForView(targetView);
        if (action != TileHoverAction.ToggleDoor)
        {
            return;
        }

        var world = new Point(targetView.X + ViewOffsetX, targetView.Y + ViewOffsetY);
        if (_session.ToggleDoorAt(world))
        {
            UpdateDisplayBuffer();
            OnPropertyChanged(nameof(DisplayTiles));
            NotifyMiniMapIfChanged();
        }
    }

    public bool CanClickGoToView(Point targetView) => GetHoverActionForView(targetView) == TileHoverAction.GoTo;

    public bool CanClickPickupAtView(Point targetView) => GetHoverActionForView(targetView) == TileHoverAction.Pickup;

    [RelayCommand]
    public void PickupAtView(Point targetView)
    {
        if (!CanClickPickupAtView(targetView))
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
        UpdateDisplayBuffer();
        OnPropertyChanged(nameof(DisplayTiles));
        NotifyMiniMapIfChanged();
    }

    [RelayCommand]
    public void Move(Direction direction)
    {
        _session.MovePlayer(direction, autoPickup: AutoPickup, autoEquip: AutoEquip);
        UpdateStats();
        UpdateInventory();
        UpdateDisplayBuffer();
        OnPropertyChanged(nameof(DisplayTiles));
        NotifyMiniMapIfChanged();
    }

    [RelayCommand]
    public void Wait()
    {
        _session.Update();
        UpdateStats();
        UpdateDisplayBuffer();
        OnPropertyChanged(nameof(DisplayTiles));
        NotifyMiniMapIfChanged();
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

        // Stepwise re-pathing:
        // Only route through explored tiles for the current map state.
        // If new tiles become explored while moving, the next iteration can see them.
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
            var dir = TryGetDirection(start, next);
            if (dir is null)
            {
                break;
            }

            Move(dir.Value);

            await Task.Delay(25, token);
        }
    }
}
