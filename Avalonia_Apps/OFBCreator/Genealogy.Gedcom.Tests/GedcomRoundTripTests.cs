using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genealogy.Drivers;
using Genealogy.Gedcom;
using Genealogy.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Genealogy.Gedcom.Tests;

[TestClass]
public sealed class GedcomRoundTripTests
{
    private readonly GedcomInputDriver _input = new();
    private readonly GedcomOutputDriver _output = new();

    [TestMethod]
    [DataRow("5.5.1", "5.5.1")]
    [DataRow("7.0", "7.0")]
    public async Task SameVersionRoundTripRetainsSemanticTree(string sourceVersion, string targetVersion)
    {
        var source = CreateGedcom(sourceVersion);
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(source));
        var document = await _input.ReadAsync(input);

        Assert.IsFalse(document.HasRecoveryIssues);
        Assert.AreEqual(6, document.Records.Count);
        Assert.AreEqual("Person", document.Records[0].Kind);
        Assert.IsInstanceOfType<GenealogyPerson>(document.Records[0]);
        Assert.AreEqual("Given", document.Records[0].GivenName);
        Assert.AreEqual("Example", document.Records[0].Surname);
        Assert.IsTrue(document.Records[0].Content.Any(node => node.TypeCode == "_PRIVATE"));
        Assert.AreEqual("Godparent", document.Records[0].Associations[0].Role);
        Assert.AreEqual(document.Records[1].Id, document.Records[0].Associations[0].TargetRecordId);
        Assert.AreEqual("Husband", document.Records[2].Associations[0].Role);
        Assert.AreEqual("Child", document.Records[2].Associations[1].Role);

        using var output = new MemoryStream();
        await _output.WriteAsync(document, output, new GenealogyOutputOptions { TargetVersion = targetVersion });
        output.Position = 0;
        var reparsed = await _input.ReadAsync(output);

