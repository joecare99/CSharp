using GenFree.Data.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenFree.Helper;

namespace GenFree.Data.DB.Tests;

[TestClass()]
public class CDatabaseTests
{
    DBImplementOleDB.CDatabase database;
    DBImplementOleDB.DBEngine dbEngine;

    [TestInitialize()]
    public void Initialize()
    {
        dbEngine = new DBImplementOleDB.DBEngine();
        database = (DBImplementOleDB.CDatabase)dbEngine.OpenDatabase("Resources\\test.mdb", false, false, "");
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
    public void CDatabaseTest()
    {
        Assert.IsNotNull(database);
        Assert.IsInstanceOfType(database, typeof(DBImplementOleDB.CDatabase));
        Assert.IsTrue(database.IsOpen);
        //   Assert.IsFalse(database.IsReadOnly);
    }

    public void CreateTestData()
    {
        Assert.IsNotNull(database);
        Assert.IsTrue(database.IsOpen);
        database.Execute("CREATE TABLE TestTable (ID AUTOINCREMENT, Name TEXT(50))");
        database.Execute("CREATE UNIQUE INDEX idx_ID ON TestTable (ID)");
        database.Execute("INSERT INTO TestTable (Name) VALUES ('Test Name')");
        Assert.AreEqual(1, database.Execute("INSERT INTO TestTable (Name) VALUES ('Another Test Name')"));
        Assert.IsTrue(database.TableExists("TestTable"), "TestTable was not created successfully.");
    }

    [TestMethod()]
    public void CloseTest()
    {
        database.Close();
        Assert.IsFalse(database.IsOpen);
    }

    [TestMethod()]
    public void ExecuteTest()
    {
        CreateTestData();
    }

    [TestMethod()]
    public void ExecuteTest1()
    {
        CreateTestData();
        var rslt= database.Execute("UPDATE TestTable SET Name = @Name WHERE ID = @ID", new Dictionary<string,object>() { { "@Name", "Updated Name" }, { "@ID", 1 } });
        Assert.IsTrue(rslt == 1, "Update operation did not affect any rows.");
    }

    [TestMethod()]
    public void GetSchemaTest()
    {
        var cnt = 0;
        foreach (DataRow row in database.GetSchema("Tables").Rows)
            cnt++;

        Assert.IsTrue(cnt > 0, "No tables found in the database schema.");
    }

    [TestMethod()]
    public void OpenRecordsetTest()
    {
        CreateTestData();
        Assert.IsTrue(database.TableExists("TestTable"), "TestTable does not exist in the database.");

        // Open a recordset and check its properties
        var rs = database.OpenRecordset("SELECT * FROM TestTable");
        Assert.IsNotNull(rs, "Recordset should not be null.");
        Assert.AreEqual(2, rs.RecordCount, "Recordset should contain 2 records.");
        rs.Close();
    }

    [TestMethod()]
    public void OpenRecordsetTest1()
    {
        CreateTestData();
        Assert.IsTrue(database.TableExists("TestTable"), "TestTable does not exist in the database.");

        // Open a recordset and check its properties
        var rs = database.OpenRecordset("TestTable","",null,null);
        Assert.IsNotNull(rs, "Recordset should not be null.");
        Assert.AreEqual(2, rs.RecordCount, "Recordset should contain 2 records.");
        rs.Close();
    }

    [TestMethod()]
    public void OpenRecordsetTest2()
    {
        CreateTestData();
        Assert.IsTrue(database.TableExists("TestTable"), "TestTable does not exist in the database.");
        // Open a recordset with a specific type
        var rs = database.OpenRecordset(ETestTable.TestTable);
        Assert.IsNotNull(rs, "Recordset should not be null.");
        Assert.AreEqual(2, rs.RecordCount, "Recordset should contain 2 records.");
        rs.Close();
    }

    private enum ETestTable
    {
        TestTable,
        AnotherTestTable
    }
}