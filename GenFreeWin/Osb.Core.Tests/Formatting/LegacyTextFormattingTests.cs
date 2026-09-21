using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Formatting;

namespace Osb.Core.Tests.Formatting;

[TestClass]
public sealed class LegacyTextFormattingTests
{
    [TestMethod]
    public void FormatGeoCoordinate_FormatsCommaSeparatedCoordinate()
    {
        Assert.AreEqual("51\u00b0 12' 34''", LegacyTextFormatting.FormatGeoCoordinate("51,1234"));
    }

    [TestMethod]
    public void FormatGeoCoordinate_AcceptsDotSeparator()
    {
        Assert.AreEqual("7\u00b0 05' 09''", LegacyTextFormatting.FormatGeoCoordinate("7.0509"));
    }

    [TestMethod]
    public void FormatName_CombinesNamePartsAndUppercasesName()
    {
        Assert.AreEqual("Dr. MAX Mustermann", LegacyTextFormatting.FormatName("Max", "Dr.", "Mustermann"));
    }

    [TestMethod]
    public void FormatName_OmitsEmptyParts()
    {
        Assert.AreEqual("MAX", LegacyTextFormatting.FormatName("Max", string.Empty, string.Empty));
    }

    [TestMethod]
    public void RemoveQuotes_RemovesAllDoubleQuotes()
    {
        Assert.AreEqual("Max Mustermann", LegacyTextFormatting.RemoveQuotes("\"Max\" \"Mustermann\""));
    }

    [TestMethod]
    public void DecodeLegacyText_ReplacesEncodedSharpS()
    {
        Assert.AreEqual("Straße", LegacyTextFormatting.DecodeLegacyText("Stra ssss e".Replace(" ", "")));
    }

    [TestMethod]
    public void DecodeLegacyText_PreservesOtherText()
    {
        Assert.AreEqual("Mustertext", LegacyTextFormatting.DecodeLegacyText("Mustertext"));
    }

    [TestMethod]
    public void EncodeLegacyText_ReplacesSharpSWithLegacyToken()
    {
        Assert.AreEqual("Stra ssss e".Replace(" ", ""), LegacyTextFormatting.EncodeLegacyText("Straße"));
    }

    [TestMethod]
    public void EncodeAndDecodeLegacyText_RoundTripsSharpS()
    {
        const string original = "Straße aus Köln";

        Assert.AreEqual(original, LegacyTextFormatting.DecodeLegacyText(LegacyTextFormatting.EncodeLegacyText(original)));
    }

    [TestMethod]
    public void NormalizeText_ReplacesLineBreaksAndCollapsesSpaces()
    {
        Assert.AreEqual("A B C", LegacyTextFormatting.NormalizeText("A  B\r\n C", true));
    }

    [TestMethod]
    public void NormalizeText_CanPreserveLineBreaks()
    {
        Assert.AreEqual("A\n B", LegacyTextFormatting.NormalizeText("A\n  B", false));
    }

    [TestMethod]
    public void LegacyFormattingOptions_ReadsLegacySlotsInvariantly()
    {
        var aus = new string[206];
        var oAus = new string[21];
        aus[46] = "1";
        aus[108] = "1.0";
        aus[173] = "20201231";
        aus[176] = "1";
        aus[185] = "0";
        oAus[5] = "1";

        var actual = LegacyFormattingOptions.FromLegacySlots(aus, oAus);

        Assert.IsTrue(actual.QualifyDatesWithAm);
        Assert.IsTrue(actual.SuppressDatesAfterCutoff);
        Assert.AreEqual("20201231", actual.DateCutoff);
        Assert.IsTrue(actual.UseReplacementDateMarker);
        Assert.IsFalse(actual.PreserveLineBreaks);
        Assert.IsTrue(actual.IncludeSourceDate);
    }

    [TestMethod]
    public void LegacyFormattingOptions_UsesSafeDefaultsForMissingSlots()
    {
        var actual = LegacyFormattingOptions.FromLegacySlots(new string[0], new string[0]);

        Assert.IsFalse(actual.QualifyDatesWithAm);
        Assert.IsFalse(actual.SuppressDatesAfterCutoff);
        Assert.AreEqual(string.Empty, actual.DateCutoff);
        Assert.IsFalse(actual.UseReplacementDateMarker);
        Assert.IsFalse(actual.PreserveLineBreaks);
        Assert.IsFalse(actual.IncludeSourceDate);
    }

