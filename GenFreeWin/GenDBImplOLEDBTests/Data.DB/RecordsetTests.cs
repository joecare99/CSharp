using BaseLib.Helper;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenFree.Data.DB.Tests;

[TestClass()]
public class RecordsetTests
{
    DBImplementOleDB.CDatabase database;
    private IRecordset TestRS;

    //  DBImplementOleDB.DBEngine dbEngine;

    [TestInitialize()]
    public void Initialize()
    {
        var dbEngine = new DBImplementOleDB.DBEngine();
        database = (DBImplementOleDB.CDatabase)dbEngine.OpenDatabase("Resources\\test.mdb", false, false, "");
        if (!database.IsOpen)
        {
            Assert.Fail("Database could not be opened.");
        }
        database.Execute("CREATE TABLE TestTable (ID AUTOINCREMENT, Name TEXT(50))");
        database.Execute("CREATE UNIQUE INDEX idx_ID ON TestTable (ID)");
        database.Execute("INSERT INTO TestTable (Name) VALUES ('Test Name')");
        Assert.AreEqual(1, database.Execute("INSERT INTO TestTable (Name) VALUES ('Another Test Name')"));
        TestRS = database.OpenRecordset("TestTable",RecordsetTypeEnum.dbOpenTable);
    }

    [TestCleanup()]
    public void Cleanup()
    {
        if (database.IsOpen)
        {
            var sTable = database.GetSchema("Tables");
            foreach (DataRow row in sTable.Rows)
            {
                var tableName = row["TABLE_NAME"].ToString();
                if (!tableName?.StartsWith("MSys") ?? false)
                {
                    try
                    {
                        database.Execute($"DROP TABLE [{tableName}];");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error dropping table {tableName}: {ex.Message}");
                    }
                }
            }
            database.Close();
        }
    }

    [TestMethod()]
    public void RecordsetTest()
    {
        Assert.IsNotNull(TestRS);
        Assert.AreEqual(2, TestRS.RecordCount);
    }

    [TestMethod()]
    public void CloseTest()
    {
        if (TestRS != null && TestRS.IsOpen)
        {
            TestRS.Close();
        }
        Assert.IsFalse(TestRS.IsOpen);
    }

    [TestMethod()]
    public void EditTest()
    {
        TestRS.Edit();
        TestRS.Fields["Name"].Value = "Updated Test Name";
        TestRS.Update();
        
        TestRS.MoveFirst();
        Assert.AreEqual("Updated Test Name", TestRS.Fields["Name"].Value);
        TestRS.Close();

    }

    [TestMethod()]
    public void MoveNextTest()
    {
        TestRS.MoveNext();
        Assert.AreEqual("Another Test Name", TestRS.Fields["Name"].Value);
        TestRS.Close();
    }

    [TestMethod()]
    public void SeekTest()
    {
        TestRS.MoveFirst();
        TestRS.Seek("=", "Test Name");
        Assert.IsFalse(TestRS.NoMatch);
        Assert.AreEqual("Test Name", TestRS.Fields["Name"].Value);
        TestRS.Close();
    }

    [TestMethod()]
    public void UpdateTest()
    {
        TestRS.Edit();
        TestRS.Fields["Name"].Value = "Updated Name";
        TestRS.Update();
        
        TestRS.MoveFirst();
        Assert.AreEqual("Updated Name", TestRS.Fields["Name"].Value);
        TestRS.Close();

        var TestRS2 = database.OpenRecordset("TestTable", RecordsetTypeEnum.dbOpenTable);
        TestRS2.MoveFirst();
        Assert.AreEqual("Updated Name", TestRS2.Fields["Name"].Value);
        TestRS2.Close();
    }

    [TestMethod()]
    public void AddNewTest()
    {
        TestRS.AddNew();
        TestRS.Fields["Name"].Value = "New Test Name";
        TestRS.Update();
        
        TestRS.MoveLast();
        Assert.AreEqual("New Test Name", TestRS.Fields["Name"].Value);
        Assert.AreEqual(3, TestRS.RecordCount);
        TestRS.Close();

        var TestRS2 = database.OpenRecordset("TestTable", RecordsetTypeEnum.dbOpenTable);
        TestRS2.MoveLast();
        Assert.AreEqual("New Test Name", TestRS2.Fields["Name"].Value);
        TestRS2.Close();
    }

    [TestMethod()]
    public void DeleteTest()
    {
        TestRS.MoveFirst();
        TestRS.Delete();
        
        Assert.AreEqual(1, TestRS.RecordCount);
        TestRS.MoveFirst();
        Assert.AreEqual("Another Test Name", TestRS.Fields["Name"].Value);
        TestRS.Close();
        var TestRS2 = database.OpenRecordset("TestTable", RecordsetTypeEnum.dbOpenTable);
        Assert.AreEqual(1, TestRS2.RecordCount);
        TestRS2.Close();
    }

    [TestMethod()]
    public void MoveLastTest()
    {
        TestRS.MoveLast();
        Assert.AreEqual("Another Test Name", TestRS.Fields["Name"].Value);
        TestRS.Close();
    }

    [TestMethod()]
    public void FindFirstTest()
    {
        Assert.Fail();
    }

    [TestMethod()]
    public void MoveFirstTest()
    {
        TestRS.MoveFirst();
        Assert.AreEqual("Test Name", TestRS.Fields["Name"].Value);
        TestRS.Close();
    }

    [TestMethod()]
    public void MovePreviousTest()
    {
        Assert.Fail();
    }
}