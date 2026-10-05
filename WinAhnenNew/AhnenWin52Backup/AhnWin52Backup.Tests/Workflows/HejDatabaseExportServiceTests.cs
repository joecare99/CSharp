using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;
using AhnWin52Backup.Core.Workflows;
using NSubstitute;

namespace AhnWin52Backup.Tests.Workflows;

[TestClass]
public sealed class HejDatabaseExportServiceTests
{
    private static readonly string[] IndividualDatabaseFields =
    [
        "Nummer", "Vater", "Mutter", "Name", "Vornamen", "Geschlecht", "Religion", "Beruf",
        "Gebtag", "Gebmonat", "Gebjahr", "Gebort", "Tauftag", "Taufmonat", "Taufjahr", "Taufort",
        "Taufpat", "Lebensort", "Sttag", "Stmonat", "Stjahr", "Stort", "Todesurs", "Begtag",
        "Begmonat", "Begjahr", "Begort", "Quelleg", "Quellet", "Quelles", "Quelleb", "Kommentar",
        "Lebt", "Bild", "Namex", "IDNR", "Kistat", "Hausname", "Adr1", "Adr2", "PLZ", "Ort",
        "Adrzus", "Indj", "Indm", "Indt", "Alter", "Tel", "Ema", "Ur", "Quelle", "Rufname"
    ];

    private static readonly string[] MarriageDatabaseFields =
    [
        "Numr", "Nummer", "Epnum", "Htag", "Hmonat", "Hjahr", "Hort", "Trauz", "Satag", "Samonat",
        "Sajahr", "Saort", "Satrauz", "Verbind", "Schtag", "Schmonat", "Schjahr", "Schort", "Hqu",
        "Saqu", "Schqu", "Indj", "Indm"
    ];

    private static readonly string[] SourceDatabaseFields =
    [
        "N", "Titel", "Abk", "Ereig", "Von", "Bis", "Standort", "Publ", "Rep", "Bem", "Bestand", "Med"
    ];

    [TestMethod]
    public void Export_MapsAllFiveTablesAndOmitsNonHejFields()
    {
        string databaseDirectory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(databaseDirectory);

        try
        {
            Dictionary<string, ParadoxTable> tables = new(StringComparer.OrdinalIgnoreCase)
            {
                ["AWD.DB"] = CreateTable(IndividualDatabaseFields),
                ["MRG.DB"] = CreateTable(MarriageDatabaseFields),
                ["adp.DB"] = CreateTable(["Nummer", "Av", "Am"]),
                ["LOC.DB"] = CreateTable(
                    ["Ort", "PLZ", "Land", "RegBez", "Gov", "Bland", "Gde", "Pfr", "Lkr", "Abk", "Lg", "Bg", "Maid"]),
                ["sour2.DB"] = CreateTable(SourceDatabaseFields)
            };

            foreach (string fileName in tables.Keys)
            {
                File.WriteAllBytes(Path.Combine(databaseDirectory, fileName), []);
            }

            IParadoxTableReader reader = Substitute.For<IParadoxTableReader>();
            reader.Read(Arg.Any<string>()).Returns(call => tables[Path.GetFileName(call.Arg<string>())]);

            HejDocument document = new HejDatabaseExportService(reader).Export(databaseDirectory);

            Assert.AreEqual(51, document.Individuals[0].Fields.Count);
            Assert.AreEqual("Lebt", document.Individuals[0].Fields[32]);
            Assert.AreEqual("Namex", document.Individuals[0].Fields[33]);
            Assert.AreEqual(22, document.Marriages[0].Fields.Count);
            Assert.AreEqual("Nummer", document.Marriages[0].Fields[0]);
            Assert.AreEqual(3, document.Adoptions[0].Fields.Count);
            Assert.AreEqual(13, document.Places[0].Fields.Count);
            Assert.AreEqual(11, document.Sources[0].Fields.Count);
            Assert.AreEqual("Titel", document.Sources[0].Fields[0]);
        }
        finally
        {
            Directory.Delete(databaseDirectory, recursive: true);
        }
    }

    [TestMethod]
    public void Export_RejectsAChangedPhysicalTableSchema()
    {
        string databaseDirectory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(databaseDirectory);

        try
        {
            File.WriteAllBytes(Path.Combine(databaseDirectory, "AWD.DB"), []);
            IParadoxTableReader reader = Substitute.For<IParadoxTableReader>();
            reader.Read(Arg.Any<string>()).Returns(CreateTable(["Nummer"]));

            InvalidDataException exception = Assert.Throws<InvalidDataException>(
                () => new HejDatabaseExportService(reader).Export(databaseDirectory));

            StringAssert.Contains(exception.Message, "AWD.DB");
        }
        finally
        {
            Directory.Delete(databaseDirectory, recursive: true);
        }
    }

    private static ParadoxTable CreateTable(IReadOnlyList<string> fieldNames)
    {
        ParadoxField[] fields = fieldNames
            .Select(name => new ParadoxField(name, GetTypeCode(name), GetLength(name)))
            .ToArray();
        string?[] values = fieldNames.Select(static name => (string?)name).ToArray();
        return new ParadoxTable("test", fields, [values]);
    }

    private static byte GetTypeCode(string fieldName) => fieldName switch
    {
        "Numr" or "N" => 0x16,
        "Nummer" or "Vater" or "Mutter" or "Epnum" or "Av" or "Am" => 0x04,
        "Kommentar" or "Publ" or "Bem" => 0x0C,
        "Bild" => 0x10,
        _ => 0x01
    };

    private static int GetLength(string fieldName) => fieldName switch
    {
        "Numr" or "N" or "Nummer" or "Vater" or "Mutter" or "Epnum" or "Av" or "Am" => 4,
        "Kommentar" or "Publ" or "Bem" => 20,
        "Bild" => 10,
        _ => 10
    };
}
