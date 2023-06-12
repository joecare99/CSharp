using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sokoban_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseLib.Helper.TestHelper;

namespace Sokoban_Base.Model.Tests
{
    [TestClass()]
    public class PlayerTests
    {
        Player testItem;
        Playfield pf = new Playfield();

        [TestInitialize]
        public void Init()
        {
            pf.Setup(new string[] { "###", ".$ ","$  " });
            testItem = new(null);
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testItem);
            Assert.IsNotNull(pf);
        }


        [DataTestMethod()]
        [DataRow(0, 0, Direction.South, false)]
        [DataRow(1, 0, Direction.South, false)]
        [DataRow(0, 1, Direction.North, false)]
        [DataRow(0, 1, Direction.South, false)]
        [DataRow(0, 1, Direction.East, true)]
        [DataRow(1, 2, Direction.North, false)]
        [DataRow(1, 2, Direction.West, false)]
        [DataRow(1, 2, Direction.East, true)]
        public void TestMoveTest(int x, int y, Direction d, bool xExp)
        {
            if (y > 0)
                pf[new Point(x, y)].Item = testItem;
            else if (x > 0)
                new Floor(Point.Empty, null).Item = testItem;
            Assert.AreEqual(xExp, testItem.TestMove(d));
        }

        [DataTestMethod()]
        [DataRow(0, 0, Direction.South, false)]
        [DataRow(1, 0, Direction.South, false)]
        [DataRow(0, 1, Direction.North, false)]
        [DataRow(0, 1, Direction.South, false)]
        [DataRow(0, 1, Direction.East, true)]
        [DataRow(1, 2, Direction.North, false)]
        [DataRow(1, 2, Direction.West, false)]
        [DataRow(1, 2, Direction.East, true)]
        public void TestMove2Test(int x, int y, Direction d, bool xExp)
        {
            pf[new Point(0, 2)].Item = null;
            pf[new Point(0, 2)].Item = new Key(null);
            if (y > 0)
                pf[new Point(x, y)].Item = testItem;
            else if (x > 0)
                new Floor(Point.Empty, null).Item = testItem;
            Assert.AreEqual(xExp, testItem.TestMove(d));
        }

        [TestMethod()]
        [DataRow(0, 0, Direction.South, false)]
        [DataRow(1, 0, Direction.South, false)]
        [DataRow(0, 1, Direction.North, false)]
        [DataRow(0, 1, Direction.South, false)]
        [DataRow(0, 1, Direction.East, true)]
        [DataRow(1, 2, Direction.North, false)]
        [DataRow(1, 2, Direction.West, false)]
        [DataRow(1, 2, Direction.East, true)]
        public void TryMoveTest(int x, int y, Direction d, bool xExp)
        {
            if (y > 0)
                pf[new Point(x, y)].Item = testItem;
            else if (x > 0)
                new Floor(Point.Empty, null).Item = testItem;
            Assert.AreEqual(xExp, testItem.TryMove(d));
        }

        [TestMethod()]
        [DataRow(0, 0, Direction.South, false)]
        [DataRow(1, 0, Direction.South, false)]
        [DataRow(0, 1, null, false)]
        [DataRow(0, 1, Direction.North, false)]
        [DataRow(0, 1, Direction.South, false)]
        [DataRow(0, 1, Direction.East, true)]
        [DataRow(1, 2, Direction.North, false)]
        [DataRow(1, 2, Direction.West, false)]
        [DataRow(1, 2, Direction.East, true)]
        public void GoTest(int x, int y, Direction? d, bool xExp)
        {
            if (y > 0)
                pf[new Point(x, y)].Item = testItem;
            else if (x > 0)
                new Floor(Point.Empty, null).Item = testItem;
            Assert.AreEqual(xExp, testItem.Go(d));
        }
        [TestMethod()]
        [DataRow(0, 0,new Direction[] { })]
        [DataRow(1, 0, new Direction[] { })]
        [DataRow(0, 1, new Direction[] { Direction.East })]
        [DataRow(1, 2, new Direction[] { Direction.East })]
        [DataRow(2, 2, new Direction[] { Direction.North,Direction.West })]
        [DataRow(2, 1, new Direction[] { Direction.West,Direction.South })]
        public void MovableDirTest(int x, int y, Direction[] d)
        {
            if (y > 0)
                pf[new Point(x, y)].Item = testItem;
            else if (x > 0)
                new Floor(Point.Empty, null).Item = testItem;
            AssertAreEqual(d, testItem.MoveableDirs().ToArray());
        }
    }
}