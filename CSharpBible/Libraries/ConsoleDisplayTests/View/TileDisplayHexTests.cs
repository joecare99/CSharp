using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System;

namespace ConsoleDisplay.View.Tests
{
    [TestClass()]
	public class HexMathTests {
		static System.Collections.Generic.IEnumerable<object[]> PointHexTestData => new[]{
        new object[]{"Zero", new float[] { 0f, 0f }, false, new float[] { 0f, 0f }},
        new object[] { "(0,1)", new float[] { 0f, 1f }, false, new float[] { 0.5f, 1f } },
        new object[] { "(0,2)", new float[] { 0f, 2f }, false, new float[] { 0f, 2f } },
        new object[] { "(1,0)", new float[] { 1f, 0f }, false, new float[] { 1f, 0f } },
        new object[] { "(1,1)", new float[] { 1f, 1f }, false, new float[] { 1.5f, 1f } },
        new object[] { "(1,2)", new float[] { 1f, 2f }, false, new float[] { 1f, 2f } },
        new object[] { "(2,0)", new float[] { 2f, 0f }, false, new float[] { 2f, 0f } },
        new object[] { "(2,1)", new float[] { 2f, 1f }, false, new float[] { 2.5f, 1f } },
        new object[] { "(2,2)", new float[] { 2f, 2f }, false, new float[] { 2f, 2f } },
        new object[] { "Zero", new float[] { 0f, 0f }, true, new float[] { 0f, 0f } },
        new object[] { "(0,1)", new float[] { 0f, 1f }, true, new float[] { 0f, 1f } },
        new object[] { "(0,2)", new float[] { 0f, 2f }, true, new float[] { 0f, 2f } },
        new object[] { "(1,0)", new float[] { 1f, 0f }, true, new float[] { 1f, 0.5f } },
        new object[] { "(1,1)", new float[] { 1f, 1f }, true, new float[] { 1f, 1.5f } },
        new object[] { "(1,2)", new float[] { 1f, 2f }, true, new float[] { 1f, 2.5f } },
        new object[] { "(2,0)", new float[] { 2f, 0f }, true, new float[] { 2f, 0f } },
        new object[] { "(2,1)", new float[] { 2f, 1f }, true, new float[] { 2f, 1f } },
        new object[] { "(2,2)", new float[] { 2f, 2f }, true, new float[] { 2f, 2f } },
		};

		static System.Collections.Generic.IEnumerable<object[]> ZigZagTestData => new[] {
        new object[]{"Zero", 0f, 0f},
        new object[]{"One", 1f, 1f},
        new object[]{"-One", -1f, 1f},
        new object[]{"Two", 2f, 0f},
        new object[]{"-1.5", -1.5f, 0.5f},
        new object[]{"-0.5", -0.5f, 0.5f},
        new object[]{" 0.5", 0.5f, 0.5f},
        new object[]{" 1.5", 1.5f, 0.5f},
        new object[]{"-1.75", -1.75f, 0.25f},
        new object[]{"-1.25", -1.25f, 0.75f},
        new object[]{"-0.75", -0.75f, 0.75f},
        new object[]{"-0.25", -0.25f, 0.25f},
        new object[]{" 0.25", 0.25f, 0.25f},
        new object[]{" 0.75", 0.75f, 0.75f},
        new object[]{" 1.25", 1.25f, 0.75f},
        new object[]{" 1.75", 1.75f, 0.25f},
        };

		[DataTestMethod()]
		[TestProperty("Author", "J.C.")]
		[TestCategory("Math")]
		[DynamicData(nameof(ZigZagTestData))]
		public void ZigZagTest(string name, float fVal, float fExp) {
			for (var i = 0; i < 1000; i++) {
				Assert.AreEqual(fExp, HexMath.ZigZag(fVal), 1e-5, $"{name}({fVal})");
				Assert.AreEqual(fExp, HexMath.ZigZag(fVal + 1e-6f), 1e-5, $"{name}({fVal + 1e-6f})");
				Assert.AreEqual(fExp, HexMath.ZigZag(fVal - 1e-6f), 1e-5, $"{name}({fVal - 1e-6f})");
			}
		}

        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("Math")]
        [DynamicData(nameof(ZigZagTestData))]
        public void ZigZagTest1(string name, float fVal, float fExp)
        {
            for (var i = 0; i < 1000; i++)
            {
                Assert.AreEqual(fExp, fVal.ZigZag(), 1e-5, $"{name}({fVal})");
                Assert.AreEqual(fExp, (fVal + 1e-6f).ZigZag(), 1e-5, $"{name}({fVal + 1e-6f})");
                Assert.AreEqual(fExp, (fVal - 1e-6f).ZigZag(), 1e-5, $"{name}({fVal - 1e-6f})");
            }
        }

