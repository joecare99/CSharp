using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OFBCreator.Core.Services;

namespace OFBCreator.Core.Tests.Services;

[TestClass]
public sealed class OFBDateEstimationServiceTests
{
    [TestMethod]
    public async Task Apply_EstimatesMotherAndFatherFromChildrenAndPreservesSourceGraph()
    {
        var source = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: null,
            fatherBirth: "1875",
            childDates: ["1900", "1920"]));
        var sourceFamily = source.Entitys.OfType<IGenFamily>().Single();
        var sourceMother = sourceFamily.Wife!;

        var export = CreateDetachedExport(source, sourceFamily);
        var exportFamily = export.Entitys.OfType<IGenFamily>().Single();
        var diagnostics = new OFBDateEstimationService().Apply([exportFamily]);

        Assert.AreEqual("EST 1880", exportFamily.Wife?.BirthDate?.DateText);
        Assert.AreEqual(EDateModifier.Estimated, exportFamily.Wife?.BirthDate?.eDateModifier);
        Assert.IsNull(sourceMother.BirthDate);
        Assert.AreEqual("1875", exportFamily.Husband?.BirthDate?.DateText);
        Assert.AreEqual(0, diagnostics.Count);
        Assert.AreEqual(1, exportFamily.Wife!.Facts.Count(fact => fact?.eFactType == EFactType.Birth));
    }

    [TestMethod]
    public async Task Apply_UsesFiveYearGridWhenMotherInferenceUsesEstimatedChildDates()
    {
        var source = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: null,
            fatherBirth: null,
            childDates: ["EST 1902", "EST 1924"]));
        var sourceFamily = source.Entitys.OfType<IGenFamily>().Single();
        var exportFamily = CreateDetachedExport(source, sourceFamily).Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([exportFamily]);

        Assert.AreEqual("EST 1880", exportFamily.Wife?.BirthDate?.DateText);
    }

    [TestMethod]
    public async Task Apply_ExcludesPreCutoffChildDatesFromEstimationEvidence()
    {
        var source = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: null,
            fatherBirth: null,
            childDates: ["899", "1900"]));
        var sourceFamily = source.Entitys.OfType<IGenFamily>().Single();
        var exportFamily = CreateDetachedExport(source, sourceFamily).Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([exportFamily], cutoffYear: 900);

        Assert.AreEqual("EST 1880", exportFamily.Wife?.BirthDate?.DateText);
        Assert.AreEqual("899", sourceFamily.Children.ElementAt(0).BirthDate?.DateText);
    }

    [TestMethod]
    public async Task Apply_PreservesExistingBirthDatesBeforeCutoff()
    {
        var source = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: "899",
            fatherBirth: null,
            childDates: ["1900", "1920"]));
        var sourceFamily = source.Entitys.OfType<IGenFamily>().Single();
        var exportFamily = CreateDetachedExport(source, sourceFamily).Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([exportFamily], cutoffYear: 900);

        Assert.AreEqual("899", exportFamily.Wife?.BirthDate?.DateText);
    }

    [TestMethod]
    public async Task Apply_DoesNotCreateEstimatesBeforeCutoff()
    {
        var source = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: null,
            fatherBirth: null,
            childDates: ["910", "914"]));
        var sourceFamily = source.Entitys.OfType<IGenFamily>().Single();
        var exportFamily = CreateDetachedExport(source, sourceFamily).Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([exportFamily], cutoffYear: 900);

        Assert.IsNull(exportFamily.Wife?.BirthDate);
        Assert.IsNull(exportFamily.Husband?.BirthDate);
    }

    [TestMethod]
    public async Task Apply_TreatsYearZeroAsMissingAndNeverCreatesYearZero()
    {
        var source = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: null,
            fatherBirth: null,
            childDates: ["0", "1900"]));
        var sourceFamily = source.Entitys.OfType<IGenFamily>().Single();
        var exportFamily = CreateDetachedExport(source, sourceFamily).Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([exportFamily], cutoffYear: null);

        Assert.AreEqual("EST 1880", exportFamily.Wife?.BirthDate?.DateText);

        var yearZeroSource = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: null,
            fatherBirth: null,
            childDates: ["20", "24"]));
        var yearZeroFamily = yearZeroSource.Entitys.OfType<IGenFamily>().Single();
        var yearZeroExport = CreateDetachedExport(yearZeroSource, yearZeroFamily)
            .Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([yearZeroExport], cutoffYear: null);

        Assert.IsNull(yearZeroExport.Wife?.BirthDate);
        Assert.IsNull(yearZeroExport.Husband?.BirthDate);
    }

    [TestMethod]
    public async Task Apply_RetainsExistingBirthDateAndWarnsForLargeConflict()
    {
        var source = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: "1840",
            fatherBirth: "1875",
            childDates: ["1900", "1920"]));
        var sourceFamily = source.Entitys.OfType<IGenFamily>().Single();
        var exportFamily = CreateDetachedExport(source, sourceFamily).Entitys.OfType<IGenFamily>().Single();

        var diagnostics = new OFBDateEstimationService().Apply([exportFamily]);

        Assert.AreEqual("1840", exportFamily.Wife?.BirthDate?.DateText);
        var conflict = diagnostics.Single(diagnostic =>
            diagnostic.Code == "DATE_ESTIMATE_CONFLICT" && diagnostic.Message.Contains("mother-from-children", StringComparison.Ordinal));
        StringAssert.Contains(conflict.Message, "retained date 1840");
    }

    [TestMethod]
    public async Task Apply_UsesFiveYearSpousalAgeDifferenceInCorrectDirection()
    {
        var wifeBirthSource = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: "1880",
            fatherBirth: null,
            childDates: ["1900", "1924"]));
        var wifeBirthFamily = wifeBirthSource.Entitys.OfType<IGenFamily>().Single();
        var wifeBirthExport = CreateDetachedExport(wifeBirthSource, wifeBirthFamily)
            .Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([wifeBirthExport]);

        Assert.AreEqual("EST 1875", wifeBirthExport.Husband?.BirthDate?.DateText);

        var husbandBirthSource = await ImportAsync(CreateParentAndChildrenGedcom(
            motherBirth: null,
            fatherBirth: "1875",
            childDates: ["1900", "1924"]));
        var husbandBirthFamily = husbandBirthSource.Entitys.OfType<IGenFamily>().Single();
        var husbandBirthExport = CreateDetachedExport(husbandBirthSource, husbandBirthFamily)
            .Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([husbandBirthExport]);

        Assert.AreEqual("EST 1880", husbandBirthExport.Wife?.BirthDate?.DateText);
    }

    [TestMethod]
    public async Task Apply_EstimatesSingleUndatedSiblingOnTwoYearGrid()
    {
        var content = CreateParentAndChildrenGedcom(
            motherBirth: "1870",
            fatherBirth: "1865",
            childDates: ["1900", null, "1904"]);
        var source = await ImportAsync(content);
        var sourceFamily = source.Entitys.OfType<IGenFamily>().Single();
        var exportFamily = CreateDetachedExport(source, sourceFamily).Entitys.OfType<IGenFamily>().Single();

        _ = new OFBDateEstimationService().Apply([exportFamily]);

        Assert.AreEqual("EST 1902", exportFamily.Children.ElementAt(1).BirthDate?.DateText);
        Assert.IsNull(sourceFamily.Children.ElementAt(1).BirthDate);
    }

    [TestMethod]
    public async Task Apply_RepeatsRulesSoAnEstimatedBirthCanInformAnEarlierGeneration()
    {
        const string content =
            "0 HEAD\n1 GEDC\n2 VERS 5.5.1\n" +
            "0 @I1@ INDI\n1 NAME Mother /Second/\n1 SEX F\n1 FAMC @F1@\n1 FAMS @F2@\n" +
            "0 @I2@ INDI\n1 NAME Father /Second/\n1 SEX M\n1 FAMS @F2@\n" +
            "0 @I3@ INDI\n1 NAME Child1 /Second/\n1 FAMC @F2@\n1 BIRT\n2 DATE 1900\n" +
            "0 @I4@ INDI\n1 NAME Child2 /Second/\n1 FAMC @F2@\n1 BIRT\n2 DATE 1920\n" +
            "0 @I5@ INDI\n1 NAME Grandfather /First/\n1 SEX M\n1 FAMS @F1@\n" +
            "0 @I6@ INDI\n1 NAME Grandmother /First/\n1 SEX F\n1 FAMS @F1@\n" +
            "0 @F1@ FAM\n1 HUSB @I5@\n1 WIFE @I6@\n1 CHIL @I1@\n" +
            "0 @F2@ FAM\n1 HUSB @I2@\n1 WIFE @I1@\n1 CHIL @I3@\n1 CHIL @I4@\n" +
            "0 TRLR\n";
        var source = await ImportAsync(content);
        var sourceFamilies = source.Entitys.OfType<IGenFamily>().ToArray();
        var export = new OFBExportOverlayService().Apply(source, "gedcom", [], sourceFamilies).Genealogy;
        var grandparentFamily = export.Entitys.OfType<IGenFamily>().Single(family => family.FamilyRefID == "F1");

        _ = new OFBDateEstimationService().Apply(export.Entitys.OfType<IGenFamily>());

        Assert.AreEqual("EST 1880", grandparentFamily.Children.Single().BirthDate?.DateText);
        Assert.AreEqual("EST 1860", grandparentFamily.Wife?.BirthDate?.DateText);
    }

    private static async Task<IGenealogy> ImportAsync(string content)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return await new GedComDataSource().ImportAsync(stream);
    }

    private static IGenealogy CreateDetachedExport(IGenealogy source, IGenFamily family) =>
        new OFBExportOverlayService()
            .Apply(source, "gedcom", [], [family])
            .Genealogy;

    private static string CreateParentAndChildrenGedcom(
        string? motherBirth,
        string? fatherBirth,
        string?[] childDates)
    {
        var builder = new StringBuilder("0 HEAD\n1 GEDC\n2 VERS 5.5.1\n");
        builder.Append("0 @I1@ INDI\n1 NAME Mother /Example/\n1 SEX F\n");
        if (motherBirth is not null)
            builder.Append($"1 BIRT\n2 DATE {motherBirth}\n");
        builder.Append("1 FAMS @F1@\n");
        builder.Append("0 @I2@ INDI\n1 NAME Father /Example/\n1 SEX M\n");
        if (fatherBirth is not null)
            builder.Append($"1 BIRT\n2 DATE {fatherBirth}\n");
        builder.Append("1 FAMS @F1@\n");

        for (var index = 0; index < childDates.Length; index++)
        {
            var childId = $"I{index + 3}";
            builder.Append($"0 @{childId}@ INDI\n1 NAME Child{index + 1} /Example/\n1 FAMC @F1@\n");
            if (childDates[index] is not null)
                builder.Append($"1 BIRT\n2 DATE {childDates[index]}\n");
        }

        builder.Append("0 @F1@ FAM\n1 HUSB @I2@\n1 WIFE @I1@\n");
        for (var index = 0; index < childDates.Length; index++)
            builder.Append($"1 CHIL @I{index + 3}@\n");
        builder.Append("0 TRLR\n");
        return builder.ToString();
    }
}
