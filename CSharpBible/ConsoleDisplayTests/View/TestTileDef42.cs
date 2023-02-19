using System;
using System.Drawing;

namespace ConsoleDisplay.View.Tests {
	internal class TestTileDef42 : TileDefBase
    {
        private static string[][] _vTileDefStr = new string[][]{
            new string[]{"    ", "    " },
            new string[]{"=-=-", "-=-=" },
            new string[]{ "─┴┬─", "─┬┴─"},
            new string[]{ " ╓╖ ", "▓░▒▓" },
            new string[]{ "⌐°@)", " ⌡⌡‼" },
            new string[]{ @"/¯¯\", @"\__/" },
            new string[]{ "]°°[", "_!!_" },
            new string[]{ "◄°@[",@"_!!\" },
            new string[]{ "]oo[", "_!!_" },
            new string[]{ "]@°►", "/!!_" },
            new string[]{ @"/╨╨\", @"\__/" },
            new string[]{ " +*∩", "╘═◊@" } };

        private static byte[][] _vTileColors = new byte[][]{
            new byte [] {0x00 },
            new byte [] {0x6E },
            new byte [] { 0x4F },
            new byte [] { 0x0E, 0x0E, 0x0E, 0x0E, 0x2A, 0x22, 0x02, 0x22 },
            new byte [] { 0x6F },
            new byte [] { 0x6E },
            new byte [] { 0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A },
            new byte [] { 0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A },
            new byte [] { 0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A },
            new byte [] { 0x1A,0xA0,0xA0,0x1A,0x1A,0xA0,0xA0,0x1A },
            new byte [] { 0x6E },
            new byte [] { 0x6F },
            new byte [] { 0x6E },
            new byte [] { 0x4F }
        };


        public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum tile)
        {
            (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) result = default;
            result.lines = GetArrayElement(_vTileDefStr, tile);

            result.colors = new (ConsoleColor fgr, ConsoleColor bgr)[result.lines.Length * result.lines[0].Length];
            byte[] colDef = GetArrayElement(_vTileColors,tile);
            for (var i = 0; i < result.lines.Length * result.lines[0].Length; i++)
                result.colors[i] = ByteTo2ConsColor(colDef[i % colDef.Length]);
            return result;

        }

        public TestTileDef42() : base()
        {
            TileSize = new Size(4, 2);
        }
    }
}
