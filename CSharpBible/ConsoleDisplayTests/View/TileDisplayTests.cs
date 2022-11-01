using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using TestConsole;

namespace ConsoleDisplay.View.Tests
{
    public enum VTiles {
        /// <summary>
        /// The zero
        /// </summary>
        zero = 0,
        /// <summary>
        /// The tile1
        /// </summary>
        tile1,
        /// <summary>
        /// The wall
        /// </summary>
        Wall,
        /// <summary>
        /// The dest
        /// </summary>
        Dest,
        /// <summary>
        /// The player
        /// </summary>
        Player,
        /// <summary>
        /// The boulder
        /// </summary>
        Boulder,
        /// <summary>
        /// The e1
        /// </summary>
        E1,
        /// <summary>
        /// The e2
        /// </summary>
        E2,
        /// <summary>
        /// The e3
        /// </summary>
        E3,
        /// <summary>
        /// The e4
        /// </summary>
        E4,
        /// <summary>
        /// The bounder moving
        /// </summary>
        BounderMoving,
        /// <summary>
        /// The player dead
        /// </summary>
        PlayerDead
    }

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


        public override (string[] lines, (ConsoleColor fgr, ConsoleColor bgr)[] colors) GetTileDef(Enum tile)
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

    [TestClass()]
    public class TileDisplayTests
    {
        static TileDisplayTests()
        {
            TileDisplay.defaultTile = (Enum)VTiles.zero;
            TileDisplay.tileDef = new TestTileDef42();
        }

