using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class IndexDefTests
    {

        private IndexDef testClass;

        [TestInitialize]
        public void Init()
        {
            testClass = new IndexDef();
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
            Assert.AreEqual(null, testClass.Name);
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
        [DataRow(null,DisplayName ="null")]
        [DataRow("")]
        [DataRow("1")]
        [DataRow("1,3")]
        [DataRow(",1")]
        [DataRow("1,3,9")]
        [DataRow(",")]
        public void FieldsPropTest(string sAct)
        {
            Assert.AreEqual(null, testClass.Fields);
            testClass.Fields = sAct?.Split(',');
            Assert.AreEqual(sAct, testClass.Fields ==null?null: string.Join(",", testClass.Fields));
        }

    }
}
