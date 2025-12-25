using System.Windows.Media;
using SharpHack.Base.Model;

namespace SharpHack.Wpf.Services;

public interface ITileService
{
    void LoadTileset(string path, int tileSize);
    ImageSource GetTile(TileType type);
    ImageSource GetCreature(Creature creature);
    ImageSource GetItem(Item item);
    ImageSource GetPlayer();
}