        private static MyConsoleBase? console;
        private static TstConsole _tstCon;
        private readonly string cExpWriteTile=@"\c00    \x00\x00\x00\x00\x00\x00\x00\x00\c4F─┴┬─\c00\x00\x00\x00\x00\x00\x00\x00\x00\c6F⌐°@)\c00\x00\x00\x00\x00\x00\x00\x00\x00\c1A]\cA0°°\c1A[\c00
    \x00\x00\c6E=-=-\c00\x00\x00\c4F─┬┴─\c00\x00\x00\c0E ╓╖ \c00\x00\x00\c6F ⌡⌡‼\c00\x00\x00\c6E/¯¯\\\c00\x00\x00\c1A_\cA0!!\c1A_\c00\x00\x00\c1A◄\cA0°@\c1A[\c00
\c1A]\cA0oo\c1A[\c00\x00\x00\c6E-=-=\c00\x00\x00\c6E/╨╨\\\c00\x00\x00\c2A▓\c22░\c02▒\c22▓\c00\x00\x00\x00\x00\x00\x00\x00\x00\c6E\\__/\c00\x00\x00\x00\x00\x00\x00\x00\x00\c1A_\cA0!!\c1A\\\c00
\c1A_\cA0!!\c1A_\c00\x00\x00\c1A]\cA0@°\c1A►\c00\x00\x00\c6E\\__/\c00\x00\x00\c6F +*∩\c00
\x00\x00\x00\x00\x00\x00\c1A/\cA0!!\c1A_\c00\x00\x00\x00\x00\x00\x00\x00\x00\c6F╘═◊@\c00";
        private readonly string cTileDisplayTest1 = @"\c00

\x00\x00  \x00\x00\c6E=-=-\c00\x00\x00\c4F─┬\c00
\x00\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\c0E╖ \c00\x00\x00\c6F ⌡⌡‼\c00\x00\x00\c6E/¯\c00
\x00\x00\c02▒\c22▓\c00\x00\x00\x00\x00\x00\x00\x00\x00\c6E\\_\c00
\x00\x00\cA0°\c1A[\c00\x00\x00\x00\x00\x00\x00\x00\x00\c1A]\cA0o\c00
\x00\x00\cA0!\c1A_\c00\x00\x00\c1A◄\cA0°@\c1A[\c00\x00\x00\c1A_\cA0!\c00
\x00\x00\x00\x00\x00\x00\c1A_\cA0!!\c1A\\\c00
\x00\x00\x00\x00\x00\x00\c6E/╨╨\\\c00
\x00\x00\cA0°\c1A►\c00\x00\x00\c6E\\__/\c00\x00\x00\c6F +\c00";
        private readonly string cTileDisplayTest12 = @"\c00

\x00\x00  \x00\x00\c6E=-=-\c00\x00\x00\c4F─┬\c00
\x00\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\c0E╖ \c00\x00\x00\c6F ⌡⌡‼\c00\x00\x00\c6E/¯\c00
\x00\x00\c02▒\c22▓\c00\x00\x00\x00\x00\x00\x00\x00\x00\c6E\\_\c00
\x00\x00\cA0°\c1A[\c00\x00\x00\x00\x00\x00\x00\x00\x00\c1A]\cA0o\c00
\x00\x00\cA0!\c1A_\c00\x00\x00\c1A◄\cA0°@\c1A[\c00\x00\x00\c1A_\cA0!\c00
\x00\x00\x00\x00\x00\x00\c1A_\cA0!!\c1A\\\c00
\x00\x00\x00\x00\x00\x00\c6E/╨╨\\\c00
\x00\x00\cA0°\c1A►\c00\x00\x00\c6E\\__/\c00\x00\x00\c6F +\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00  \x00\x00\c6E=-=-\c00\x00\x00\c4F─┬\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c0E╖ \c00\x00\x00\c6F ⌡⌡‼\c00\x00\x00\c6E/¯\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c02▒\c22▓\c00\x00\x00\x00\x00\x00\x00\x00\x00\c6E\\_\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0°\c1A[\c00\x00\x00\x00\x00\x00\x00\x00\x00\c1A]\cA0o\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0!\c1A_\c00\x00\x00\c1A◄\cA0°@\c1A[\c00\x00\x00\c1A_\cA0!\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c1A_\cA0!!\c1A\\\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6E/╨╨\\\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0°\c1A►\c00\x00\x00\c6E\\__/\c00\x00\x00\c6F +\c00";
        private readonly string cTileDisplayTest2 = @"\c00

\x00\x00   \c6E=-=-\c4F─┬\c00
\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\c0E╓╖ \c6F ⌡⌡‼\c6E/¯\c00
\x00\x00\c2A░\c22▒\c02▓\c00\x00\x00\x00\x00\c6E\\_\c00
\x00\x00\cA0°°\c1A[\c00\x00\x00\x00\x00\c1A]\cA0o\c00
\x00\x00\c1A!\cA0!_\c1A◄\cA0°@\c1A[_!\c00
\x00\x00\x00\x00\x00\c1A_!\cA0!\\\c00
\x00\x00\x00\x00\x00\c6E/╨╨\\\c00
\x00\x00\cA0@°\c1A►\c6E\\__/\c6F +\c00";
        private readonly string cTileDisplayTest22 = @"\c00

\x00\x00   \c6E=-=-\c4F─┬\c00
\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\c0E╓╖ \c6F ⌡⌡‼\c6E/¯\c00
\x00\x00\c2A░\c22▒\c02▓\c00\x00\x00\x00\x00\c6E\\_\c00
\x00\x00\cA0°°\c1A[\c00\x00\x00\x00\x00\c1A]\cA0o\c00
\x00\x00\c1A!\cA0!_\c1A◄\cA0°@\c1A[_!\c00
\x00\x00\x00\x00\x00\c1A_!\cA0!\\\c00
\x00\x00\x00\x00\x00\c6E/╨╨\\\c00
\x00\x00\cA0@°\c1A►\c6E\\__/\c6F +\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00  \c6E=-=\c4F─\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c0E╓╖\c6F ⌡⌡\c6E/\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c0E░\c2A▒\c22▓\c00\x00\x00\c6E\\\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0°°\c1A[\c00\x00\x00\c1A]\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c1A!!◄\cA0°@_\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0_\c1A!!\cA0\\\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6E/╨╨\\\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0@°\c6E\\__\c6F \c00";
        private readonly string cTileDisplayTest23 = @"\c00

\x00\x00   \c6E=-=-\c4F─┬\c00
\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\c0E╓╖ \c6F ⌡⌡‼\c6E/¯\c00
\x00\x00\c2A░\c22▒\c02▓\c00\x00\x00\x00\x00\c6E\\_\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00  \x00\x00\c6E=-=-\c00\x00\x00\c4F─┬\c00
\x00\x00\cA0°°\c1A[\c00\x00\x00\x00\x00\c1A]\cA0o\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\c1A!\cA0!_\c1A◄\cA0°@\c1A[_!\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00 \x00\c6E=-\c00\x00\c4F|\c00\x00\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\x00\x00\x00\c1A_!\cA0!\\\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c0E╖\c00\x00\c6F⌐@\c00\x00\c6E[\c00\x00\x00\c0E╖ \c00\x00\x00\c6F ⌡⌡‼\c00\x00\x00\c6E/¯\c00
\x00\x00\x00\x00\x00\c6E/╨╨\\\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0°\c00\x00\x00\x00\x00\cA0o\c00\x00\x00\c02▒\c22▓\c00\x00\x00\x00\x00\x00\x00\x00\x00\c6E\\_\c00
\x00\x00\cA0@°\c1A►\c6E\\__/\c6F +\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c1A◄\cA0°\c00\x00\x00\x00\x00\cA0°\c1A[\c00\x00\x00\x00\x00\x00\x00\x00\x00\c1A]\cA0o\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c1A►\c00\x00\c6E╨╨\c00\x00\c6F*\c00\x00\x00\cA0!\c1A_\c00\x00\x00\c1A◄\cA0°@\c1A[\c00\x00\x00\c1A_\cA0!\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00  \c6E=-=\c4F─\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c1A_\cA0!!\c1A\\\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6E-=-=\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6E/╨╨\\\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6F⌐°@)\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0°\c1A►\c00\x00\x00\c6E\\__/\c00\x00\x00\c6F +\c00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c0E╓╖\c6F ⌡⌡\c6E/\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c0E░\c2A▒\c22▓\c00\x00\x00\c6E\\\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0°°\c1A[\c00\x00\x00\c1A]\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c1A!!◄\cA0°@_\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0_\c1A!!\cA0\\\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\c6E/╨╨\\\c00
\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\x00\cA0@°\c6E\\__\c6F \c00";
        private readonly string cTileDisplayTest3= "\\c00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00  \\c6E=-\\c4F|_\\c0E╓╖\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6F⌐@\\c6E[]\\cA0°°\\c1A◄\\cA0°\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\cA0oo°\\c1A►\\c6E╨╨\\c6F*∩\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E*∩\\c4F*∩*∩*∩\\c00";
        private readonly string[] cExpUpdateText = new string[]
        {
            "\\c00\\x00\\x00        \r\n\\x00\\x00        \r\n\\x00\\x00        \r\n\\x00\\x00",
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00",
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00",
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00",
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00",
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00",
        };

        [TestInitialize()]
        public void Init()
        {
            TileDisplay.myConsole = console ?? ( console= _tstCon = new TstConsole());
            Application.DoEvents();
            console.Clear();
        }

        [TestMethod()]
        public void TileDisplayTest()
        {
            var tileDisplay = new TileDisplay();
            TileDisplay.WriteTile(Point.Empty,PointF.Empty,VTiles.Wall);           
        }

        [TestMethod()]
        public void TileDisplayTest1()
        {
            var tileDisplay = new TileDisplay(new Point(2,2),new Size(3,5));
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplay.WriteTile(new PointF((((int)tile) % 3) * 1.5f-0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f-0.5f), tile);
                Thread.Sleep(0);
            }
            Application.DoEvents();
            Assert.AreEqual(cTileDisplayTest1, _tstCon.Content);

            tileDisplay = new TileDisplay(new Point(62, 12), new Size(3, 5));
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplay.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                Thread.Sleep(0);
            }
            Application.DoEvents();
            Assert.AreEqual(cTileDisplayTest12, _tstCon.Content);

            Thread.Sleep(500);
        }

        [TestMethod()]
        public void TileDisplayTest2()
        {
            var tileDisplay = new TileDisplay(new Point(2, 2), new Size(3, 5),new Size(3,2));
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplay.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                Thread.Sleep(0);
            }
            Application.DoEvents();
            Assert.AreEqual(cTileDisplayTest2, _tstCon.Content);

            tileDisplay = new TileDisplay(new Point(62, 12), new Size(3, 5),new Size(2, 2));
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplay.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                Thread.Sleep(0);
            }
            Application.DoEvents();
            Assert.AreEqual(cTileDisplayTest22, _tstCon.Content);

            tileDisplay = new TileDisplay(new Point(40, 6), new Size(3, 5), new Size(4, 2));
            var tileDisplay2 = new TileDisplay(new Point(32, 8), new Size(3, 5),new Size(2,1));
            tileDisplay2.TileDef = new TestTileDef21();
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplay.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                tileDisplay2.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                Thread.Sleep(0);
            }
            Application.DoEvents();
            Assert.AreEqual(cTileDisplayTest23, _tstCon.Content);

            Thread.Sleep(500);
        }

        [TestMethod()]
        public void WriteTileTest()
        {
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                TileDisplay.WriteTile(Point.Empty, new PointF((((int)tile) % 8) * 1.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 8)), tile);
                Thread.Sleep(0);
            }
            Assert.AreEqual(cExpWriteTile, _tstCon.Content);
            Thread.Sleep(500);
        }

