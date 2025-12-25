using System;
using SharpHack.Base.Model;

namespace SharpHack.Engine;

public class FieldOfView
{
    private Map _map;

    public FieldOfView(Map map)
    {
        Map = map;
    }

    public Map Map { get => _map; set => _map = value; } // Make setter public to update reference on level change

    public void Compute(Point origin, int range)
    {
        // Reset visibility
        for (int x = 0; x < _map.Width; x++)
        {
            for (int y = 0; y < _map.Height; y++)
            {
                _map[x, y].IsVisible = false;
            }
        }

        _map[origin].IsVisible = true;
        _map[origin].IsExplored = true;

        for (int octant = 0; octant < 8; octant++)
        {
            ComputeOctant(octant, origin, range, 1, new Slope(1, 1), new Slope(0, 1));
        }
    }

    private struct Slope
    {
        public int Y, X;
        public Slope(int y, int x) { Y = y; X = x; }
    }

    private void ComputeOctant(int octant, Point origin, int range, int x, Slope top, Slope bottom)
    {
        for (; x <= range; x++)
        {
            int topY = x * top.Y / top.X;
            int bottomY = x * bottom.Y / bottom.X;

            int wasOpaque = -1; // 0: false, 1: true, -1: not initialized

            for (int y = topY; y >= bottomY; y--)
            {
                int tx = origin.X, ty = origin.Y;
                switch (octant)
                {
                    case 0: tx += x; ty -= y; break;
                    case 1: tx += y; ty -= x; break;
                    case 2: tx -= y; ty -= x; break;
                    case 3: tx -= x; ty -= y; break;
                    case 4: tx -= x; ty += y; break;
                    case 5: tx -= y; ty += x; break;
                    case 6: tx += y; ty += x; break;
                    case 7: tx += x; ty += y; break;
                }

                bool inBounds = _map.IsValid(tx, ty);
                if (inBounds)
                {
                    if (x * x + y * y <= range * range) // Circular radius
                    {
                        _map[tx, ty].IsVisible = true;
                        _map[tx, ty].IsExplored = true;
                    }
                }

                bool isOpaque = !inBounds || !_map[tx, ty].IsTransparent;

                if (x < range)
                {
                    if (wasOpaque != -1) // If we have a previous state
                    {
                        if (isOpaque)
                        {
                            if (wasOpaque == 0) // Transition from transparent to opaque
                            {
                                Slope newBottom = new Slope(y * 2 + 1, x * 2 - 1);
                                if (!inBounds || y * 2 + 1 < x * 2) // Blocked
                                    ComputeOctant(octant, origin, range, x + 1, top, newBottom);
                            }
                        }
                        else // Transparent
                        {
                            if (wasOpaque == 1) // Transition from opaque to transparent
                            {
                                top = new Slope(y * 2 + 1, x * 2 + 1);
                            }
                        }
                    }
                    wasOpaque = isOpaque ? 1 : 0;
                }
            }

            if (wasOpaque != -1 && wasOpaque == 1) break; // If the last cell was opaque, stop processing this row
        }
    }
}
