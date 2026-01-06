using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.ExceptionServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SharpHack.Base.Model;
using SharpHack.Engine;

namespace SharpHack.ViewModel;

public partial class GameViewModel : ObservableObject
{
    private readonly GameSession _session;
    private DisplayTile[] _displayTiles;

    [ObservableProperty]
    private string _playerName;

    [ObservableProperty]
    private int _hP;

    [ObservableProperty]
    private int _maxHP;

    [ObservableProperty]
    private int _level;

    public ObservableCollection<string> Messages { get; } = new();
    public ObservableCollection<Item> Inventory { get; } = new();

    public Map Map => _session.Map;

    public byte[] miniMap => _session.MiniMap;
    public Creature Player => _session.Player;
    public List<Creature> Enemies => _session.Enemies;

    public int ViewWidth { get; private set; }
    public int ViewHeight { get; private set; }

    public void SetViewSize(int width, int height)
    {
        ViewWidth = width;
        ViewHeight = height;
        _displayTiles = new DisplayTile[ViewWidth * ViewHeight];
        UpdateDisplayBuffer();
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
                            if (map.IsValid(mapX + d.Item2, mapY + d.Item3) && map[mapX + d.Item2, mapY + d.Item3].IsExplored && map[mapX + d.Item2, mapY + d.Item3].Type == TileType.Wall )
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

    private static DisplayTile CreateDisplayTile(Tile tile,int surWall, Creature player, bool isPlayer)
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

    [RelayCommand]
    public void Move(Direction direction)
    {
        _session.MovePlayer(direction);
        UpdateStats();
        UpdateInventory();
        UpdateDisplayBuffer();
        OnPropertyChanged(nameof(Map));
        OnPropertyChanged(nameof(DisplayTiles));
    }

    [RelayCommand]
    public void Wait()
    {
        _session.Update();
        UpdateStats();
        UpdateDisplayBuffer();
        OnPropertyChanged(nameof(Map));
        OnPropertyChanged(nameof(DisplayTiles));
    }
}
