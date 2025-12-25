using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpHack.Base.Model;
using WPoint = System.Windows.Point; // Alias for WPF Point

namespace SharpHack.Wpf.Services;

public class TileService : ITileService
{
    private BitmapSource _tileset;
    private int _tileSize;
    private readonly Dictionary<string, CroppedBitmap> _cache = new();

    // Simplified mapping for NetHack 3.6.1 tileset (32x32)
    // Coordinates are (Column, Row) assuming 0-based index
    // These are approximate/example coordinates. In a real scenario, you'd parse a tile definition file.
    private static readonly WPoint P_Player = new(0, 4); // Example: Archeologist?
    private static readonly WPoint P_Wall = new(0, 25); 
    private static readonly WPoint P_Floor = new(1, 25);
    private static readonly WPoint P_DoorClosed = new(2, 25);
    private static readonly WPoint P_DoorOpen = new(3, 25);
    private static readonly WPoint P_StairsUp = new(4, 25);
    private static readonly WPoint P_StairsDown = new(5, 25);
    private static readonly WPoint P_Goblin = new(0, 6); // Example
    private static readonly WPoint P_Sword = new(0, 20); // Example
    private static readonly WPoint P_Armor = new(5, 20); // Example

    public void LoadTileset(string path, int tileSize)
    {
        _tileSize = tileSize;
        // In a real app, load from file. For this demo, we might need a fallback or embedded resource.
        // Assuming the user provides a valid path or we use a placeholder.
        try 
        {
            _tileset = new BitmapImage(new Uri(path, UriKind.RelativeOrAbsolute));
        }
        catch
        {
            // Fallback: Create a generated bitmap if file not found
            _tileset = CreateFallbackTileset(tileSize);
        }
    }

    private BitmapSource CreateFallbackTileset(int size)
    {
        int width = size * 40;
        int height = size * 40;
        var pf = PixelFormats.Pbgra32;
        int stride = (width * pf.BitsPerPixel + 7) / 8;
        byte[] pixels = new byte[height * stride];
        
        // Fill with random noise/colors to distinguish tiles
        var rand = new Random();
        rand.NextBytes(pixels);

        return BitmapSource.Create(width, height, 96, 96, pf, null, pixels, stride);
    }

    public ImageSource GetTile(TileType type)
    {
        WPoint p = type switch
        {
            TileType.Wall => P_Wall,
            TileType.Floor => P_Floor,
            TileType.DoorClosed => P_DoorClosed,
            TileType.DoorOpen => P_DoorOpen,
            TileType.StairsUp => P_StairsUp,
            TileType.StairsDown => P_StairsDown,
            _ => new WPoint(0, 0) // Empty/Void
        };
        return GetSubImage((int)p.X, (int)p.Y);
    }

    public ImageSource GetCreature(Creature creature)
    {
        // Simple mapping based on name
        WPoint p = creature.Name switch
        {
            "Goblin" => P_Goblin,
            _ => P_Goblin
        };
        return GetSubImage((int)p.X, (int)p.Y);
    }

    public ImageSource GetItem(Item item)
    {
        WPoint p = item switch
        {
            Weapon => P_Sword,
            Armor => P_Armor,
            _ => P_Sword
        };
        return GetSubImage((int)p.X, (int)p.Y);
    }

    public ImageSource GetPlayer()
    {
        return GetSubImage((int)P_Player.X, (int)P_Player.Y);
    }

    private ImageSource GetSubImage(int x, int y)
    {
        string key = $"{x},{y}";
        if (_cache.TryGetValue(key, out var cached)) return cached;

        // Check bounds
        if (x * _tileSize >= _tileset.PixelWidth || y * _tileSize >= _tileset.PixelHeight)
            return null; // Or fallback

        var crop = new CroppedBitmap(_tileset, new Int32Rect(x * _tileSize, y * _tileSize, _tileSize, _tileSize));
        _cache[key] = crop;
        return crop;
    }
}
