using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tetris_Base.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleDisplay.View;
using TestConsole;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Tetris_Base.Model.Tests
{
    /// <summary>
    /// Defines test class BlockTests.
    /// </summary>
    [TestClass()]
    public class BlockTests
    {
        static MyConsoleBase? console = new TstConsole();
        const string cExpHideBlock = "ConsoleDisplay.View.Display.(5;6),000000000000000000000000000000";
        static readonly string[] cExpShowBlock = {
            "ConsoleDisplay.View.Display.(5;6),000000090000900009000090000000","ConsoleDisplay.View.Display.(5;6),000000000009999000000000000000",
            "ConsoleDisplay.View.Display.(5;6),000000090000900009000090000000","ConsoleDisplay.View.Display.(5;6),000000000009999000000000000000",
            "ConsoleDisplay.View.Display.(5;6),0000000D0000D000DD000000000000","ConsoleDisplay.View.Display.(5;6),00000000000DDD0000D00000000000",
            "ConsoleDisplay.View.Display.(5;6),0000000DD000D0000D000000000000","ConsoleDisplay.View.Display.(5;6),000000D0000DDD0000000000000000",
            "ConsoleDisplay.View.Display.(5;6),0000000A0000A0000AA00000000000","ConsoleDisplay.View.Display.(5;6),00000000A00AAA0000000000000000",
            "ConsoleDisplay.View.Display.(5;6),000000AA0000A0000A000000000000","ConsoleDisplay.View.Display.(5;6),00000000000AAA00A0000000000000",
            "ConsoleDisplay.View.Display.(5;6),000000EE000EE00000000000000000","ConsoleDisplay.View.Display.(5;6),000000EE000EE00000000000000000",
            "ConsoleDisplay.View.Display.(5;6),000000EE000EE00000000000000000","ConsoleDisplay.View.Display.(5;6),000000EE000EE00000000000000000",
            "ConsoleDisplay.View.Display.(5;6),0000000B000BB000B0000000000000","ConsoleDisplay.View.Display.(5;6),00000000000BB0000BB00000000000",
            "ConsoleDisplay.View.Display.(5;6),0000000B000BB000B0000000000000","ConsoleDisplay.View.Display.(5;6),00000000000BB0000BB00000000000",
            "ConsoleDisplay.View.Display.(5;6),000000040000440000400000000000","ConsoleDisplay.View.Display.(5;6),000000044004400000000000000000",
            "ConsoleDisplay.View.Display.(5;6),000000040000440000400000000000","ConsoleDisplay.View.Display.(5;6),000000044004400000000000000000",
            "ConsoleDisplay.View.Display.(5;6),0000000C0000CC000C000000000000","ConsoleDisplay.View.Display.(5;6),0000000C000CCC0000000000000000",
            "ConsoleDisplay.View.Display.(5;6),0000000C000CC0000C000000000000","ConsoleDisplay.View.Display.(5;6),00000000000CCC000C000000000000"};
        static readonly int[] cExpCnt = { 0, 6, 6, 6, 0, 6, 6, 6, 0, 6, 6, 6, 0, 0, 0, 0, 0, 4, 4, 4, 0, 4, 4, 4, 0, 2, 2, 2 };

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        [TestInitialize()]
        public void Init()
        {
            if (console == null) Assert.Fail("console not initialized");
            console.Clear();
            Display.myConsole = console;
            Application.DoEvents();
        }



        /// <summary>
        /// Defines the test method ShowTest.
        /// </summary>
        [TestMethod()]
        public void ShowTest()
        {
            if (console == null) Assert.Fail("console not initialized");
            foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                {
                    var d = new Display((int)bt * 6, (int)ba * 4, 5, 6);
                    var b = new Block(bt, ba);
                    b.prTestPixel = (o) => false;
                    b.acPaint = (p, c) => d.PutPixel(p.X + 2, p.Y + 2, c);
                    b.Show();
                    d.Update();
                    console.SetCursorPosition((int)bt * 6, (int)ba * 4 + 3);
                    console.Write($"{bt}-{ba.ToString()[4..]}");
                    Application.DoEvents();
                    Assert.AreEqual(cExpShowBlock[(int)bt * 4 + (int)ba], d.ToString());
                }
            Thread.Sleep(500);

        }

        /// <summary>
        /// Defines the test method HideTest.
        /// </summary>
        [TestMethod()]
        public void HideTest()
        {
            if (console == null) Assert.Fail("console not initialized");
            Display[] d = new Display[7 * 4];
            Block[] b = new Block[7 * 4];
            foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                {
                    var Ix = (int)bt * 4 + (int)ba;
                    d[Ix] = new Display((int)bt * 6, (int)ba * 4, 5, 6);
                    b[Ix] = new Block(bt, ba);
                    console.SetCursorPosition((int)bt * 6, (int)ba * 4 + 3);
                    console.Write($"{bt}-{ba.ToString()[4..]}");
                    b[Ix].prTestPixel = (o) => false;
                    b[Ix].acPaint = (p, c) => d[Ix].PutPixel(p.X + 2, p.Y + 2, c);
                    b[Ix].Show();
                    d[Ix].Update();
                    Application.DoEvents();
                    Assert.AreEqual(cExpShowBlock[Ix], d[Ix].ToString());
                }
            foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                {
                    var Ix = (int)bt * 4 + (int)ba;
                    b[Ix].Hide();
                    d[Ix].Update();
                    Application.DoEvents();
                    Assert.AreEqual(cExpHideBlock, d[Ix].ToString());
                }
            Thread.Sleep(500);
        }

        /// <summary>
        /// Defines the test method RotateTest.
        /// </summary>
        [TestMethod()]
        public void RotateTest()
        {
            if (console == null) Assert.Fail("console not initialized");
            Display[] d = new Display[7];
            Block[] b = new Block[7];
            var cnt = 0;
            foreach (BlockType bt in typeof(BlockType).GetEnumValues())
            {
                var ba = BlockAngle.Degr0;
                var Ix = (int)bt;
                d[Ix] = new Display((int)bt * 6, 1, 5, 6);
                b[Ix] = new Block(bt);
                console.SetCursorPosition((int)bt * 6, 3);
                console.Write($"{bt}-{ba.ToString()[4..]}");
                b[Ix].prTestPixel = (o) => false;
                b[Ix].acPaint = (p, c) =>
                {
                    d[Ix].PutPixel(p.X + 2, p.Y + 2, c);
                    cnt++;
                };
                cnt = 0;
                b[Ix].Show();
                d[Ix].Update();
                Application.DoEvents();
                Assert.AreEqual(4, cnt);
                Assert.AreEqual(cExpShowBlock[Ix * 4], d[Ix].ToString());
            }
            foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
            {
                foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                {
                    var Ix = (int)bt;
                    console.SetCursorPosition((int)bt * 6, 3);
                    console.Write($"{bt}-{ba.ToString()[4..]}  ");
                    cnt = 0;
                    b[Ix].ActBlockAngle = ba;
                    d[Ix].Update();
                    Application.DoEvents();
                    Assert.AreEqual(cExpCnt[Ix * 4 + (int)ba], cnt);
                    Assert.AreEqual(cExpShowBlock[Ix * 4 + (int)ba], d[Ix].ToString());
                }
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Defines the test method MoveTest.
        /// </summary>
        [TestMethod()]
        public void MoveTest()
        {
            if (console == null) Assert.Fail("console not initialized");
            int[] Off = new int[] { 0, -1, -1, -1, 0, 1, 1, 1 };
            var cnt = 0;
            Display[] d = new Display[7 * 4];
            Block[] b = new Block[7 * 4];
            foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                {
                    var Ix = (int)bt + (int)ba * 7;
                    d[Ix] = new Display((int)bt * 6, (int)ba * 4, 5, 6);
                    b[Ix] = new Block(bt, ba);
                    b[Ix].Position = new Point(Off[Ix % 8], Off[(Ix + 2) % 8]);
                    console.SetCursorPosition((int)bt * 6, (int)ba * 4 + 3);
                    console.Write($"{bt}-{ba.ToString()[4..]}");
                    b[Ix].prTestPixel = (o) => false;
                    b[Ix].acPaint = (p, c) =>
                    {
                        d[Ix].PutPixel(p.X + 2, p.Y + 2, c);
                        cnt++;
                    };
                    cnt = 0;
                    b[Ix].Show();
                    d[Ix].Update();
                    Application.DoEvents();
                    Assert.AreEqual(4, cnt);
                    Assert.AreNotEqual(cExpShowBlock[(int)bt * 4 + (int)ba], d[Ix].ToString());
                }
            Thread.Sleep(500);
            foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                {
                    var Ix = (int)bt + (int)ba * 7;
                    cnt = 0;
                    b[Ix].Position = Point.Empty;
                    d[Ix].Update();
                    Application.DoEvents();
                    // Assert.AreEqual(cExpCntM[Ix * 4 + (int)ba], cnt);
                    Assert.AreEqual(cExpShowBlock[(int)bt * 4 + (int)ba], d[Ix].ToString());
                }
            Thread.Sleep(500);

        }

        /// <summary>
        /// Defines the test method CollisionTestTest.
        /// </summary>
        [TestMethod()]
        public void CollisionTestTest()
        {
            if (console == null) Assert.Fail("console not initialized");
            int[] Off = new int[] { 0, -1, -1, -1, 0, 1, 1, 1 };
            Point[] Dlt = new Point[7 * 4];
            var cnt = 0;
            Display[] d = new Display[7 * 4];
            Block[] b = new Block[7 * 4];
            foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                {
                    var Ix = (int)bt + (int)ba * 7;
                    d[Ix] = new Display((int)bt * 6, (int)ba * 4, 5, 6);
                    d[Ix].DefaultOutsideColor = ConsoleColor.White;
                    b[Ix] = new Block(bt, ba);
                    Dlt[Ix] = new Point(Off[Ix % 8], Off[(Ix + 2) % 8]);
                    console.SetCursorPosition((int)bt * 6, (int)ba * 4 + 3);
                    console.Write($"{bt}-{ba.ToString()[4..]}");
                    b[Ix].prTestPixel = (o) =>
                    {
                        cnt++;
                        return d[Ix].GetPixel(o.X+2, o.Y+2) != ConsoleColor.Black;
                    };
                    b[Ix].acPaint = (p, c) =>
                    {
                        d[Ix].PutPixel(p.X + 2, p.Y + 2, c);
                    };
                    cnt = 0;
                    b[Ix].Show();
                    d[Ix].Update();
                    Application.DoEvents();
                  //  Assert.AreEqual(4, cnt);
                    Assert.AreEqual(cExpShowBlock[(int)bt * 4 + (int)ba], d[Ix].ToString());
                }
            var rnd = new Random();
            for (int i = 0; i < 100; i++)
            {
                foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                    foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                    {
                        var Ix = (int)bt + (int)ba * 7;
                        var tcnt = 0;
                        cnt = 0;
                        if (!b[Ix].CollisionTest(Dlt[Ix], b[Ix].ActBlockAngle))
                        {
                            b[Ix].Position = new Point(b[Ix].Position.X + Dlt[Ix].X, b[Ix].Position.Y + Dlt[Ix].Y);
                            Assert.IsTrue(cnt <= 4, $"{bt}-{ba.ToString()[4..]},cnt({cnt}) <= 4");
                        }
                        else do
                            {
                                Assert.IsTrue(cnt <= 4, $"{bt}-{ba.ToString()[4..]},cnt <= 4");
                                cnt = 0;
                                var nDx = rnd.Next(0, 8);
                                Dlt[Ix] = new Point(Off[nDx % 8], Off[(nDx + 2) % 8]);
                            } while (b[Ix].CollisionTest(Dlt[Ix], b[Ix].ActBlockAngle) && tcnt++ < 16);

                        d[Ix].Update();
                        Assert.IsTrue(b[Ix].Position.X >= -2, $"{bt}-{ba.ToString()[4..]},Pos.X >= -2");
                        Assert.IsTrue(b[Ix].Position.Y >= -2, $"{bt}-{ba.ToString()[4..]},Pos.Y >= -2");
                        Assert.IsTrue(b[Ix].Position.X < 3, $"{bt}-{ba.ToString()[4..]},Pos.X < 3");
                        Assert.IsTrue(b[Ix].Position.Y < 4, $"{bt}-{ba.ToString()[4..]},Pos.Y > 3");
                    }
                Application.DoEvents();
            }
        }

        /// <summary>
        /// Defines the test method CollisionTestTest2.
        /// </summary>
        [TestMethod()]
        public void CollisionTestTest2()
        {
            int[] Off = new int[] { 0, -1, -1, -1, 0, 1, 1, 1 };
            Point[] Dlt = new Point[7 * 4];
            var cnt = 0;
            var d = new Display(1,1,25,20);
            d.DefaultOutsideColor = ConsoleColor.White;

            Block[] b = new Block[7 * 4];
            foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                {
                    var Ix = (int)bt + (int)ba * 7;
                    b[Ix] = new Block(bt, ba);
                    b[Ix].Position = new Point((Ix % 6) * 4, (Ix / 6) * 4);
                    Dlt[Ix] = new Point(Off[Ix % 8], Off[(Ix + 2) % 8]);
                    b[Ix].prTestPixel = (o) => d.GetPixel(o.X + 2, o.Y + 2) != ConsoleColor.Black;
                    b[Ix].acPaint = (p, c) =>
                    {
                        if (c != ConsoleColor.Black && d.GetPixel(p.X + 2, p.Y + 2) != ConsoleColor.Black) cnt++;
                        d.PutPixel(p.X + 2, p.Y + 2, c);
                    };
                    cnt = 0;
                    b[Ix].Show();
                    d.Update();
                    Application.DoEvents();
                    Assert.AreEqual(0, cnt);
                    //  Assert.AreEqual(cExpShowBlock[(int)bt * 4 + (int)ba], d.ToString());
                }
            var rnd = new Random();
            for (int i = 0; i < 300; i++)
            {
                foreach (BlockType bt in typeof(BlockType).GetEnumValues())
                    foreach (BlockAngle ba in typeof(BlockAngle).GetEnumValues())
                    {
                        var Ix = (int)bt + (int)ba * 7;
                        var tcnt = 0;
                        if (!b[Ix].CollisionTest(Dlt[Ix], b[Ix].ActBlockAngle))
                        {
                            b[Ix].Position = new Point(b[Ix].Position.X + Dlt[Ix].X, b[Ix].Position.Y + Dlt[Ix].Y);
                        }
                        else
                            while (b[Ix].CollisionTest(Dlt[Ix], b[Ix].ActBlockAngle) && tcnt++ < 16)
                            {
                                var nDx = rnd.Next(0, 8);
                                Dlt[Ix] = new Point(Off[nDx % 8], Off[(nDx + 2) % 8]);
                            }
                        d.Update();
                    }
                Assert.AreEqual(0, cnt);
                Application.DoEvents();
            }
        }

    }
}