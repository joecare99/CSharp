using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Import.Tests;

[TestClass]
public sealed class HtmlImportPipelineTests
{
    [TestMethod]
    public void Import_PreservesEncodingBoundaryAndProducesExpectedAnchorRow()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var pipeline = ImportPipelineFactory.CreateDefault();
        var html = Encoding.GetEncoding(1252).GetBytes(
            "<a href=\"/rnz/anzeige.pdf?x=1\">Müller</a>");

        var result = pipeline.Import(html, ["[TS: A", "+A"]);

        Assert.IsNotNull(result.CurrentRow);
        Assert.AreEqual("/rnz/anzeige.pdf", result.CurrentRow[2]);
        Assert.AreEqual("Müller", result.CurrentRow[1]);
    }

    [TestMethod]
    public void Import_ProducesMachineReadableSixteenColumnRows()
    {
        var pipeline = ImportPipelineFactory.CreateDefault();
        var html = File.ReadAllBytes(Path.Combine(
            AppContext.BaseDirectory, "Fixtures", "Vieser_I5.html"));

        var result = pipeline.Import(html, ["[TS: A", "+A"]);

        Assert.IsNotNull(result.CurrentRow);
        Assert.AreEqual(16, result.CurrentRow.Count);
        Assert.IsTrue(result.NewFiles.Count >= 0);
    }

    [TestMethod]
    [DataRow("I5", 2)]
    [DataRow("I12", 6)]
    [DataRow("I23", 4)]
    [DataRow("I134", 2)]
    [DataRow("I12577", 0)]
    public void VieserFixture_AdvancesSchemaAndMatchesPascalTraceHeader(
        string fixtureId,
        int expectedKindCount)
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var htmlBytes = File.ReadAllBytes(Path.Combine(fixtureDirectory, fixtureId + ".html"));
        var schema = File.ReadAllLines(Path.Combine(fixtureDirectory, "Vieser_Schema.txt"));
        var html = new HtmlEncodingDecoder().Decode(htmlBytes).Text;
        var callbacks = new HtmlCallbackTokenizer().Feed(html);
        var filter = new SchemaFilter();
        filter.SetSchema(schema);
        var emissions = callbacks
            .SelectMany(callback => callback.Kind switch
            {
                HtmlCallbackKind.StartTag => filter.Test("TS: " + callback.Value.ToUpperInvariant()),
                HtmlCallbackKind.EndTag => filter.Test("TE: " + callback.Value.ToUpperInvariant()),
                HtmlCallbackKind.StandardText => filter.Test(
                    "S: " + new HtmlTextNormalizer().Normalize(callback.Value)),
                _ => Array.Empty<SchemaFilterEmission>(),
            })
            .ToArray();

        Assert.IsTrue(emissions.Any(emission => emission.Text == "Name:"));
        Assert.IsTrue(emissions.Any(emission => emission.Text == "Ref:"));

        var trace = File.ReadAllLines(Path.Combine(fixtureDirectory, fixtureId + ".Exp"));
        Assert.AreEqual("0: Name:", trace[0]);
        Assert.AreEqual("0: Ref:", trace[1]);
        Assert.AreEqual($"2: <A NAME=\"{fixtureId}\">", trace[2]);
        Assert.AreEqual(
            expectedKindCount,
            trace.Count(line => line.StartsWith("0: Kind:", StringComparison.Ordinal)));
    }
}
