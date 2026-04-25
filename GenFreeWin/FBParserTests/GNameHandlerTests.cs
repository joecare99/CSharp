using FBParser;
using FBParserTests.Models;

namespace FBParserTests;

[TestClass]
public sealed class GNameHandlerTests
{
    [TestMethod]
    public void Init_ResetsChangedStateAndAllowsLearning()
    {
        var sut = new GNameHandler();
        sut.Init();

        sut.LearnSexOfGivnName("Peter", 'M');

        Assert.IsTrue(sut.Changed);
        Assert.AreEqual('M', sut.GuessSexOfGivnName("Peter", false));
    }

    [TestMethod]
    public void LearnSexOfGivnName_LearnsKnownNames()
    {
        var sut = new GNameHandler();
        sut.Init();

        sut.LearnSexOfGivnName("Anna Maria", 'F');

        Assert.AreEqual('F', sut.GuessSexOfGivnName("Anna", false));
        Assert.AreEqual('F', sut.GuessSexOfGivnName("Maria", false));
    }

    [TestMethod]
    public void LearnSexOfGivnName_IgnoresAbbreviationsAndUnknownMarkers()
    {
        var sut = new GNameHandler();
        sut.Init();

        sut.LearnSexOfGivnName("A. ...", 'M');

        Assert.AreEqual('U', sut.GuessSexOfGivnName("A.", false));
        Assert.AreEqual('U', sut.GuessSexOfGivnName("...", false));
    }

    [TestMethod]
    public void LearnSexOfGivnName_RaisesErrorForInvalidCase()
    {
        var sut = new GNameHandler();
        sut.Init();
        string? message = null;
        sut.OnError = (msg, _) => message = msg;

        sut.LearnSexOfGivnName("peter", 'M');

        Assert.IsNotNull(message);
        StringAssert.Contains(message, "valid Name (case)");
    }

    [TestMethod]
    public void GuessSexOfGivnName_LearnsUnknownWhenConfigured()
    {
        var sut = new GNameHandler();
        sut.Init();
        sut.CfgLearnUnknown = true;

        var result = sut.GuessSexOfGivnName("Unbekannt", true);

        Assert.AreEqual('U', result);
        Assert.IsTrue(sut.Changed);
        Assert.AreEqual('U', sut.GuessSexOfGivnName("Unbekannt", false));
    }

    [TestMethod]
    public void GuessSexOfGivnName_DoesNotLearnUnknownWhenDisabled()
    {
        var sut = new GNameHandler();
        sut.Init();
        sut.CfgLearnUnknown = false;

        var result = sut.GuessSexOfGivnName("Unbekannt", true);

        Assert.AreEqual('U', result);
        Assert.IsFalse(sut.Changed);
    }

    [TestMethod]
    public void SaveAndLoadGNameList_RoundTripsLearnedData()
    {
        var fakeFileSystem = new FakeFileSystem();
        var writer = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, fakeFileSystem.CreateStreamWriter);
        writer.Init();
        writer.LearnSexOfGivnName("Peter", 'M');
        writer.SaveGNameList("ROOT:\\names.txt");

        var reader = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, fakeFileSystem.CreateStreamWriter);
        reader.Init();
        reader.LoadGNameList("ROOT:\\names.txt");

        Assert.AreEqual('M', reader.GuessSexOfGivnName("Peter", false));
        Assert.IsFalse(reader.Changed);
    }
}
