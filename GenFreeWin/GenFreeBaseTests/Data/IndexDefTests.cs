using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class IndexDefTests
    {
        private ITableDef td;
        private IndexDef testClass;
        private List<IIndexDef> _ix;

        [TestInitialize]
        public void Init()
        {
            td = Substitute.For<ITableDef>();
            td.Indexes.Returns(_ix = new List<IIndexDef>());
            testClass = new IndexDef(td, "Name", "sField", true, false);
        }

        [TestMethod()]
        public void SetUpTest()
        {
            Assert.IsNotNull(testClass);
        }

        [TestMethod()]
        [DataRow("test")]
        [DataRow("")]
        [DataRow(null)]
        public void NamePropTest(string? sAct)
        {
            Assert.AreEqual("Name", testClass.Name);
            testClass.Name = sAct;
            Assert.AreEqual(sAct, testClass.Name);
        }

        [TestMethod()]
        [DataRow(true)]
        [DataRow(false)]
        public void UniquePropTest(bool xAct)
        {
            Assert.AreEqual(false, testClass.Unique);
            testClass.Unique = xAct;
            Assert.AreEqual(xAct, testClass.Unique);
        }

        [TestMethod()]
        [DataRow(true)]
        [DataRow(false)]
        public void IgnoreNullsPropTest(bool xAct)
        {
            Assert.AreEqual(false, testClass.IgnoreNulls);
            testClass.IgnoreNulls = xAct;
            Assert.AreEqual(xAct, testClass.IgnoreNulls);
        }

        [TestMethod()]
        [DataRow(null, DisplayName = "null")]
        [DataRow("")]
        [DataRow("1")]
        [DataRow("1,3")]
        [DataRow(",1")]
        [DataRow("1,3,9")]
        [DataRow(",")]
        public void FieldsPropTest(string sAct)
        {
            Assert.AreEqual("sField", testClass.Fields == null ? null : string.Join(",", testClass.Fields));
            testClass.Fields = sAct?.Split(',');
            Assert.AreEqual(sAct, testClass.Fields == null ? null : string.Join(",", testClass.Fields));
        }

        [TestMethod()]
        public void IndexDefTest()
        {
            // Act
            var id = new IndexDef(td, "Name", "Field2", true, false);

            // Assert
            Assert.IsNotNull(id);
            Assert.AreEqual(null, id.Name);
            Assert.IsTrue(id.Fields != null && id.Fields.Length == 2 && id.Fields[1] == "Field2");
        }
    }
}
