using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class IndexDefTests
    {
        private ITableDef td;
        private IndexDef testClass;

        [TestInitialize]
        public void Init()
        {
            td = Substitute.For<ITableDef>();
            testClass = new IndexDef(td, "Name", "sField", true, false);
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
            Assert.AreEqual("Name", testClass.Name);
            testClass.Name = sAct;
            Assert.AreEqual(sAct, testClass.Name);
        }

        [DataTestMethod()]
        [DataRow(true)]
        [DataRow(false)]
        public void UniquePropTest(bool xAct)
        {
            Assert.AreEqual(false, testClass.Unique);
            testClass.Unique = xAct;
            Assert.AreEqual(xAct, testClass.Unique);
        }

        [DataTestMethod()]
        [DataRow(true)]
        [DataRow(false)]
        public void IgnoreNullsPropTest(bool xAct)
        {
            Assert.AreEqual(false, testClass.IgnoreNulls);
            testClass.IgnoreNulls = xAct;
            Assert.AreEqual(xAct, testClass.IgnoreNulls);
        }

        [DataTestMethod()]
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
            Assert.AreEqual("Name", id.Name);
            Assert.IsTrue(id.Fields != null && id.Fields.Length == 2 && id.Fields[1] == "Field2");
        }
    }
}
