using System;
using System.IO;
using FBParser;
using BaseLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FBParserTests;

[TestClass]
public sealed class GNameHandlerTests
{
    [TestMethod]
    public void Properties_GetAndSetExpectedValues()
    {
        var sut = new GNameHandler();
        sut.Init();
        GNameHandler.SendMessage handler = (_, _) => { };

        sut.OnError = handler;
        sut.CfgLearnUnknown = false;
        sut.Changed = true;

        Assert.AreSame(handler, sut.OnError);
        Assert.IsFalse(sut.CfgLearnUnknown);
        Assert.IsTrue(sut.Changed);
    }

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
    public void GuessSexOfGivnName_RaisesErrorForInvalidShortTokenWhenLearning()
    {
        var sut = new GNameHandler();
        sut.Init();
        string? message = null;
        sut.OnError = (msg, _) => message = msg;

        var result = sut.GuessSexOfGivnName("Al", true);

        Assert.AreEqual('U', result);
        Assert.IsNotNull(message);
        StringAssert.Contains(message, "valid Name");
    }

    [TestMethod]
    public void GuessSexOfGivnName_SkipsQuotedAndBracketedTokens()
    {
        var sut = new GNameHandler();
        sut.Init();
        sut.LearnSexOfGivnName("Peter", 'M');

        var result = sut.GuessSexOfGivnName("\"Alias\" (Hinweis) Peter", false);

        Assert.AreEqual('M', result);
    }

    [TestMethod]
    public void SaveGNameList_WithoutConfiguredFile_DoesNothing()
    {
        var fakeFileSystem = new FakeFileSystem();
        var sut = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, path => fakeFileSystem.CreateStreamWriter(path));
        sut.Init();
        sut.LearnSexOfGivnName("Peter", 'M');

        sut.SaveGNameList();

        Assert.IsTrue(sut.Changed);
    }

    [TestMethod]
    public void SetGnlFileName_WithoutExistingNameAndEmptyInput_DoesNothing()
    {
        var fakeFileSystem = new FakeFileSystem();
        var sut = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, path => fakeFileSystem.CreateStreamWriter(path));
        sut.Init();
        sut.LearnSexOfGivnName("Peter", 'M');

        sut.SetGnlFileName();
        sut.SaveGNameList();

        Assert.IsTrue(sut.Changed);
    }

    [TestMethod]
    public void LoadGNameList_MissingFile_ClearsLearnedValues()
    {
        var fakeFileSystem = new FakeFileSystem();
        var sut = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, path => fakeFileSystem.CreateStreamWriter(path));
        sut.Init();
        sut.LearnSexOfGivnName("Peter", 'M');

        sut.LoadGNameList("ROOT:\\missing.txt");

        Assert.AreEqual('U', sut.GuessSexOfGivnName("Peter", false));
        Assert.IsFalse(sut.Changed);
    }

    [TestMethod]
    public void LoadGNameList_ParsesAssignedAndUnassignedEntries()
    {
        var fakeFileSystem = new FakeFileSystem();
        fakeFileSystem.WriteAllText("ROOT:\\names.txt", "Anna=F\r\nPeter\r\nPeter\r\n");
        var sut = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, path => fakeFileSystem.CreateStreamWriter(path));
        sut.Init();

        sut.LoadGNameList("ROOT:\\names.txt");

        Assert.AreEqual('F', sut.GuessSexOfGivnName("Anna", false));
        Assert.AreEqual('U', sut.GuessSexOfGivnName("Peter", false));
        Assert.IsFalse(sut.Changed);
    }

    [TestMethod]
    public void Done_WithChangedStateAndConfiguredFile_PersistsAndClearsData()
    {
        var fakeFileSystem = new FakeFileSystem();
        var sut = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, path => fakeFileSystem.CreateStreamWriter(path));
        sut.Init();
        sut.SetGnlFileName("ROOT:\\names.txt");
        sut.LearnSexOfGivnName("Peter", 'M');

        sut.Done();

        Assert.AreEqual("Peter=M\r\n", fakeFileSystem.ReadAllText("ROOT:\\names.txt"));
        Assert.AreEqual('U', sut.GuessSexOfGivnName("Peter", false));
    }

    [TestMethod]
    public void SetGnlFileName_WithExplicitPath_IsUsedBySave()
    {
        var fakeFileSystem = new FakeFileSystem();
        var sut = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, path => fakeFileSystem.CreateStreamWriter(path));
        sut.Init();
        sut.LearnSexOfGivnName("Peter", 'M');

        sut.SetGnlFileName("ROOT:\\sub\\names.txt");
        sut.SaveGNameList();

        Assert.AreEqual("Peter=M\r\n", fakeFileSystem.ReadAllText("ROOT:\\sub\\names.txt"));
        Assert.IsFalse(sut.Changed);
        Assert.IsTrue(fakeFileSystem.Exists("ROOT:\\sub"));
    }

    [TestMethod]
    public void SaveGNameList_WithExistingDestination_ReplacesFile()
    {
        var fakeFileSystem = new FakeFileSystem();
        fakeFileSystem.WriteAllText("ROOT:\\names.txt", "Alt=F\r\n");
        var sut = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, path => fakeFileSystem.CreateStreamWriter(path));
        sut.Init();
        sut.LearnSexOfGivnName("Peter", 'M');

        sut.SaveGNameList("ROOT:\\names.txt");

        Assert.AreEqual("Peter=M\r\n", fakeFileSystem.ReadAllText("ROOT:\\names.txt"));
        Assert.IsFalse(sut.Changed);
    }

    [TestMethod]
    public void SaveAndLoadGNameList_RoundTripsLearnedData()
    {
        var fakeFileSystem = new FakeFileSystem();
        Func<string, StreamWriter> streamWriterFactory = path => fakeFileSystem.CreateStreamWriter(path);
        var writer = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, streamWriterFactory);
        writer.Init();
        writer.LearnSexOfGivnName("Peter", 'M');
        writer.SaveGNameList("ROOT:\\names.txt");

        var reader = new GNameHandler(fakeFileSystem, fakeFileSystem, fakeFileSystem, path => new StreamWriter(fakeFileSystem.CreateStreamWriter(path).BaseStream));
        reader.Init();
        reader.LoadGNameList("ROOT:\\names.txt");

        Assert.AreEqual('M', reader.GuessSexOfGivnName("Peter", false));
        Assert.IsFalse(reader.Changed);
    }
}
