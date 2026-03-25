using System.Globalization;
using System.Text;
using Treppen.Base;
using Treppen.Print.Services;
using Treppen.Export.Models;
using Treppen.Export.Services.Interfaces;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Treppen.Print.Rendering;

[PrintExporter("Wavefront OBJ", ".obj")]
public sealed class ObjPrintRenderer : IPrintRenderer
{
    public async Task RenderAsync(IHeightLabyrinth labyrinth, PrintOptions options, Stream output)
    {
        ArgumentNullException.ThrowIfNull(labyrinth);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(output);

        var dim = labyrinth.Dimension;
        int w = dim.Width;
        int h = dim.Height;
        if (w <= 0 || h <= 0)
            throw new InvalidOperationException("Leeres Labyrinth kann nicht exportiert werden.");

        // Simple voxel export: each cell height produces stacked unit cubes.
        // Coordinate system: x to the right, y forward, z up.
        // Scale cell size to options.CellSize, height to same unit.
        double cell = Math.Max(1, options.CellSize);
        var sb = new StringBuilder(1024 * 1024);
        sb.AppendLine("# Treppen Labyrinth OBJ export");
        sb.AppendLine("# One unit equals CellSize");
        int vertexOffset = 1; // OBJ indices are 1-based

        // Helper to append a single cube at (cx, cy, cz) with size cell
        void AppendCube(double cx, double cy, double height)
        {
            // 8 vertices
            // bottom
            sb.AppendLine(V(cx - cell / 2, 0, cy - cell / 2)); // v1
            sb.AppendLine(V(cx + cell / 2, 0, cy - cell / 2)); // v2
            sb.AppendLine(V(cx + cell / 2, 0  , cy + cell / 2)); // v3
            sb.AppendLine(V(cx - cell / 2, 0 , cy + cell / 2)); // v4
            // top
            sb.AppendLine(V(cx - cell / 2, height, cy - cell / 2)); // v5
            sb.AppendLine(V(cx + cell / 2, height, cy - cell / 2)); // v6
            sb.AppendLine(V(cx + cell / 2, height , cy + cell / 2)); // v7
            sb.AppendLine(V(cx - cell / 2, height , cy + cell / 2)); // v8

            // 6 faces (quads). Indices relative to current vertexOffset
            int v = vertexOffset;
            // bottom (1,2,3,4)
            sb.AppendLine($"f {v} {v + 1} {v + 2} {v + 3}");
            // top (5,6,7,8)
            sb.AppendLine($"f {v + 4} {v + 7} {v + 6} {v + 5}");
            // front (2,6,7,3)
            sb.AppendLine($"f {v + 1} {v + 5} {v + 6} {v + 2}");
            // back (1,4,8,5)
            sb.AppendLine($"f {v} {v + 3} {v + 7} {v + 4}");
            // left (1,5,6,2)
            sb.AppendLine($"f {v} {v + 4} {v + 5} {v + 1}");
            // right (4,3,7,8)
            sb.AppendLine($"f {v + 3} {v + 2} {v + 6} {v + 7}");

            vertexOffset += 8;
        }

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int heightVal = labyrinth[x, y];
                if (heightVal <= 0) continue;
                    // center position for the cube
                    double cx = (-x+h/2d) * cell;
                    double cy = (y-h/2d) * cell;
                    double cz = heightVal*cell*0.25d;
                    AppendCube(cy, cx, cz);
            }
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        await output.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
    }

    private static string V(double x, double y, double z)
    {
        return $"v {Fmt(x)} {Fmt(y)} {Fmt(z)}";
    }

    private static string Fmt(double d) => d.ToString("0.####", CultureInfo.InvariantCulture);
}
