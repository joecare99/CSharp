using BaseLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace BaseLib.Tests.Models;

[TestClass]
public sealed class PathProxyTests
{
    private readonly PathProxy _pathProxy = new();
    private string _sTestDirectory = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _sTestDirectory = Path.Combine(Path.GetTempPath(), "BaseLib.Tests", nameof(PathProxyTests), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_sTestDirectory);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (Directory.Exists(_sTestDirectory))
            Directory.Delete(_sTestDirectory, recursive: true);
    }

    [TestMethod]
    public void GetFullPath_ForRelativePath_ReturnsAbsolutePath()
    {
        var sRelativePath = Path.Combine(".", "test", "file.txt");

        var sResult = _pathProxy.GetFullPath(sRelativePath);

        Assert.AreEqual(Path.GetFullPath(sRelativePath), sResult);
    }

    [TestMethod]
    public void GetDirectoryName_ForNestedFilePath_ReturnsParentDirectory()
    {
        var sPath = Path.Combine(_sTestDirectory, "sub", "file.txt");

        var sResult = _pathProxy.GetDirectoryName(sPath);

        Assert.AreEqual(Path.GetDirectoryName(sPath), sResult);
    }

    [TestMethod]
    public void GetDirectoryName_ForPathWithoutDirectory_ReturnsExpectedValue()
    {
        const string sPath = "file.txt";

        var sResult = _pathProxy.GetDirectoryName(sPath);

        Assert.AreEqual(Path.GetDirectoryName(sPath), sResult);
    }

    [TestMethod]
    public void GetTempFileName_CreatesExistingTempFile()
    {
        var sTempPath = _pathProxy.GetTempFileName();

        try
        {
            Assert.IsTrue(File.Exists(sTempPath));
            Assert.AreEqual(Path.GetTempPath(), Path.GetDirectoryName(sTempPath) + Path.DirectorySeparatorChar);
        }
        finally
        {
            if (File.Exists(sTempPath))
                File.Delete(sTempPath);
        }
    }
}
