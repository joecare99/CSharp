using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

[TestClass]
public sealed class OccupationDesignationParserTests
{
    [TestMethod]
    [DataRow("Ackersmann, Taglöhner", "Ackersmann", "Taglöhner")]
    [DataRow("Bürger und Schöffe", "Bürger", "Schöffe")]
    [DataRow("Bauer & Schmied", "Bauer", "Schmied")]
    public void Parse_SplitsCompoundDesignations(string value, string first, string second)
    {
        var result = OccupationDesignationParser.Parse(value);

        CollectionAssert.AreEqual(new[] { first, second }, result.Select(designation => designation.Name).ToArray());
    }

    [TestMethod]
    public void Parse_PreservesTrailingPlaceContextForAllDesignations()
    {
        var result = OccupationDesignationParser.Parse("Ackersmann, Taglöhner in Ulm, Baden-Württemberg");

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Ulm, Baden-Württemberg", result[0].Place);
        Assert.AreEqual("Ulm, Baden-Württemberg", result[1].Place);
        Assert.IsNull(result[0].Employer);
    }

    [TestMethod]
    public void Parse_PreservesTrailingEmployerContextWithoutIndexingItAsOccupation()
    {
        var result = OccupationDesignationParser.Parse("Bauer & Schmied bei Graf Müller");

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Bauer", result[0].Name);
        Assert.AreEqual("Schmied", result[1].Name);
        Assert.AreEqual("Graf Müller", result[0].Employer);
        Assert.AreEqual("Graf Müller", result[1].Employer);
        Assert.IsTrue(result.All(designation => designation.Place is null));
    }

    [TestMethod]
    public void Parse_EmptyValue_ReturnsNoDesignations()
    {
        Assert.AreEqual(0, OccupationDesignationParser.Parse("  ").Count);
    }
}
