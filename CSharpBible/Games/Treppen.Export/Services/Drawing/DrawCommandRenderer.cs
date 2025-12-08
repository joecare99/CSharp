using System.Windows.Media;
using Treppen.Export.Services.Interfaces;

namespace Treppen.Export.Services.Drawing;

public static class DrawCommandRenderer
{
    public static void Render(this IEnumerable<IDrawCommand> commands, DrawingContext dc)
    {
        foreach (var cmd in commands)
        {
            cmd.Render(dc);
        }
    }
}