        Assert.IsFalse(reparsed.HasRecoveryIssues);
        Assert.AreEqual(targetVersion, FindVersion(output.ToArray()));
        Assert.AreEqual(6, reparsed.Records.Count);
        for (var index = 0; index < document.Records.Count; index++)
            AssertRecordContentEqual(document.Records[index], reparsed.Records[index]);
        Assert.AreEqual(document.OtherContent.Count, reparsed.OtherContent.Count);
        for (var index = 0; index < document.OtherContent.Count; index++)
            AssertNodeEqual(document.OtherContent[index], reparsed.OtherContent[index]);
    }

    [TestMethod]
    public async Task DefaultOutputVersionIsHighestSupportedAndUtf8WithoutBom()
    {
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(CreateGedcom("5.5.1")));
        var document = await _input.ReadAsync(input);
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output, new GenealogyOutputOptions());

        Assert.IsFalse(output.ToArray().AsSpan().StartsWith(Encoding.UTF8.Preamble));
        Assert.AreEqual(GedcomVersion.HighestSupported, FindVersion(output.ToArray()));
    }

    [TestMethod]
    public async Task CrossVersionOutputRetainsExtensionsAndReportsConversion()
    {
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(CreateGedcom(GedcomVersion.Gedcom70)));
        var document = await _input.ReadAsync(input);
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output,
            new GenealogyOutputOptions { TargetVersion = GedcomVersion.Gedcom551 });

        Assert.AreEqual(GedcomVersion.Gedcom551, FindVersion(output.ToArray()));
        Assert.IsTrue(document.Diagnostics.Any(d => d.Code == "GEDCOM_VERSION_CONVERTED"));
        output.Position = 0;
        var converted = await _input.ReadAsync(output);
        Assert.IsTrue(converted.Records[0].Content.Any(node => node.TypeCode == "_PRIVATE"));
        Assert.IsTrue(converted.Records[0].Content.Any(node => node.TypeCode == "ASSO"));
    }

    [TestMethod]
    public async Task LegacyToGedcom7MapsKnownStructuresAndFallsBackForRemovedRecords()
    {
        var source = CreateGedcom(GedcomVersion.Gedcom551)
            .Replace("1 SEX M\r\n", "1 SEX M\r\n1 AFN 12345\r\n1 NAME Given /Example/\r\n2 ROMN Given /Example/\r\n3 TYPE romaji\r\n", StringComparison.Ordinal)
            .Replace("2 SURN Example\r\n", "2 SURN Example\r\n2 TYPE birth\r\n", StringComparison.Ordinal)
            .Replace("1 SEX M\r\n", "1 SEX M\r\n1 RESN privacy\r\n1 FAMC @F1@\r\n2 PEDI birth\r\n2 STAT proven\r\n1 SLGC\r\n2 STAT dns/can\r\n", StringComparison.Ordinal)
            .Replace("2 RELA Godparent", "2 RELA Witness", StringComparison.Ordinal)
            .Replace("1 NOTE first line\r\n2 CONT second line", "1 NOTE first line\r\n2 CONC joined\r\n2 CONT second line", StringComparison.Ordinal)
            .Replace("0 TRLR", "0 @N1@ NOTE shared note\r\n1 SOUR @S1@\r\n0 @SN1@ SUBN\r\n1 SUBM @I1@\r\n0 TRLR", StringComparison.Ordinal)
            .Replace("1 _PRIVATE private value", "1 NOTE @N1@\r\n1 _PRIVATE private value", StringComparison.Ordinal);
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(source));
        var document = await _input.ReadAsync(input);
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output,
            new GenealogyOutputOptions { TargetVersion = GedcomVersion.Gedcom70 });

        var text = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(text.Contains("1 EXID 12345", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 TYPE https://gedcom.io/terms/v7/AFN", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 TRAN Given /Example/", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("3 LANG ja-Latn", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 ROLE WITN", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 TYPE BIRTH", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("1 RESN PRIVACY", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 PEDI BIRTH", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 STAT PROVEN", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 STAT DNS_CAN", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("1 SNOTE @N1@", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("0 @N1@ SNOTE shared note", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("first linejoined", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("[GEDCOM conversion fallback:", StringComparison.Ordinal));
        Assert.IsTrue(document.Diagnostics.Any(d => d.Code == "GEDCOM_VERSION_FALLBACK"));
        Assert.IsTrue(document.Diagnostics.Any(d => d.Code == "GEDCOM_RELA_TO_ROLE"));
    }

    [TestMethod]
    public async Task Gedcom7ToLegacyMapsSupportedStructuresAndMarksUnknownData()
    {
        const string source =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 7.0\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Given /Example/\r\n2 TYPE BIRTH\r\n1 RESN PRIVACY\r\n" +
            "1 FAMC @F1@\r\n2 PEDI BIRTH\r\n2 STAT PROVEN\r\n1 SLGC\r\n2 STAT DNS_CAN\r\n" +
            "1 ASSO @I2@\r\n2 ROLE OTHER\r\n3 PHRASE Honorary uncle\r\n" +
            "1 EXID 12345\r\n2 TYPE https://gedcom.io/terms/v7/AFN\r\n" +
            "1 EXID custom-value\r\n2 TYPE https://example.org/identifier/custom\r\n" +
            "1 SNOTE @N1@\r\n0 @I2@ INDI\r\n0 @F1@ FAM\r\n1 CHIL @I1@\r\n" +
            "0 @N1@ SNOTE Shared text\r\n0 TRLR\r\n";
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(source));
        var document = await _input.ReadAsync(input);
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output,
            new GenealogyOutputOptions { TargetVersion = GedcomVersion.Gedcom551 });

        var text = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(text.Contains("2 RELA Honorary uncle", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("1 AFN 12345", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("1 NOTE @N1@", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("0 @N1@ NOTE Shared text", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 TYPE birth", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("1 RESN privacy", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 PEDI birth", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 STAT proven", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 STAT DNS/CAN", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("[GEDCOM conversion fallback:", StringComparison.Ordinal));
        Assert.IsTrue(text.Contains("2 CONC ", StringComparison.Ordinal));
        Assert.IsTrue(text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .All(line => line.Length <= 255));
        Assert.IsTrue(document.Diagnostics.Any(d => d.Code == "GEDCOM_VERSION_FALLBACK"));
    }

    [TestMethod]
    public async Task CanonicalContentEditsAreWrittenWithoutDroppingSourceExtensions()
    {
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(CreateGedcom(GedcomVersion.Gedcom551)));
        var document = await _input.ReadAsync(input);
        document.Records[0].GivenName = "Changed";
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output, new GenealogyOutputOptions());

        output.Position = 0;
        var reparsed = await _input.ReadAsync(output);
        Assert.AreEqual("Changed", reparsed.Records[0].GivenName);
        Assert.IsTrue(reparsed.Records[0].Content.Any(node => node.TypeCode == "_PRIVATE"));
        Assert.IsTrue(reparsed.Records[0].Content.Single(node => node.TypeCode == "NAME")
            .Children.Any(node => node.TypeCode == "SURN" && node.Value == "Example"));
    }

    [TestMethod]
    public async Task CanonicalAssociationEditsAreWritten()
    {
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(CreateGedcom(GedcomVersion.Gedcom551)));
        var document = await _input.ReadAsync(input);
        var family = document.Records[2];
        family.Associations[1].TargetRecordId = document.Records[0].Id;
        family.Associations[1].TargetIdentifier = new GenealogyIdentifier { Provider = "gedcom", Value = "@I1@" };
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output, new GenealogyOutputOptions());

        output.Position = 0;
        var reparsed = await _input.ReadAsync(output);
        Assert.AreEqual("@I1@", reparsed.Records[2].Content.Single(node => node.TypeCode == "CHIL").Value);
    }

    [TestMethod]
    public async Task CanonicalDocumentCanBeWrittenWithoutProviderSnapshot()
    {
        var document = new GenealogyDocument();
        var person = new GenealogyPerson { GivenName = "New", Surname = "Person" };
        person.Identifiers.Add(new GenealogyIdentifier { Provider = "gedcom", Value = "@I1@" });
        var name = new GenealogyFact { TypeCode = "NAME", Value = "New /Person/" };
        name.Children.Add(new GenealogyFact { TypeCode = "GIVN", Value = "New" });
        name.Children.Add(new GenealogyFact { TypeCode = "SURN", Value = "Person" });
        person.Content.Add(name);
        document.Records.Add(person);
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output, new GenealogyOutputOptions());

        output.Position = 0;
        var imported = await _input.ReadAsync(output);
        Assert.AreEqual(GedcomVersion.HighestSupported, FindVersion(output.ToArray()));
        Assert.AreEqual("New", imported.Records[0].GivenName);
        Assert.AreEqual("@I1@", imported.Records[0].Identifiers[0].Value);
    }

    [TestMethod]
    public async Task MalformedLinesAreDiagnosedAndCannotBeExportedAsAcceptedRoundTrip()
    {
        var malformed = CreateGedcom("5.5.1").Replace("1 NAME Given /Example/", "not a gedcom record", StringComparison.Ordinal);
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(malformed));
        var document = await _input.ReadAsync(input);

        Assert.IsTrue(document.HasRecoveryIssues);
        Assert.IsTrue(document.Diagnostics.Any(d => d.Code == "GEDCOM_MALFORMED_LINE"));
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _output.WriteAsync(document, new MemoryStream(), new GenealogyOutputOptions()));
    }

    [TestMethod]
    public async Task RecoveredDocumentCanOnlyBeWrittenWhenBestEffortIsExplicitlyAllowed()
    {
        var malformed = CreateGedcom("5.5.1").Replace("1 NAME Given /Example/", "not a gedcom record", StringComparison.Ordinal);
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(malformed));
        var document = await _input.ReadAsync(input);
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output, new GenealogyOutputOptions { AllowRecovery = true });

        var written = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(written.StartsWith("0 HEAD", StringComparison.Ordinal));
        Assert.IsFalse(written.Contains("not a gedcom record", StringComparison.Ordinal));
        Assert.IsTrue(document.Diagnostics.Any(d => d.Code == "GEDCOM_BEST_EFFORT_OUTPUT"
            && d.Severity == GenealogyDiagnosticSeverity.Warning));
    }

    [TestMethod]
    public async Task DeclaredLegacyCharsetIsDecodedAndOutputHeaderIsNormalizedToUtf8()
    {
        var source = CreateGedcom(GedcomVersion.Gedcom551)
            .Replace("1 CHAR UTF-8", "1 CHAR ASCII", StringComparison.Ordinal)
            .Replace("Given /Example/", "M?nchen /Example/", StringComparison.Ordinal);
        var bytes = Encoding.ASCII.GetBytes(source);
        var marker = Array.IndexOf(bytes, (byte)'?');
        Assert.IsTrue(marker >= 0);
        bytes[marker] = 0xFC;
        using var input = new MemoryStream(bytes);
        var document = await _input.ReadAsync(input);

        Assert.AreEqual("München Example", document.Records[0].DisplayName);
        Assert.IsTrue(document.Diagnostics.Any(d => d.Code == "GEDCOM_LEGACY_ENCODING"));
        using var output = new MemoryStream();

        await _output.WriteAsync(document, output, new GenealogyOutputOptions { AllowRecovery = true });

        var written = Encoding.UTF8.GetString(output.ToArray());
        Assert.IsTrue(written.Contains("1 CHAR UTF-8", StringComparison.Ordinal));
        Assert.IsTrue(written.Contains("München", StringComparison.Ordinal));
    }

    [TestMethod]
    public async Task UnresolvedReferencesAreRetainedAndMarkTheImportAsRecovered()
    {
        var invalid = CreateGedcom(GedcomVersion.Gedcom551)
            .Replace("1 CHIL @I2@", "1 CHIL @IX@", StringComparison.Ordinal);
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(invalid));
        var document = await _input.ReadAsync(input);

        Assert.IsTrue(document.HasRecoveryIssues);
        Assert.IsTrue(document.Diagnostics.Any(d => d.Code == "GEDCOM_UNRESOLVED_REFERENCE"));
        Assert.AreEqual("@IX@", document.Records[2].Associations[1].TargetIdentifier?.Value);
    }

    [TestMethod]
    public async Task DuplicateXrefsReportBothRecordTypesAndDefinitionLines()
    {
        const string source =
            "0 HEAD\r\n1 GEDC\r\n2 VERS 5.5.1\r\n2 FORM LINEAGE-LINKED\r\n" +
            "0 @X1@ INDI\r\n1 NAME First /Record/\r\n" +
            "0 @X1@ FAM\r\n0 TRLR\r\n";
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(source));

        var document = await _input.ReadAsync(input);

        var diagnostic = document.Diagnostics.Single(item => item.Code == "GEDCOM_DUPLICATE_XREF");
        Assert.AreEqual(7, diagnostic.LineNumber);
        Assert.AreEqual("@X1@", diagnostic.Subject);
        StringAssert.Contains(diagnostic.Message, "INDI at line 5");
        StringAssert.Contains(diagnostic.Message, "FAM at line 7");
        StringAssert.Contains(diagnostic.Message, "first definition remains the reference target");
    }

    [TestMethod]
    public async Task InputDriverRecognizesHeaderAndRestoresStreamPosition()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(CreateGedcom("7.0")));
        stream.Position = 3;

        Assert.IsTrue(_input.CanRead(stream));
        Assert.AreEqual(3, stream.Position);
    }

    private static string CreateGedcom(string version)
    {
        return
            $"0 HEAD\r\n1 SOUR TEST\r\n1 GEDC\r\n2 VERS {version}\r\n2 FORM LINEAGE-LINKED\r\n1 CHAR UTF-8\r\n" +
            "0 @I1@ INDI\r\n1 NAME Given /Example/\r\n2 GIVN Given\r\n2 SURN Example\r\n1 SEX M\r\n" +
            "1 BIRT\r\n2 DATE 1 JAN 1900\r\n2 PLAC Test Town\r\n1 OCCU Farmer\r\n2 DATE BET 1920 AND 1930\r\n" +
            "1 ASSO @I2@\r\n2 RELA Godparent\r\n1 NOTE first line\r\n2 CONT second line\r\n" +
            "1 _PRIVATE private value\r\n2 _NESTED nested value\r\n" +
            "0 @I2@ INDI\r\n1 NAME Child /Example/\r\n2 GIVN Child\r\n2 SURN Example\r\n" +
            "0 @F1@ FAM\r\n1 HUSB @I1@\r\n1 CHIL @I2@\r\n1 MARR\r\n2 DATE 1 JAN 1920\r\n" +
            "1 SOUR @S1@\r\n2 PAGE 3\r\n0 @S1@ SOUR\r\n1 TITL Sample source\r\n1 REPO @R1@\r\n" +
            "0 @R1@ REPO\r\n1 NAME Sample archive\r\n0 @M1@ OBJE\r\n1 FILE image.jpg\r\n2 FORM jpg\r\n" +
            "0 TRLR\r\n";
    }

    private static string FindVersion(byte[] bytes)
    {
        var text = Encoding.UTF8.GetString(bytes);
        var inGedcomHeader = false;
        foreach (var line in text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.StartsWith("1 GEDC", StringComparison.Ordinal))
            {
                inGedcomHeader = true;
                continue;
            }

            if (inGedcomHeader && line.StartsWith("1 ", StringComparison.Ordinal))
                inGedcomHeader = false;
            if (inGedcomHeader && line.StartsWith("2 VERS ", StringComparison.Ordinal))
                return line["2 VERS ".Length..];
        }
        Assert.Fail("GEDCOM header does not contain a GEDC/VERS line.");
        return string.Empty;
    }

    private static void AssertRecordContentEqual(GenealogyRecord expected, GenealogyRecord actual)
    {
        Assert.AreEqual(expected.Kind, actual.Kind);
        Assert.AreEqual(expected.TypeCode, actual.TypeCode);
        Assert.AreEqual(expected.DisplayName, actual.DisplayName);
        Assert.AreEqual(expected.GivenName, actual.GivenName);
        Assert.AreEqual(expected.Surname, actual.Surname);
        Assert.AreEqual(expected.Sex, actual.Sex);
        Assert.AreEqual(expected.Identifiers.Count, actual.Identifiers.Count);
        for (var index = 0; index < expected.Identifiers.Count; index++)
        {
            Assert.AreEqual(expected.Identifiers[index].Provider, actual.Identifiers[index].Provider);
            Assert.AreEqual(expected.Identifiers[index].Value, actual.Identifiers[index].Value);
        }
        Assert.AreEqual(expected.Associations.Count, actual.Associations.Count);
        for (var index = 0; index < expected.Associations.Count; index++)
        {
            Assert.AreEqual(expected.Associations[index].Role, actual.Associations[index].Role);
            Assert.AreEqual(expected.Associations[index].Detail, actual.Associations[index].Detail);
            Assert.AreEqual(expected.Associations[index].TargetIdentifier?.Value,
                actual.Associations[index].TargetIdentifier?.Value);
        }
        for (var index = 0; index < expected.Content.Count; index++)
            AssertNodeEqual(expected.Content[index], actual.Content[index]);
    }

    private static void AssertNodeEqual(GenealogyNode expected, GenealogyNode actual)
    {
        Assert.AreEqual(expected.GetType(), actual.GetType());
        Assert.AreEqual(expected.TypeCode, actual.TypeCode);
        Assert.AreEqual(expected.Value, actual.Value);
        Assert.AreEqual((expected as UserDefinedEvent)?.Context, (actual as UserDefinedEvent)?.Context);
        Assert.AreEqual(expected.References.Count, actual.References.Count);
        for (var index = 0; index < expected.References.Count; index++)
        {
            Assert.AreEqual(expected.References[index].Provider, actual.References[index].Provider);
            Assert.AreEqual(expected.References[index].Value, actual.References[index].Value);
        }
        Assert.AreEqual(expected.Children.Count, actual.Children.Count);
        for (var index = 0; index < expected.Children.Count; index++)
            AssertNodeEqual(expected.Children[index], actual.Children[index]);
    }
}