        [DataTestMethod()]
		[TestProperty("Author", "J.C.")]
		[TestCategory("Math")]
        [DynamicData(nameof(PointHexTestData))]
        public void HexPointFTest(string name, float[] fVal,bool xVal, float[] fExp) {
			var pfExp = new PointF(fExp[0], fExp[1]);
			Assert.AreEqual(pfExp, HexMath.HexPointF((fVal[0], fVal[1]),xVal), $"{name}({fVal[0]},{fVal[1]})");
		}

        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("Math")]
        [DynamicData(nameof(PointHexTestData))]
        public void HexPointFTest2(string name, float[] fVal, bool xVal, float[] fExp)
        {
            var pfExp = new PointF(fExp[0], fExp[1]);
            Assert.AreEqual(pfExp, (fVal[0], fVal[1]).HexPointF(xVal), $"{name}({fVal[0]},{fVal[1]})");
        }

        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("Math")]
        [DynamicData(nameof(PointHexTestData))]
        public void HexPointTest(string name, float[] fVal, bool xVal, float[] fExp)
        {
            var pfExp = new Point((int)Math.Round(fExp[0]), (int)Math.Round(fExp[1]));
            Assert.AreEqual(pfExp, HexMath.HexKPoint((fVal[0], fVal[1]), xVal), $"{name}({fVal[0]},{fVal[1]})");
        }

        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [TestCategory("Math")]
        [DynamicData(nameof(PointHexTestData))]
        public void HexPointTest2(string name, float[] fVal, bool xVal, float[] fExp)
        {
            var pfExp = new Point((int)Math.Round(fExp[0]), (int)Math.Round(fExp[1]));
            Assert.AreEqual(pfExp, (fVal[0], fVal[1]).HexKPoint(xVal), $"{name}({fVal[0]},{fVal[1]})");
        }

    }

    [TestClass()]
    public class TileDisplayHexTests
    {
        static TileDisplayHexTests()
        {
            TileDisplayHex.defaultTile = VTiles.zero;
            TileDisplayHex.tileDef = new TestTileDef42();
        }

		private void Application_DoEvents() =>
			System.Windows.Forms.Application.DoEvents();

        private void Thread_Sleep(int _) =>
			System.Threading.Thread.Sleep(0);

        private static MyConsoleBase? console;
        private static TstConsole? _tstCon;
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
        private readonly string cTileDisplayTest3= "\\c00\r\n\\x00\\x00  \\c6E=-\\c4F|_\\c0E╓╖\\c00\r\n\\x00\\x00\\x00\\c6F⌐@\\c6E[]\\cA0°°\\c1A◄\\c00\r\n\\x00\\x00\\cA0oo°\\c1A►\\c6E╨╨\\c6F*∩\\c00\r\n\\x00\\x00\\x00\\c6E*∩\\c4F*∩*∩*\\c00";
		private readonly string cTileDisplayTest4 = "\\c00\r\n\\x00\\x00    \\c6E=-=-\\c4F─┴┬─\\c0E ╓╖ \\c00\r\n\\x00\\x00    \\c6E-=-=\\c4F─┬┴─\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c6E/¯¯\\\\\\c1A]\\cA0°°\\c1A[◄\\cA0°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼\\c6E\\\\__/\\c1A_\\cA0!!\\c1A__\\cA0!\\c00\r\n\\x00\\x00\\c1A]\\cA0oo\\c1A[]\\cA0@°\\c1A►\\c6E/╨╨\\\\\\c6F +*∩\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_/\\cA0!!\\c1A_\\c6E\\\\__/\\c6F╘═◊@\\c00\r\n\\x00\\x00\\x00\\x00\\c6E +*∩\\c4F +*∩ +*∩ +\\c00\r\n\\x00\\x00\\x00\\x00\\c6E╘═◊@\\c4F╘═◊@╘═◊@╘═\\c00";
		private readonly string cTileDisplayTest5 = "\\c00\r\n\\x00\\x00    \\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00    \\c6E=-=-\\c4F─┬┴─\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c6F⌐°@)\\c6E-=-=\\c1A]\\cA0°°\\c1A[\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c6E/¯¯\\\\\\c1A_\\cA0!!\\c1A_◄\\cA0°@\\c1A[\\c00\r\n\\x00\\x00\\c1A]\\cA0oo\\c1A[\\c6E\\\\__//╨╨\\\\\\c1A_\\cA0!!\\c1A\\\\\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0@°\\c1A►\\c6E\\\\__/\\c6F +*∩\\c00\r\n\\x00\\x00\\c6E +*∩\\c1A/\\cA0!!\\c1A_\\c4F +*∩\\c6F╘═◊@\\c00\r\n\\x00\\x00\\c6E╘═◊@\\c4F +*∩╘═◊@ +*∩\\c00";
		private readonly string[] cExpUpdateText = new string[]
        {
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00       \r\n\\x00\\x00        \r\n\\x00\\x00\\x00",
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00",
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00",
			"\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00",
			"\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00",
			"\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00",
        };
		private readonly string[] cExpUpdateText2 = new string[]
		{
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00",
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00",
			"\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00\r\n\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00",
			"\\c00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00\r\n\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00",
			"\\c00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00\r\n\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00",
			"\\c00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00",
		};
		private readonly string[] cExpUpdateText3 = new string[]
		{
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00",
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00",
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00",
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00",
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00",
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00",
		};

		[TestInitialize()]
        public void Init()
        {
            TileDisplayHex.myConsole = console ?? ( console= _tstCon = new TstConsole());
            Application_DoEvents();
            console.Clear();
        }

        [TestMethod()]
        public void TileDisplayHexTest()
        {            
            TileDisplayHex.WriteTile(Point.Empty,PointF.Empty,VTiles.Wall);           
        }

        [TestMethod()]
        public void TileDisplayTest1()
        {
            var tileDisplayHex = new TileDisplayHex(new Point(2,2),new Size(3,5));
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplayHex.WriteTile(new PointF((((int)tile) % 3) * 1.5f-0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f-0.5f), tile);
                Thread_Sleep(0);
            }
            System.Windows.Forms.Application.DoEvents();
            Assert.AreEqual(cTileDisplayTest1, _tstCon?.Content);

            tileDisplayHex = new TileDisplayHex(new Point(62, 12), new Size(3, 5));
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplayHex.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                Thread_Sleep(0);
            }
            System.Windows.Forms.Application.DoEvents();
            Assert.AreEqual(cTileDisplayTest12, _tstCon?.Content);

            Thread_Sleep(100);
        }

        [TestMethod()]
        public void TileDisplayTest2()
        {
            var tileDisplayHex = new TileDisplayHex(new Point(2, 2), new Size(3, 5),new Size(3,2));
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplayHex.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                Thread_Sleep(0);
            }
            Application_DoEvents();
            Assert.AreEqual(cTileDisplayTest2, _tstCon?.Content);

            tileDisplayHex = new TileDisplayHex(new Point(62, 12), new Size(3, 5),new Size(2, 2));
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplayHex.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                Thread_Sleep(0);
            }
            Application_DoEvents();
            Assert.AreEqual(cTileDisplayTest22, _tstCon?.Content);

            tileDisplayHex = new TileDisplayHex(new Point(40, 6), new Size(3, 5), new Size(4, 2));
            var tileDisplay2 = new TileDisplayHex(new Point(32, 8), new Size(3, 5), new Size(2, 1))
            {
                TileDef = new TestTileDef21()
            };
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                tileDisplayHex.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                tileDisplay2.WriteTile(new PointF((((int)tile) % 3) * 1.5f - 0.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 3) * 1.5f - 0.5f), tile);
                Thread_Sleep(0);
            }
            Application_DoEvents();
            Assert.AreEqual(cTileDisplayTest23, _tstCon?.Content);

            Thread_Sleep(100);
        }

        [TestMethod()]
        public void WriteTileTest()
        {
            foreach (VTiles tile in typeof(VTiles).GetEnumValues())
            {
                TileDisplayHex.WriteTile(Point.Empty, new PointF((((int)tile) % 8) * 1.5f, (((int)tile) % 2) * 0.5f + (((int)tile) / 8)), tile);
                Thread_Sleep(0);
            }
            Assert.AreEqual(cExpWriteTile, _tstCon?.Content);
            Thread_Sleep(100);
        }

        [DataTestMethod()]
        [TestProperty("Author","J.C.")]
        [DataRow("0 _",VTiles.zero, "\\c00\r\n\\x00\\x00        \r\n\\x00\\x00\\x00       \r\n\\x00\\x00        \r\n\\x00\\x00\\x00")]
        [DataRow("1 _", VTiles.tile1, "\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00")]
        [DataRow("2 _", VTiles.Wall, "\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00")]
        [DataRow("3 _", VTiles.Dest, "\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00\r\n\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00")]
        [DataRow("4 _", VTiles.Player, "\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00\r\n\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00")]
        [DataRow("5 _", VTiles.Boulder, "\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00\r\n\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00")]
        [DataRow("6 _", VTiles.E1, "\\c00\r\n\\x00\\x00\\cA0°°°°°°°°\\c00\r\n\\x00\\x00\\x00\\cA0°°°°°°°\\c00\r\n\\x00\\x00\\cA0°°°°°°°°\\c00\r\n\\x00\\x00\\x00\\cA0°°°°°°°\\c00")]
        [DataRow("7 _", VTiles.E2, "\\c00\r\n\\x00\\x00\\c1A◄\\cA0°\\c1A◄\\cA0°\\c1A◄\\cA0°\\c1A◄\\cA0°\\c00\r\n\\x00\\x00\\x00\\c1A◄\\cA0°\\c1A◄\\cA0°\\c1A◄\\cA0°\\c1A◄\\c00\r\n\\x00\\x00\\c1A◄\\cA0°\\c1A◄\\cA0°\\c1A◄\\cA0°\\c1A◄\\cA0°\\c00\r\n\\x00\\x00\\x00\\c1A◄\\cA0°\\c1A◄\\cA0°\\c1A◄\\cA0°\\c1A◄\\c00")]
        [DataRow("8 _", VTiles.E3, "\\c00\r\n\\x00\\x00\\cA0oooooooo\\c00\r\n\\x00\\x00\\x00\\cA0ooooooo\\c00\r\n\\x00\\x00\\cA0oooooooo\\c00\r\n\\x00\\x00\\x00\\cA0ooooooo\\c00")]
        [DataRow("9 _", VTiles.E4, "\\c00\r\n\\x00\\x00\\cA0°\\c1A►\\cA0°\\c1A►\\cA0°\\c1A►\\cA0°\\c1A►\\c00\r\n\\x00\\x00\\x00\\cA0°\\c1A►\\cA0°\\c1A►\\cA0°\\c1A►\\cA0°\\c00\r\n\\x00\\x00\\cA0°\\c1A►\\cA0°\\c1A►\\cA0°\\c1A►\\cA0°\\c1A►\\c00\r\n\\x00\\x00\\x00\\cA0°\\c1A►\\cA0°\\c1A►\\cA0°\\c1A►\\cA0°\\c00")]
        [DataRow("10 _", VTiles.BounderMoving, "\\c00\r\n\\x00\\x00\\c6E╨╨╨╨╨╨╨╨\\c00\r\n\\x00\\x00\\x00\\c6E╨╨╨╨╨╨╨\\c00\r\n\\x00\\x00\\c6E╨╨╨╨╨╨╨╨\\c00\r\n\\x00\\x00\\x00\\c6E╨╨╨╨╨╨╨\\c00")]
        [DataRow("11 _", VTiles.PlayerDead, "\\c00\r\n\\x00\\x00\\c6F*∩*∩*∩*∩\\c00\r\n\\x00\\x00\\x00\\c6F*∩*∩*∩*\\c00\r\n\\x00\\x00\\c6F*∩*∩*∩*∩\\c00\r\n\\x00\\x00\\x00\\c6F*∩*∩*∩*\\c00")]
        public void FullRedrawTest(string name,VTiles vt,string sExp)
        {
            var tileDisplayHex = new TileDisplayHex(new Point(2, 1), new Size(4, 4), new TestTileDef21());
            if (vt == VTiles.zero)
            {
                tileDisplayHex.FncGetTile = (p) => (VTiles)(p.X + p.Y * tileDisplayHex.DispSize.Width);
                tileDisplayHex.FullRedraw();
                Application_DoEvents();
                Assert.AreEqual(cTileDisplayTest3, _tstCon?.Content,name);
                Thread_Sleep(100);
            }
            tileDisplayHex.FncGetTile = (p) => vt;
            tileDisplayHex.FullRedraw();
            Application_DoEvents();
            Assert.AreEqual(sExp, _tstCon?.Content,name);

            Thread_Sleep(100);
        }

        [DataTestMethod()]
        [TestProperty("Author", "J.C.")]
        [DataRow("00 _", VTiles.zero, VTiles.zero, new string[] {
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00       \r\n\\x00\\x00        \r\n\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00       \r\n\\x00\\x00        \r\n\\x00\\x00\\x00"})]
        [DataRow("01 _", VTiles.tile1, VTiles.zero, new string[] {
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00 \\c6E=-\\c00  \\c6E=-\\c00\r\n\\x00\\x00 \\c6E=-\\c00  \\c6E=-\\c00 \r\n\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00  \\c6E=-=-\\c00 \r\n\\x00\\x00  \\c6E=-=-\\c00  \r\n\\x00\\x00\\x00"})]
		[DataRow("02 _", VTiles.Wall, VTiles.zero, new string[] {
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00 \\c4F|_\\c00  \\c4F|_\\c00\r\n\\x00\\x00 \\c4F|_\\c00  \\c4F|_\\c00 \r\n\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00  \\c4F|_|_\\c00 \r\n\\x00\\x00  \\c4F|_|_\\c00  \r\n\\x00\\x00\\x00"})]
		[DataRow("03 _", VTiles.Dest, VTiles.zero, new string[] {
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00 \\c0E╓╖\\c00  \\c0E╓╖\\c00\r\n\\x00\\x00 \\c0E╓╖\\c00  \\c0E╓╖\\c00 \r\n\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00  \\c0E╓╖╓╖\\c00 \r\n\\x00\\x00  \\c0E╓╖╓╖\\c00  \r\n\\x00\\x00\\x00"})]
		[DataRow("04 _", VTiles.Player, VTiles.zero, new string[] {
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00 \\c6F⌐@\\c00  \\c6F⌐@\\c00\r\n\\x00\\x00 \\c6F⌐@\\c00  \\c6F⌐@\\c00 \r\n\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00  \\c6F⌐@⌐@\\c00 \r\n\\x00\\x00  \\c6F⌐@⌐@\\c00  \r\n\\x00\\x00\\x00"})]
		[DataRow("05 _", VTiles.Boulder, VTiles.zero, new string[] {
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00 \\c6E[]\\c00  \\c6E[]\\c00\r\n\\x00\\x00 \\c6E[]\\c00  \\c6E[]\\c00 \r\n\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00        \r\n\\x00\\x00\\x00  \\c6E[][]\\c00 \r\n\\x00\\x00  \\c6E[][]\\c00  \r\n\\x00\\x00\\x00"})]
		[DataRow("10 _", VTiles.zero, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=\\c00  \\c6E-=\\c00  \r\n\\x00\\x00\\c6E=\\c00  \\c6E-=\\c00  \\c6E-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-\\c00    \\c6E=\\c00\r\n\\x00\\x00\\c6E=-\\c00    \\c6E=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00"})]
		[DataRow("11 _", VTiles.tile1, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E==--==-\\c00\r\n\\x00\\x00\\c6E==--==--\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00\r\n\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00"})]
		[DataRow("12 _", VTiles.Wall, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=\\c4F|_\\c6E-=\\c4F|_\\c00\r\n\\x00\\x00\\c6E=\\c4F|_\\c6E-=\\c4F|_\\c6E-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-\\c4F|_|_\\c6E=\\c00\r\n\\x00\\x00\\c6E=-\\c4F|_|_\\c6E=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00"})]
		[DataRow("13 _", VTiles.Dest, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=\\c0E╓╖\\c6E-=\\c0E╓╖\\c00\r\n\\x00\\x00\\c6E=\\c0E╓╖\\c6E-=\\c0E╓╖\\c6E-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-\\c0E╓╖╓╖\\c6E=\\c00\r\n\\x00\\x00\\c6E=-\\c0E╓╖╓╖\\c6E=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00"})]
		[DataRow("14 _", VTiles.Player, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=\\c6F⌐@\\c6E-=\\c6F⌐@\\c00\r\n\\x00\\x00\\c6E=\\c6F⌐@\\c6E-=\\c6F⌐@\\c6E-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-\\c6F⌐@⌐@\\c6E=\\c00\r\n\\x00\\x00\\c6E=-\\c6F⌐@⌐@\\c6E=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00"})]
		[DataRow("15 _", VTiles.Boulder, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=[]-=[]\\c00\r\n\\x00\\x00\\c6E=[]-=[]-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\c6E=-[][]=\\c00\r\n\\x00\\x00\\c6E=-[][]=-\\c00\r\n\\x00\\x00\\x00\\c6E=-=-=-=\\c00"})]
		[DataRow("20 _", VTiles.zero, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|\\c00  \\c4F_|\\c00  \r\n\\x00\\x00\\c4F|\\c00  \\c4F_|\\c00  \\c4F_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00" ,
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|_\\c00    \\c4F|\\c00\r\n\\x00\\x00\\c4F|_\\c00    \\c4F|_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00"})]
		[DataRow("21 _", VTiles.tile1, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|\\c6E=-\\c4F_|\\c6E=-\\c00\r\n\\x00\\x00\\c4F|\\c6E=-\\c4F_|\\c6E=-\\c4F_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00",
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|_\\c6E=-=-\\c4F|\\c00\r\n\\x00\\x00\\c4F|_\\c6E=-=-\\c4F|_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00" })]
        [DataRow("22 _", VTiles.Wall, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F||__||_\\c00\r\n\\x00\\x00\\c4F||__||__\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00",
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00\r\n\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00" })]
        [DataRow("23 _", VTiles.Dest, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|\\c0E╓╖\\c4F_|\\c0E╓╖\\c00\r\n\\x00\\x00\\c4F|\\c0E╓╖\\c4F_|\\c0E╓╖\\c4F_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00",
            @"\c00\x00\x00\c4F|_|_|_|_\c00
\x00\x00\x00\c4F|_\c0E╓╖╓╖\c4F|\c00
\x00\x00\c4F|_\c0E╓╖╓╖\c4F|_\c00
\x00\x00\x00\c4F|_|_|_|\c00" })]
        [DataRow("24 _", VTiles.Player, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|\\c6F⌐@\\c4F_|\\c6F⌐@\\c00\r\n\\x00\\x00\\c4F|\\c6F⌐@\\c4F_|\\c6F⌐@\\c4F_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00",
            @"\c00\x00\x00\c4F|_|_|_|_\c00
\x00\x00\x00\c4F|_\c6F⌐@⌐@\c4F|\c00
\x00\x00\c4F|_\c6F⌐@⌐@\c4F|_\c00
\x00\x00\x00\c4F|_|_|_|\c00" })]
		[DataRow("25 _", VTiles.Boulder, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F|_|_|_|_\\c00\r\n\\x00\\x00\\x00\\c4F|\\c6E[]\\c4F_|\\c6E[]\\c00\r\n\\x00\\x00\\c4F|\\c6E[]\\c4F_|\\c6E[]\\c4F_\\c00\r\n\\x00\\x00\\x00\\c4F|_|_|_|\\c00",
            @"\c00\x00\x00\c4F|_|_|_|_\c00
\x00\x00\x00\c4F|_\c6E[][]\c4F|\c00
\x00\x00\c4F|_\c6E[][]\c4F|_\c00
\x00\x00\x00\c4F|_|_|_|\c00" })]
		[DataRow("30 _", VTiles.zero, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓\\c00  \\c0E╖╓\\c00  \r\n\\x00\\x00\\c0E╓\\c00  \\c0E╖╓\\c00  \\c0E╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00",
            @"\c00\x00\x00\c0E╓╖╓╖╓╖╓╖\c00
\x00\x00\x00\c0E╓╖\c00    \c0E╓\c00
\x00\x00\c0E╓╖\c00    \c0E╓╖\c00
\x00\x00\x00\c0E╓╖╓╖╓╖╓\c00" })]
		[DataRow("31 _", VTiles.tile1, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓\\c6E=-\\c0E╖╓\\c6E=-\\c00\r\n\\x00\\x00\\c0E╓\\c6E=-\\c0E╖╓\\c6E=-\\c0E╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00",
            @"\c00\x00\x00\c0E╓╖╓╖╓╖╓╖\c00
\x00\x00\x00\c0E╓╖\c6E=-=-\c0E╓\c00
\x00\x00\c0E╓╖\c6E=-=-\c0E╓╖\c00
\x00\x00\x00\c0E╓╖╓╖╓╖╓\c00" })]
		[DataRow("32 _", VTiles.Wall, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓\\c4F|_\\c0E╖╓\\c4F|_\\c00\r\n\\x00\\x00\\c0E╓\\c4F|_\\c0E╖╓\\c4F|_\\c0E╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00",
            @"\c00\x00\x00\c0E╓╖╓╖╓╖╓╖\c00
\x00\x00\x00\c0E╓╖\c4F|_|_\c0E╓\c00
\x00\x00\c0E╓╖\c4F|_|_\c0E╓╖\c00
\x00\x00\x00\c0E╓╖╓╖╓╖╓\c00" })]
		[DataRow("33 _", VTiles.Dest, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╓╖╖╓╓╖\\c00\r\n\\x00\\x00\\c0E╓╓╖╖╓╓╖╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00",
            @"\c00\x00\x00\c0E╓╖╓╖╓╖╓╖\c00
\x00\x00\x00\c0E╓╖╓╖╓╖╓\c00
\x00\x00\c0E╓╖╓╖╓╖╓╖\c00
\x00\x00\x00\c0E╓╖╓╖╓╖╓\c00" })]
		[DataRow("34 _", VTiles.Player, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓\\c6F⌐@\\c0E╖╓\\c6F⌐@\\c00\r\n\\x00\\x00\\c0E╓\\c6F⌐@\\c0E╖╓\\c6F⌐@\\c0E╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00",
            @"\c00\x00\x00\c0E╓╖╓╖╓╖╓╖\c00
\x00\x00\x00\c0E╓╖\c6F⌐@⌐@\c0E╓\c00
\x00\x00\c0E╓╖\c6F⌐@⌐@\c0E╓╖\c00
\x00\x00\x00\c0E╓╖╓╖╓╖╓\c00" })]
		[DataRow("35 _", VTiles.Boulder, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E╓╖╓╖╓╖╓╖\\c00\r\n\\x00\\x00\\x00\\c0E╓\\c6E[]\\c0E╖╓\\c6E[]\\c00\r\n\\x00\\x00\\c0E╓\\c6E[]\\c0E╖╓\\c6E[]\\c0E╖\\c00\r\n\\x00\\x00\\x00\\c0E╓╖╓╖╓╖╓\\c00",
            @"\c00\x00\x00\c0E╓╖╓╖╓╖╓╖\c00
\x00\x00\x00\c0E╓╖\c6E[][]\c0E╓\c00
\x00\x00\c0E╓╖\c6E[][]\c0E╓╖\c00
\x00\x00\x00\c0E╓╖╓╖╓╖╓\c00" })]
		[DataRow("40 _", VTiles.zero, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐\\c00  \\c6F@⌐\\c00  \r\n\\x00\\x00\\c6F⌐\\c00  \\c6F@⌐\\c00  \\c6F@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00",
            @"\c00\x00\x00\c6F⌐@⌐@⌐@⌐@\c00
\x00\x00\x00\c6F⌐@\c00    \c6F⌐\c00
\x00\x00\c6F⌐@\c00    \c6F⌐@\c00
\x00\x00\x00\c6F⌐@⌐@⌐@⌐\c00" })]
		[DataRow("41 _", VTiles.tile1, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐\\c6E=-\\c6F@⌐\\c6E=-\\c00\r\n\\x00\\x00\\c6F⌐\\c6E=-\\c6F@⌐\\c6E=-\\c6F@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00",
            @"\c00\x00\x00\c6F⌐@⌐@⌐@⌐@\c00
\x00\x00\x00\c6F⌐@\c6E=-=-\c6F⌐\c00
\x00\x00\c6F⌐@\c6E=-=-\c6F⌐@\c00
\x00\x00\x00\c6F⌐@⌐@⌐@⌐\c00" })]
		[DataRow("42 _", VTiles.Wall, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐\\c4F|_\\c6F@⌐\\c4F|_\\c00\r\n\\x00\\x00\\c6F⌐\\c4F|_\\c6F@⌐\\c4F|_\\c6F@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00",
            @"\c00\x00\x00\c6F⌐@⌐@⌐@⌐@\c00
\x00\x00\x00\c6F⌐@\c4F|_|_\c6F⌐\c00
\x00\x00\c6F⌐@\c4F|_|_\c6F⌐@\c00
\x00\x00\x00\c6F⌐@⌐@⌐@⌐\c00" })]
		[DataRow("43 _", VTiles.Dest, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐\\c0E╓╖\\c6F@⌐\\c0E╓╖\\c00\r\n\\x00\\x00\\c6F⌐\\c0E╓╖\\c6F@⌐\\c0E╓╖\\c6F@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00",
            @"\c00\x00\x00\c6F⌐@⌐@⌐@⌐@\c00
\x00\x00\x00\c6F⌐@\c0E╓╖╓╖\c6F⌐\c00
\x00\x00\c6F⌐@\c0E╓╖╓╖\c6F⌐@\c00
\x00\x00\x00\c6F⌐@⌐@⌐@⌐\c00" })]
		[DataRow("44 _", VTiles.Player, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐⌐@@⌐⌐@\\c00\r\n\\x00\\x00\\c6F⌐⌐@@⌐⌐@@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00",
            @"\c00\x00\x00\c6F⌐@⌐@⌐@⌐@\c00
\x00\x00\x00\c6F⌐@⌐@⌐@⌐\c00
\x00\x00\c6F⌐@⌐@⌐@⌐@\c00
\x00\x00\x00\c6F⌐@⌐@⌐@⌐\c00" })]
		[DataRow("45 _", VTiles.Boulder, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐@⌐@⌐@⌐@\\c00\r\n\\x00\\x00\\x00\\c6F⌐\\c6E[]\\c6F@⌐\\c6E[]\\c00\r\n\\x00\\x00\\c6F⌐\\c6E[]\\c6F@⌐\\c6E[]\\c6F@\\c00\r\n\\x00\\x00\\x00\\c6F⌐@⌐@⌐@⌐\\c00",
            @"\c00\x00\x00\c6F⌐@⌐@⌐@⌐@\c00
\x00\x00\x00\c6F⌐@\c6E[][]\c6F⌐\c00
\x00\x00\c6F⌐@\c6E[][]\c6F⌐@\c00
\x00\x00\x00\c6F⌐@⌐@⌐@⌐\c00" })]
		[DataRow("50 _", VTiles.zero, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[\\c00  \\c6E][\\c00  \r\n\\x00\\x00\\c6E[\\c00  \\c6E][\\c00  \\c6E]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00",
            @"\c00\x00\x00\c6E[][][][]\c00
\x00\x00\x00\c6E[]\c00    \c6E[\c00
\x00\x00\c6E[]\c00    \c6E[]\c00
\x00\x00\x00\c6E[][][][\c00" })]
		[DataRow("51 _", VTiles.tile1, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[=-][=-\\c00\r\n\\x00\\x00\\c6E[=-][=-]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00",
            @"\c00\x00\x00\c6E[][][][]\c00
\x00\x00\x00\c6E[]=-=-[\c00
\x00\x00\c6E[]=-=-[]\c00
\x00\x00\x00\c6E[][][][\c00" })]
		[DataRow("52 _", VTiles.Wall, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[\\c4F|_\\c6E][\\c4F|_\\c00\r\n\\x00\\x00\\c6E[\\c4F|_\\c6E][\\c4F|_\\c6E]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00",
            @"\c00\x00\x00\c6E[][][][]\c00
\x00\x00\x00\c6E[]\c4F|_|_\c6E[\c00
\x00\x00\c6E[]\c4F|_|_\c6E[]\c00
\x00\x00\x00\c6E[][][][\c00" })]
		[DataRow("53 _", VTiles.Dest, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[\\c0E╓╖\\c6E][\\c0E╓╖\\c00\r\n\\x00\\x00\\c6E[\\c0E╓╖\\c6E][\\c0E╓╖\\c6E]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00",
            @"\c00\x00\x00\c6E[][][][]\c00
\x00\x00\x00\c6E[]\c0E╓╖╓╖\c6E[\c00
\x00\x00\c6E[]\c0E╓╖╓╖\c6E[]\c00
\x00\x00\x00\c6E[][][][\c00" })]
		[DataRow("54 _", VTiles.Player, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[\\c6F⌐@\\c6E][\\c6F⌐@\\c00\r\n\\x00\\x00\\c6E[\\c6F⌐@\\c6E][\\c6F⌐@\\c6E]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00",
            @"\c00\x00\x00\c6E[][][][]\c00
\x00\x00\x00\c6E[]\c6F⌐@⌐@\c6E[\c00
\x00\x00\c6E[]\c6F⌐@⌐@\c6E[]\c00
\x00\x00\x00\c6E[][][][\c00" })]
		[DataRow("55 _", VTiles.Boulder, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E[][][][]\\c00\r\n\\x00\\x00\\x00\\c6E[[]][[]\\c00\r\n\\x00\\x00\\c6E[[]][[]]\\c00\r\n\\x00\\x00\\x00\\c6E[][][][\\c00",
            @"\c00\x00\x00\c6E[][][][]\c00
\x00\x00\x00\c6E[][][][\c00
\x00\x00\c6E[][][][]\c00
\x00\x00\x00\c6E[][][][\c00" })]

		public void UpdateTest(string name, VTiles vt, VTiles vt2, string[] sExp)
        {
            var tileDisplayHex = new TileDisplayHex(new Point(2, 0), new Size(4, 4), new TestTileDef21())
            {
                FncGetTile = (p) => vt2
            };
            tileDisplayHex.FullRedraw();
            Application_DoEvents();
            AssertAreEqual(cExpUpdateText[(int)vt2], _tstCon?.Content ?? "");

            Thread_Sleep(10);

            tileDisplayHex.FncGetTile = (p) => xTst(p)? vt:vt2;
            tileDisplayHex.FncOldPos = (p) => xTst(p) ? new Point(p.X * 3 - 3, p.Y) : p;
            tileDisplayHex.Update(true); //HalfStep
            Application_DoEvents();
            AssertAreEqual(sExp[0], _tstCon?.Content ?? "",$"Test:{name}.HalfStep");

            Thread_Sleep(10);

            tileDisplayHex.Update(false); //FullStep
            Application_DoEvents();
            AssertAreEqual(sExp[1], _tstCon?.Content??"", $"Test:{name}.FullStep");

            Thread_Sleep(10);
            Application_DoEvents();

            console!.Clear();
            tileDisplayHex.FullRedraw(); 
            Application_DoEvents();
            Assert.AreEqual(sExp[1], _tstCon?.Content, $"Test:{name}.FullRedraw");

            static bool xTst(Point p) => p.X > 0 && p.Y > 0 && p.X < 3 && p.Y < 3;
        }

		[DataTestMethod()]
		[TestProperty("Author", "J.C.")]
		[DataRow("00 _", VTiles.zero, "\\c00\r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00")]
		[DataRow("01 _", VTiles.tile1, "\\c00\r\n\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00")]
		[DataRow("02 _", VTiles.Wall, "\\c00\r\n\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00\r\n\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00")]
		[DataRow("03 _", VTiles.Dest, "\\c00\r\n\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00\r\n\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00")]
		[DataRow("04 _", VTiles.Player, "\\c00\r\n\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00\r\n\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00")]
		[DataRow("05 _", VTiles.Boulder, "\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00")]
		[DataRow("06 _", VTiles.E1, "\\c00\r\n\\x00\\x00\\c1A]\\cA0°°\\c1A[]\\cA0°°\\c1A[]\\cA0°°\\c1A[]\\cA0°°\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\x00\\x00\\c1A]\\cA0°°\\c1A[]\\cA0°°\\c1A[]\\cA0°°\\c1A[]\\cA0°\\c00\r\n\\x00\\x00\\x00\\x00\\c1A_\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!\\c00\r\n\\x00\\x00\\c1A]\\cA0°°\\c1A[]\\cA0°°\\c1A[]\\cA0°°\\c1A[]\\cA0°°\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\x00\\x00\\c1A]\\cA0°°\\c1A[]\\cA0°°\\c1A[]\\cA0°°\\c1A[]\\cA0°\\c00\r\n\\x00\\x00\\x00\\x00\\c1A_\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!\\c00")]
		[DataRow("07 _", VTiles.E2, "\\c00\r\n\\x00\\x00\\c1A◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\\\c00\r\n\\x00\\x00\\x00\\x00\\c1A◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[◄\\cA0°\\c00\r\n\\x00\\x00\\x00\\x00\\c1A_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\_\\cA0!\\c00\r\n\\x00\\x00\\c1A◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\\\c00\r\n\\x00\\x00\\x00\\x00\\c1A◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[◄\\cA0°@\\c1A[◄\\cA0°\\c00\r\n\\x00\\x00\\x00\\x00\\c1A_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\_\\cA0!!\\c1A\\\\_\\cA0!\\c00")]
		[DataRow("08 _", VTiles.E3, "\\c00\r\n\\x00\\x00\\c1A]\\cA0oo\\c1A[]\\cA0oo\\c1A[]\\cA0oo\\c1A[]\\cA0oo\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\x00\\x00\\c1A]\\cA0oo\\c1A[]\\cA0oo\\c1A[]\\cA0oo\\c1A[]\\cA0o\\c00\r\n\\x00\\x00\\x00\\x00\\c1A_\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!\\c00\r\n\\x00\\x00\\c1A]\\cA0oo\\c1A[]\\cA0oo\\c1A[]\\cA0oo\\c1A[]\\cA0oo\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\x00\\x00\\c1A]\\cA0oo\\c1A[]\\cA0oo\\c1A[]\\cA0oo\\c1A[]\\cA0o\\c00\r\n\\x00\\x00\\x00\\x00\\c1A_\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!!\\c1A__\\cA0!\\c00")]
		[DataRow("09 _", VTiles.E4, "\\c00\r\n\\x00\\x00\\c1A]\\cA0@°\\c1A►]\\cA0@°\\c1A►]\\cA0@°\\c1A►]\\cA0@°\\c1A►\\c00\r\n\\x00\\x00\\c1A/\\cA0!!\\c1A_/\\cA0!!\\c1A_/\\cA0!!\\c1A_/\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\x00\\x00\\c1A]\\cA0@°\\c1A►]\\cA0@°\\c1A►]\\cA0@°\\c1A►]\\cA0@\\c00\r\n\\x00\\x00\\x00\\x00\\c1A/\\cA0!!\\c1A_/\\cA0!!\\c1A_/\\cA0!!\\c1A_/\\cA0!\\c00\r\n\\x00\\x00\\c1A]\\cA0@°\\c1A►]\\cA0@°\\c1A►]\\cA0@°\\c1A►]\\cA0@°\\c1A►\\c00\r\n\\x00\\x00\\c1A/\\cA0!!\\c1A_/\\cA0!!\\c1A_/\\cA0!!\\c1A_/\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\x00\\x00\\c1A]\\cA0@°\\c1A►]\\cA0@°\\c1A►]\\cA0@°\\c1A►]\\cA0@\\c00\r\n\\x00\\x00\\x00\\x00\\c1A/\\cA0!!\\c1A_/\\cA0!!\\c1A_/\\cA0!!\\c1A_/\\cA0!\\c00")]
		[DataRow("10 _", VTiles.BounderMoving, "\\c00\r\n\\x00\\x00\\c6E/╨╨\\\\/╨╨\\\\/╨╨\\\\/╨╨\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/╨╨\\\\/╨╨\\\\/╨╨\\\\/╨\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00\r\n\\x00\\x00\\c6E/╨╨\\\\/╨╨\\\\/╨╨\\\\/╨╨\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/╨╨\\\\/╨╨\\\\/╨╨\\\\/╨\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00")]
		[DataRow("11 _", VTiles.PlayerDead, "\\c00\r\n\\x00\\x00\\c6F +*∩ +*∩ +*∩ +*∩\\c00\r\n\\x00\\x00\\c6F╘═◊@╘═◊@╘═◊@╘═◊@\\c00\r\n\\x00\\x00\\x00\\x00\\c6F +*∩ +*∩ +*∩ +\\c00\r\n\\x00\\x00\\x00\\x00\\c6F╘═◊@╘═◊@╘═◊@╘═\\c00\r\n\\x00\\x00\\c6F +*∩ +*∩ +*∩ +*∩\\c00\r\n\\x00\\x00\\c6F╘═◊@╘═◊@╘═◊@╘═◊@\\c00\r\n\\x00\\x00\\x00\\x00\\c6F +*∩ +*∩ +*∩ +\\c00\r\n\\x00\\x00\\x00\\x00\\c6F╘═◊@╘═◊@╘═◊@╘═\\c00")]
		public void FullRedrawTest2(string name, VTiles vt, string sExp) {
			var tileDisplayHex = new TileDisplayHex(new Point(2, 1), new Size(4, 4), new TestTileDef42());
			if (vt == VTiles.zero) {
				tileDisplayHex.FncGetTile = (p) => (VTiles)(p.X + p.Y * tileDisplayHex.DispSize.Width);
				tileDisplayHex.FullRedraw();
				Application_DoEvents();
				Assert.AreEqual(cTileDisplayTest4, _tstCon?.Content);
				Thread_Sleep(100);
			}
			tileDisplayHex.FncGetTile = (p) => vt;
			tileDisplayHex.FullRedraw();
			Application_DoEvents();
			Assert.AreEqual(sExp, _tstCon?.Content);

			Thread_Sleep(100);
		}

		[DataTestMethod()]
		[TestProperty("Author", "J.C.")]
		[DataRow("00 _", VTiles.zero, "\\c00\r\n\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00")]
		[DataRow("01 _", VTiles.tile1, "\\c00\r\n\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00")]
		[DataRow("02 _", VTiles.Wall, "\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00")]
		[DataRow("03 _", VTiles.Dest, "\\c00\r\n\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00")]
		[DataRow("04 _", VTiles.Player, "\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00")]
		[DataRow("05 _", VTiles.Boulder, "\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00")]
		[DataRow("06 _", VTiles.E1, "\\c00\r\n\\x00\\x00\\c1A]\\cA0°°\\c1A[\\c00\\x00\\x00\\x00\\x00\\c1A]\\cA0°°\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0°°\\c1A[_\\cA0!!\\c1A_]\\cA0°°\\c1A[\\c00\r\n\\x00\\x00\\c1A]\\cA0°°\\c1A[_\\cA0!!\\c1A_]\\cA0°°\\c1A[_\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0°°\\c1A[_\\cA0!!\\c1A_]\\cA0°°\\c1A[\\c00\r\n\\x00\\x00\\c1A]\\cA0°°\\c1A[_\\cA0!!\\c1A_]\\cA0°°\\c1A[_\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0°°\\c1A[_\\cA0!!\\c1A_]\\cA0°°\\c1A[\\c00\r\n\\x00\\x00\\c1A]\\cA0°°\\c1A[_\\cA0!!\\c1A_]\\cA0°°\\c1A[_\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0°°\\c1A[_\\cA0!!\\c1A_]\\cA0°°\\c1A[\\c00")]
		[DataRow("07 _", VTiles.E2, "\\c00\r\n\\x00\\x00\\c1A◄\\cA0°@\\c1A[\\c00\\x00\\x00\\x00\\x00\\c1A◄\\cA0°@\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[\\c00\r\n\\x00\\x00\\c1A◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[\\c00\r\n\\x00\\x00\\c1A◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[\\c00\r\n\\x00\\x00\\c1A◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[_\\cA0!!\\c1A\\\\◄\\cA0°@\\c1A[\\c00")]
		[DataRow("08 _", VTiles.E3, "\\c00\r\n\\x00\\x00\\c1A]\\cA0oo\\c1A[\\c00\\x00\\x00\\x00\\x00\\c1A]\\cA0oo\\c1A[\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0oo\\c1A[_\\cA0!!\\c1A_]\\cA0oo\\c1A[\\c00\r\n\\x00\\x00\\c1A]\\cA0oo\\c1A[_\\cA0!!\\c1A_]\\cA0oo\\c1A[_\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0oo\\c1A[_\\cA0!!\\c1A_]\\cA0oo\\c1A[\\c00\r\n\\x00\\x00\\c1A]\\cA0oo\\c1A[_\\cA0!!\\c1A_]\\cA0oo\\c1A[_\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0oo\\c1A[_\\cA0!!\\c1A_]\\cA0oo\\c1A[\\c00\r\n\\x00\\x00\\c1A]\\cA0oo\\c1A[_\\cA0!!\\c1A_]\\cA0oo\\c1A[_\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A_\\cA0!!\\c1A_]\\cA0oo\\c1A[_\\cA0!!\\c1A_]\\cA0oo\\c1A[\\c00")]
		[DataRow("09 _", VTiles.E4, "\\c00\r\n\\x00\\x00\\c1A]\\cA0@°\\c1A►\\c00\\x00\\x00\\x00\\x00\\c1A]\\cA0@°\\c1A►\\c00\r\n\\x00\\x00\\c1A/\\cA0!!\\c1A_]\\cA0@°\\c1A►/\\cA0!!\\c1A_]\\cA0@°\\c1A►\\c00\r\n\\x00\\x00\\c1A]\\cA0@°\\c1A►/\\cA0!!\\c1A_]\\cA0@°\\c1A►/\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A/\\cA0!!\\c1A_]\\cA0@°\\c1A►/\\cA0!!\\c1A_]\\cA0@°\\c1A►\\c00\r\n\\x00\\x00\\c1A]\\cA0@°\\c1A►/\\cA0!!\\c1A_]\\cA0@°\\c1A►/\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A/\\cA0!!\\c1A_]\\cA0@°\\c1A►/\\cA0!!\\c1A_]\\cA0@°\\c1A►\\c00\r\n\\x00\\x00\\c1A]\\cA0@°\\c1A►/\\cA0!!\\c1A_]\\cA0@°\\c1A►/\\cA0!!\\c1A_\\c00\r\n\\x00\\x00\\c1A/\\cA0!!\\c1A_]\\cA0@°\\c1A►/\\cA0!!\\c1A_]\\cA0@°\\c1A►\\c00")]
		[DataRow("10 _", VTiles.BounderMoving, "\\c00\r\n\\x00\\x00\\c6E/╨╨\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/╨╨\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//╨╨\\\\\\\\__//╨╨\\\\\\c00\r\n\\x00\\x00\\c6E/╨╨\\\\\\\\__//╨╨\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//╨╨\\\\\\\\__//╨╨\\\\\\c00\r\n\\x00\\x00\\c6E/╨╨\\\\\\\\__//╨╨\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//╨╨\\\\\\\\__//╨╨\\\\\\c00\r\n\\x00\\x00\\c6E/╨╨\\\\\\\\__//╨╨\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//╨╨\\\\\\\\__//╨╨\\\\\\c00")]
		[DataRow("11 _", VTiles.PlayerDead, "\\c00\r\n\\x00\\x00\\c6F +*∩\\c00\\x00\\x00\\x00\\x00\\c6F +*∩\\c00\r\n\\x00\\x00\\c6F╘═◊@ +*∩╘═◊@ +*∩\\c00\r\n\\x00\\x00\\c6F +*∩╘═◊@ +*∩╘═◊@\\c00\r\n\\x00\\x00\\c6F╘═◊@ +*∩╘═◊@ +*∩\\c00\r\n\\x00\\x00\\c6F +*∩╘═◊@ +*∩╘═◊@\\c00\r\n\\x00\\x00\\c6F╘═◊@ +*∩╘═◊@ +*∩\\c00\r\n\\x00\\x00\\c6F +*∩╘═◊@ +*∩╘═◊@\\c00\r\n\\x00\\x00\\c6F╘═◊@ +*∩╘═◊@ +*∩\\c00")]
		public void FullRedrawTest3(string name, VTiles vt, string sExp) {
			var tileDisplayHex = new TileDisplayHex(new Point(2, 1), new Size(4, 4), new TestTileDef42(),true);
			if (vt == VTiles.zero) {
				tileDisplayHex.FncGetTile = (p) => (VTiles)(p.X + p.Y * tileDisplayHex.DispSize.Width);
				tileDisplayHex.FullRedraw();
				Application_DoEvents();
				Assert.AreEqual(cTileDisplayTest5, _tstCon?.Content);
				Thread_Sleep(100);
			}
			tileDisplayHex.FncGetTile = (p) => vt;
			tileDisplayHex.FullRedraw();
			Application_DoEvents();
			Assert.AreEqual(sExp, _tstCon?.Content);

			Thread_Sleep(100);
		}

		[DataTestMethod()]
		[TestProperty("Author", "J.C.")]
		[DataRow("00 _", VTiles.zero, VTiles.zero, new string[] {
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00"})]
		[DataRow("01 _", VTiles.tile1, VTiles.zero, new string[] {
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00  \\c6E=-=-\\c00    \\c6E=-=-\\c00\r\n\\x00\\x00\\x00\\x00  \\c6E-=-=\\c00    \\c6E-=-=\\c00\r\n\\x00\\x00  \\c6E=-=-\\c00    \\c6E=-=-\\c00  \r\n\\x00\\x00  \\c6E-=-=\\c00    \\c6E-=-=\\c00  \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00    \\c6E=-=-=-=-\\c00  \r\n\\x00\\x00\\x00\\x00    \\c6E-=-=-=-=\\c00  \r\n\\x00\\x00    \\c6E=-=-=-=-\\c00    \r\n\\x00\\x00    \\c6E-=-=-=-=\\c00    \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00"})]
		[DataRow("02 _", VTiles.Wall, VTiles.zero, new string[] {
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00  \\c4F─┴┬─\\c00    \\c4F─┴┬─\\c00\r\n\\x00\\x00\\x00\\x00  \\c4F─┬┴─\\c00    \\c4F─┬┴─\\c00\r\n\\x00\\x00  \\c4F─┴┬─\\c00    \\c4F─┴┬─\\c00  \r\n\\x00\\x00  \\c4F─┬┴─\\c00    \\c4F─┬┴─\\c00  \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00    \\c4F─┴┬──┴┬─\\c00  \r\n\\x00\\x00\\x00\\x00    \\c4F─┬┴──┬┴─\\c00  \r\n\\x00\\x00    \\c4F─┴┬──┴┬─\\c00    \r\n\\x00\\x00    \\c4F─┬┴──┬┴─\\c00    \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00"})]
		[DataRow("03 _", VTiles.Dest, VTiles.zero, new string[] {
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00  \\c0E ╓╖ \\c00    \\c0E ╓╖ \\c00\r\n\\x00\\x00\\x00\\x00  \\c2A▓\\c22░\\c02▒\\c22▓\\c00    \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00  \\c0E ╓╖ \\c00    \\c0E ╓╖ \\c00  \r\n\\x00\\x00  \\c2A▓\\c22░\\c02▒\\c22▓\\c00    \\c2A▓\\c22░\\c02▒\\c22▓\\c00  \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00" ,
            @"\c00\x00\x00                
\x00\x00                
\x00\x00\x00\x00    \c0E ╓╖  ╓╖ \c00  
\x00\x00\x00\x00    \c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00  
\x00\x00    \c0E ╓╖  ╓╖ \c00    
\x00\x00    \c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00    
\x00\x00\x00\x00              
\x00\x00\x00\x00"})]
		[DataRow("04 _", VTiles.Player, VTiles.zero, new string[] {
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00  \\c6F⌐°@)\\c00    \\c6F⌐°@)\\c00\r\n\\x00\\x00\\x00\\x00  \\c6F ⌡⌡‼\\c00    \\c6F ⌡⌡‼\\c00\r\n\\x00\\x00  \\c6F⌐°@)\\c00    \\c6F⌐°@)\\c00  \r\n\\x00\\x00  \\c6F ⌡⌡‼\\c00    \\c6F ⌡⌡‼\\c00  \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00    \\c6F⌐°@)⌐°@)\\c00  \r\n\\x00\\x00\\x00\\x00    \\c6F ⌡⌡‼ ⌡⌡‼\\c00  \r\n\\x00\\x00    \\c6F⌐°@)⌐°@)\\c00    \r\n\\x00\\x00    \\c6F ⌡⌡‼ ⌡⌡‼\\c00    \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00"})]
		[DataRow("05 _", VTiles.Boulder, VTiles.zero, new string[] {
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00  \\c6E/¯¯\\\\\\c00    \\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\x00\\x00  \\c6E\\\\__/\\c00    \\c6E\\\\__/\\c00\r\n\\x00\\x00  \\c6E/¯¯\\\\\\c00    \\c6E/¯¯\\\\\\c00  \r\n\\x00\\x00  \\c6E\\\\__/\\c00    \\c6E\\\\__/\\c00  \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00" ,
			"\\c00\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00\\x00\\x00    \\c6E/¯¯\\\\/¯¯\\\\\\c00  \r\n\\x00\\x00\\x00\\x00    \\c6E\\\\__/\\\\__/\\c00  \r\n\\x00\\x00    \\c6E/¯¯\\\\/¯¯\\\\\\c00    \r\n\\x00\\x00    \\c6E\\\\__/\\\\__/\\c00    \r\n\\x00\\x00\\x00\\x00              \r\n\\x00\\x00\\x00\\x00"})]
		[DataRow("10 _", VTiles.zero, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-\\c00    \\c6E=-=-\\c00    \r\n\\x00\\x00\\x00\\x00\\c6E-=\\c00    \\c6E-=-=\\c00    \r\n\\x00\\x00\\c6E=-\\c00    \\c6E=-=-\\c00    \\c6E=-\\c00\r\n\\x00\\x00\\c6E-=\\c00    \\c6E-=-=\\c00    \\c6E-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-\\c00        \\c6E=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=\\c00        \\c6E-=\\c00\r\n\\x00\\x00\\c6E=-=-\\c00        \\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-=\\c00        \\c6E-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00"})]
		[DataRow("11 _", VTiles.tile1, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00"})]
		[DataRow("12 _", VTiles.Wall, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-\\c4F─┴┬─\\c6E=-=-\\c4F─┴┬─\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=\\c4F─┬┴─\\c6E-=-=\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c6E=-\\c4F─┴┬─\\c6E=-=-\\c4F─┴┬─\\c6E=-\\c00\r\n\\x00\\x00\\c6E-=\\c4F─┬┴─\\c6E-=-=\\c4F─┬┴─\\c6E-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-\\c4F─┴┬──┴┬─\\c6E=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=\\c4F─┬┴──┬┴─\\c6E-=\\c00\r\n\\x00\\x00\\c6E=-=-\\c4F─┴┬──┴┬─\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-=\\c4F─┬┴──┬┴─\\c6E-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00"})]
		[DataRow("13 _", VTiles.Dest, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-\\c0E ╓╖ \\c6E=-=-\\c0E ╓╖ \\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=\\c2A▓\\c22░\\c02▒\\c22▓\\c6E-=-=\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c6E=-\\c0E ╓╖ \\c6E=-=-\\c0E ╓╖ \\c6E=-\\c00\r\n\\x00\\x00\\c6E-=\\c2A▓\\c22░\\c02▒\\c22▓\\c6E-=-=\\c2A▓\\c22░\\c02▒\\c22▓\\c6E-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-\\c0E ╓╖  ╓╖ \\c6E=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c6E-=\\c00\r\n\\x00\\x00\\c6E=-=-\\c0E ╓╖  ╓╖ \\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-=\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c6E-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00"})]
		[DataRow("14 _", VTiles.Player, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-\\c6F⌐°@)\\c6E=-=-\\c6F⌐°@)\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=\\c6F ⌡⌡‼\\c6E-=-=\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6E=-\\c6F⌐°@)\\c6E=-=-\\c6F⌐°@)\\c6E=-\\c00\r\n\\x00\\x00\\c6E-=\\c6F ⌡⌡‼\\c6E-=-=\\c6F ⌡⌡‼\\c6E-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-\\c6F⌐°@)⌐°@)\\c6E=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=\\c6F ⌡⌡‼ ⌡⌡‼\\c6E-=\\c00\r\n\\x00\\x00\\c6E=-=-\\c6F⌐°@)⌐°@)\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-=\\c6F ⌡⌡‼ ⌡⌡‼\\c6E-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00"})]
		[DataRow("15 _", VTiles.Boulder, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-/¯¯\\\\=-=-/¯¯\\\\\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=\\\\__/-=-=\\\\__/\\c00\r\n\\x00\\x00\\c6E=-/¯¯\\\\=-=-/¯¯\\\\=-\\c00\r\n\\x00\\x00\\c6E-=\\\\__/-=-=\\\\__/-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\c6E-=-=-=-=-=-=-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-/¯¯\\\\/¯¯\\\\=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=\\\\__/\\\\__/-=\\c00\r\n\\x00\\x00\\c6E=-=-/¯¯\\\\/¯¯\\\\=-=-\\c00\r\n\\x00\\x00\\c6E-=-=\\\\__/\\\\__/-=-=\\c00\r\n\\x00\\x00\\x00\\x00\\c6E=-=-=-=-=-=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E-=-=-=-=-=-=-=\\c00"})]
		[DataRow("20 _", VTiles.zero, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴\\c00    \\c4F┬──┴\\c00    \r\n\\x00\\x00\\x00\\x00\\c4F─┬\\c00    \\c4F┴──┬\\c00    \r\n\\x00\\x00\\c4F─┴\\c00    \\c4F┬──┴\\c00    \\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┬\\c00    \\c4F┴──┬\\c00    \\c4F┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00",
			"\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00        \\c4F─┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴─\\c00        \\c4F─┬\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c00        \\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c00        \\c4F─┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00"})]
		[DataRow("21 _", VTiles.tile1, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴\\c6E=-=-\\c4F┬──┴\\c6E=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬\\c6E-=-=\\c4F┴──┬\\c6E-=-=\\c00\r\n\\x00\\x00\\c4F─┴\\c6E=-=-\\c4F┬──┴\\c6E=-=-\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┬\\c6E-=-=\\c4F┴──┬\\c6E-=-=\\c4F┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00",
            "\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c6E=-=-=-=-\\c4F─┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴─\\c6E-=-=-=-=\\c4F─┬\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c6E=-=-=-=-\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c6E-=-=-=-=\\c4F─┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00" })]
		[DataRow("22 _", VTiles.Wall, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴─┴┬─┬──┴─┴┬─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬─┬┴─┴──┬─┬┴─\\c00\r\n\\x00\\x00\\c4F─┴─┴┬─┬──┴─┴┬─┬─\\c00\r\n\\x00\\x00\\c4F─┬─┬┴─┴──┬─┬┴─┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00",
            "\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00\r\n\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00" })]
		[DataRow("23 _", VTiles.Dest, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴\\c0E ╓╖ \\c4F┬──┴\\c0E ╓╖ \\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬\\c2A▓\\c22░\\c02▒\\c22▓\\c4F┴──┬\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c4F─┴\\c0E ╓╖ \\c4F┬──┴\\c0E ╓╖ \\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┬\\c2A▓\\c22░\\c02▒\\c22▓\\c4F┴──┬\\c2A▓\\c22░\\c02▒\\c22▓\\c4F┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00",
            @"\c00\x00\x00\c4F─┴┬──┴┬──┴┬──┴┬─\c00
\x00\x00\c4F─┬┴──┬┴──┬┴──┬┴─\c00
\x00\x00\x00\x00\c4F─┴┬─\c0E ╓╖  ╓╖ \c4F─┴\c00
\x00\x00\x00\x00\c4F─┬┴─\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c4F─┬\c00
\x00\x00\c4F─┴┬─\c0E ╓╖  ╓╖ \c4F─┴┬─\c00
\x00\x00\c4F─┬┴─\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c4F─┬┴─\c00
\x00\x00\x00\x00\c4F─┴┬──┴┬──┴┬──┴\c00
\x00\x00\x00\x00\c4F─┬┴──┬┴──┬┴──┬\c00" })]
		[DataRow("24 _", VTiles.Player, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴\\c6F⌐°@)\\c4F┬──┴\\c6F⌐°@)\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬\\c6F ⌡⌡‼\\c4F┴──┬\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c4F─┴\\c6F⌐°@)\\c4F┬──┴\\c6F⌐°@)\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┬\\c6F ⌡⌡‼\\c4F┴──┬\\c6F ⌡⌡‼\\c4F┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00",
            @"\c00\x00\x00\c4F─┴┬──┴┬──┴┬──┴┬─\c00
\x00\x00\c4F─┬┴──┬┴──┬┴──┬┴─\c00
\x00\x00\x00\x00\c4F─┴┬─\c6F⌐°@)⌐°@)\c4F─┴\c00
\x00\x00\x00\x00\c4F─┬┴─\c6F ⌡⌡‼ ⌡⌡‼\c4F─┬\c00
\x00\x00\c4F─┴┬─\c6F⌐°@)⌐°@)\c4F─┴┬─\c00
\x00\x00\c4F─┬┴─\c6F ⌡⌡‼ ⌡⌡‼\c4F─┬┴─\c00
\x00\x00\x00\x00\c4F─┴┬──┴┬──┴┬──┴\c00
\x00\x00\x00\x00\c4F─┬┴──┬┴──┬┴──┬\c00" })]
		[DataRow("25 _", VTiles.Boulder, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴\\c6E/¯¯\\\\\\c4F┬──┴\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬\\c6E\\\\__/\\c4F┴──┬\\c6E\\\\__/\\c00\r\n\\x00\\x00\\c4F─┴\\c6E/¯¯\\\\\\c4F┬──┴\\c6E/¯¯\\\\\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┬\\c6E\\\\__/\\c4F┴──┬\\c6E\\\\__/\\c4F┴─\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┴┬──┴┬──┴┬──┴\\c00\r\n\\x00\\x00\\x00\\x00\\c4F─┬┴──┬┴──┬┴──┬\\c00",
            @"\c00\x00\x00\c4F─┴┬──┴┬──┴┬──┴┬─\c00
\x00\x00\c4F─┬┴──┬┴──┬┴──┬┴─\c00
\x00\x00\x00\x00\c4F─┴┬─\c6E/¯¯\\/¯¯\\\c4F─┴\c00
\x00\x00\x00\x00\c4F─┬┴─\c6E\\__/\\__/\c4F─┬\c00
\x00\x00\c4F─┴┬─\c6E/¯¯\\/¯¯\\\c4F─┴┬─\c00
\x00\x00\c4F─┬┴─\c6E\\__/\\__/\c4F─┬┴─\c00
\x00\x00\x00\x00\c4F─┴┬──┴┬──┴┬──┴\c00
\x00\x00\x00\x00\c4F─┬┴──┬┴──┬┴──┬\c00" })]
		[DataRow("30 _", VTiles.zero, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓\\c00    \\c0E╖  ╓\\c00    \r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c00    \\c02▒\\c22▓\\c2A▓\\c22░\\c00    \r\n\\x00\\x00\\c0E ╓\\c00    \\c0E╖  ╓\\c00    \\c0E╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c00    \\c02▒\\c22▓\\c2A▓\\c22░\\c00    \\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00",
            @"\c00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖ \c00        \c0E ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c00        \c2A▓\c22░\c00
\x00\x00\c0E ╓╖ \c00        \c0E ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c00        \c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c00" })]
		[DataRow("31 _", VTiles.tile1, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓\\c6E=-=-\\c0E╖  ╓\\c6E=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c6E-=-=\\c02▒\\c22▓\\c2A▓\\c22░\\c6E-=-=\\c00\r\n\\x00\\x00\\c0E ╓\\c6E=-=-\\c0E╖  ╓\\c6E=-=-\\c0E╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6E-=-=\\c02▒\\c22▓\\c2A▓\\c22░\\c6E-=-=\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00",
            @"\c00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖ \c6E=-=-=-=-\c0E ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c6E-=-=-=-=\c2A▓\c22░\c00
\x00\x00\c0E ╓╖ \c6E=-=-=-=-\c0E ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c6E-=-=-=-=\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c00" })]
		[DataRow("32 _", VTiles.Wall, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓\\c4F─┴┬─\\c0E╖  ╓\\c4F─┴┬─\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c4F─┬┴─\\c02▒\\c22▓\\c2A▓\\c22░\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c0E ╓\\c4F─┴┬─\\c0E╖  ╓\\c4F─┴┬─\\c0E╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c4F─┬┴─\\c02▒\\c22▓\\c2A▓\\c22░\\c4F─┬┴─\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00",
            @"\c00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖ \c4F─┴┬──┴┬─\c0E ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c4F─┬┴──┬┴─\c2A▓\c22░\c00
\x00\x00\c0E ╓╖ \c4F─┴┬──┴┬─\c0E ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c4F─┬┴──┬┴─\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c00" })]
		[DataRow("33 _", VTiles.Dest, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓ ╓╖ ╖  ╓ ╓╖ \\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c2A▓\\c22░\\c02▒\\c22▓\\c02▒\\c22▓\\c2A▓\\c22░\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c0E ╓ ╓╖ ╖  ╓ ╓╖ ╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c2A▓\\c22░\\c02▒\\c22▓\\c02▒\\c22▓\\c2A▓\\c22░\\c2A▓\\c22░\\c02▒\\c22▓\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00",
            @"\c00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c00
\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c00" })]
		[DataRow("34 _", VTiles.Player, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓\\c6F⌐°@)\\c0E╖  ╓\\c6F⌐°@)\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c6F ⌡⌡‼\\c02▒\\c22▓\\c2A▓\\c22░\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c0E ╓\\c6F⌐°@)\\c0E╖  ╓\\c6F⌐°@)\\c0E╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6F ⌡⌡‼\\c02▒\\c22▓\\c2A▓\\c22░\\c6F ⌡⌡‼\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00",
            @"\c00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖ \c6F⌐°@)⌐°@)\c0E ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c6F ⌡⌡‼ ⌡⌡‼\c2A▓\c22░\c00
\x00\x00\c0E ╓╖ \c6F⌐°@)⌐°@)\c0E ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c6F ⌡⌡‼ ⌡⌡‼\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c00" })]
		[DataRow("35 _", VTiles.Boulder, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓\\c6E/¯¯\\\\\\c0E╖  ╓\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c6E\\\\__/\\c02▒\\c22▓\\c2A▓\\c22░\\c6E\\\\__/\\c00\r\n\\x00\\x00\\c0E ╓\\c6E/¯¯\\\\\\c0E╖  ╓\\c6E/¯¯\\\\\\c0E╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6E\\\\__/\\c02▒\\c22▓\\c2A▓\\c22░\\c6E\\\\__/\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\x00\\x00\\c0E ╓╖  ╓╖  ╓╖  ╓\\c00\r\n\\x00\\x00\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c02▒\\c22▓\\c2A▓\\c22░\\c00",
            @"\c00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖ \c6E/¯¯\\/¯¯\\\c0E ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c6E\\__/\\__/\c2A▓\c22░\c00
\x00\x00\c0E ╓╖ \c6E/¯¯\\/¯¯\\\c0E ╓╖ \c00
\x00\x00\c2A▓\c22░\c02▒\c22▓\c6E\\__/\\__/\c2A▓\c22░\c02▒\c22▓\c00
\x00\x00\x00\x00\c0E ╓╖  ╓╖  ╓╖  ╓\c00
\x00\x00\x00\x00\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c00" })]
		[DataRow("40 _", VTiles.zero, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°\\c00    \\c6F@)⌐°\\c00    \r\n\\x00\\x00\\x00\\x00\\c6F ⌡\\c00    \\c6F⌡‼ ⌡\\c00    \r\n\\x00\\x00\\c6F⌐°\\c00    \\c6F@)⌐°\\c00    \\c6F@)\\c00\r\n\\x00\\x00\\c6F ⌡\\c00    \\c6F⌡‼ ⌡\\c00    \\c6F⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00",
            @"\c00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°@)\c00
\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)\c00        \c6F⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼\c00        \c6F ⌡\c00
\x00\x00\c6F⌐°@)\c00        \c6F⌐°@)\c00
\x00\x00\c6F ⌡⌡‼\c00        \c6F ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\c00" })]
		[DataRow("41 _", VTiles.tile1, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°\\c6E=-=-\\c6F@)⌐°\\c6E=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡\\c6E-=-=\\c6F⌡‼ ⌡\\c6E-=-=\\c00\r\n\\x00\\x00\\c6F⌐°\\c6E=-=-\\c6F@)⌐°\\c6E=-=-\\c6F@)\\c00\r\n\\x00\\x00\\c6F ⌡\\c6E-=-=\\c6F⌡‼ ⌡\\c6E-=-=\\c6F⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00",
            @"\c00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°@)\c00
\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)\c6E=-=-=-=-\c6F⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼\c6E-=-=-=-=\c6F ⌡\c00
\x00\x00\c6F⌐°@)\c6E=-=-=-=-\c6F⌐°@)\c00
\x00\x00\c6F ⌡⌡‼\c6E-=-=-=-=\c6F ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\c00" })]
		[DataRow("42 _", VTiles.Wall, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°\\c4F─┴┬─\\c6F@)⌐°\\c4F─┴┬─\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡\\c4F─┬┴─\\c6F⌡‼ ⌡\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c6F⌐°\\c4F─┴┬─\\c6F@)⌐°\\c4F─┴┬─\\c6F@)\\c00\r\n\\x00\\x00\\c6F ⌡\\c4F─┬┴─\\c6F⌡‼ ⌡\\c4F─┬┴─\\c6F⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00",
            @"\c00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°@)\c00
\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)\c4F─┴┬──┴┬─\c6F⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼\c4F─┬┴──┬┴─\c6F ⌡\c00
\x00\x00\c6F⌐°@)\c4F─┴┬──┴┬─\c6F⌐°@)\c00
\x00\x00\c6F ⌡⌡‼\c4F─┬┴──┬┴─\c6F ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\c00" })]
		[DataRow("43 _", VTiles.Dest, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°\\c0E ╓╖ \\c6F@)⌐°\\c0E ╓╖ \\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡\\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌡‼ ⌡\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c6F⌐°\\c0E ╓╖ \\c6F@)⌐°\\c0E ╓╖ \\c6F@)\\c00\r\n\\x00\\x00\\c6F ⌡\\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌡‼ ⌡\\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00",
            @"\c00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°@)\c00
\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)\c0E ╓╖  ╓╖ \c6F⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c6F ⌡\c00
\x00\x00\c6F⌐°@)\c0E ╓╖  ╓╖ \c6F⌐°@)\c00
\x00\x00\c6F ⌡⌡‼\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c6F ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\c00" })]
		[DataRow("44 _", VTiles.Player, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°⌐°@)@)⌐°⌐°@)\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡ ⌡⌡‼⌡‼ ⌡ ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F⌐°⌐°@)@)⌐°⌐°@)@)\\c00\r\n\\x00\\x00\\c6F ⌡ ⌡⌡‼⌡‼ ⌡ ⌡⌡‼⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00",
            @"\c00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°@)\c00
\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\c00
\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°@)\c00
\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\c00" })]
		[DataRow("45 _", VTiles.Boulder, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°\\c6E/¯¯\\\\\\c6F@)⌐°\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡\\c6E\\\\__/\\c6F⌡‼ ⌡\\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6F⌐°\\c6E/¯¯\\\\\\c6F@)⌐°\\c6E/¯¯\\\\\\c6F@)\\c00\r\n\\x00\\x00\\c6F ⌡\\c6E\\\\__/\\c6F⌡‼ ⌡\\c6E\\\\__/\\c6F⌡‼\\c00\r\n\\x00\\x00\\x00\\x00\\c6F⌐°@)⌐°@)⌐°@)⌐°\\c00\r\n\\x00\\x00\\x00\\x00\\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\\c00",
            @"\c00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°@)\c00
\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)\c6E/¯¯\\/¯¯\\\c6F⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼\c6E\\__/\\__/\c6F ⌡\c00
\x00\x00\c6F⌐°@)\c6E/¯¯\\/¯¯\\\c6F⌐°@)\c00
\x00\x00\c6F ⌡⌡‼\c6E\\__/\\__/\c6F ⌡⌡‼\c00
\x00\x00\x00\x00\c6F⌐°@)⌐°@)⌐°@)⌐°\c00
\x00\x00\x00\x00\c6F ⌡⌡‼ ⌡⌡‼ ⌡⌡‼ ⌡\c00" })]
		[DataRow("50 _", VTiles.zero, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯\\c00    \\c6E¯\\\\/¯\\c00    \r\n\\x00\\x00\\x00\\x00\\c6E\\\\_\\c00    \\c6E_/\\\\_\\c00    \r\n\\x00\\x00\\c6E/¯\\c00    \\c6E¯\\\\/¯\\c00    \\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\_\\c00    \\c6E_/\\\\_\\c00    \\c6E_/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00",
            @"\c00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯¯\\\c00
\x00\x00\c6E\\__/\\__/\\__/\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\\c00        \c6E/¯\c00
\x00\x00\x00\x00\c6E\\__/\c00        \c6E\\_\c00
\x00\x00\c6E/¯¯\\\c00        \c6E/¯¯\\\c00
\x00\x00\c6E\\__/\c00        \c6E\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯\c00
\x00\x00\x00\x00\c6E\\__/\\__/\\__/\\_\c00" })]
		[DataRow("51 _", VTiles.tile1, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯=-=-¯\\\\/¯=-=-\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\_-=-=_/\\\\_-=-=\\c00\r\n\\x00\\x00\\c6E/¯=-=-¯\\\\/¯=-=-¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\_-=-=_/\\\\_-=-=_/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00",
            @"\c00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯¯\\\c00
\x00\x00\c6E\\__/\\__/\\__/\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\=-=-=-=-/¯\c00
\x00\x00\x00\x00\c6E\\__/-=-=-=-=\\_\c00
\x00\x00\c6E/¯¯\\=-=-=-=-/¯¯\\\c00
\x00\x00\c6E\\__/-=-=-=-=\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯\c00
\x00\x00\x00\x00\c6E\\__/\\__/\\__/\\_\c00" })]
		[DataRow("52 _", VTiles.Wall, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯\\c4F─┴┬─\\c6E¯\\\\/¯\\c4F─┴┬─\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\_\\c4F─┬┴─\\c6E_/\\\\_\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c6E/¯\\c4F─┴┬─\\c6E¯\\\\/¯\\c4F─┴┬─\\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\_\\c4F─┬┴─\\c6E_/\\\\_\\c4F─┬┴─\\c6E_/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00",
            @"\c00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯¯\\\c00
\x00\x00\c6E\\__/\\__/\\__/\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\\c4F─┴┬──┴┬─\c6E/¯\c00
\x00\x00\x00\x00\c6E\\__/\c4F─┬┴──┬┴─\c6E\\_\c00
\x00\x00\c6E/¯¯\\\c4F─┴┬──┴┬─\c6E/¯¯\\\c00
\x00\x00\c6E\\__/\c4F─┬┴──┬┴─\c6E\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯\c00
\x00\x00\x00\x00\c6E\\__/\\__/\\__/\\_\c00" })]
		[DataRow("53 _", VTiles.Dest, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯\\c0E ╓╖ \\c6E¯\\\\/¯\\c0E ╓╖ \\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\_\\c2A▓\\c22░\\c02▒\\c22▓\\c6E_/\\\\_\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c6E/¯\\c0E ╓╖ \\c6E¯\\\\/¯\\c0E ╓╖ \\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\_\\c2A▓\\c22░\\c02▒\\c22▓\\c6E_/\\\\_\\c2A▓\\c22░\\c02▒\\c22▓\\c6E_/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00",
            @"\c00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯¯\\\c00
\x00\x00\c6E\\__/\\__/\\__/\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\\c0E ╓╖  ╓╖ \c6E/¯\c00
\x00\x00\x00\x00\c6E\\__/\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c6E\\_\c00
\x00\x00\c6E/¯¯\\\c0E ╓╖  ╓╖ \c6E/¯¯\\\c00
\x00\x00\c6E\\__/\c2A▓\c22░\c02▒\c22▓\c2A▓\c22░\c02▒\c22▓\c6E\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯\c00
\x00\x00\x00\x00\c6E\\__/\\__/\\__/\\_\c00" })]
		[DataRow("54 _", VTiles.Player, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯\\c6F⌐°@)\\c6E¯\\\\/¯\\c6F⌐°@)\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\_\\c6F ⌡⌡‼\\c6E_/\\\\_\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6E/¯\\c6F⌐°@)\\c6E¯\\\\/¯\\c6F⌐°@)\\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\_\\c6F ⌡⌡‼\\c6E_/\\\\_\\c6F ⌡⌡‼\\c6E_/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00",
            @"\c00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯¯\\\c00
\x00\x00\c6E\\__/\\__/\\__/\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\\c6F⌐°@)⌐°@)\c6E/¯\c00
\x00\x00\x00\x00\c6E\\__/\c6F ⌡⌡‼ ⌡⌡‼\c6E\\_\c00
\x00\x00\c6E/¯¯\\\c6F⌐°@)⌐°@)\c6E/¯¯\\\c00
\x00\x00\c6E\\__/\c6F ⌡⌡‼ ⌡⌡‼\c6E\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯\c00
\x00\x00\x00\x00\c6E\\__/\\__/\\__/\\_\c00" })]
		[DataRow("55 _", VTiles.Boulder, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\__/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯/¯¯\\\\¯\\\\/¯/¯¯\\\\\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\_\\\\__/_/\\\\_\\\\__/\\c00\r\n\\x00\\x00\\c6E/¯/¯¯\\\\¯\\\\/¯/¯¯\\\\¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\_\\\\__/_/\\\\_\\\\__/_/\\c00\r\n\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\/¯¯\\\\/¯¯\\\\/¯\\c00\r\n\\x00\\x00\\x00\\x00\\c6E\\\\__/\\\\__/\\\\__/\\\\_\\c00",
            @"\c00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯¯\\\c00
\x00\x00\c6E\\__/\\__/\\__/\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯\c00
\x00\x00\x00\x00\c6E\\__/\\__/\\__/\\_\c00
\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯¯\\\c00
\x00\x00\c6E\\__/\\__/\\__/\\__/\c00
\x00\x00\x00\x00\c6E/¯¯\\/¯¯\\/¯¯\\/¯\c00
\x00\x00\x00\x00\c6E\\__/\\__/\\__/\\_\c00" })]

		public void UpdateTest2(string name, VTiles vt, VTiles vt2, string[] sExp) {
            var tileDisplayHex = new TileDisplayHex(new Point(2, 0), new Size(4, 4), new TestTileDef42())
            {
                FncGetTile = (p) => vt2
            };
            tileDisplayHex.FullRedraw();
			Application_DoEvents();
			AssertAreEqual(cExpUpdateText2[(int)vt2], _tstCon?.Content??"");

			Thread_Sleep(10);

			tileDisplayHex.FncGetTile = (p) => xTst(p) ? vt : vt2;
			tileDisplayHex.FncOldPos = (p) => xTst(p) ? new Point(p.X * 3 - 3, p.Y) : p;
			tileDisplayHex.Update(true); //HalfStep
			Application_DoEvents();
			AssertAreEqual(sExp[0], _tstCon?.Content ??"", $"Test:{name}.HalfStep");

			Thread_Sleep(10);

			tileDisplayHex.Update(false); //FullStep
			Application_DoEvents();
			AssertAreEqual(sExp[1], _tstCon?.Content ?? "", $"Test:{name}.FullStep");

			Thread_Sleep(10);
			Application_DoEvents();

			console!.Clear();
			tileDisplayHex.FullRedraw();
			Application_DoEvents();
			AssertAreEqual(sExp[1],	_tstCon?.Content ?? "", $"Test:{name}.FullRedraw");

            static bool xTst(Point p) => p.X > 0 && p.Y > 0 && p.X < 3 && p.Y < 3;
		}

		[DataTestMethod()]
		[TestProperty("Author", "J.C.")]
		[DataRow("00 _", VTiles.zero, VTiles.zero, new string[] {
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00" ,
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00                \r\n\\x00\\x00"})]
		[DataRow("01 _", VTiles.tile1, VTiles.zero, new string[] {
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00  \\c6E=-=-\\c00    \\c6E=-=-\\c00  \r\n\\x00\\x00  \\c6E-=-=\\c00    \\c6E-=-=\\c00  \r\n\\x00\\x00  \\c6E=-=-\\c00    \\c6E=-=-\\c00  \r\n\\x00\\x00  \\c6E-=-=\\c00    \\c6E-=-=\\c00  \r\n\\x00\\x00                \r\n\\x00\\x00" ,
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00        \\c6E=-=-\\c00    \r\n\\x00\\x00    \\c6E=-=--=-=\\c00    \r\n\\x00\\x00    \\c6E-=-==-=-\\c00    \r\n\\x00\\x00    \\c6E=-=--=-=\\c00    \r\n\\x00\\x00    \\c6E-=-=\\c00        \r\n\\x00\\x00"})]
		[DataRow("02 _", VTiles.Wall, VTiles.zero, new string[] {
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00  \\c4F─┴┬─\\c00    \\c4F─┴┬─\\c00  \r\n\\x00\\x00  \\c4F─┬┴─\\c00    \\c4F─┬┴─\\c00  \r\n\\x00\\x00  \\c4F─┴┬─\\c00    \\c4F─┴┬─\\c00  \r\n\\x00\\x00  \\c4F─┬┴─\\c00    \\c4F─┬┴─\\c00  \r\n\\x00\\x00                \r\n\\x00\\x00" ,
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00        \\c4F─┴┬─\\c00    \r\n\\x00\\x00    \\c4F─┴┬──┬┴─\\c00    \r\n\\x00\\x00    \\c4F─┬┴──┴┬─\\c00    \r\n\\x00\\x00    \\c4F─┴┬──┬┴─\\c00    \r\n\\x00\\x00    \\c4F─┬┴─\\c00        \r\n\\x00\\x00"})]
		[DataRow("03 _", VTiles.Dest, VTiles.zero, new string[] {
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00  \\c0E ╓╖ \\c00    \\c0E ╓╖ \\c00  \r\n\\x00\\x00  \\c2A▓\\c22░\\c02▒\\c22▓\\c00    \\c2A▓\\c22░\\c02▒\\c22▓\\c00  \r\n\\x00\\x00  \\c0E ╓╖ \\c00    \\c0E ╓╖ \\c00  \r\n\\x00\\x00  \\c2A▓\\c22░\\c02▒\\c22▓\\c00    \\c2A▓\\c22░\\c02▒\\c22▓\\c00  \r\n\\x00\\x00                \r\n\\x00\\x00" ,
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00        \\c0E ╓╖ \\c00    \r\n\\x00\\x00    \\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00    \r\n\\x00\\x00    \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00    \r\n\\x00\\x00    \\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00    \r\n\\x00\\x00    \\c2A▓\\c22░\\c02▒\\c22▓\\c00        \r\n\\x00\\x00"})]
		[DataRow("04 _", VTiles.Player, VTiles.zero, new string[] {
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00  \\c6F⌐°@)\\c00    \\c6F⌐°@)\\c00  \r\n\\x00\\x00  \\c6F ⌡⌡‼\\c00    \\c6F ⌡⌡‼\\c00  \r\n\\x00\\x00  \\c6F⌐°@)\\c00    \\c6F⌐°@)\\c00  \r\n\\x00\\x00  \\c6F ⌡⌡‼\\c00    \\c6F ⌡⌡‼\\c00  \r\n\\x00\\x00                \r\n\\x00\\x00" ,
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00        \\c6F⌐°@)\\c00    \r\n\\x00\\x00    \\c6F⌐°@) ⌡⌡‼\\c00    \r\n\\x00\\x00    \\c6F ⌡⌡‼⌐°@)\\c00    \r\n\\x00\\x00    \\c6F⌐°@) ⌡⌡‼\\c00    \r\n\\x00\\x00    \\c6F ⌡⌡‼\\c00        \r\n\\x00\\x00"})]
		[DataRow("05 _", VTiles.Boulder, VTiles.zero, new string[] {
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00  \\c6E/¯¯\\\\\\c00    \\c6E/¯¯\\\\\\c00  \r\n\\x00\\x00  \\c6E\\\\__/\\c00    \\c6E\\\\__/\\c00  \r\n\\x00\\x00  \\c6E/¯¯\\\\\\c00    \\c6E/¯¯\\\\\\c00  \r\n\\x00\\x00  \\c6E\\\\__/\\c00    \\c6E\\\\__/\\c00  \r\n\\x00\\x00                \r\n\\x00\\x00" ,
			"\\c00\\x00\\x00    \\x00\\x00\\x00\\x00    \r\n\\x00\\x00                \r\n\\x00\\x00        \\c6E/¯¯\\\\\\c00    \r\n\\x00\\x00    \\c6E/¯¯\\\\\\\\__/\\c00    \r\n\\x00\\x00    \\c6E\\\\__//¯¯\\\\\\c00    \r\n\\x00\\x00    \\c6E/¯¯\\\\\\\\__/\\c00    \r\n\\x00\\x00    \\c6E\\\\__/\\c00        \r\n\\x00\\x00"})]
		[DataRow("10 _", VTiles.zero, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-\\c00    \\c6E-==-\\c00    \\c6E-=\\c00\r\n\\x00\\x00\\c6E-=\\c00    \\c6E=--=\\c00    \\c6E=-\\c00\r\n\\x00\\x00\\c6E=-\\c00    \\c6E-==-\\c00    \\c6E-=\\c00\r\n\\x00\\x00\\c6E-=\\c00    \\c6E=--=\\c00    \\c6E=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-=\\c00    \\c6E-=-=\\c00\r\n\\x00\\x00\\c6E-=-=\\c00        \\c6E=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\c00        \\c6E-=-=\\c00\r\n\\x00\\x00\\c6E-=-=\\c00        \\c6E=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\c00    \\c6E=-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00"})]
		[DataRow("11 _", VTiles.tile1, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=-=--==-=-=--=\\c00\r\n\\x00\\x00\\c6E-=-=-==--=-=-==-\\c00\r\n\\x00\\x00\\c6E=-=-=--==-=-=--=\\c00\r\n\\x00\\x00\\c6E-=-=-==--=-=-==-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00"})]
		[DataRow("12 _", VTiles.Wall, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-\\c4F─┴┬─\\c6E-==-\\c4F─┴┬─\\c6E-=\\c00\r\n\\x00\\x00\\c6E-=\\c4F─┬┴─\\c6E=--=\\c4F─┬┴─\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-\\c4F─┴┬─\\c6E-==-\\c4F─┴┬─\\c6E-=\\c00\r\n\\x00\\x00\\c6E-=\\c4F─┬┴─\\c6E=--=\\c4F─┬┴─\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-=\\c4F─┴┬─\\c6E-=-=\\c00\r\n\\x00\\x00\\c6E-=-=\\c4F─┴┬──┬┴─\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\c4F─┬┴──┴┬─\\c6E-=-=\\c00\r\n\\x00\\x00\\c6E-=-=\\c4F─┴┬──┬┴─\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\c4F─┬┴─\\c6E=-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00"})]
		[DataRow("13 _", VTiles.Dest, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-\\c0E ╓╖ \\c6E-==-\\c0E ╓╖ \\c6E-=\\c00\r\n\\x00\\x00\\c6E-=\\c2A▓\\c22░\\c02▒\\c22▓\\c6E=--=\\c2A▓\\c22░\\c02▒\\c22▓\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-\\c0E ╓╖ \\c6E-==-\\c0E ╓╖ \\c6E-=\\c00\r\n\\x00\\x00\\c6E-=\\c2A▓\\c22░\\c02▒\\c22▓\\c6E=--=\\c2A▓\\c22░\\c02▒\\c22▓\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-=\\c0E ╓╖ \\c6E-=-=\\c00\r\n\\x00\\x00\\c6E-=-=\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c6E-=-=\\c00\r\n\\x00\\x00\\c6E-=-=\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\c2A▓\\c22░\\c02▒\\c22▓\\c6E=-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00"})]
		[DataRow("14 _", VTiles.Player, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-\\c6F⌐°@)\\c6E-==-\\c6F⌐°@)\\c6E-=\\c00\r\n\\x00\\x00\\c6E-=\\c6F ⌡⌡‼\\c6E=--=\\c6F ⌡⌡‼\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-\\c6F⌐°@)\\c6E-==-\\c6F⌐°@)\\c6E-=\\c00\r\n\\x00\\x00\\c6E-=\\c6F ⌡⌡‼\\c6E=--=\\c6F ⌡⌡‼\\c6E=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-=\\c6F⌐°@)\\c6E-=-=\\c00\r\n\\x00\\x00\\c6E-=-=\\c6F⌐°@) ⌡⌡‼\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\c6F ⌡⌡‼⌐°@)\\c6E-=-=\\c00\r\n\\x00\\x00\\c6E-=-=\\c6F⌐°@) ⌡⌡‼\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\c6F ⌡⌡‼\\c6E=-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00"})]
		[DataRow("15 _", VTiles.Boulder, VTiles.tile1, new string[] {
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-/¯¯\\\\-==-/¯¯\\\\-=\\c00\r\n\\x00\\x00\\c6E-=\\\\__/=--=\\\\__/=-\\c00\r\n\\x00\\x00\\c6E=-/¯¯\\\\-==-/¯¯\\\\-=\\c00\r\n\\x00\\x00\\c6E-=\\\\__/=--=\\\\__/=-\\c00\r\n\\x00\\x00\\c6E=-=--=-==-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00" ,
			"\\c00\\x00\\x00\\c6E=-=-\\c00\\x00\\x00\\x00\\x00\\c6E=-=-\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00\r\n\\x00\\x00\\c6E=-=--=-=/¯¯\\\\-=-=\\c00\r\n\\x00\\x00\\c6E-=-=/¯¯\\\\\\\\__/=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\\\__//¯¯\\\\-=-=\\c00\r\n\\x00\\x00\\c6E-=-=/¯¯\\\\\\\\__/=-=-\\c00\r\n\\x00\\x00\\c6E=-=-\\\\__/=-=--=-=\\c00\r\n\\x00\\x00\\c6E-=-==-=--=-==-=-\\c00"})]
		[DataRow("20 _", VTiles.zero, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c00    \\c4F┴──┴\\c00    \\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c00    \\c4F┬──┬\\c00    \\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c00    \\c4F┴──┴\\c00    \\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c00    \\c4F┬──┬\\c00    \\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00" ,
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴─\\c00    \\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c00        \\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c00        \\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c00        \\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c00    \\c4F─┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00"})]
		[DataRow("21 _", VTiles.tile1, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c6E=-=-\\c4F┴──┴\\c6E=-=-\\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c6E-=-=\\c4F┬──┬\\c6E-=-=\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c6E=-=-\\c4F┴──┴\\c6E=-=-\\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c6E-=-=\\c4F┬──┬\\c6E-=-=\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00",
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴─\\c6E=-=-\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c6E=-=--=-=\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c6E-=-==-=-\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c6E=-=--=-=\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c6E-=-=\\c4F─┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00" })]
		[DataRow("22 _", VTiles.Wall, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴─┴┬─┴──┴─┴┬─┴─\\c00\r\n\\x00\\x00\\c4F─┬─┬┴─┬──┬─┬┴─┬─\\c00\r\n\\x00\\x00\\c4F─┴─┴┬─┴──┴─┴┬─┴─\\c00\r\n\\x00\\x00\\c4F─┬─┬┴─┬──┬─┬┴─┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00",
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00" })]
		[DataRow("23 _", VTiles.Dest, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c0E ╓╖ \\c4F┴──┴\\c0E ╓╖ \\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c2A▓\\c22░\\c02▒\\c22▓\\c4F┬──┬\\c2A▓\\c22░\\c02▒\\c22▓\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c0E ╓╖ \\c4F┴──┴\\c0E ╓╖ \\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c2A▓\\c22░\\c02▒\\c22▓\\c4F┬──┬\\c2A▓\\c22░\\c02▒\\c22▓\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00",
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴─\\c0E ╓╖ \\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c2A▓\\c22░\\c02▒\\c22▓\\c4F─┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00" })]
		[DataRow("24 _", VTiles.Player, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c6F⌐°@)\\c4F┴──┴\\c6F⌐°@)\\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c6F ⌡⌡‼\\c4F┬──┬\\c6F ⌡⌡‼\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c6F⌐°@)\\c4F┴──┴\\c6F⌐°@)\\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c6F ⌡⌡‼\\c4F┬──┬\\c6F ⌡⌡‼\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00",
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴─\\c6F⌐°@)\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c6F⌐°@) ⌡⌡‼\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c6F ⌡⌡‼⌐°@)\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c6F⌐°@) ⌡⌡‼\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c6F ⌡⌡‼\\c4F─┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00" })]
		[DataRow("25 _", VTiles.Boulder, VTiles.Wall, new string[] {
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c6E/¯¯\\\\\\c4F┴──┴\\c6E/¯¯\\\\\\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c6E\\\\__/\\c4F┬──┬\\c6E\\\\__/\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴\\c6E/¯¯\\\\\\c4F┴──┴\\c6E/¯¯\\\\\\c4F┴─\\c00\r\n\\x00\\x00\\c4F─┬\\c6E\\\\__/\\c4F┬──┬\\c6E\\\\__/\\c4F┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴──┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00",
			"\\c00\\x00\\x00\\c4F─┴┬─\\c00\\x00\\x00\\x00\\x00\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬──┬┴─\\c6E/¯¯\\\\\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c6E/¯¯\\\\\\\\__/\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c6E\\\\__//¯¯\\\\\\c4F─┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴─\\c6E/¯¯\\\\\\\\__/\\c4F─┴┬─\\c00\r\n\\x00\\x00\\c4F─┴┬─\\c6E\\\\__/\\c4F─┴┬──┬┴─\\c00\r\n\\x00\\x00\\c4F─┬┴──┴┬──┬┴──┴┬─\\c00" })]
		[DataRow("30 _", VTiles.zero, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c00    \\c02▒\\c22▓\\c0E ╓\\c00    \\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c00    \\c0E╖ \\c2A▓\\c22░\\c00    \\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c00    \\c02▒\\c22▓\\c0E ╓\\c00    \\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c00    \\c0E╖ \\c2A▓\\c22░\\c00    \\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00",
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00    \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c00        \\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c00        \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c00        \\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c00    \\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00" })]
		[DataRow("31 _", VTiles.tile1, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c6E=-=-\\c02▒\\c22▓\\c0E ╓\\c6E=-=-\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6E-=-=\\c0E╖ \\c2A▓\\c22░\\c6E-=-=\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c6E=-=-\\c02▒\\c22▓\\c0E ╓\\c6E=-=-\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6E-=-=\\c0E╖ \\c2A▓\\c22░\\c6E-=-=\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00",
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6E=-=-\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c6E=-=--=-=\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c6E-=-==-=-\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c6E=-=--=-=\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c6E-=-=\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00" })]
		[DataRow("32 _", VTiles.Wall, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c4F─┴┬─\\c02▒\\c22▓\\c0E ╓\\c4F─┴┬─\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c4F─┬┴─\\c0E╖ \\c2A▓\\c22░\\c4F─┬┴─\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c4F─┴┬─\\c02▒\\c22▓\\c0E ╓\\c4F─┴┬─\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c4F─┬┴─\\c0E╖ \\c2A▓\\c22░\\c4F─┬┴─\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00",
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c4F─┴┬─\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c4F─┴┬──┬┴─\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c4F─┬┴──┴┬─\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c4F─┴┬──┬┴─\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c4F─┬┴─\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00" })]
		[DataRow("33 _", VTiles.Dest, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓ ╓╖ \\c02▒\\c22▓\\c0E ╓ ╓╖ \\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c2A▓\\c22░\\c02▒\\c22▓\\c0E╖ \\c2A▓\\c22░\\c2A▓\\c22░\\c02▒\\c22▓\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓ ╓╖ \\c02▒\\c22▓\\c0E ╓ ╓╖ \\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c2A▓\\c22░\\c02▒\\c22▓\\c0E╖ \\c2A▓\\c22░\\c2A▓\\c22░\\c02▒\\c22▓\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00",
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00" })]
		[DataRow("34 _", VTiles.Player, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c6F⌐°@)\\c02▒\\c22▓\\c0E ╓\\c6F⌐°@)\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6F ⌡⌡‼\\c0E╖ \\c2A▓\\c22░\\c6F ⌡⌡‼\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c6F⌐°@)\\c02▒\\c22▓\\c0E ╓\\c6F⌐°@)\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6F ⌡⌡‼\\c0E╖ \\c2A▓\\c22░\\c6F ⌡⌡‼\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00",
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌐°@)\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌐°@) ⌡⌡‼\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c6F ⌡⌡‼⌐°@)\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌐°@) ⌡⌡‼\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c6F ⌡⌡‼\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00" })]
		[DataRow("35 _", VTiles.Boulder, VTiles.Dest, new string[] {
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c6E/¯¯\\\\\\c02▒\\c22▓\\c0E ╓\\c6E/¯¯\\\\\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6E\\\\__/\\c0E╖ \\c2A▓\\c22░\\c6E\\\\__/\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓\\c6E/¯¯\\\\\\c02▒\\c22▓\\c0E ╓\\c6E/¯¯\\\\\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c6E\\\\__/\\c0E╖ \\c2A▓\\c22░\\c6E\\\\__/\\c0E╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00",
			"\\c00\\x00\\x00\\c0E ╓╖ \\c00\\x00\\x00\\x00\\x00\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6E/¯¯\\\\\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c6E/¯¯\\\\\\\\__/\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c6E\\\\__//¯¯\\\\\\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c6E/¯¯\\\\\\\\__/\\c0E ╓╖ \\c00\r\n\\x00\\x00\\c0E ╓╖ \\c6E\\\\__/\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c00\r\n\\x00\\x00\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c00" })]
		[DataRow("40 _", VTiles.zero, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c00    \\c6F⌡‼⌐°\\c00    \\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c00    \\c6F@) ⌡\\c00    \\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c00    \\c6F⌡‼⌐°\\c00    \\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c00    \\c6F@) ⌡\\c00    \\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00",
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼\\c00    \\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c00        \\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c00        \\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c00        \\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c00    \\c6F⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00" })]
		[DataRow("41 _", VTiles.tile1, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c6E=-=-\\c6F⌡‼⌐°\\c6E=-=-\\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c6E-=-=\\c6F@) ⌡\\c6E-=-=\\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c6E=-=-\\c6F⌡‼⌐°\\c6E=-=-\\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c6E-=-=\\c6F@) ⌡\\c6E-=-=\\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00",
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼\\c6E=-=-\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c6E=-=--=-=\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c6E-=-==-=-\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c6E=-=--=-=\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c6E-=-=\\c6F⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00" })]
		[DataRow("42 _", VTiles.Wall, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c4F─┴┬─\\c6F⌡‼⌐°\\c4F─┴┬─\\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c4F─┬┴─\\c6F@) ⌡\\c4F─┬┴─\\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c4F─┴┬─\\c6F⌡‼⌐°\\c4F─┴┬─\\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c4F─┬┴─\\c6F@) ⌡\\c4F─┬┴─\\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00",
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼\\c4F─┴┬─\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c4F─┴┬──┬┴─\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c4F─┬┴──┴┬─\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c4F─┴┬──┬┴─\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c4F─┬┴─\\c6F⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00" })]
		[DataRow("43 _", VTiles.Dest, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c0E ╓╖ \\c6F⌡‼⌐°\\c0E ╓╖ \\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c2A▓\\c22░\\c02▒\\c22▓\\c6F@) ⌡\\c2A▓\\c22░\\c02▒\\c22▓\\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c0E ╓╖ \\c6F⌡‼⌐°\\c0E ╓╖ \\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c2A▓\\c22░\\c02▒\\c22▓\\c6F@) ⌡\\c2A▓\\c22░\\c02▒\\c22▓\\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00",
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼\\c0E ╓╖ \\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c2A▓\\c22░\\c02▒\\c22▓\\c6F⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00" })]
		[DataRow("44 _", VTiles.Player, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°⌐°@)⌡‼⌐°⌐°@)⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡ ⌡⌡‼@) ⌡ ⌡⌡‼@)\\c00\r\n\\x00\\x00\\c6F⌐°⌐°@)⌡‼⌐°⌐°@)⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡ ⌡⌡‼@) ⌡ ⌡⌡‼@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00",
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00" })]
		[DataRow("45 _", VTiles.Boulder, VTiles.Player, new string[] {
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c6E/¯¯\\\\\\c6F⌡‼⌐°\\c6E/¯¯\\\\\\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c6E\\\\__/\\c6F@) ⌡\\c6E\\\\__/\\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°\\c6E/¯¯\\\\\\c6F⌡‼⌐°\\c6E/¯¯\\\\\\c6F⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡\\c6E\\\\__/\\c6F@) ⌡\\c6E\\\\__/\\c6F@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00",
			"\\c00\\x00\\x00\\c6F⌐°@)\\c00\\x00\\x00\\x00\\x00\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@) ⌡⌡‼\\c6E/¯¯\\\\\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c6E/¯¯\\\\\\\\__/\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c6E\\\\__//¯¯\\\\\\c6F ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼\\c6E/¯¯\\\\\\\\__/\\c6F⌐°@)\\c00\r\n\\x00\\x00\\c6F⌐°@)\\c6E\\\\__/\\c6F⌐°@) ⌡⌡‼\\c00\r\n\\x00\\x00\\c6F ⌡⌡‼⌐°@) ⌡⌡‼⌐°@)\\c00" })]
		[DataRow("50 _", VTiles.zero, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯\\c00    \\c6E_//¯\\c00    \\c6E_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\c00    \\c6E¯\\\\\\\\_\\c00    \\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯\\c00    \\c6E_//¯\\c00    \\c6E_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\c00    \\c6E¯\\\\\\\\_\\c00    \\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00",
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__/\\c00    \\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/\\c00        \\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c00        \\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/\\c00        \\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c00    \\c6E/¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00" })]
		[DataRow("51 _", VTiles.tile1, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯=-=-_//¯=-=-_/\\c00\r\n\\x00\\x00\\c6E\\\\_-=-=¯\\\\\\\\_-=-=¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯=-=-_//¯=-=-_/\\c00\r\n\\x00\\x00\\c6E\\\\_-=-=¯\\\\\\\\_-=-=¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00",
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__/=-=-\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/=-=--=-=/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\-=-==-=-\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/=-=--=-=/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\-=-=/¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00" })]
		[DataRow("52 _", VTiles.Wall, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯\\c4F─┴┬─\\c6E_//¯\\c4F─┴┬─\\c6E_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\c4F─┬┴─\\c6E¯\\\\\\\\_\\c4F─┬┴─\\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯\\c4F─┴┬─\\c6E_//¯\\c4F─┴┬─\\c6E_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\c4F─┬┴─\\c6E¯\\\\\\\\_\\c4F─┬┴─\\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00",
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__/\\c4F─┴┬─\\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/\\c4F─┴┬──┬┴─\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c4F─┬┴──┴┬─\\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/\\c4F─┴┬──┬┴─\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c4F─┬┴─\\c6E/¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00" })]
		[DataRow("53 _", VTiles.Dest, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯\\c0E ╓╖ \\c6E_//¯\\c0E ╓╖ \\c6E_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\c2A▓\\c22░\\c02▒\\c22▓\\c6E¯\\\\\\\\_\\c2A▓\\c22░\\c02▒\\c22▓\\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯\\c0E ╓╖ \\c6E_//¯\\c0E ╓╖ \\c6E_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\c2A▓\\c22░\\c02▒\\c22▓\\c6E¯\\\\\\\\_\\c2A▓\\c22░\\c02▒\\c22▓\\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00",
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__/\\c0E ╓╖ \\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c2A▓\\c22░\\c02▒\\c22▓\\c0E ╓╖ \\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/\\c0E ╓╖ \\c2A▓\\c22░\\c02▒\\c22▓\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c2A▓\\c22░\\c02▒\\c22▓\\c6E/¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00" })]
		[DataRow("54 _", VTiles.Player, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯\\c6F⌐°@)\\c6E_//¯\\c6F⌐°@)\\c6E_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\c6F ⌡⌡‼\\c6E¯\\\\\\\\_\\c6F ⌡⌡‼\\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯\\c6F⌐°@)\\c6E_//¯\\c6F⌐°@)\\c6E_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\c6F ⌡⌡‼\\c6E¯\\\\\\\\_\\c6F ⌡⌡‼\\c6E¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00",
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__/\\c6F⌐°@)\\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/\\c6F⌐°@) ⌡⌡‼\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c6F ⌡⌡‼⌐°@)\\c6E\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__/\\c6F⌐°@) ⌡⌡‼\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\c6F ⌡⌡‼\\c6E/¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00" })]
		[DataRow("55 _", VTiles.Boulder, VTiles.Boulder, new string[] {
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯/¯¯\\\\_//¯/¯¯\\\\_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\\\__/¯\\\\\\\\_\\\\__/¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯/¯¯\\\\_//¯/¯¯\\\\_/\\c00\r\n\\x00\\x00\\c6E\\\\_\\\\__/¯\\\\\\\\_\\\\__/¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00",
			"\\c00\\x00\\x00\\c6E/¯¯\\\\\\c00\\x00\\x00\\x00\\x00\\c6E/¯¯\\\\\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00\r\n\\x00\\x00\\c6E/¯¯\\\\\\\\__//¯¯\\\\\\\\__/\\c00\r\n\\x00\\x00\\c6E\\\\__//¯¯\\\\\\\\__//¯¯\\\\\\c00" })]

		public void UpdateTest3(string name, VTiles vt, VTiles vt2, string[] sExp) {
            var tileDisplayHex = new TileDisplayHex(new Point(2, 0), new Size(4, 4), new TestTileDef42(), true)
            {
                FncGetTile = (p) => vt2
            };
            tileDisplayHex.FullRedraw();
			Application_DoEvents();
			AssertAreEqual(cExpUpdateText3[(int)vt2], _tstCon?.Content ?? "");

			Thread_Sleep(10);

			tileDisplayHex.FncGetTile = (p) => xTst(p) ? vt : vt2;
			tileDisplayHex.FncOldPos = (p) => xTst(p) ? new Point(p.X * 3 - 3, p.Y) : p;
			tileDisplayHex.Update(true); //HalfStep
			Application_DoEvents();
			AssertAreEqual(sExp[0], _tstCon?.Content ?? "", $"Test:{name}.HalfStep");

			Thread_Sleep(10);

			tileDisplayHex.Update(false); //FullStep
			Application_DoEvents();
			AssertAreEqual(sExp[1], _tstCon?.Content ??"", $"Test:{name}.FullStep");

			Thread_Sleep(10);
			Application_DoEvents();

			console!.Clear();
			tileDisplayHex.FullRedraw();
			Application_DoEvents();
			AssertAreEqual(sExp[1], _tstCon?.Content ?? "", $"Test:{name}.FullRedraw");

            static bool xTst(Point p) => p.X > 0 && p.Y > 0 && p.X < 3 && p.Y < 3;
		}
	}
}
