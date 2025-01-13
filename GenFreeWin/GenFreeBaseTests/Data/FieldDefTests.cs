using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class FieldDefTests
    {
        TableDef? testTable = null;
#pragma warning disable CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.
        FieldDef testClass;
#pragma warning restore CS8618 // Ein Non-Nullable-Feld muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten. Erwägen Sie die Deklaration als Nullable.

        [TestInitialize]
        public void Init()
        {
            testTable = new(null!, "test");
            testClass = new FieldDef(testTable, "test", nameof(TypeCode.Int32), 4);
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(testClass);
        }

        [DataTestMethod()]
        [DataRow("test")]
        [DataRow("")]
        [DataRow(null)]
        public void NamePropTest(string? sAct)
        {
            Assert.AreEqual("test", testClass.Name);
            testClass.Name = sAct;
            Assert.AreEqual(sAct, testClass.Name);
        }

        [DataTestMethod()]
        [DataRow(TypeCode.Byte)]
        [DataRow(TypeCode.Double)]
        [DataRow(TypeCode.String)]
        public void TypePropTest(TypeCode eAct)
        {
            Assert.AreEqual(TypeCode.Int32, testClass.Type);
            testClass.Type = eAct;
            Assert.AreEqual(eAct, testClass.Type);
        }

        [DataTestMethod()]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(16)]
        public void SizePropTest(int iAct)
        {
            Assert.AreEqual(4, testClass.Size);
            testClass.Size = iAct;
            Assert.AreEqual(iAct, testClass.Size);
        }

        [DataTestMethod()]
        [DataRow(true)]
        [DataRow(false)]
        public void xNullPropTest(bool xAct)
        {
            Assert.AreEqual(false, testClass.xNull);
            testClass.xNull = xAct;
            Assert.AreEqual(xAct, testClass.xNull);
        }
        [DataTestMethod()]
        public void RequiredTest()
        {
            Assert.AreEqual(false, testClass.Required);
        }
    }
}