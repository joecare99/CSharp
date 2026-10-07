using System;
using System.Collections.Generic;
using System.Linq;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Tests.Workflows;

[TestClass]
public sealed class HejParadoxRecordMapperTests
{
    [TestMethod]
    public void Map_MapsFiveSectionsAndLeavesNonHejFieldsForStorageDefaults()
    {
        HejDocument document = CreateDocument();
        Dictionary<string, IReadOnlyList<ParadoxField>> schemas = new(StringComparer.OrdinalIgnoreCase)
        {
            ["AWD.DB"] = Fields(InsertAt(HejDatabaseExportService.IndividualFields, 33, "Bild")),
            ["MRG.DB"] = Fields(InsertAt(HejDatabaseExportService.MarriageFields, 0, "Numr")),
            ["adp.DB"] = Fields(HejDatabaseExportService.AdoptionFields),
            ["LOC.DB"] = Fields(HejDatabaseExportService.PlaceFields),
            ["sour2.DB"] = Fields(InsertAt(HejDatabaseExportService.SourceFields, 0, "N"))
        };

        IReadOnlyDictionary<string, IReadOnlyList<IReadOnlyDictionary<string, string?>>> result =
            new HejParadoxRecordMapper().Map(document, schemas);

        Assert.AreEqual(5, result.Count);
        Assert.AreEqual(5, result["AWD.DB"].Count + result["MRG.DB"].Count +
            result["adp.DB"].Count + result["LOC.DB"].Count + result["sour2.DB"].Count);
        Assert.IsNull(result["AWD.DB"][0]["Bild"]);
        Assert.IsNull(result["MRG.DB"][0]["Numr"]);
        Assert.IsNull(result["sour2.DB"][0]["N"]);
        Assert.AreEqual("Individuals-1-32", result["AWD.DB"][0]["Kommentar"]);
        Assert.AreEqual("Marriages-1-1", result["MRG.DB"][0]["Nummer"]);
        Assert.AreEqual("Adoptions-1-2", result["adp.DB"][0]["Av"]);
        Assert.AreEqual("Places-1-1", result["LOC.DB"][0]["Ort"]);
        Assert.AreEqual("Sources-1-1", result["sour2.DB"][0]["Titel"]);
    }

    [TestMethod]
    public void Map_RejectsChangedPhysicalFieldOrderBeforeWriting()
    {
        HejDocument document = CreateDocument();
        Dictionary<string, IReadOnlyList<ParadoxField>> schemas = new(StringComparer.OrdinalIgnoreCase)
        {
            ["AWD.DB"] = Fields(["Nummer", "Name"]),
            ["MRG.DB"] = Fields(InsertAt(HejDatabaseExportService.MarriageFields, 0, "Numr")),
            ["adp.DB"] = Fields(HejDatabaseExportService.AdoptionFields),
            ["LOC.DB"] = Fields(HejDatabaseExportService.PlaceFields),
            ["sour2.DB"] = Fields(InsertAt(HejDatabaseExportService.SourceFields, 0, "N"))
        };

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(
            () => new HejParadoxRecordMapper().Map(document, schemas));

        StringAssert.Contains(exception.Message, "AWD.DB");
    }

    private static HejDocument CreateDocument() => new(
        [Record(HejSection.Individuals)],
        [Record(HejSection.Marriages)],
        [Record(HejSection.Adoptions)],
        [Record(HejSection.Places)],
        [Record(HejSection.Sources)]);

    private static HejRecord Record(HejSection section)
    {
        int count = HejSchema.GetFieldCount(section);
        return new HejRecord(Enumerable.Range(0, count).Select(index => $"{section}-{1}-{index + 1}"));
    }

    private static IReadOnlyList<ParadoxField> Fields(IReadOnlyList<string> names) =>
        names.Select(static name => new ParadoxField(name, 1, 1)).ToArray();

    private static string[] InsertAt(IReadOnlyList<string> fields, int index, string fieldName)
    {
        string[] result = new string[fields.Count + 1];
        for (int source = 0, destination = 0; destination < result.Length; destination++)
        {
            result[destination] = destination == index ? fieldName : fields[source++];
        }

        return result;
    }
}
