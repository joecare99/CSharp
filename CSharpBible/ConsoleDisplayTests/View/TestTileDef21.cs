using System;
using System.Drawing;

namespace ConsoleDisplay.View.Tests {
	internal class TestTileDef21 : TileDefBase
    {
        private static string[][] _vTileDefStr = new string[][]{
            new string[]{ "  " },
            new string[]{ "=-" },
            new string[]{ "|_" },
            new string[]{ "╓╖" },
            new string[]{ "⌐@" },
            new string[]{ "[]" },
            new string[]{ "°°" },
            new string[]{ "◄°" },
            new string[]{ "oo" },
            new string[]{ "°►" },
            new string[]{ "╨╨" },
            new string[]{ "*∩" } };

        private static byte[][] _vTileColors = new byte[][]{
            new byte [] {0x00 },
            new byte [] {0x6E },
            new byte [] { 0x4F },
            new byte [] { 0x0E, 0x0E,  },
            new byte [] { 0x6F },
            new byte [] { 0x6E },
            new byte [] { 0xA0 },
            new byte [] { 0x1A, 0xA0 },
            new byte [] { 0xA0 },
            new byte [] { 0xA0,0x1A },
            new byte [] { 0x6E },
            new byte [] { 0x6F },
            new byte [] { 0x6E },
            new byte [] { 0x4F }
        };


        public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum? tile)
        {
            (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) result = default;
            result.lines = GetArrayElement(_vTileDefStr, tile);

            result.colors = new (ConsoleColor fgr, ConsoleColor bgr)[result.lines.Length * result.lines[0].Length];
            byte[] colDef = GetArrayElement(_vTileColors, tile);
            for (var i = 0; i < result.lines.Length * result.lines[0].Length; i++)
                result.colors[i] = ByteTo2ConsColor(colDef[i % colDef.Length]);
            return result;

        }
        public TestTileDef21() : base()
        {
            TileSize = new Size(2, 1);
        }

    }
}
