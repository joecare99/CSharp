using System;
using SharpHack.Base.Model;

namespace SharpHack.Engine;

/// <summary>
/// Provides recursive shadow-casting visibility calculations for a given map instance.
/// </summary>
public class FieldOfView
{
    private Map _map;

    /// <summary>
    /// Initializes a new instance of the <see cref="FieldOfView"/> class bound to the supplied map.
    /// </summary>
    /// <param name="map">The map that will be queried and updated with visibility information.</param>
    public FieldOfView(Map map)
    {
        Map = map;
    }

    /// <summary>
    /// Gets or sets the map currently associated with this field-of-view calculator.
    /// </summary>
    public Map Map { get => _map; set => _map = value; } // Make setter public to update reference on level change

    /// <summary>
    /// Computes visibility information for all tiles within the provided range around the origin.
    /// </summary>
    /// <param name="origin">The tile used as the center of vision.</param>
    /// <param name="range">The maximum distance, measured as a circular radius, that can be seen.</param>
    /// <remarks>
    /// The method clears visibility flags for the entire map, marks the origin as visible, then traverses all octants
    /// via recursive shadow casting to determine which tiles become visible and explored.
    /// </remarks>
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

    /// <summary>
    /// Encapsulates the upper and lower slope boundaries that bound the recursive shadow-casting beam.
    /// </summary>
    private struct Slope
    {
        /// <summary>
        /// Represents the numerator (Y) and denominator (X) parts of a slope used to track current scan boundaries.
        /// </summary>
        public int Y, X;

        /// <summary>
        /// Initializes a new <see cref="Slope"/> instance with the provided numerator and denominator components.
        /// </summary>
        /// <param name="y">The numerator portion of the slope.</param>
        /// <param name="x">The denominator portion of the slope.</param>
        public Slope(int y, int x) { Y = y; X = x; }
    }

    /// <summary>
    /// Recursively scans a single octant to determine which tiles remain visible while respecting opaque blockers.
    /// </summary>
    /// <param name="octant">The octant index (0-7) currently being processed.</param>
    /// <param name="origin">The point about which visibility is measured.</param>
    /// <param name="range">The maximum radius allowed for visibility checks.</param>
    /// <param name="x">The current distance from the origin along the primary axis.</param>
    /// <param name="top">The slope describing the upper boundary of the scan beam.</param>
    /// <param name="bottom">The slope describing the lower boundary of the scan beam.</param>
    /// <remarks>
    /// The algorithm walks tiles row by row, updating visibility when within range, and splits the scan beam when
    /// encountering opaque transitions so that shadows are correctly propagated deeper into the octant.
    /// </remarks>
    private void ComputeOctant(int octant, Point origin, int range, int x, Slope top, Slope bottom)
    {
        for (; x <= range; x++)
        {
            int topY = ProjectTopY(x, top);
            int bottomY = ProjectBottomY(x, bottom);

            if (topY < bottomY)
            {
                continue;
            }

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
    
    private static int ProjectTopY(int x, Slope slope)
    {
        return ((x * 2 - 1) * slope.Y + slope.X) / (slope.X * 2);
    }

    private static int ProjectBottomY(int x, Slope slope)
    {
        return ((x * 2 + 1) * slope.Y - slope.X) / (slope.X * 2);
    }
}
