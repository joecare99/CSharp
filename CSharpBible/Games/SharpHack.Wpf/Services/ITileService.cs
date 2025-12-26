using System.Windows.Media;
using SharpHack.ViewModel;

namespace SharpHack.Wpf.Services;

public interface ITileService
{
    void LoadTileset(string path, int tileSize);
    ImageSource GetTile(DisplayTile type);
}
