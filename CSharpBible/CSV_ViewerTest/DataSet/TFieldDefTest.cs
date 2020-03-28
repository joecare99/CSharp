using System;
using CSharpBible.CSV_Viewer.DataSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpBible.CSV_Viewer.DataSet.Tests
{
    [TestClass()]
    public class TFieldDefTest
    {
        private TFieldDef TestFieldDef;

        [TestInitialize()]
        public void InitTest()
        {
            TestFieldDef = new TFieldDef("TestFieldDef",typeof(string));
        }

        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(TestFieldDef);
            Assert.AreEqual("TestFieldDef", TestFieldDef.FieldName);
            Assert.AreEqual(typeof(string), TestFieldDef.FieldType);
            Assert.AreEqual("CSharpBible.CSV_Viewer.DataSet.TFieldDef(\"TestFieldDef\", System.String)", TestFieldDef.ToString());
        }

        [TestMethod()]
        public void TestFieldDef1()
        {
            var lFieldDef = new TFieldDef("ID", typeof(ulong));
            Assert.IsNotNull(lFieldDef);
            Assert.AreEqual("ID", lFieldDef.FieldName);
            Assert.AreEqual(typeof(UInt64), lFieldDef.FieldType);
            Assert.AreEqual("CSharpBible.CSV_Viewer.DataSet.TFieldDef(\"ID\", System.UInt64)", lFieldDef.ToString());
            lFieldDef.FieldType = typeof(int);
            Assert.AreEqual(typeof(int), lFieldDef.FieldType);
        }

        [TestMethod()]
        public void TestFieldDef2()
        {
            var lFieldDef = TestFieldDef;
            Assert.IsNotNull(lFieldDef);
            Assert.AreEqual("TestFieldDef", lFieldDef.FieldName);
            Assert.AreEqual(typeof(string), lFieldDef.FieldType);
            Assert.AreEqual("CSharpBible.CSV_Viewer.DataSet.TFieldDef(\"TestFieldDef\", System.String)", lFieldDef.ToString());
            TestFieldDef.FieldType = typeof(int);
            Assert.AreEqual(typeof(int), lFieldDef.FieldType);
        }
    }
}
