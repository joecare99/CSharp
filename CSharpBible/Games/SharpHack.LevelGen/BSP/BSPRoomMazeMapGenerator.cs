using System;
using System.Collections.Generic;
using System.Drawing;
using BaseLib.Models.Interfaces;
using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;
using SharpHack.Base.Model;
using SharpHack.Engine.Pathfinding;
using Point = SharpHack.Base.Model.Point;

namespace SharpHack.LevelGen.BSP;

/// <summary>
    /// BSP based generator that splits the whole map into partitions (like <see cref="BSPMapGenerator"/>),
    /// but only creates fixed 3x3 rooms with an even/even center coordinate (and a guaranteed room around the start position).
/// The remaining area becomes a corridor maze (perfect maze), and rooms are connected to the maze via doors.
/// </summary>
public sealed class BSPRoomMazeMapGenerator : IMapGenerator
{
    private readonly IRandom _random;
    private BSPNode _root = null!;

    private const int MinNodeSize = 8;
    private const int RoomSize = 3;

    private readonly record struct DoorCandidate(Point Position);

    public BSPRoomMazeMapGenerator(IRandom random)
    {
        _random = random;
    }

    public IMap Generate(int width, int height, Point? startPos = null)
    {
        var map = new Map(width, height);

        // Init walls
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y].Type = TileType.Wall;

        // Split
        var maxRooms = (width * height * 2) / (MinNodeSize * MinNodeSize * 3);
        var maxSplits = Math.Max(1, _random.Next(Math.Max(1, maxRooms - 4), Math.Max(2, maxRooms)));
        _root = new BSPNode(new Rectangle(0, 0, width, height));
        Split(_root, maxSplits);

        // Create rooms
        var roomCenters = new List<Point>();
        if (startPos.HasValue)
        {
            CreateStartRoom(map, startPos.Value, roomCenters);
        }
        CreateFixedRooms(_root, map, roomCenters);

        // Create maze in remaining walls (odd grid)
        CarveMaze(map);

        // Connect each room to the maze with a door
        var doorCandidates = new List<DoorCandidate>();
        ConnectRoomsToMaze(map, roomCenters, doorCandidates);
        ApplyDoors(map, doorCandidates);

