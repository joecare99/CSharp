using SharpHack.ViewModel;

namespace SharpHack.Server.Game;

internal static class TileGlyphs
{
    public static (char ch, int fg) Map(DisplayTile tile)
        => tile switch
        {
            DisplayTile.Archaeologist => ('@', 226),
            DisplayTile.Goblin => ('g', 82),
            DisplayTile.Sword => ('/', 51),
            DisplayTile.Armor => ('[', 51),

            DisplayTile.Floor_Lit or DisplayTile.Floor_Dark => ('.', 245),

            DisplayTile.Wall_NS or DisplayTile.Wall_EW or DisplayTile.Wall_EN or DisplayTile.Wall_NW or DisplayTile.Wall_ES or DisplayTile.Wall_WS or DisplayTile.Wall_ENWS or DisplayTile.Wall_ENW or DisplayTile.Wall_EWS or DisplayTile.Wall_NWS or DisplayTile.Wall_ENS => ('#', 250),

            DisplayTile.Door_Closed_EW or DisplayTile.Door_Closed_NS => ('+', 220),
            DisplayTile.Door_Open_EW or DisplayTile.Door_Open_NS => ('/', 220),

            DisplayTile.Stairs_Up => ('<', 214),
            DisplayTile.Stairs_Down => ('>', 214),

            _ => (' ', 15)
        };
}
