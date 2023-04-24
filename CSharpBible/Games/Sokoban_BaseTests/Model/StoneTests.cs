using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sokoban_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Base.Model.Tests
{
    [TestClass()]
    public class StoneTests
    {
        Stone testItem;
        Playfield pf = new Playfield();

        [TestInitialize]
        public void Init()
        {
            pf.Setup(new string[] {"###",". $" });
            testItem = new Stone(null);
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
        [DataRow(0, 1, Direction.North,false)]
        [DataRow(0, 1, Direction.South, false)]
        [DataRow(0, 1, Direction.East, true)]
        [DataRow(1, 1, Direction.North, false)]
        [DataRow(1, 1, Direction.West, true)]
        [DataRow(1, 1, Direction.East, false)]
        public void TestMoveTest(int x,int y,Direction d,bool xExp)
        {
            if (y>0)
              pf[new Point(x,y)].Item = testItem;
            else if (x > 0)
                new Floor(Point.Empty,null).Item = testItem;
            Assert.AreEqual(xExp,testItem.TestMove(d));
        }

        [TestMethod()]
        [DataRow(0, 0, Direction.South, false)]
        [DataRow(1, 0, Direction.South, false)]
        [DataRow(0, 1, Direction.North, false)]
        [DataRow(0, 1, Direction.South, false)]
        [DataRow(0, 1, Direction.East, true)]
        [DataRow(1, 1, Direction.North, false)]
        [DataRow(1, 1, Direction.West, true)]
        [DataRow(1, 1, Direction.East, false)]
        public void TryMoveTest(int x, int y, Direction d, bool xExp)
        {
            if (y > 0)
                pf[new Point(x, y)].Item = testItem;
            else if (x>0)
                new Floor(Point.Empty,null).Item = testItem;
            Assert.AreEqual(xExp, testItem.TryMove(d));
        }
    }
}