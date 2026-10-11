using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

[TestClass]
public sealed class GedComDataSourceTests
{
    [TestMethod]
    [DataRow("5.5")]
    [DataRow("5.5.1")]
    [DataRow("7.0")]
    public async Task ImportAsync_PopulatesPeopleFamiliesAndRelationships(string version)
    {
        var source = new GedComDataSource();
        using var stream = CreateGedcomStream(version);

        var genealogy = await source.ImportAsync(stream);

        var people = genealogy.Entitys.OfType<IGenPerson>().ToArray();
        var family = genealogy.Entitys.OfType<IGenFamily>().Single();
        var mother = people.Single(person => person.IndRefID == "I1");
        var father = people.Single(person => person.IndRefID == "I2");
        var child = people.Single(person => person.IndRefID == "I3");

        Assert.AreEqual(3, people.Length);
        Assert.AreEqual("Anna", mother.GivenName);
        Assert.AreEqual("Müller", mother.Surname);
        Assert.AreSame(mother, family.Wife);
        Assert.AreSame(father, family.Husband);
        Assert.AreSame(family, child.ParentFamily);
        Assert.AreSame(father, child.Father);
        Assert.AreSame(mother, child.Mother);
        Assert.AreEqual(1, family.Children.Count);
        Assert.AreEqual("München", mother.BirthPlace?.Name);
        Assert.AreEqual(EDateModifier.About, mother.BirthDate?.eDateModifier);
        Assert.AreEqual("ABT 12 MAR 1900", mother.BirthDate?.DateText);
        Assert.AreEqual(EDateModifier.Between, family.MarriageDate?.eDateModifier);
        Assert.AreEqual("BET 1 JAN 1920 AND 31 DEC 1920", family.MarriageDate?.DateText);
    }

    [TestMethod]
    public async Task ImportAsync_PreservesNamesWithoutSlashDelimiters()
    {
        const string content = "0 HEAD\n1 GEDC\n2 VERS 5.5.1\n0 @I1@ INDI\n1 NAME Paul Fischer\n0 @F1@ FAM\n1 HUSB @I1@\n0 TRLR\n";
        var source = new GedComDataSource();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var genealogy = await source.ImportAsync(stream);
        var person = genealogy.Entitys.OfType<IGenPerson>().Single();

        Assert.AreEqual("Paul Fischer", person.Name);
        Assert.AreEqual("Paul Fischer", person.GivenName);
        Assert.IsNull(person.Surname);
    }

    [TestMethod]
    public async Task ImportAsync_PreservesMarriageFactWithoutDateOrPlace()
    {
        const string content = "0 HEAD\n1 GEDC\n2 VERS 5.5.1\n" +
            "0 @I1@ INDI\n1 NAME Paul /Fischer/\n0 @F1@ FAM\n1 HUSB @I1@\n1 MARR\n0 TRLR\n";
        var source = new GedComDataSource();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var genealogy = await source.ImportAsync(stream);
        var family = genealogy.Entitys.OfType<IGenFamily>().Single();

        Assert.IsNotNull(family.Marriage);
        Assert.IsNull(family.MarriageDate);
        Assert.IsNull(family.MarriagePlace);
    }

    [TestMethod]
    public async Task ImportAsync_LoadsSyntheticFixtureAndResolvesFamilyRelationships()
    {
        var fixturePath = Path.Combine(AppContext.BaseDirectory, "TestData", "gedcom-5.5.1-synthetic.ged");
        var source = new GedComDataSource();
        await using var stream = File.OpenRead(fixturePath);

        var genealogy = await source.ImportAsync(stream);

        var people = genealogy.Entitys.OfType<IGenPerson>().ToArray();
        var family = genealogy.Entitys.OfType<IGenFamily>().Single();

        Assert.AreEqual(3, people.Length);
        Assert.AreEqual("Ada", family.Wife?.GivenName);
        Assert.AreEqual("Beispiel", family.Wife?.Surname);
        Assert.AreEqual("München", family.MarriagePlace?.Name);
        Assert.AreEqual("Kind", family.Children.Single().GivenName);
    }

    [TestMethod]
    public async Task ImportAsync_SkipsMalformedLinesAndKeepsFirstDuplicateCrossReference()
    {
        const string content = "0 HEAD\n1 GEDC\n2 VERS 5.5.1\n" +
            "malformed line\n" +
            "0 @I1@ INDI\n1 NAME First /Person/\n" +
            "0 @I1@ INDI\n1 NAME Duplicate /Person/\n" +
            "0 @F1@ FAM\n1 HUSB @I1@\n0 TRLR\n";
        var source = new GedComDataSource();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var genealogy = await source.ImportAsync(stream);

        var person = genealogy.Entitys.OfType<IGenPerson>().Single();
        Assert.AreEqual("First /Person/", person.Name);
        Assert.AreEqual("First Person", string.Join(" ", person.GivenName, person.Surname));
        Assert.AreEqual(1, genealogy.Entitys.OfType<IGenFamily>().Count());
    }

    [TestMethod]
    public async Task ImportAsync_DetectsUtf8BomAndPreservesUnicode()
    {
        const string content = "0 HEAD\n1 CHAR UTF-8\n1 GEDC\n2 VERS 5.5.1\n" +
            "0 @I1@ INDI\n1 NAME Zoë /Müller/\n0 TRLR\n";
        var preamble = Encoding.UTF8.GetPreamble();
        var contentBytes = Encoding.UTF8.GetBytes(content);
        var bytes = preamble.Concat(contentBytes).ToArray();
        var source = new GedComDataSource();
        using var stream = new MemoryStream(bytes);

        var genealogy = await source.ImportAsync(stream);

        Assert.AreEqual("Zoë", genealogy.Entitys.OfType<IGenPerson>().Single().GivenName);
    }

    private static MemoryStream CreateGedcomStream(string version)
    {
        var content = $"0 HEAD\n1 CHAR UTF-8\n1 GEDC\n2 VERS {version}\n" +
            "0 @I1@ INDI\n1 NAME Anna /Müller/\n2 GIVN Anna\n2 SURN Müller\n1 SEX F\n" +
            "1 BIRT\n2 DATE ABT 12 MAR 1900\n2 PLAC München\n1 _RUFNAME Anni\n1 FAMS @F1@\n" +
            "0 @I2@ INDI\n1 NAME Max /Schmidt/\n1 SEX M\n1 FAMS @F1@\n" +
            "0 @I3@ INDI\n1 NAME Kind /Schmidt/\n1 FAMC @F1@\n" +
            "0 @F1@ FAM\n1 HUSB @I2@\n1 WIFE @I1@\n1 CHIL @I3@\n" +
            "1 MARR\n2 DATE BET 1 JAN 1920 AND 31 DEC 1920\n2 PLAC Berlin\n0 TRLR\n";
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }
}
