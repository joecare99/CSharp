using System;
using System.Collections.Generic;
using System.Drawing;
using SharpHack.Base.Model;
using BaseLib.Models.Interfaces;
using Point = SharpHack.Base.Model.Point;
using SharpHack.Base.Data;
using SharpHack.Base.Interfaces;
using SharpHack.Engine.Pathfinding; // Resolve ambiguity with System.Drawing.Point

namespace SharpHack.LevelGen.BSP;

public class BSPMapGenerator : IMapGenerator
{
    private readonly IRandom _random;
    private BSPNode root;
    private const int MinNodeSize = 10;
    private const int MinRoomSize = 6;

    private enum DoorCandidateKind
    {
        RoomConnector,
        CorridorJunction
    }

    private readonly record struct DoorCandidate(Point Position, DoorCandidateKind Kind);

    public BSPMapGenerator(IRandom random)
    {
        _random = random;
    }

    public IMap Generate(int width, int height, Point? point=null)
    {
        var map = new Map(width, height);

        // Initialize with walls
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y].Type = TileType.Wall;

        var maxRooms = (width * height*2) / (MinNodeSize * MinNodeSize*3);
        var maxSplits = _random.Next(maxRooms-4, maxRooms);
        root = new BSPNode(new Rectangle(0, 0, width, height));
        Split(root, maxSplits);
        if (point.HasValue)
          CreateStart(root, map, point.Value);
        CreateRooms(root, map);

        var doorCandidates = new List<DoorCandidate>();
        ConnectRooms(root, map, doorCandidates);
        ApplyDoorCandidates(map, doorCandidates);

