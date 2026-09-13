using ConsoleLib.CommonControls;

namespace ConsoleLib.Interfaces;

/// <summary>Optional renderer capability for tile collection controls.</summary>
public interface ITileViewRenderer
{
    void DrawTileView(TileView tileView);
}
