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
    class Key : PlayObject
    {
        public Key(Field? aField) : base(aField)
        {
        }

        public override bool TestMove(Direction dir)
        {
           return false;
        }

        public override bool TryMove(Direction dir)
        {
            return false;
        }
    }

    [TestClass()]
    public class FloorTests
    {
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        Floor testItem;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        [TestInitialize()]
        public void Init()
        {
            testItem = new Floor(new Point(2, 1), null);
        }

        [TestMethod()]
        public void SetupTest()
        {
            Assert.IsNotNull(testItem);
            Assert.IsInstanceOfType(testItem, typeof(Field));
            Assert.IsInstanceOfType(testItem, typeof(Floor));
            Assert.AreEqual(new Point(2, 1), testItem.Position);
            Assert.IsNull(testItem.Item);
            Assert.IsNull(testItem.Parent);
        }

        /// <summary>
        /// Defines the test method WallTest.
        /// </summary>
        [DataTestMethod()]
        [DataRow(null,FieldDef.Floor)]
        [DataRow(FieldDef.Player, FieldDef.Player)]
        [DataRow(FieldDef.Stone, FieldDef.Stone)]
        public void GetFieldDefTest(FieldDef? item, FieldDef? fdExp)
        {
            switch (item) {
                case FieldDef.Stone: testItem.Item = new Stone(null); break;
                case FieldDef.Player: testItem.Item = new Player(null); break;
                default:break;
            }
            Assert.AreEqual(fdExp, testItem.fieldDef);
        }

        [TestMethod]
        public void GetFieldDef2Test()
        {
            testItem.Item= new Key(null);
            Assert.ThrowsException<ArgumentException>(()=> testItem.fieldDef);
        }

        /// <summary>
        /// Defines the test method WallTest.
        /// </summary>
        [DataTestMethod()]
        [DataRow(null, null,FieldDef.Floor)]
        [DataRow(null,FieldDef.Player, FieldDef.Player)]
        [DataRow(null, FieldDef.Stone, FieldDef.Stone)]
        [DataRow(FieldDef.Stone, FieldDef.Player, FieldDef.Stone)]
        [DataRow(FieldDef.Empty,FieldDef.Stone, FieldDef.Stone)]
        public void SetItemTest(FieldDef? item1, FieldDef? item2, FieldDef fdExp)
        {
            DoSetItem(item1);
            if (item1 != FieldDef.Stone) 
                new Floor(Point.Empty,null).Item= testItem.Item ;
            DoSetItem(item2);
            Assert.AreEqual(fdExp, testItem.fieldDef);

            void DoSetItem(FieldDef? item)
            {
                switch (item)
                {
                    case FieldDef.Stone: testItem.Item = new Stone(null); break;
                    case FieldDef.Player: testItem.Item = new Player(null); break;
                    case FieldDef.Empty: testItem.Item = new Key(null); break;
                    default: break;
                }
            }
        }



    }
}