        return map;
    }

    private void CreateStart(BSPNode root, Map map, Point point)
    {
        // Find a leaf node to place the starting room at given point
        BSPNode node = FindLeafNode(root, point);
        int w = _random.Next(MinRoomSize, Math.Max(MinRoomSize, node.Bounds.Width - 2));
        int h = _random.Next(MinRoomSize, Math.Max(MinRoomSize, node.Bounds.Height - 2));
        // make sure the random room contains the point
        int x = Math.Clamp(point.X - w / 2, node.Bounds.X, node.Bounds.Right - w);
        int y = Math.Clamp(point.Y - h / 2, node.Bounds.Y, node.Bounds.Bottom - h);
        node.Room = new Rectangle(x, y, w, h);
        for (int rx = x; rx < x + w; rx++)
        {
            for (int ry = y; ry < y + h; ry++)
            {
                if (map.IsValid(rx, ry))
                    map[rx, ry].Type = TileType.Room;
            }
        }
    }

    private static BSPNode FindLeafNode(BSPNode root, Point point)
    {
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

        return node;
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
                        map[rx, ry].Type = TileType.Room;
                }
            }
        }
        else
        {
            if (node.Left != null) CreateRooms(node.Left, map);
            if (node.Right != null) CreateRooms(node.Right, map);
        }
    }

    private void ConnectRooms(BSPNode node, Map map, List<DoorCandidate> doorCandidates)
    {
        if (node.IsLeaf) return;

        ConnectRooms(node.Left!, map, doorCandidates);
        ConnectRooms(node.Right!, map, doorCandidates);

        var leftRoom = GetRandomRoom(node.Left!)!.Value;
        var rightRoom = GetRandomRoom(node.Right!)!.Value;

        // Pick wall breakthrough points (on room wall) for both rooms.
        var (leftDoor, leftOutside) = PickRoomDoorAndOutside(leftRoom, rightRoom, map);
        var (rightDoor, rightOutside) = PickRoomDoorAndOutside(rightRoom, leftRoom, map);

        // Carve from each room wall one step outward (keeps most wall intact).
        if (map.IsValid(leftDoor.X, leftDoor.Y) && (map[leftDoor.X, leftDoor.Y].Type == TileType.Wall || map[leftDoor.X, leftDoor.Y].Type == TileType.Empty))
            map[leftDoor.X, leftDoor.Y].Type = TileType.Floor;

        if (map.IsValid(rightDoor.X, rightDoor.Y) && (map[rightDoor.X, rightDoor.Y].Type == TileType.Wall || map[rightDoor.X, rightDoor.Y].Type == TileType.Empty))
            map[rightDoor.X, rightDoor.Y].Type = TileType.Floor;

        // Carve from each room wall one step outward (keeps most wall intact).
        if (map.IsValid(leftOutside.X, leftOutside.Y) && (map[leftOutside.X, leftOutside.Y].Type == TileType.Wall || map[leftOutside.X, leftOutside.Y].Type == TileType.Empty))
            map[leftOutside.X, leftOutside.Y].Type = TileType.Floor;

        if (map.IsValid(rightOutside.X, rightOutside.Y) && (map[rightOutside.X, rightOutside.Y].Type == TileType.Wall || map[rightOutside.X, rightOutside.Y].Type == TileType.Empty))
            map[rightOutside.X, rightOutside.Y].Type = TileType.Floor;

        // Door candidates are exactly on the room wall breakthrough tiles.
        doorCandidates.Add(new DoorCandidate(leftDoor, DoorCandidateKind.RoomConnector));
        doorCandidates.Add(new DoorCandidate(rightDoor, DoorCandidateKind.RoomConnector));

        CreateCorridor(root, map, leftOutside, rightOutside, doorCandidates);
    }

    private static (Point door, Point outside) PickRoomDoorAndOutside(Rectangle room, Rectangle otherRoom, Map map)
    {
        // Decide which wall to use based on relative position of the other room.
        int roomCenterX = room.X + room.Width / 2;
        int roomCenterY = room.Y + room.Height / 2;
        int otherCenterX = otherRoom.X + otherRoom.Width / 2;
        int otherCenterY = otherRoom.Y + otherRoom.Height / 2;

        int dx = otherCenterX - roomCenterX;
        int dy = otherCenterY - roomCenterY;

        // Choose dominant axis.
        bool horizontal = Math.Abs(dx) >= Math.Abs(dy);

        if (horizontal)
        {
            bool toRight = dx >= 0;
            int doorX = toRight ? room.Right  : room.Left-1;
            int doorY = Math.Clamp(otherCenterY, room.Top + 1, room.Bottom - 1);
            var door = new Point(doorX, doorY);
            var outside = new Point(doorX + (toRight ? 1 : -1), doorY);
            return (door, outside);
        }
        else
        {
            bool toDown = dy >= 0;
            int doorY = toDown ? room.Bottom : room.Top-1;
            int doorX = Math.Clamp(otherCenterX, room.Left + 1, room.Right - 1);
            var door = new Point(doorX, doorY);
            var outside = new Point(doorX, doorY + (toDown ? 1 : -1));
            return (door, outside);
        }
    }

    private Rectangle? GetRandomRoom(BSPNode node)
    {
        if (node.IsLeaf) return node.Room;
        // If not leaf, pick a random room from one of its children to connect to
        return _random.Next(2) == 0 ? GetRandomRoom(node.Left!) : GetRandomRoom(node.Right!);
    }

    private void CreateCorridor(BSPNode node, IMap map, Point start, Point end, List<DoorCandidate> doorCandidates)
    {
        int x = start.X;
        int y = start.Y;

        // 1. Try to find a path with no intersections 
        List<Point>? path = default;
        if(map[start].Type == TileType.Wall && map[end].Type == TileType.Wall)
           path =  map.FindPath(start, end, (p) => p.GetNeighbors8().All(p=> map[p]?.Type is TileType.Wall or TileType.Empty ) , false);

        // 2. Try to find a path allowing floors (may intersect corridors)
        if (path == null)
            path = map.FindPath(start, end, (p) => { 
                var room = FindLeafNode(node,p)?.Room ?? Rectangle.Empty; 
                if (room != Rectangle.Empty) room.Inflate(1,1); 
                return room == Rectangle.Empty || !room.Contains(p.X, p.Y);  
            }, false );

        if (path == null || path.Count == 0)
        {
            // Fallback to simple straight-line carving if no path found
            x = start.X;
            y = start.Y;
        }
        else
        {
            // Carve along the found path
            foreach (var p in path)
            {
                if (map.IsValid(p.X, p.Y))
                {
                    var cur = map[p.X, p.Y];
                    if (cur.Type == TileType.Wall || cur.Type == TileType.Empty)
                    {
                        cur.Type = TileType.Floor;
                    }
                }
            }
            return;
        }

        // Ensure endpoints are floors
        if (map.IsValid(x, y) && (map[x, y].Type == TileType.Wall || map[x, y].Type == TileType.Empty))
            map[x, y].Type = TileType.Floor;

        if (map.IsValid(end.X, end.Y) && (map[end.X, end.Y].Type == TileType.Wall || map[end.X, end.Y].Type == TileType.Empty))
            map[end.X, end.Y].Type = TileType.Floor;

        bool started = false;
        Point? lastWall = null;
        Size One = new Size(1, 1);
        while (x != end.X || y != end.Y)
        {
            if (map.IsValid(x, y))
            {
                var cur = map[x, y];

                // Junction candidate: we hit an existing corridor/room floor while carving (ignore the initial start tile).
                if (started && cur.Type == TileType.Floor && lastWall != null)
                {
                    doorCandidates.Add(new DoorCandidate(lastWall.Value, DoorCandidateKind.CorridorJunction));
                    lastWall = null;
                }
                else if(cur.Type == TileType.Wall)
                  lastWall = new Point(x,y);

                if (cur.Type == TileType.Wall || cur.Type == TileType.Empty)
                {
                    cur.Type = TileType.Floor;
                }
            }

            started = true;

            Point cand = new Point(x+ (x < end.X ? 1 : -1), y);
            if (Math.Abs(x-end.X)<Math.Abs(y-end.Y))
            {
                (cand.X,cand.Y) = (x,y + (y < end.Y ? 1 : -1));
            }
            // teste ob cand is the wall of a room.
            var lNode = FindLeafNode(node, cand);
            var blocked = false;
            if (lNode.Room.HasValue )
            {
                var room = lNode.Room.Value;
                room.Inflate(2,2);
                if (room.Contains(x, y))
                {
                    blocked = true;
                    room.Inflate(-1, -1);
                    var d = (room.X + room.Width / 2 - end.X, room.Y + room.Height / 2 - end.Y);
                    d = Math.Abs(d.Item1)>Math.Abs(d.Item2)?(Math.Clamp(d.Item1, -1, 1), 0):(0, Math.Clamp(d.Item2, -1, 1));
                    room.Offset(d.Item1, d.Item2);
                    if (d.Item1 * (end.X - x) + d.Item2 * (end.Y - y) < 3)
                    {
                        (x, y) = (cand.X, cand.Y);
                    }
                    else if(room.Contains(x, y))
                    {
                        var rm = (room.X + room.Width / 2 - x, room.Y + room.Height / 2 - y);
                        // we are about to carve into a room wall, compute, to get around it.
                        (x, y) = (x + Math.Clamp((rm.Item1 * 2 + 1) * -Math.Abs(d.Item2), -1, 1), y + Math.Clamp((rm.Item2 * 2 + 1) * -Math.Abs(d.Item1), -1, 1));
                    }
                    else 
                        (x, y) = (x - d.Item1, y - d.Item2);


                }

            }
            if (!blocked)
                if (_random.Next(2) == 0)
                {
                    if (x != end.X) 
                        x += x < end.X ? 1 : -1;
                    else if (y != end.Y) 
                        y += y < end.Y ? 1 : -1;
                }
                else
                {
                    if (y != end.Y) 
                        y += y < end.Y ? 1 : -1;
                    else if (x != end.X) 
                        x += x < end.X ? 1 : -1;
                }
        }
    }

    private static Point? FindDoorCandidateNearEnd(Map map, Point end)
    {
        // Look for a floor neighbor adjacent to end; prefer one that borders at least one wall (doorway-like).
        var candidates = new List<Point>();
        foreach (var (dx, dy) in new (int dx, int dy)[] { (1,0), (-1,0), (0,1), (0,-1) })
        {
            int x = end.X + dx;
            int y = end.Y + dy;
            if (!map.IsValid(x, y)) continue;
            if (map[x, y].Type != TileType.Floor) continue;
            candidates.Add(new Point(x, y));
        }

        if (candidates.Count == 0) return null;

        Point best = candidates[0];
        int bestScore = -1;
        foreach (var p in candidates)
        {
            int score = 0;
            if (map.IsValid(p.X + 1, p.Y) && map[p.X + 1, p.Y].Type == TileType.Wall) score++;
            if (map.IsValid(p.X - 1, p.Y) && map[p.X - 1, p.Y].Type == TileType.Wall) score++;
            if (map.IsValid(p.X, p.Y + 1) && map[p.X, p.Y + 1].Type == TileType.Wall) score++;
            if (map.IsValid(p.X, p.Y - 1) && map[p.X, p.Y - 1].Type == TileType.Wall) score++;
            if (score > bestScore)
            {
                bestScore = score;
                best = p;
            }
        }

        return best;
    }

    private void ApplyDoorCandidates(Map map, List<DoorCandidate> doorCandidates)
    {
        const int roomDoorChancePercent = 99;
        const int junctionDoorChancePercent = 30;

        // Deduplicate positions; prioritize room connectors over junctions.
        var merged = new Dictionary<Point, DoorCandidateKind>();
        foreach (var dc in doorCandidates)
        {
            if (merged.TryGetValue(dc.Position, out var existing))
            {
                if (existing == DoorCandidateKind.CorridorJunction && dc.Kind == DoorCandidateKind.RoomConnector)
                {
                    merged[dc.Position] = dc.Kind;
                }
            }
            else
            {
                merged.Add(dc.Position, dc.Kind);
            }
        }

        foreach (var kv in merged)
        {
            var pos = kv.Key;
            var kind = kv.Value;

            if (!map.IsValid(pos.X, pos.Y))
            {
                continue;
            }

            var t = map[pos.X, pos.Y];
            if (t.Type != TileType.Floor)
            {
                continue;
            }

            // Avoid placing doors inside room interiors: if too many adjacent floors, skip.
            int neighbors8 = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    if (map.IsValid(pos.X + dx, pos.Y + dy) && map[pos.X + dx, pos.Y + dy].Type == TileType.Floor)
                        neighbors8++;
                }
            }
            if (neighbors8 >= 6 && kind != DoorCandidateKind.RoomConnector)
            {
                continue;
            }

            int chance = kind == DoorCandidateKind.RoomConnector ? roomDoorChancePercent : junctionDoorChancePercent;
            if (_random.Next(100) < chance)
            {
                t.Type = TileType.DoorClosed;
            }
        }
    }
}
