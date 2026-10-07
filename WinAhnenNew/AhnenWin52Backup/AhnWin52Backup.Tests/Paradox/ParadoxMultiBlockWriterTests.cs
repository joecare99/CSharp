using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Paradox;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Tests.Paradox;

[TestClass]
public sealed class ParadoxMultiBlockWriterTests
{
    [TestMethod]
    public void AppendAndRebuildSecondaryIndexes_SupportsRecordsAcrossBlocks()
    {
        string directory = Path.Combine(Path.GetTempPath(), $"AhnWin52Backup-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            string databaseDirectory = Path.Combine(directory, "database");
            new StructureTemplateMaterializer().Materialize(databaseDirectory);
            string databasePath = Path.Combine(databaseDirectory, "adp.DB");
            byte[] header = File.ReadAllBytes(databasePath);
            ParadoxRecordWriter.TableHeader tableHeader =
                ParadoxRecordWriter.ReadHeader(header, 0, databasePath);
            int recordsPerBlock = (tableHeader.BlockSize - 6) / tableHeader.RecordSize;
            int requestedRecords = recordsPerBlock + 3;
            IReadOnlyDictionary<string, string?>[] records = Enumerable
                .Range(1, requestedRecords)
                .Select(static key => (IReadOnlyDictionary<string, string?>)new Dictionary<string, string?>
                {
                    ["Nummer"] = key.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    ["Av"] = "1",
                    ["Am"] = "2"
                })
                .ToArray();

            ParadoxTableReader reader = new();
            new ParadoxRecordWriter(reader).AppendRecords(databasePath, "Nummer", records);
            new ParadoxSecondaryIndexWriter(reader).RebuildForTable(databasePath);

            ParadoxTable restored = reader.Read(databasePath);
            Assert.AreEqual(requestedRecords, restored.Records.Count);
            Assert.AreEqual("1", restored.Records[0][0]);
            Assert.AreEqual(requestedRecords.ToString(System.Globalization.CultureInfo.InvariantCulture),
                restored.Records[^1][0]);

            byte[] outputHeader = File.ReadAllBytes(databasePath);
            ParadoxRecordWriter.TableHeader outputTableHeader =
                ParadoxRecordWriter.ReadHeader(outputHeader, 0, databasePath);
            Assert.AreEqual(2, outputTableHeader.BlockCount);
            Assert.AreEqual(requestedRecords, outputTableHeader.RecordCount);

            string primaryPath = Path.ChangeExtension(databasePath, ".PX");
            ParadoxRecordWriter.TableHeader primaryHeader = ParadoxRecordWriter.ReadHeader(
                File.ReadAllBytes(primaryPath),
                1,
                primaryPath);
            Assert.AreEqual(2, primaryHeader.RecordCount);

            foreach (string xgPath in Directory.EnumerateFiles(databaseDirectory, "adp.XG*", SearchOption.TopDirectoryOnly))
            {
                string xgFileName = Path.GetFileName(xgPath);
                string suffix = Path.GetExtension(xgPath);
                string ygPath = Path.Combine(
                    databaseDirectory,
                    xgFileName.Replace(".XG", ".YG", StringComparison.OrdinalIgnoreCase));
                Assert.IsTrue(File.Exists(ygPath), $"Missing paired index for {xgPath}.");
                ParadoxRecordWriter.TableHeader xgHeader = ParadoxRecordWriter.ReadHeader(
                    File.ReadAllBytes(xgPath),
                    8,
                    xgPath);
                Assert.AreEqual(requestedRecords, xgHeader.RecordCount, suffix);
            }
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
