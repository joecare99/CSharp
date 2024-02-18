using GenFree.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Data.Tests
{
    [TestClass()]
    public class TableDefTests
    {
        TableDef testClass;
        [TestInitialize]
        public void Init()
        {
            testClass = new TableDef(null!,"test");
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
            Assert.AreEqual("test",testClass.Name);
            testClass.Name = sAct;
            Assert.AreEqual(sAct,testClass.Name);
        }
    }
}