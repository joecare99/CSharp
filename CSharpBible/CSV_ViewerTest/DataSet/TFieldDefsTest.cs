using System;
using CSharpBible.CSV_Viewer.CustomDataSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpBible.CSV_Viewer.DataSet.Tests
{
    [TestClass()]
    public class TFieldDefsTest
    {
        private TFieldDefs TestFieldDefs;

        [TestInitialize]
        public void InitTest()
        {
            TestFieldDefs = new TFieldDefs();
        }

        public void CreateTestData()
        {
            TestFieldDefs.Add(new TFieldDef("ID", typeof(UInt64)));
            TestFieldDefs.Add(new TFieldDef("Description", typeof(String)));
            TestFieldDefs.Add(new TFieldDef("InfoA", typeof(Int32)));
            TestFieldDefs.Add(new TFieldDef("InfoB", typeof(float)));
        }

        [TestMethod]
        public void TestSetup()
        {
            Assert.IsNotNull(TestFieldDefs);
            Assert.IsInstanceOfType(TestFieldDefs, typeof(TFieldDefs));
            Assert.AreEqual(0, TestFieldDefs.Count);
            Assert.AreEqual("CSharpBible.CSV_Viewer.DataSet.TFieldDefs", TestFieldDefs.ToString());
        }

        [TestMethod]
        public void TestCreateData()
        {
            CreateTestData();
            Assert.AreEqual(4, TestFieldDefs.Count);
            Assert.AreEqual("ID", TestFieldDefs[0].FieldName);
            Assert.AreEqual(typeof(UInt64), TestFieldDefs[0].FieldType);
            Assert.AreEqual("Description", TestFieldDefs[1].FieldName);
            Assert.AreEqual(typeof(String), TestFieldDefs[1].FieldType);
            Assert.AreEqual("InfoA", TestFieldDefs[2].FieldName);
            Assert.AreEqual(typeof(Int32), TestFieldDefs[2].FieldType);
            Assert.AreEqual("InfoB", TestFieldDefs[3].FieldName);
            Assert.AreEqual(typeof(float), TestFieldDefs[3].FieldType);
        }

        [TestMethod]
        public void TestFind()
        {
            CreateTestData();
            var lTest = TestFieldDefs.Find(x => x.FieldName == "ID");
            Assert.AreEqual("ID", lTest.FieldName);
            Assert.AreEqual(typeof(UInt64), lTest.FieldType);
            var lTest2 = TestFieldDefs.Find(x => x.FieldName == "ID");
            Assert.AreEqual("ID", lTest2.FieldName);
            Assert.AreEqual(typeof(UInt64), lTest2.FieldType);
            lTest.FieldType = typeof(int);
            Assert.AreEqual(typeof(int), lTest2.FieldType);
            Assert.AreEqual(typeof(int), TestFieldDefs[0].FieldType);
        }



    }
}
