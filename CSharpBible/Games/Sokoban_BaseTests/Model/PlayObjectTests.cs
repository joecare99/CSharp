using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sokoban_Base.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sokoban_Base.Model.Tests
{
    [TestClass()]
    public class PlayObjectTests 
    {
        Key testItem;
        Floor? field1;
        [TestInitialize]
        public void Init()
        {
            testItem = new Key( null);
            field1 = new Floor(new Point(2,1),null);
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testItem);
            Assert.IsInstanceOfType(testItem, typeof(PlayObject));
            Assert.IsInstanceOfType(testItem, typeof(Key));
            Assert.AreEqual(new Point(0, 0), testItem.Position);
            Assert.AreEqual(new Point(0, 0), testItem.OldPosition);
            Assert.IsNull(testItem.field);
        }

        [TestMethod()]
        public void TestMoveTest()
        {
            Assert.AreEqual(false,testItem.TestMove(Direction.North));
        }

        [TestMethod()]
        public void TryMoveTest()
        {
            Assert.AreEqual(false, testItem.TestMove(Direction.North));
        }

        [TestMethod()]
        public void Position()
        {
            field1!.Item = testItem;
            Assert.AreEqual(new Point(2, 1), testItem.Position);
            Assert.AreEqual(new Point(0, 0), testItem.OldPosition);
            field1!.Item = null;
            Assert.AreEqual(new Point(0, 0), testItem.Position);
            Assert.AreEqual(new Point(2, 1), testItem.OldPosition);

        }

    }
}