        [DataTestMethod()]
        [TestProperty("Author","J.C.")]
        [DataRow("0 _",VTiles.zero, "\\c00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00        \r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00        \r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00        \r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00")]
        [DataRow("1 _", VTiles.tile1, "\\c00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E=-=-=-=-\\c00")]
        [DataRow("2 _", VTiles.Wall, "\\c00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c4F|_|_|_|_\\c00")]
        [DataRow("3 _", VTiles.Dest, "\\c00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00")]
        [DataRow("4 _", VTiles.Player, "\\c00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00")]
        [DataRow("5 _", VTiles.Boulder, "\\c00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\x00\\c6E[][][][]\\c00")]
        public void FullRedrawTest(string name,VTiles vt,string sExp)
        {
            var tileDisplay = new TileDisplay(new Point(22, 0), new Size(4, 4), new TestTileDef21());
            if (vt == VTiles.zero)
            {
                tileDisplay.FncGetTile = (p) => (VTiles)(p.X + p.Y * tileDisplay.DispSize.Width);
                tileDisplay.FullRedraw();
                Application.DoEvents();
                Assert.AreEqual(cTileDisplayTest3, _tstCon.Content);
                Thread.Sleep(500);
            }
            tileDisplay.FncGetTile = (p) => vt;
            tileDisplay.FullRedraw();
            Application.DoEvents();
            Assert.AreEqual(sExp, _tstCon.Content);

            Thread.Sleep(500);
        }

        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [DataRow("00 _", VTiles.zero, VTiles.zero, new string[] {
            "\\c00\\x00\\x00        \r\n\\x00\\x00        \r\n\\x00\\x00        \r\n\\x00\\x00" ,
            "\\c00\\x00\\x00        \r\n\\x00\\x00        \r\n\\x00\\x00        \r\n\\x00\\x00"})]
        [DataRow("01 _", VTiles.tile1, VTiles.zero, new string[] { 
            "\\c00\\x00\\x00        \r\n\\x00\\x00 \\c6E=-\\c00  \\c6E=-\\c00 \r\n\\x00\\x00 \\c6E=-\\c00  \\c6E=-\\c00 \r\n\\x00\\x00",
            "\\c00\\x00\\x00        \r\n\\x00\\x00  \\c6E=-=-\\c00  \r\n\\x00\\x00  \\c6E=-=-\\c00  \r\n\\x00\\x00" })]
        [DataRow("02 _", VTiles.Wall, VTiles.zero, new string[] { 
            "\\c00\\x00\\x00        \r\n\\x00\\x00 \\c4F|_\\c00  \\c4F|_\\c00 \r\n\\x00\\x00 \\c4F|_\\c00  \\c4F|_\\c00 \r\n\\x00\\x00",
            "\\c00\\x00\\x00        \r\n\\x00\\x00  \\c4F|_|_\\c00  \r\n\\x00\\x00  \\c4F|_|_\\c00  \r\n\\x00\\x00" })]
        [DataRow("03 _", VTiles.Dest, VTiles.zero, new string[] { 
            "\\c00\\x00\\x00        \r\n\\x00\\x00 \\c0E╓╖\\c00  \\c0E╓╖\\c00 \r\n\\x00\\x00 \\c0E╓╖\\c00  \\c0E╓╖\\c00 \r\n\\x00\\x00",
            "\\c00\\x00\\x00        \r\n\\x00\\x00  \\c0E╓╖╓╖\\c00  \r\n\\x00\\x00  \\c0E╓╖╓╖\\c00  \r\n\\x00\\x00" })]
        [DataRow("04 _", VTiles.Player, VTiles.zero, new string[] { 
            "\\c00\\x00\\x00        \r\n\\x00\\x00 \\c6F⌐@\\c00  \\c6F⌐@\\c00 \r\n\\x00\\x00 \\c6F⌐@\\c00  \\c6F⌐@\\c00 \r\n\\x00\\x00",
            "\\c00\\x00\\x00        \r\n\\x00\\x00  \\c6F⌐@⌐@\\c00  \r\n\\x00\\x00  \\c6F⌐@⌐@\\c00  \r\n\\x00\\x00" })]
        [DataRow("05 _", VTiles.Boulder, VTiles.zero, new string[] { 
            "\\c00\\x00\\x00        \r\n\\x00\\x00 \\c6E[]\\c00  \\c6E[]\\c00 \r\n\\x00\\x00 \\c6E[]\\c00  \\c6E[]\\c00 \r\n\\x00\\x00",
            "\\c00\\x00\\x00        \r\n\\x00\\x00  \\c6E[][]\\c00  \r\n\\x00\\x00  \\c6E[][]\\c00  \r\n\\x00\\x00"})]
        [DataRow("10 _", VTiles.zero, VTiles.tile1, new string[] {
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=\\c00  \\c6E-=\\c00  \\c6E-\\c00\r\n\\x00\\x00\\c6E=\\c00  \\c6E-=\\c00  \\c6E-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00" ,
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-\\c00    \\c6E=-\\c00\r\n\\x00\\x00\\c6E=-\\c00    \\c6E=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00"})]
        [DataRow("11 _", VTiles.tile1, VTiles.tile1, new string[] {
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E==--==--\\c00\r\n\\x00\\x00\\c6E==--==--\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00",
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00" })]
        [DataRow("12 _", VTiles.Wall, VTiles.tile1, new string[] {
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=\\c4F|_\\c6E-=\\c4F|_\\c6E-\\c00\r\n\\x00\\x00\\c6E=\\c4F|_\\c6E-=\\c4F|_\\c6E-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00",
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-\\c4F|_|_\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-\\c4F|_|_\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00" })]
        [DataRow("13 _", VTiles.Dest, VTiles.tile1, new string[] {
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=\\c0E╓╖\\c6E-=\\c0E╓╖\\c6E-\\c00\r\n\\x00\\x00\\c6E=\\c0E╓╖\\c6E-=\\c0E╓╖\\c6E-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00",
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-\\c0E╓╖╓╖\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-\\c0E╓╖╓╖\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00" })]
        [DataRow("14 _", VTiles.Player, VTiles.tile1, new string[] {
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=\\c6F⌐@\\c6E-=\\c6F⌐@\\c6E-\\c00\r\n\\x00\\x00\\c6E=\\c6F⌐@\\c6E-=\\c6F⌐@\\c6E-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00",
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-\\c6F⌐@⌐@\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-\\c6F⌐@⌐@\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00" })]
        [DataRow("15 _", VTiles.Boulder, VTiles.tile1, new string[] {
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=[]-=[]-\\c00\r\n\\x00\\x00\\c6E=[]-=[]-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00",
            "\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\c6E=-[][]=-\\c00\r\n\\x00\\x00\\c6E=-[][]=-\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00"})]
        [DataRow("20 _", VTiles.zero, VTiles.Wall, new string[] {
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|\\c00  \\c4F_|\\c00  \\c4F_\\c00\r\n\\x00\\x00\\c4F|\\c00  \\c4F_|\\c00  \\c4F_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00" ,
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_\\c00    \\c4F|_\\c00\r\n\\x00\\x00\\c4F|_\\c00    \\c4F|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00"})]
        [DataRow("21 _", VTiles.tile1, VTiles.Wall, new string[] {
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|\\c6E=-\\c4F_|\\c6E=-\\c4F_\\c00\r\n\\x00\\x00\\c4F|\\c6E=-\\c4F_|\\c6E=-\\c4F_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00",
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_\\c6E=-=-\\c4F|_\\c00\r\n\\x00\\x00\\c4F|_\\c6E=-=-\\c4F|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00" })]
        [DataRow("22 _", VTiles.Wall, VTiles.Wall, new string[] {
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F||__||__\\c00\r\n\\x00\\x00\\c4F||__||__\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00",
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00" })]
        [DataRow("23 _", VTiles.Dest, VTiles.Wall, new string[] {
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|\\c0E╓╖\\c4F_|\\c0E╓╖\\c4F_\\c00\r\n\\x00\\x00\\c4F|\\c0E╓╖\\c4F_|\\c0E╓╖\\c4F_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00",
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_\\c0E╓╖╓╖\\c4F|_\\c00\r\n\\x00\\x00\\c4F|_\\c0E╓╖╓╖\\c4F|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00" })]
        [DataRow("24 _", VTiles.Player, VTiles.Wall, new string[] {
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|\\c6F⌐@\\c4F_|\\c6F⌐@\\c4F_\\c00\r\n\\x00\\x00\\c4F|\\c6F⌐@\\c4F_|\\c6F⌐@\\c4F_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00",
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_\\c6F⌐@⌐@\\c4F|_\\c00\r\n\\x00\\x00\\c4F|_\\c6F⌐@⌐@\\c4F|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00" })]
        [DataRow("25 _", VTiles.Boulder, VTiles.Wall, new string[] {
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|\\c6E[]\\c4F_|\\c6E[]\\c4F_\\c00\r\n\\x00\\x00\\c4F|\\c6E[]\\c4F_|\\c6E[]\\c4F_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00",
            "\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\c4F|_\\c6E[][]\\c4F|_\\c00\r\n\\x00\\x00\\c4F|_\\c6E[][]\\c4F|_\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00"})]
        [DataRow("30 _", VTiles.zero, VTiles.Dest, new string[] {
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓\\c00  \\c0E╖╓\\c00  \\c0E╖\\c00\r\n\\x00\\x00\\c0E╓\\c00  \\c0E╖╓\\c00  \\c0E╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00" ,
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c00    \\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c00    \\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00"})]
        [DataRow("31 _", VTiles.tile1, VTiles.Dest, new string[] {
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓\\c6E=-\\c0E╖╓\\c6E=-\\c0E╖\\c00\r\n\\x00\\x00\\c0E╓\\c6E=-\\c0E╖╓\\c6E=-\\c0E╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00",
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c6E=-=-\\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c6E=-=-\\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00" })]
        [DataRow("32 _", VTiles.Wall, VTiles.Dest, new string[] {
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓\\c4F|_\\c0E╖╓\\c4F|_\\c0E╖\\c00\r\n\\x00\\x00\\c0E╓\\c4F|_\\c0E╖╓\\c4F|_\\c0E╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00",
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c4F|_|_\\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c4F|_|_\\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00" })]
        [DataRow("33 _", VTiles.Dest, VTiles.Dest, new string[] {
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╓╖╖╓╓╖╖\\c00\r\n\\x00\\x00\\c0E╓╓╖╖╓╓╖╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00",
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00" })]
        [DataRow("34 _", VTiles.Player, VTiles.Dest, new string[] {
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓\\c6F⌐@\\c0E╖╓\\c6F⌐@\\c0E╖\\c00\r\n\\x00\\x00\\c0E╓\\c6F⌐@\\c0E╖╓\\c6F⌐@\\c0E╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00",
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c6F⌐@⌐@\\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c6F⌐@⌐@\\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00" })]
        [DataRow("35 _", VTiles.Boulder, VTiles.Dest, new string[] {
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓\\c6E[]\\c0E╖╓\\c6E[]\\c0E╖\\c00\r\n\\x00\\x00\\c0E╓\\c6E[]\\c0E╖╓\\c6E[]\\c0E╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00",
            "\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c6E[][]\\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖\\c6E[][]\\c0E╓╖\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00"})]
        [DataRow("40 _", VTiles.zero, VTiles.Player, new string[] {
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐\\c00  \\c6F@⌐\\c00  \\c6F@\\c00\r\n\\x00\\x00\\c6F⌐\\c00  \\c6F@⌐\\c00  \\c6F@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00" ,
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c00    \\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c00    \\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00"})]
        [DataRow("41 _", VTiles.tile1, VTiles.Player, new string[] {
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐\\c6E=-\\c6F@⌐\\c6E=-\\c6F@\\c00\r\n\\x00\\x00\\c6F⌐\\c6E=-\\c6F@⌐\\c6E=-\\c6F@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00",
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c6E=-=-\\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c6E=-=-\\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00" })]
        [DataRow("42 _", VTiles.Wall, VTiles.Player, new string[] {
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐\\c4F|_\\c6F@⌐\\c4F|_\\c6F@\\c00\r\n\\x00\\x00\\c6F⌐\\c4F|_\\c6F@⌐\\c4F|_\\c6F@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00",
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c4F|_|_\\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c4F|_|_\\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00" })]
        [DataRow("43 _", VTiles.Dest, VTiles.Player, new string[] {
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐\\c0E╓╖\\c6F@⌐\\c0E╓╖\\c6F@\\c00\r\n\\x00\\x00\\c6F⌐\\c0E╓╖\\c6F@⌐\\c0E╓╖\\c6F@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00",
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c0E╓╖╓╖\\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c0E╓╖╓╖\\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00" })]
        [DataRow("44 _", VTiles.Player, VTiles.Player, new string[] {
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐⌐@@⌐⌐@@\\c00\r\n\\x00\\x00\\c6F⌐⌐@@⌐⌐@@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00",
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00" })]
        [DataRow("45 _", VTiles.Boulder, VTiles.Player, new string[] {
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐\\c6E[]\\c6F@⌐\\c6E[]\\c6F@\\c00\r\n\\x00\\x00\\c6F⌐\\c6E[]\\c6F@⌐\\c6E[]\\c6F@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00",
            "\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c6E[][]\\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@\\c6E[][]\\c6F⌐@\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00"})]
        [DataRow("50 _", VTiles.zero, VTiles.Boulder, new string[] {
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[\\c00  \\c6E][\\c00  \\c6E]\\c00\r\n\\x00\\x00\\c6E[\\c00  \\c6E][\\c00  \\c6E]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00" ,
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[]\\c00    \\c6E[]\\c00\r\n\\x00\\x00\\c6E[]\\c00    \\c6E[]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00"})]
        [DataRow("51 _", VTiles.tile1, VTiles.Boulder, new string[] {
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[=-][=-]\\c00\r\n\\x00\\x00\\c6E[=-][=-]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00",
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[]=-=-[]\\c00\r\n\\x00\\x00\\c6E[]=-=-[]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00" })]
        [DataRow("52 _", VTiles.Wall, VTiles.Boulder, new string[] {
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[\\c4F|_\\c6E][\\c4F|_\\c6E]\\c00\r\n\\x00\\x00\\c6E[\\c4F|_\\c6E][\\c4F|_\\c6E]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00",
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[]\\c4F|_|_\\c6E[]\\c00\r\n\\x00\\x00\\c6E[]\\c4F|_|_\\c6E[]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00" })]
        [DataRow("53 _", VTiles.Dest, VTiles.Boulder, new string[] {
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[\\c0E╓╖\\c6E][\\c0E╓╖\\c6E]\\c00\r\n\\x00\\x00\\c6E[\\c0E╓╖\\c6E][\\c0E╓╖\\c6E]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00",
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[]\\c0E╓╖╓╖\\c6E[]\\c00\r\n\\x00\\x00\\c6E[]\\c0E╓╖╓╖\\c6E[]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00" })]
        [DataRow("54 _", VTiles.Player, VTiles.Boulder, new string[] {
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[\\c6F⌐@\\c6E][\\c6F⌐@\\c6E]\\c00\r\n\\x00\\x00\\c6E[\\c6F⌐@\\c6E][\\c6F⌐@\\c6E]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00",
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[]\\c6F⌐@⌐@\\c6E[]\\c00\r\n\\x00\\x00\\c6E[]\\c6F⌐@⌐@\\c6E[]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00" })]
        [DataRow("55 _", VTiles.Boulder, VTiles.Boulder, new string[] {
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[[]][[]]\\c00\r\n\\x00\\x00\\c6E[[]][[]]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00",
            "\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00"})]
        public void UpdateTest(string name, VTiles vt, VTiles vt2, string[] sExp)
        {
            var tileDisplay = new TileDisplay(new Point(2, 0), new Size(4, 4), new TestTileDef21());
           
            tileDisplay.FncGetTile = (p) => vt2;
            tileDisplay.FullRedraw();
            Application.DoEvents();
            Assert.AreEqual(cExpUpdateText[(int)vt2], _tstCon.Content);

            Thread.Sleep(100);

            tileDisplay.FncGetTile = (p) => xTst(p)? vt:vt2;
            tileDisplay.FncOldPos = (p) => xTst(p) ? new Point(p.X * 3 - 3, p.Y) : p;
            tileDisplay.Update(true); //Halvstep
            Application.DoEvents();
            Assert.AreEqual(sExp[0], _tstCon.Content);

            Thread.Sleep(100);

            tileDisplay.Update(false);
            Application.DoEvents();
            Assert.AreEqual(sExp[1], _tstCon.Content);

            Thread.Sleep(100);
            Application.DoEvents();
            bool xTst(Point p) => p.X > 0 && p.Y > 0 && p.X < 3 && p.Y < 3;
        }

    }
}