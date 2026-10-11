using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genealogy.Gedcom;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Publishing.Services;
using OFBCreator.Core.Services;

namespace OFBCreator.Publishing.Tests;

[TestClass]
public sealed class CanonicalGedcomFamilyDataSourceTests
{
    [TestMethod]
    public async Task ImportUsesCanonicalGedcomDriverAndPreservesOFBFamilyBehavior()
    {
        const string gedcom =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 5.5.1\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Alex /Example/\r\n2 GIVN Alex\r\n2 SURN Example\r\n1 SEX M\r\n" +
            "1 BIRT\r\n2 DATE 1 JAN 1900\r\n2 PLAC Example Town\r\n1 OCCU Farmer\r\n" +
            "0 @I2@ INDI\r\n1 NAME Chris /Example/\r\n2 GIVN Chris\r\n2 SURN Example\r\n" +
            "0 @F1@ FAM\r\n1 HUSB @I1@\r\n1 CHIL @I2@\r\n0 TRLR\r\n";
        var source = new CanonicalGedcomFamilyDataSource(
            new GedcomInputDriver(),
            new CanonicalGenealogyAdapter());
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(gedcom));

        var genealogy = await source.ImportAsync(stream);
        var family = genealogy.Entitys.OfType<IGenFamily>().Single();
        Assert.IsNotNull(family.Husband, "Family husband association was not adapted.");
        Assert.AreEqual(1, family.Children.Count, "Family child association was not adapted.");
        Assert.IsFalse(family.Children.Any(child => child is null), "Family child list contains a null child.");
        var father = family.Husband!;
        var child = family.Children.First()!;

        Assert.AreEqual("Alex", father.GivenName);
        Assert.AreEqual("Example Town", father.BirthPlace?.Name);
        Assert.AreEqual("Farmer", father.Occupation);
        Assert.AreSame(family, child.ParentFamily);
    }
}
