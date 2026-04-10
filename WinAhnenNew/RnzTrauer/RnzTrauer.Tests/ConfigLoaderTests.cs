using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RnzTrauer.Core;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class ConfigLoaderTests
{
    [TestMethod]
    public void Load_Uses_File_Abstraction_And_Deserializes_Content()
    {
        var xFile = Substitute.For<IFile>();
        xFile.Exists("config.json").Returns(true);
        xFile.ReadAllText("config.json").Returns(
            """
            {
              "name": "RNZ",
              "count": 5
            }
            """);
        var xLoader = new ConfigLoader(xFile);

        var xResult = xLoader.Load<TestConfig>("config.json");

        Assert.IsNotNull(xResult);
        Assert.AreEqual("RNZ", xResult.Name);
        Assert.AreEqual(5, xResult.Count);
        _ = xFile.Received(1).Exists("config.json");
        _ = xFile.Received(1).ReadAllText("config.json");
    }

    [TestMethod]
    public void Load_Throws_When_File_Does_Not_Exist()
    {
        var xFile = Substitute.For<IFile>();
        xFile.Exists("missing.json").Returns(false);
        var xLoader = new ConfigLoader(xFile);

        try
        {
            _ = xLoader.Load<TestConfig>("missing.json");
            Assert.Fail("Expected FileNotFoundException was not thrown.");
        }
        catch (FileNotFoundException xException)
        {
            StringAssert.Contains(xException.Message, "missing.json");
        }
    }

    [TestMethod]
    public void FileProxy_Delegates_To_File_Class()
    {
        var sPath = Path.GetTempFileName();
        var sTextPath = Path.ChangeExtension(sPath, ".txt");
        var sBytesPath = Path.ChangeExtension(sPath, ".bin");

        try
        {
            File.WriteAllText(sPath, "proxy-test");
            var xProxy = new FileProxy();

            Assert.IsTrue(xProxy.Exists(sPath));
            Assert.AreEqual("proxy-test", xProxy.ReadAllText(sPath));
            CollectionAssert.AreEqual(File.ReadAllBytes(sPath), xProxy.ReadAllBytes(sPath));

            xProxy.WriteAllText(sTextPath, "written-text");
            xProxy.WriteAllBytes(sBytesPath, [1, 2, 3]);

            Assert.AreEqual("written-text", File.ReadAllText(sTextPath));
            CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, File.ReadAllBytes(sBytesPath));
        }
        finally
        {
            if (File.Exists(sPath))
            {
                File.Delete(sPath);
            }

            if (File.Exists(sTextPath))
            {
                File.Delete(sTextPath);
            }

            if (File.Exists(sBytesPath))
            {
                File.Delete(sBytesPath);
            }
        }
    }

    [TestMethod]
    public void RnzConfig_Load_Uses_Injected_ConfigLoader()
    {
        var xConfigLoader = Substitute.For<IConfigLoader>();
        var xExpected = new RnzConfig
        {
            Url = "https://example.invalid",
            Title = "RNZ"
        };
        xConfigLoader.Load<RnzConfig>("rnz.json").Returns(xExpected);

        var xConfig = new RnzConfig(xConfigLoader);

        var xResult = xConfig.Load("rnz.json");

        Assert.AreSame(xExpected, xResult);
        _ = xConfigLoader.Received(1).Load<RnzConfig>("rnz.json");
    }

    [TestMethod]
    public void AmtsblattConfig_Load_Uses_Injected_ConfigLoader()
    {
        var xConfigLoader = Substitute.For<IConfigLoader>();
        var xExpected = new AmtsblattConfig
        {
            Url = "https://example.invalid",
            Title = "Amtsblatt"
        };
        xConfigLoader.Load<AmtsblattConfig>("amtsblatt.json").Returns(xExpected);

        var xConfig = new AmtsblattConfig(xConfigLoader);

        var xResult = xConfig.Load("amtsblatt.json");

        Assert.AreSame(xExpected, xResult);
        _ = xConfigLoader.Received(1).Load<AmtsblattConfig>("amtsblatt.json");
    }

    private sealed class TestConfig
    {
        public string Name { get; set; } = string.Empty;

        public int Count { get; set; }
    }
}
