using BaseLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BaseLib.Tests.Models;

[TestClass]
public sealed class DirectoryProxyTests
{
    private readonly DirectoryProxy _directoryProxy = new();
    private string _sTestDirectory = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _sTestDirectory = Path.Combine(Path.GetTempPath(), "BaseLib.Tests", nameof(DirectoryProxyTests), Guid.NewGuid().ToString("N"));
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (Directory.Exists(_sTestDirectory))
            Directory.Delete(_sTestDirectory, recursive: true);
    }

    [TestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Exists_ForDirectoryPresence_ReturnsExpectedResult(bool xCreateDirectory)
    {
        if (xCreateDirectory)
            Directory.CreateDirectory(_sTestDirectory);

        var xResult = _directoryProxy.Exists(_sTestDirectory);

        Assert.AreEqual(xCreateDirectory, xResult);
    }

    [TestMethod]
    public void CreateDirectory_ForMissingDirectory_CreatesDirectory()
    {
        _directoryProxy.CreateDirectory(_sTestDirectory);

        Assert.IsTrue(Directory.Exists(_sTestDirectory));
    }

    [TestMethod]
    public void CreateDirectory_ForNestedDirectory_CreatesAllSegments()
    {
        var sNestedDirectory = Path.Combine(_sTestDirectory, "level1", "level2");

        _directoryProxy.CreateDirectory(sNestedDirectory);

        Assert.IsTrue(Directory.Exists(sNestedDirectory));
    }
}
