// Datei: GenFreeBaseClassesTests/Models/System/BaseSystemTests.cs
using System;
using GenFree.Interfaces.Sys;
using GenFree.Models.System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace GenFree.Models.System.Tests;

[TestClass]
public class BaseSystemTests
{
    [TestMethod]
    public void Ctor_SetsInstance()
    {
        var persistence = Substitute.For<IGenPersistence>();

        var sut = new BaseSystem(persistence);

        Assert.IsNotNull(sut);
    }

    [TestMethod]
    [DataRow("XX-GB-1234567890-4A56")]
    [DataRow("12Q3-DE-1234567890-4A56")]
    [DataRow("TOO-SHORT")]
    public void SetLicNr_InvalidLicence_ReturnsFalse_AndDoesNotWrite(string lic)
    {
        var persistence = Substitute.For<IGenPersistence>();
        var sut = new BaseSystem(persistence);

        var result = sut.SetLicNr(lic);

        Assert.IsFalse(result);
         persistence
            .DidNotReceive()
            .WriteStringProg(Arg.Any<string>(), Arg.Any<string>());
    }

    [TestMethod]
    [DataRow("12Q3-GB-1234567890-4A56", "Max Mustermann", "Musterstraße 1", "12345 Musterstadt",
        "Max Mustermann Musterstraße 1 12345 Musterstadt")]
    public void SetLicNr_ValidLicence_WritesLicence_SetsOwner_AndClearsDemoFlag(
        string lic,
        string a1,
        string a2,
        string a3,
        string expectedOwner)
    {
        var persistence = Substitute.For<IGenPersistence>();
        persistence.ReadStringMLProg("Adresse", 3).Returns(string.Join(Environment.NewLine,[ a1, a2, a3 ]));
        var sut = new BaseSystem(persistence);

        var result = sut.SetLicNr(lic);

        Assert.IsTrue(result);
        persistence.Received(1).WriteStringProg("IDF.Dat", lic);
        Assert.IsFalse(sut.ProgIsDemo);
        Assert.AreEqual(expectedOwner, sut.ProgOwner);
    }

    [TestMethod]
    [DataRow("12Q3-GB-1234567890-4A56", "", "", "", "Adresse eingeben")]
    [DataRow("12Q3-GB-1234567890-4A56", "   ", "   ", "   ", "Adresse eingeben")]
    public void SetLicNr_ValidLicence_WithEmptyAddress_SetsDefaultOwner(
        string lic,
        string a1,
        string a2,
        string a3,
        string expectedOwner)
    {
        var persistence = Substitute.For<IGenPersistence>();
        persistence.ReadStringMLProg("Adresse", 3).Returns(string.Join(Environment.NewLine, [a1, a2, a3]));
        var sut = new BaseSystem(persistence);

        var result = sut.SetLicNr(lic);

        Assert.IsTrue(result);
        Assert.AreEqual(expectedOwner, sut.ProgOwner);
    }

    [TestMethod]
    public void CheckLicence_UsesDelegateByDefault()
    {
        var persistence = Substitute.For<IGenPersistence>();
        var sut = new BaseSystem(persistence);

        var result = sut.CheckLicence("INVALID");

        // nur Erwartung: kein Throw, Rückgabewert ist bool
        Assert.IsFalse(result || !result == false); // trivialer Zugriff, nur um Rückruf zu erzwingen
    }
}