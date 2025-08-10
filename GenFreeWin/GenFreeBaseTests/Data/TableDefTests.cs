﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

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


        [TestMethod()]
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