    [TestMethod]
    public void FormatLegacyDate_FormatsYearMonthAndDay()
    {
        Assert.AreEqual("31.12.1980", LegacyTextFormatting.FormatLegacyDate("19801231"));
    }

    [TestMethod]
    public void FormatLegacyDate_UsesDotsForMissingDayOrMonth()
    {
        Assert.AreEqual("1980", LegacyTextFormatting.FormatLegacyDate("19800000"));
    }

    [TestMethod]
    public void FormatLegacyDate_PadsShortLegacyValues()
    {
        Assert.AreEqual("01.01.1980", LegacyTextFormatting.FormatLegacyDate("19800101"));
        Assert.AreEqual("1980", LegacyTextFormatting.FormatLegacyDate("1980"));
    }

    [TestMethod]
    [DataRow("", "31.12.1980")]
    [DataRow("U", "um 31.12.1980")]
    [DataRow("V", "vor 31.12.1980")]
    [DataRow("N", "nach 31.12.1980")]
    [DataRow("R", "errech. 31.12.1980")]
    [DataRow("Z", "zwischen 31.12.1980")]
    [DataRow("A", " und 31.12.1980")]
    [DataRow("B", " bis 31.12.1980")]
    [DataRow("C", "calc. 31.12.1980")]
    public void LegacyDateFormatting_AppliesQualifiers(string qualifier, string expected)
    {
        var options = LegacyFormattingOptions.FromLegacySlots(new string[206], new string[21]);
        var result = LegacyDateFormatting.Format(
            new LegacyDateFormattingRequest("19801231", qualifier, options));

        Assert.AreEqual(expected, result.Text);
        Assert.IsFalse(result.WasSuppressed);
    }

    [TestMethod]
    public void LegacyDateFormatting_SuppressesDatesAfterCutoff()
    {
        var aus = new string[206];
        aus[108] = "1";
        aus[173] = "19800101";
        var options = LegacyFormattingOptions.FromLegacySlots(aus, new string[21]);

        var result = LegacyDateFormatting.Format(
            new LegacyDateFormattingRequest("19801231", string.Empty, options));

        Assert.AreEqual(string.Empty, result.Text);
        Assert.IsTrue(result.WasSuppressed);
    }

    [TestMethod]
    public void LegacyDateFormatting_AppliesReplacementMarkerOnlyWhenAllowed()
    {
        var aus = new string[206];
        aus[176] = "1";
        var options = LegacyFormattingOptions.FromLegacySlots(aus, new string[21]);

        Assert.AreEqual(
            "1980",
            LegacyDateFormatting.Format(
                new LegacyDateFormattingRequest("19800000", string.Empty, options)).Text);
        Assert.AreEqual(
            "1980",
            LegacyDateFormatting.Format(
                new LegacyDateFormattingRequest("19800000", string.Empty, options, 1)).Text);
    }

    [TestMethod]
    public void FormatRepositoryLocation_OrdersAndSeparatesFields()
    {
        Assert.AreEqual(
            "Standort: Musterarchiv, Hauptstraße, 12345 Ort, 0123, info@example.test",
            LegacyTextFormatting.FormatRepositoryLocation(
                "Musterarchiv",
                "Hauptstraße",
                "12345",
                "Ort",
                "0123",
                "info@example.test"));
    }

    [TestMethod]
    public void FormatRepositoryLocation_OmitsEmptyFields()
    {
        Assert.AreEqual(
            "Standort: Ort,",
            LegacyTextFormatting.FormatRepositoryLocation("", "", "", "Ort", "", ""));
    }

    [TestMethod]
    public void LegacyTextLookupResult_NormalizesAndDecodesValues()
    {
        var result = LegacyTextLookupResult.FromRawValues("  Stra ssss e  ".Replace(" ", ""), "  Leitssssname ");

        Assert.AreEqual("Straße", result.Text);
        Assert.AreEqual("Leitßname", result.LeadName);
    }

    [TestMethod]
    public void LegacyTextLookupResult_PreservesMissingLeadName()
    {
        var result = LegacyTextLookupResult.FromRawValues(null, null);

        Assert.AreEqual(string.Empty, result.Text);
        Assert.IsNull(result.LeadName);
    }
}
