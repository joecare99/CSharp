﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NSubstitute;
using GenFree.Interfaces.DB;
using System.Data;
using GenFree.Data;

namespace GenFree.Helper.Tests
{
    [TestClass()]
    public class DBHelperTests
    {

        [TestMethod()]
        [DataRow("Exception", PersonFields.Sex, false)]
        [DataRow("NoException", FamilyFields.Suf, false)]
        [DataRow("Test", LinkFields.Kennz, true)]
        public void DbFieldExistsTest(string Name, Enum eField, bool xExp)
        {
            var testWS = Substitute.For<IDatabase>();
            var dt = new DataTable("Columns");
            dt.Columns.Add("TABLE_NAME");
            dt.Columns.Add("COLUMN_NAME");
            dt.Rows.Add(nameof(LinkFields.Kennz), "Test");
            if (Name == "Exception")
                testWS.When(x => x.GetSchema("Columns")).Do(x => { throw new ArgumentException("Name"); });
            else
                testWS.GetSchema("Columns").Returns(dt);
            Assert.AreEqual(xExp, testWS.DbFieldExists(eField, Name));
            testWS.Received(1).GetSchema("Columns");
        }

        [TestMethod()]
        [DataRow("Exception", false)]
        [DataRow("NoException", false)]
        [DataRow("Test", true)]
        public void TableExistsTest(string Name, bool xExp)
        {
            var testWS = Substitute.For<IDatabase>();
            var dt = new DataTable("Tables");
            dt.Columns.Add("TABLE_NAME");
            dt.Rows.Add("Test");
            if (Name == "Exception")
                testWS.When(x => x.GetSchema("Tables")).Do(x => { throw new ArgumentException("Name"); });
            else
                testWS.GetSchema("Tables").Returns(dt);
            Assert.AreEqual(xExp, testWS.TableExists(Name));
            testWS.Received(1).GetSchema("Tables");

        }

        [TestMethod()]
        [DataRow("Exception", null)]
        [DataRow("NoException", null)]
        [DataRow("NoException", "Test")]
        public void TryExecuteTest(string Name, object oAct)
        {
            var testWS = Substitute.For<IDatabase>();
            if (Name == "Exception")
                testWS.When(x => x.Execute(Name, oAct)).Do(x => { throw new ArgumentException("Name"); });
            testWS.TryExecute(Name, oAct);
            testWS.Received(1).Execute(Name, oAct);
        }

        [TestMethod()]
        public void BeginTransTest()
        {
            var testWS = Substitute.For<IDBWorkSpace>();
            testWS.BeginTrans();
            testWS.Received(1).Begin();
        }

        [TestMethod()]
        public void CommitTransTest()
        {
            var testWS = Substitute.For<IDBWorkSpace>();
            testWS.CommitTrans();
            testWS.Received(1).Commit();

        }

        [TestMethod()]
        public void TableDefsTest()
        {
            //Arrange
            var testDB = Substitute.For<IDatabase>();
            var tTables = new DataTable("Tables");
            tTables.Columns.Add("TABLE_NAME", typeof(string));
            tTables.Rows.Add("Test");
            var tCols = new DataTable("Tables");
            tCols.Columns.Add("COLUMN_NAME", typeof(string));
            tCols.Columns.Add("DATA_TYPE", typeof(string));
            tCols.Columns.Add("CHARACTER_MAXIMUM_LENGTH", typeof(int));
            tCols.Rows.Add("Test", "Int32", 0);
            testDB.GetSchema("Tables").Returns(tTables);
            testDB.GetSchema("Columns", new[] { null, null, "Test" }).Returns(tCols);
            var tIdx = new DataTable("Tables");
            tIdx.Columns.Add("TABLE_NAME", typeof(string));
            tIdx.Columns.Add("INDEX_NAME", typeof(string));
            tIdx.Columns.Add("COLUMN_NAME", typeof(string));
            tIdx.Columns.Add("PRIMARY_KEY", typeof(bool));
            tIdx.Columns.Add("UNIQUE", typeof(bool));
            tIdx.Rows.Add("Test", "TestIdx", "Test", true,false);
            testDB.GetSchema("Indexes").Returns(tIdx);

            //Act
            foreach (var td in testDB.TableDefs())
            {
                Assert.AreEqual(1, td.Fields.Count);
                Assert.AreEqual(1, td.Indexes.Count);
            }
        }
    }
}