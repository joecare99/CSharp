using ConsoleDisplay.View;
using Snake_Base.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snake_Base.View
{
    /// <summary>
    /// Class TileDef.
    /// Implements the <see cref="ConsoleDisplay.View.TileDef{Snake_Base.ViewModel.Tiles}" /></summary>
    /// <seealso cref="ConsoleDisplay.View.TileDef{Snake_Base.ViewModel.Tiles}" />
    public class TileDef : TileDef<Tiles>
    {
        private static string[][] _vTileDefStr = new string[][]{
            new string[]{ "  " },
            new string[]{ "{}" },
            new string[]{ "{}" },
            new string[]{ "{}" },
            new string[]{ "{}" },
            new string[]{ "''" },
            new string[]{ "- " },
            new string[]{ ",," },
            new string[]{ " -" },
            new string[]{ "++" },
            new string[]{ "++" },
            new string[]{ "++" },
            new string[]{ "++" },
            new string[]{ "||" },
            new string[]{ "==" },
            new string[]{ "##" },
            new string[]{ "[]" }
        };

        private static byte[][] _vTileColors = new byte[][]{
            new byte [] {0x07 },
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


        public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Tiles tile)
        {
            (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) result = default;
            result.lines = GetArrayElement(_vTileDefStr, tile);

            result.colors = new (ConsoleColor fgr, ConsoleColor bgr)[result.lines.Length * result.lines[0].Length];
            byte[] colDef = GetArrayElement(_vTileColors, tile);
            for (var i = 0; i < result.lines.Length * result.lines[0].Length; i++)
                result.colors[i] = ByteTo2ConsColor(colDef[i % colDef.Length]);
            return result;
        }
    }
}
