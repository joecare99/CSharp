using System;
using System.Collections.Generic;
using System.Drawing;
using SharpHack.Base.Model;
using BaseLib.Models.Interfaces;
using Point = SharpHack.Base.Model.Point; // Resolve ambiguity with System.Drawing.Point

namespace SharpHack.LevelGen.BSP;

public class BSPMapGenerator : IMapGenerator
{
    private readonly IRandom _random;
    private const int MinNodeSize = 10;
    private const int MinRoomSize = 6;

    public BSPMapGenerator(IRandom random)
    {
        _random = random;
    }

    public Map Generate(int width, int height, Point? point=null)
    {
        var map = new Map(width, height);
        
        // Initialize with walls
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y].Type = TileType.Wall;

        var maxRooms = (width * height) / (MinNodeSize * MinNodeSize);
        var maxSplits = _random.Next(maxRooms-4, maxRooms);
        var root = new BSPNode(new Rectangle(0, 0, width, height));
        Split(root, maxSplits);
        if (point.HasValue)
          CreateStart(root, map, point.Value);
        CreateRooms(root, map);
        ConnectRooms(root, map);

        return map;
    }

    private void CreateStart(BSPNode root, Map map, Point point)
    {
        // Find a leaf node to place the starting room at given point
        BSPNode? node = root;
        while (!node.IsLeaf)
        {
            if (node.Left != null && node.Left.Bounds.Contains(point.X, point.Y))
                node = node.Left;
            else if (node.Right != null && node.Right.Bounds.Contains(point.X, point.Y))
                node = node.Right;
            else
                break; // Point is not in either child, break out
        }
        int w = _random.Next(MinRoomSize, Math.Max(MinRoomSize, node.Bounds.Width - 2));
        int h = _random.Next(MinRoomSize, Math.Max(MinRoomSize, node.Bounds.Height - 2));
        // make sure the random room contains the point
        int x = Math.Clamp(point.X - w / 2, node.Bounds.X + 1, node.Bounds.Right - w - 1);
        int y = Math.Clamp(point.Y - h / 2, node.Bounds.Y + 1, node.Bounds.Bottom - h - 1);
        node.Room = new Rectangle(x, y, w, h);
        for (int rx = x; rx < x + w; rx++)
        {
            for (int ry = y; ry < y + h; ry++)
            {
                if (map.IsValid(rx, ry))
                    map[rx, ry].Type = TileType.Floor;
            }
        }
    }

    private void Split(BSPNode node, int maxspl)
    {
        if (maxspl==0 || node.Bounds.Width < MinNodeSize * 2 && node.Bounds.Height < MinNodeSize * 2)
            return;

        bool splitH = _random.Next(2) == 0;
        if (node.Bounds.Width > node.Bounds.Height && node.Bounds.Width / (double)node.Bounds.Height >= 1.25) splitH = false;
        else if (node.Bounds.Height > node.Bounds.Width && node.Bounds.Height / (double)node.Bounds.Width >= 1.25) splitH = true;

        int max = (splitH ? node.Bounds.Height : node.Bounds.Width) - MinNodeSize;
        if (max <= MinNodeSize) return;

        int split = _random.Next(MinNodeSize, max);

        if (splitH)
        {
            node.Left = new BSPNode(new Rectangle(node.Bounds.X, node.Bounds.Y, node.Bounds.Width, split));
            node.Right = new BSPNode(new Rectangle(node.Bounds.X, node.Bounds.Y + split, node.Bounds.Width, node.Bounds.Height - split));
        }
        else
        {
            node.Left = new BSPNode(new Rectangle(node.Bounds.X, node.Bounds.Y, split, node.Bounds.Height));
            node.Right = new BSPNode(new Rectangle(node.Bounds.X + split, node.Bounds.Y, node.Bounds.Width - split, node.Bounds.Height));
        }

        node.Left.Parent = node;
        node.Right.Parent = node;

        if (_random.Next(2) == 0)
        {
            Split(node.Left, (max + 1) / 2);
            Split(node.Right, max / 2);
        }
        else
        {
            Split(node.Right, (max + 1) / 2);
            Split(node.Left, max / 2);
        }
    }
    private void CreateRooms(BSPNode node, Map map)
    {
        if (node.IsLeaf && node.Room==null)
        {
            int w = _random.Next(MinRoomSize, Math.Max(MinRoomSize, node.Bounds.Width - 2));
            int h = _random.Next(MinRoomSize, Math.Max(MinRoomSize, node.Bounds.Height - 2));
            int x = _random.Next(node.Bounds.X + 1, node.Bounds.Right - w - 1);
            int y = _random.Next(node.Bounds.Y + 1, node.Bounds.Bottom - h - 1);

            node.Room = new Rectangle(x, y, w, h);

            for (int rx = x; rx < x + w; rx++)
            {
                for (int ry = y; ry < y + h; ry++)
                {
                    if (map.IsValid(rx, ry))
                        map[rx, ry].Type = TileType.Floor;
                }
            }
        }
        else
        {
            if (node.Left != null) CreateRooms(node.Left, map);
            if (node.Right != null) CreateRooms(node.Right, map);
        }
    }

    private void ConnectRooms(BSPNode node, Map map)
    {
        if (node.IsLeaf) return;

        ConnectRooms(node.Left!, map);
        ConnectRooms(node.Right!, map);

        // Connect the rooms of the left and right children
        var leftRoom = GetRoom(node.Left!);
        var rightRoom = GetRoom(node.Right!);

        var start = new Point(leftRoom.X + leftRoom.Width / 2, leftRoom.Y + leftRoom.Height / 2);
        var end = new Point(rightRoom.X + rightRoom.Width / 2, rightRoom.Y + rightRoom.Height / 2);

        CreateCorridor(map, start, end);
    }

    private Rectangle GetRoom(BSPNode node)
    {
        if (node.IsLeaf) return node.Room.Value;
        // If not leaf, pick a random room from one of its children to connect to
        return _random.Next(2) == 0 ? GetRoom(node.Left!) : GetRoom(node.Right!);
    }

    private void CreateCorridor(Map map, Point start, Point end)
    {
        int x = start.X;
        int y = start.Y;

        while (x != end.X || y != end.Y)
        {
            if (map.IsValid(x, y))
            {
                // Only overwrite walls, don't overwrite existing floor (rooms)
                if (map[x, y].Type == TileType.Wall || map[x, y].Type == TileType.Empty)
                    map[x, y].Type = TileType.Floor;
            }

            if (_random.Next(2) == 0)
            {
                if (x != end.X) x += x < end.X ? 1 : -1;
                else if (y != end.Y) y += y < end.Y ? 1 : -1;
            }
            else
            {
                if (y != end.Y) y += y < end.Y ? 1 : -1;
                else if (x != end.X) x += x < end.X ? 1 : -1;
            }
        }
    }
}