        return map;
    }

    private void Split(BSPNode node, int maxspl)
    {
        if (maxspl == 0 || (node.Bounds.Width < MinNodeSize * 2 && node.Bounds.Height < MinNodeSize * 2))
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

    private void CreateStartRoom(Map map, Point start, List<Point> roomCenters)
    {
        // Force a 3x3 room centered on the (clamped) even coordinate near start.
        var center = new Point(ClampEven(start.X, 1, map.Width - 2), ClampEven(start.Y, 1, map.Height - 2));
        CarveRoom3x3(map, center);
        roomCenters.Add(center);
    }

    private void CreateFixedRooms(BSPNode node, Map map, List<Point> roomCenters)
    {
        if (node.IsLeaf)
        {
            // Choose a 3x3 room center on odd coordinates inside node bounds with a 1-tile margin.
            // room footprint: center +/-1.
            int minX = node.Bounds.Left + 2;
            int maxX = node.Bounds.Right - 3;
            int minY = node.Bounds.Top + 2;
            int maxY = node.Bounds.Bottom - 3;

            if (minX > maxX || minY > maxY) return;

            int cx = ClampEven(_random.Next(minX, maxX + 1), minX, maxX);
            int cy = ClampEven(_random.Next(minY, maxY + 1), minY, maxY);

            var center = new Point(cx, cy);

            // Avoid duplicates / overlaps with existing rooms (simple check on center distance)
            foreach (var c in roomCenters)
            {
                if (Math.Abs(c.X - center.X) <= 2 && Math.Abs(c.Y - center.Y) <= 2)
                    return;
            }

            CarveRoom3x3(map, center);
            roomCenters.Add(center);
            node.Room = new Rectangle(center.X - 1, center.Y - 1, RoomSize, RoomSize);
            return;
        }

        if (node.Left != null) CreateFixedRooms(node.Left, map, roomCenters);
        if (node.Right != null) CreateFixedRooms(node.Right, map, roomCenters);
    }

    private static void CarveRoom3x3(Map map, Point center)
    {
        for (int x = center.X - 1; x <= center.X + 1; x++)
        {
            for (int y = center.Y - 1; y <= center.Y + 1; y++)
            {
                if (map.IsValid(x, y))
                    map[x, y].Type = TileType.Room;
            }
        }
    }

    private void CarveMaze(Map map)
    {
        // Standard DFS perfect maze on odd grid.
        // We carve floors only where there are walls (do not overwrite rooms).

        if (map.Width < 3 || map.Height < 3) return;

        var visited = new bool[map.Width, map.Height];

        bool IsBlockedByRoomBuffer(int x, int y)
        {
            // Keep a wall ring around rooms: do not carve inside room tiles nor directly adjacent to them.
            // (adjacent means 8-neighborhood)
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = x + dx;
                    int ny = y + dy;
                    if (!map.IsValid(nx, ny)) continue;
                    if (map[nx, ny].Type == TileType.Room) return true;
                }
            }
            return false;
        }

        Point start = new Point(1, 1);
        // find a start cell that is not blocked by a room buffer
        for (int x = 1; x < map.Width - 1; x += 2)
        {
            bool found = false;
            for (int y = 1; y < map.Height - 1; y += 2)
            {
                if (!IsBlockedByRoomBuffer(x, y))
                {
                    start = new Point(x, y);
                    found = true;
                    break;
                }
            }
            if (found) break;
        }

        var stack = new Stack<Point>();
        stack.Push(start);
        MarkCell(start);

        while (stack.Count > 0)
        {
            var cur = stack.Peek();

            var neighbors = GetUnvisitedNeighbors(cur);
            if (neighbors.Count == 0)
            {
                stack.Pop();
                continue;
            }

            var next = neighbors[_random.Next(neighbors.Count)];
            CarveBetween(cur, next);
            MarkCell(next);
            stack.Push(next);
        }

        void MarkCell(Point p)
        {
            visited[p.X, p.Y] = true;
            if (map[p.X, p.Y].Type == TileType.Wall || map[p.X, p.Y].Type == TileType.Empty)
                map[p.X, p.Y].Type = TileType.Floor;
        }

        void CarveBetween(Point a, Point b)
        {
            int mx = (a.X + b.X) / 2;
            int my = (a.Y + b.Y) / 2;

            if (map.IsValid(mx, my) && !IsBlockedByRoomBuffer(mx, my) && (map[mx, my].Type == TileType.Wall || map[mx, my].Type == TileType.Empty))
                map[mx, my].Type = TileType.Floor;

            if (map.IsValid(b.X, b.Y) && !IsBlockedByRoomBuffer(b.X, b.Y) && (map[b.X, b.Y].Type == TileType.Wall || map[b.X, b.Y].Type == TileType.Empty))
                map[b.X, b.Y].Type = TileType.Floor;
        }

        List<Point> GetUnvisitedNeighbors(Point p)
        {
            var list = new List<Point>(4);
            foreach (var (dx, dy) in new (int dx, int dy)[] { (2, 0), (-2, 0), (0, 2), (0, -2) })
            {
                int nx = p.X + dx;
                int ny = p.Y + dy;
                if (nx <= 0 || ny <= 0 || nx >= map.Width - 1 || ny >= map.Height - 1) continue;
                if (visited[nx, ny]) continue;
                if (IsBlockedByRoomBuffer(nx, ny)) continue;
                list.Add(new Point(nx, ny));
            }
            return list;
        }
    }

    private void ConnectRoomsToMaze(Map map, List<Point> roomCenters, List<DoorCandidate> doorCandidates)
    {
        foreach (var center in roomCenters)
        {
            var door = FindRoomToMazeDoor(map, center);
            if (door.HasValue)
            {
                doorCandidates.Add(new DoorCandidate(door.Value));
            }
        }
    }

    private static Point? FindRoomToMazeDoor(Map map, Point roomCenter)
    {
        // Keep a 1-tile wall ring around the room. The first wall ring is at distance 2 from center.
        // We place the door into that wall ring (distance 2) and make sure the corridor starts at distance 3.
        // Prefer cardinal directions.
        foreach (var (dx, dy) in new (int dx, int dy)[] { (0, -1), (1, 0), (0, 1), (-1, 0) })
        {
            // For 3x3 room, the outer wall candidate sits 2 steps away from center.
            int wallX = roomCenter.X + dx * 2;
            int wallY = roomCenter.Y + dy * 2;
            int corridorX = roomCenter.X + dx * 3;
            int corridorY = roomCenter.Y + dy * 3;

            if (!map.IsValid(wallX, wallY) || !map.IsValid(corridorX, corridorY)) continue;

            if (map[wallX, wallY].Type == TileType.Wall && map[corridorX, corridorY].Type == TileType.Floor)
                return new Point(wallX, wallY);
        }

        // Fallback: search small radius around room for a wall bordering floor
        for (int x = roomCenter.X - 3; x <= roomCenter.X + 3; x++)
        {
            for (int y = roomCenter.Y - 3; y <= roomCenter.Y + 3; y++)
            {
                if (!map.IsValid(x, y)) continue;
                if (map[x, y].Type != TileType.Wall) continue;

                if (IsAdjacentToType(map, x, y, TileType.Room) && IsAdjacentToType(map, x, y, TileType.Floor))
                    return new Point(x, y);
            }
        }

        return null;
    }

    private static bool IsAdjacentToType(Map map, int x, int y, TileType t)
    {
        foreach (var (dx, dy) in new (int dx, int dy)[] { (1,0), (-1,0), (0,1), (0,-1) })
        {
            int nx = x + dx;
            int ny = y + dy;
            if (!map.IsValid(nx, ny)) continue;
            if (map[nx, ny].Type == t) return true;
        }
        return false;
    }

    private void ApplyDoors(Map map, List<DoorCandidate> doorCandidates)
    {
        // Deduplicate
        var used = new HashSet<Point>();
        foreach (var dc in doorCandidates)
        {
            if (!used.Add(dc.Position)) continue;
            if (!map.IsValid(dc.Position.X, dc.Position.Y)) continue;

            var tile = map[dc.Position.X, dc.Position.Y];
            if (tile.Type != TileType.Wall) continue;

            // Only doors that connect room <-> floor
            if (IsAdjacentToType(map, dc.Position.X, dc.Position.Y, TileType.Room) &&
                IsAdjacentToType(map, dc.Position.X, dc.Position.Y, TileType.Floor))
            {
                tile.Type = TileType.DoorClosed;
            }
        }
    }

    private static int ClampEven(int value, int min, int max)
    {
        int v = Math.Clamp(value, min, max);
        if ((v & 1) == 1)
        {
            if (v + 1 <= max) v++;
            else if (v - 1 >= min) v--;
        }
        return v;
    }
}
