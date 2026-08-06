using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core.Services;
using RnzTrauer.Import.Services;

namespace RnzTrauer.Core.Tests;

[TestClass]
public sealed class HtmlSchemaImporterTests
{
    [TestMethod]
    public void Import_CombinesSchemaModeWithAnchorAndTextCallbacks()
    {
        var importer = CreateImporter();

        var result = importer.Import(
            "<a href=\"/rnz/notice.pdf?x=1\">A-100</a>",
            ["[TS: A", "+A"]);

        Assert.IsNotNull(result.CurrentRow);
        Assert.AreEqual("/rnz/notice.pdf", result.CurrentRow[2]);
        Assert.AreEqual("A-100", result.CurrentRow[1]);
    }

    [TestMethod]
    public void Import_EmitsNextFileFromImageMarkup()
    {
        var importer = CreateImporter();

        var result = importer.Import(
            "<img src=\"/rnz/next.pdf?download=1\">",
            ["[TS: IMG", "+N"]);

        Assert.AreEqual(1, result.NewFiles.Count);
        Assert.AreEqual("/rnz/next.pdf", result.NewFiles[0]);
    }

    [TestMethod]
    public void Import_ResetsAccumulatorWhenReused()
    {
        var importer = CreateImporter();
        var schema = new[] { "[TS: A", "+A" };

        var first = importer.Import("<a href=\"/first.pdf?x=1\">A-1</a>", schema);
        var second = importer.Import("<a href=\"/second.pdf?x=1\">A-2</a>", schema);

        Assert.IsNotNull(first.CurrentRow);
        Assert.IsNotNull(second.CurrentRow);
        Assert.AreEqual("A-2", second.CurrentRow[1]);
        Assert.AreEqual("/second.pdf", second.CurrentRow[2]);
        Assert.AreEqual(0, second.CompletedRows.Count);
    }

    [TestMethod]
    public void Import_MatchesImmutableGoldenFixture()
    {
        var fixture = JsonSerializer.Deserialize<GoldenFixture>(
            File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "rnz-import-golden.json")),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.IsNotNull(fixture);

        var result = CreateImporter().Import(fixture.Html, fixture.Schema);

        Assert.IsNotNull(result.CurrentRow);
        Assert.IsTrue(
            fixture.ExpectedColumns.SequenceEqual(result.CurrentRow.Columns),
            "Golden positional columns differ. Actual: " + string.Join("|", result.CurrentRow.Columns));
    }

    [TestMethod]
    public void VieserI5Fixture_AdvancesSchemaThroughNameAnchor()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var html = File.ReadAllText(Path.Combine(fixtureDirectory, "Vieser_I5.html"));
        var schema = File.ReadAllLines(Path.Combine(fixtureDirectory, "Vieser_Schema.txt"));

        var callbacks = new HtmlCallbackTokenizer().Feed(html);
        var filter = new SchemaFilter();
        filter.SetSchema(schema);
        var emissions = new List<SchemaFilterEmission>();

        foreach (var callback in callbacks)
        {
            var token = callback.Kind switch
            {
                HtmlCallbackKind.StartTag => "TS: " + callback.Value.ToUpperInvariant(),
                HtmlCallbackKind.EndTag => "TE: " + callback.Value.ToUpperInvariant(),
                HtmlCallbackKind.StandardText => "S: " + new HtmlTextNormalizer().Normalize(callback.Value),
                _ => string.Empty,
            };
            if (!string.IsNullOrEmpty(token))
                emissions.AddRange(filter.Test(token));
        }

        Assert.IsTrue(emissions.Any(emission => emission.Text == "Name:"));
        Assert.IsTrue(emissions.Any(emission => emission.Text == "Ref:"));
    }

    [TestMethod]
    public void VieserI12Trace_PreservesBurialMarriageAndChildrenFacts()
    {
        var fixtureDirectory = Path.Combine(AppContext.BaseDirectory, "Fixtures", "Pascal");
        var trace = File.ReadAllLines(Path.Combine(fixtureDirectory, "Vieser_I12.Exp"));

        Assert.IsTrue(trace.Any(line => line.Contains("BEGRÄBNIS", StringComparison.Ordinal)));
        Assert.IsTrue(trace.Any(line => line.Contains("HEIRAT", StringComparison.Ordinal)));
        Assert.AreEqual(6, trace.Count(line => line.StartsWith("0: Kind:", StringComparison.Ordinal)));
    }

    private static IHtmlSchemaImporter CreateImporter()
    {
        return new HtmlSchemaImporter(
            new HtmlTextNormalizer(),
            new HtmlCallbackTokenizer(),
            new SchemaFilter(),
            new SchemaImportAccumulator(),
            new HtmlEncodingDecoder());
    }

    private sealed record GoldenFixture(
        string Html,
        IReadOnlyList<string> Schema,
        IReadOnlyList<string> ExpectedColumns);
}
