using System.Windows.Media.Imaging;
using Treppen.Base;

namespace Treppen.WPF.Services.Interfaces;

public interface IDrawingService
{
    BitmapSource CreateLabyrinthPreview(IHeightLabyrinth labyrinth);
}
