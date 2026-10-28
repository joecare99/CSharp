using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxBdeFixtureExporterTests
{
    [TestMethod]
    [DoNotParallelize]
    public void ExportSinglePrimaryKeyFixture_WhenOutputDirectoryIsConfigured()
    {
        string? outputDirectory = Environment.GetEnvironmentVariable("DBPARADOX_BDE_FIXTURE_OUTPUT");
        string? templateDirectory = Environment.GetEnvironmentVariable("DBPARADOX_BDE_TEMPLATE_DIR");
        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            Assert.Inconclusive("Set DBPARADOX_BDE_FIXTURE_OUTPUT to export the disposable BDE fixture.");
        }

        if (string.IsNullOrWhiteSpace(templateDirectory))
        {
            Assert.Inconclusive("Set DBPARADOX_BDE_TEMPLATE_DIR to the disposable BDE-created template directory.");
        }

        Directory.CreateDirectory(outputDirectory);
        string databasePath = Path.Combine(outputDirectory, "Synthetic.DB");
        File.Copy(Path.Combine(templateDirectory, "Synthetic.DB"), databasePath, overwrite: true);
        File.Copy(
            Path.Combine(templateDirectory, "Synthetic.PX"),
            Path.ChangeExtension(databasePath, ".PX"),
            overwrite: true);

        ParadoxTableReader reader = new();
        string[] keyValues = ParadoxConformanceVectors.Read("bde-integer-primary-key-values").Split(',');
        IReadOnlyDictionary<string, string?>[] records = keyValues
            .Select(key => (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?> { ["ID"] = key })
            .ToArray();
        new ParadoxRecordWriter(reader).AppendRecords(databasePath, "ID", records);

        ParadoxTable table = reader.Read(databasePath);
        Assert.AreEqual(3, table.Records.Count);
        Assert.AreEqual("1", table.Records[0][0]);
        Assert.AreEqual("2", table.Records[1][0]);
        Assert.AreEqual("10", table.Records[2][0]);
    }

}
