using System.Collections.Generic;
using System.Windows;
using Treppen.Base;

namespace Treppen.Export.Services.Interfaces;

public interface ILabyrinth3dDrawer
{
    // Liefert abstrakte Zeichenkommandos anstatt direkt zu zeichnen
    IReadOnlyList<IDrawCommand> Build(IHeightLabyrinth labyrinth, Size printableSize, IDrawCommandFactory dcFactory);
}
