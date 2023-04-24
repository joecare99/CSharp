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
    public class DestinationTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        Destination testItem;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        [TestInitialize()]
        public void Init()
        {
            testItem = new Destination(new Point(3, 4), null);
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testItem);
            Assert.IsInstanceOfType(testItem, typeof(Field));
            Assert.IsInstanceOfType(testItem, typeof(Floor));
            Assert.IsInstanceOfType(testItem, typeof(Destination));
            Assert.AreEqual(new Point(3, 4), testItem.Position);
            Assert.IsNull(testItem.Item);
            Assert.IsNull(testItem.Parent);
        }

        /// <summary>
        /// Defines the test method WallTest.
        /// </summary>
        [DataTestMethod()]
        [DataRow(null, FieldDef.Destination)]
        [DataRow(FieldDef.Player, FieldDef.PlayerOverDest)]
        [DataRow(FieldDef.Stone, FieldDef.StoneInDest)]
        public void GetFieldDefTest(FieldDef? item, FieldDef? fdExp)
        {
            switch (item)
            {
                case FieldDef.Stone: testItem.Item = new Stone(null); break;
                case FieldDef.Player: testItem.Item = new Player(null); break;
                default: break;
            }
            Assert.AreEqual(fdExp, testItem.fieldDef);
        }

        [TestMethod]
        public void GetFieldDef2Test()
        {
            testItem.Item = new Key(null);
            Assert.ThrowsException<ArgumentException>(() => testItem.fieldDef);
        }
    }
}