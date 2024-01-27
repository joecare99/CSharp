// ***********************************************************************
// Assembly         : CSV_ViewerTest
// Author           : Mir
// Created          : 12-19-2021
//
// Last Modified By : Mir
// Last Modified On : 12-19-2021
// ***********************************************************************
// <copyright file="TFieldDefTest.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using CSharpBible.CSV_Viewer.CustomDataSet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpBible.CSV_Viewer.DataSet.Tests
{
    /// <summary>
    /// Defines test class TFieldDefTest.
    /// </summary>
    [TestClass()]
    public class TFieldDefTest
    {
        /// <summary>
        /// The test field definition
        /// </summary>
        private TFieldDef TestFieldDef;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [TestInitialize()]
        public void InitTest()
        {
            TestFieldDef = new TFieldDef("TestFieldDef",typeof(string));
        }

        /// <summary>
        /// Defines the test method TestSetup.
        /// </summary>
        [TestMethod()]
        public void TestSetup()
        {
            Assert.IsNotNull(TestFieldDef);
            Assert.AreEqual("TestFieldDef", TestFieldDef.FieldName);
            Assert.AreEqual(typeof(string), TestFieldDef.FieldType);
            Assert.AreEqual("CSharpBible.CSV_Viewer.CustomDataSet.TFieldDef(\"TestFieldDef\", System.String)", TestFieldDef.ToString());
        }

        /// <summary>
        /// Defines the test method TestFieldDef1.
        /// </summary>
        [TestMethod()]
        public void TestFieldDef1()
        {
            var lFieldDef = new TFieldDef("ID", typeof(ulong));
            Assert.IsNotNull(lFieldDef);
            Assert.AreEqual("ID", lFieldDef.FieldName);
            Assert.AreEqual(typeof(UInt64), lFieldDef.FieldType);
            Assert.AreEqual("CSharpBible.CSV_Viewer.CustomDataSet.TFieldDef(\"ID\", System.UInt64)", lFieldDef.ToString());
            lFieldDef.FieldType = typeof(int);
            Assert.AreEqual(typeof(int), lFieldDef.FieldType);
        }

        /// <summary>
        /// Defines the test method TestFieldDef2.
        /// </summary>
        [TestMethod()]
        public void TestFieldDef2()
        {
            var lFieldDef = TestFieldDef;
            Assert.IsNotNull(lFieldDef);
            Assert.AreEqual("TestFieldDef", lFieldDef.FieldName);
            Assert.AreEqual(typeof(string), lFieldDef.FieldType);
            Assert.AreEqual("CSharpBible.CSV_Viewer.CustomDataSet.TFieldDef(\"TestFieldDef\", System.String)", lFieldDef.ToString());
            TestFieldDef.FieldType = typeof(int);
            Assert.AreEqual(typeof(int), lFieldDef.FieldType);
        }
    }
}
