using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;

namespace Sokoban_Base.Model.Tests
{
    /// <summary>
    /// Defines test class WallTests.
    /// </summary>
    [TestClass()]
    public class WallTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        Wall testItem;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        [TestInitialize()]
        public void Init()
        {
            testItem = new Wall(new Point(2, 2), null);
        }

        /// <summary>
        /// Defines the test method WallTest.
        /// </summary>
        [TestMethod()]
        public void WallTest()
        {
            var w = new Wall(new Point(2, 3), null);
            Assert.AreEqual(new Point(2, 3), w.Position);
            Assert.IsNull(w.Item);
            w.Item = null;
            Assert.ThrowsException<ArgumentException>(()=>w.Item = new Stone(null));
        }

        /// <summary>
        /// Defines the test method WallTest.
        /// </summary>
        [TestMethod()]
        public void GetFieldDefTest()
        {
            Assert.AreEqual(FieldDef.Wall, testItem.fieldDef);
        }
    }
}