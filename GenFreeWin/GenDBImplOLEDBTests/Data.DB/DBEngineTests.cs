using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.IO;
using GenFree.Data.DB;
using GenFree.Interfaces.DB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GenFree.Data.DB.Tests;

[TestClass]
public class DBEngineTests
{
    private DBImplementOleDB.DBEngine _dbEngine;

    [TestInitialize]
    public void Setup()
    {
        _dbEngine = new DBImplementOleDB.DBEngine();
    }

    [TestMethod]
    public void DBEngine_ShouldInitializeWorkspaces()
    {
        // Arrange & Act
        var workspaces = _dbEngine.Workspaces;

        // Assert
        Assert.IsNotNull(workspaces);
        Assert.AreEqual(1, workspaces.Count);
        Assert.IsInstanceOfType(workspaces[0], typeof(DBImplementOleDB.CWorkSpace));
    }

    [TestMethod]
    [DataRow("resources\\test.mdb", false, false, "")]
    public void OpenDatabase_ShouldReturnDatabaseInstance(string path, bool v2, bool v3, string v4)
    {
        // Arrange
        var mockDatabase = Substitute.For<IDatabase>();
        Assert.IsTrue(File.Exists(path), "Test file does not exist at the specified path.");

        // Act
        var database = _dbEngine.OpenDatabase(path, v2, v3, v4);

        // Assert
        Assert.IsNotNull(database);
        Assert.IsInstanceOfType(database, typeof(DBImplementOleDB.CDatabase));
    }

    [TestMethod]
    [DataRow("source.mdb", "destination.mdb")]
    public void CompactDatabase_ShouldThrowIfSourceFileNotFound(string source, string destination)
    {
        // Arrange
        System.IO.File.Delete(source);

        // Act & Assert
        Assert.ThrowsException<System.IO.FileNotFoundException>(() => _dbEngine.CompactDatabase(source, destination));
    }

    [TestMethod]
    [DataRow("resources\\test.mdb", "resources\\test1.mdb")]
    public void CompactDatabase_ShouldWork(string source, string destination)
    {
        // arrange
        System.IO.File.Delete(destination);

        // Act & Assert
        _dbEngine.CompactDatabase(source, destination);
        
        // Assert
        Assert.IsTrue(File.Exists(destination), "Destination file should exist.");
    }

    [TestMethod]
    [DataRow("source.mdb", "destination.mdb")]
    public void CompactDatabase_ShouldThrowIfJRONotAvailable(string source, string destination)
    {
        // Arrange
        System.IO.File.Create(source).Dispose();
        System.IO.File.Delete(destination);

        var jroType = Type.GetTypeFromProgID("JRO.JetEngine");
        if (jroType != null)
        {
            Assert.Inconclusive("JRO.JetEngine is available on this system, skipping test.");
        }

        // Act & Assert
        Assert.ThrowsException<NotSupportedException>(() => _dbEngine.CompactDatabase(source, destination));
    }

    [TestMethod]
    public void DBEngine_ShouldLogRootEnumerator()
    {
        // Arrange
        var mockEnumerator = Substitute.For<DbDataRecord>();

        // Act
        var dbEngine = new DBImplementOleDB.DBEngine();

        // Assert
        dbEngine.Workspaces[0].CreateDatabase("test.mdb", ";LANGID=0x0409;CP=1252;COUNTRY=0"); // Create a test database

    }
}
