using BaseLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;

namespace BaseLib.Tests.Models;

[TestClass]
public sealed class FileProxyTests
{
    private readonly FileProxy _fileProxy = new();
    private string _sTestDirectory = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _sTestDirectory = Path.Combine(Path.GetTempPath(), "BaseLib.Tests", nameof(FileProxyTests), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_sTestDirectory);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (Directory.Exists(_sTestDirectory))
            Directory.Delete(_sTestDirectory, recursive: true);
    }

    [DataTestMethod]
    [DataRow(false)]
    [DataRow(true)]
    public void Exists_ForFilePresence_ReturnsExpectedResult(bool xCreateFile)
    {
        var sPath = Path.Combine(_sTestDirectory, "exists.txt");
        if (xCreateFile)
            File.WriteAllText(sPath, "content");

        var xResult = _fileProxy.Exists(sPath);

        Assert.AreEqual(xCreateFile, xResult);
    }

    [TestMethod]
    public void Create_CreatesWritableFile()
    {
        var sPath = Path.Combine(_sTestDirectory, "create.bin");
        var rgBytes = new byte[] { 1, 2, 3, 4 };

        using (var stm = _fileProxy.Create(sPath))
        {
            stm.Write(rgBytes, 0, rgBytes.Length);
        }

        CollectionAssert.AreEqual(rgBytes, File.ReadAllBytes(sPath));
    }

    [TestMethod]
    public void OpenWrite_ForExistingFile_OverwritesFromStart()
    {
        var sPath = Path.Combine(_sTestDirectory, "write.bin");
        File.WriteAllBytes(sPath, new byte[] { 9, 9, 9, 9 });
        var rgBytes = new byte[] { 1, 2 };

        using (var stm = _fileProxy.OpenWrite(sPath))
        {
            stm.Write(rgBytes, 0, rgBytes.Length);
            stm.SetLength(rgBytes.Length);
        }

        CollectionAssert.AreEqual(rgBytes, File.ReadAllBytes(sPath));
    }

    [TestMethod]
    public void OpenRead_ForExistingFile_ReturnsReadableStream()
    {
        var sPath = Path.Combine(_sTestDirectory, "read.bin");
        var rgBytes = new byte[] { 5, 6, 7 };
        File.WriteAllBytes(sPath, rgBytes);

        using var stm = _fileProxy.OpenRead(sPath);
        using var stmMemory = new MemoryStream();
        stm.CopyTo(stmMemory);

        CollectionAssert.AreEqual(rgBytes, stmMemory.ToArray());
    }

    [DataTestMethod]
    [DataRow("Plain UTF8 text")]
    [DataRow("äöü ß Ελληνικά")]
    public void WriteAllText_WithoutEncoding_WritesReadableUtf8Content(string sContents)
    {
        var sPath = Path.Combine(_sTestDirectory, "utf8.txt");

        _fileProxy.WriteAllText(sPath, sContents);

        Assert.AreEqual(sContents, File.ReadAllText(sPath));
    }

    [DataTestMethod]
    [DataRow("Plain UTF8 text")]
    [DataRow("äöü ß Ελληνικά")]
    public void ReadAllText_WithoutEncoding_ReturnsFileContent(string sContents)
    {
        var sPath = Path.Combine(_sTestDirectory, "read-text.txt");
        File.WriteAllText(sPath, sContents);

        var sResult = _fileProxy.ReadAllText(sPath);

        Assert.AreEqual(sContents, sResult);
    }

    [TestMethod]
    public void WriteAllText_WithEncoding_WritesEncodedContent()
    {
        var sPath = Path.Combine(_sTestDirectory, "encoded-write.txt");
        var sContents = "Grüße aus Köln";
        var encEncoding = Encoding.Unicode;

        _fileProxy.WriteAllText(sPath, sContents, encEncoding);

        Assert.AreEqual(sContents, File.ReadAllText(sPath, encEncoding));
    }

    [TestMethod]
    public void ReadAllText_WithEncoding_ReturnsEncodedContent()
    {
        var sPath = Path.Combine(_sTestDirectory, "encoded-read.txt");
        var sContents = "Grüße aus Köln";
        var encEncoding = Encoding.Unicode;
        File.WriteAllText(sPath, sContents, encEncoding);

        var sResult = _fileProxy.ReadAllText(sPath, encEncoding);

        Assert.AreEqual(sContents, sResult);
    }

    [TestMethod]
    public void WriteAllBytes_WritesBinaryContent()
    {
        var sPath = Path.Combine(_sTestDirectory, "bytes-write.bin");
        var rgBytes = new byte[] { 10, 20, 30, 40 };

        _fileProxy.WriteAllBytes(sPath, rgBytes);

        CollectionAssert.AreEqual(rgBytes, File.ReadAllBytes(sPath));
    }

    [TestMethod]
    public void ReadAllBytes_ReturnsBinaryContent()
    {
        var sPath = Path.Combine(_sTestDirectory, "bytes-read.bin");
        var rgBytes = new byte[] { 10, 20, 30, 40 };
        File.WriteAllBytes(sPath, rgBytes);

        var rgResult = _fileProxy.ReadAllBytes(sPath);

        CollectionAssert.AreEqual(rgBytes, rgResult);
    }

    [TestMethod]
    public void Delete_RemovesFile()
    {
        var sPath = Path.Combine(_sTestDirectory, "delete.txt");
        File.WriteAllText(sPath, "content");

        _fileProxy.Delete(sPath);

        Assert.IsFalse(File.Exists(sPath));
    }

    [DataTestMethod]
    [DataRow(false, "source")]
    [DataRow(true, "new")]
    public void Copy_CopiesFileToDestination(bool xOverwrite, string sExpectedDestinationContent)
    {
        var sSourcePath = Path.Combine(_sTestDirectory, "source.txt");
        var sDestinationPath = Path.Combine(_sTestDirectory, "destination.txt");
        File.WriteAllText(sSourcePath, sExpectedDestinationContent);
        if (xOverwrite)
            File.WriteAllText(sDestinationPath, "old");

        _fileProxy.Copy(sSourcePath, sDestinationPath, xOverwrite);

        Assert.AreEqual(sExpectedDestinationContent, File.ReadAllText(sDestinationPath));
    }

    [TestMethod]
    public void Move_MovesFileToDestination()
    {
        var sSourcePath = Path.Combine(_sTestDirectory, "source-move.txt");
        var sDestinationPath = Path.Combine(_sTestDirectory, "destination-move.txt");
        File.WriteAllText(sSourcePath, "moved");

        _fileProxy.Move(sSourcePath, sDestinationPath);

        Assert.IsFalse(File.Exists(sSourcePath));
        Assert.AreEqual("moved", File.ReadAllText(sDestinationPath));
    }
}
