using System.Windows.Controls;
using Treppen.Base;

namespace Treppen.WPF.Services;

public interface ILabyrinth3dDrawer
{
    void DrawLabyrinth(Canvas canvas, IHeightLabyrinth labyrinth);
}
