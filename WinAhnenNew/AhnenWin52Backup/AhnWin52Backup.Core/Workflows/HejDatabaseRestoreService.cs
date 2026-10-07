using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Hej;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Stages and verifies HEJ restores before publishing a database directory.</summary>
public sealed class HejDatabaseRestoreService : IHejDatabaseRestoreService
{
    private static readonly string[] MappedTableNames = ["AWD.DB", "MRG.DB", "adp.DB", "LOC.DB", "sour2.DB"];

    private readonly IHejReader _hejReader;
    private readonly IHejDatabaseExportService _databaseExportService;
    private readonly IParadoxTableReader _tableReader;
    private readonly StructureTemplateMaterializer _templateMaterializer;
    private readonly HejParadoxRecordMapper _recordMapper = new();
    private readonly ParadoxDirectoryInventory _inventory = new();

    public HejDatabaseRestoreService(
        IHejReader hejReader,
        IHejDatabaseExportService databaseExportService,
        IParadoxTableReader tableReader,
        StructureTemplateMaterializer templateMaterializer)
    {
        ArgumentNullException.ThrowIfNull(hejReader);
        ArgumentNullException.ThrowIfNull(databaseExportService);
        ArgumentNullException.ThrowIfNull(tableReader);
        ArgumentNullException.ThrowIfNull(templateMaterializer);
        _hejReader = hejReader;
        _databaseExportService = databaseExportService;
        _tableReader = tableReader;
        _templateMaterializer = templateMaterializer;
    }

    public HejDatabaseRestoreResult Restore(
        string hejFilePath,
        string destinationDirectory,
        HejDatabaseRestoreMode mode,
        bool force)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hejFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);
        string inputPath = Path.GetFullPath(hejFilePath);
        string destinationPath = Path.GetFullPath(destinationDirectory);
        if (!string.Equals(Path.GetExtension(inputPath), ".hej", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The restore input must use the .hej file extension.", nameof(hejFilePath));
        }

        HejDocument document;
        using (FileStream source = new(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            document = _hejReader.Read(source);
        }

        if (document.Warnings.Count > 0 && !force)
        {
            throw new InvalidDataException(
                $"The HEJ input has compatibility warnings; review them and pass --force to accept classified losses. First warning: {document.Warnings[0]}");
        }

        string parentDirectory = Path.GetDirectoryName(destinationPath)
            ?? throw new ArgumentException("The destination must have a parent directory.", nameof(destinationDirectory));
        if (!Directory.Exists(parentDirectory))
        {
            throw new DirectoryNotFoundException($"The restore destination parent does not exist: {parentDirectory}");
        }

        if (File.Exists(destinationPath))
        {
            throw new IOException($"The restore destination is a file: {destinationPath}");
        }

        StructureTemplateManifest manifest = _templateMaterializer.ReadManifest();
        return mode switch
        {
            HejDatabaseRestoreMode.FromScratch =>
                RestoreFromScratch(destinationPath, document, manifest),
            HejDatabaseRestoreMode.ExistingTemplate =>
                RestoreExistingTemplate(destinationPath, document, manifest),
            HejDatabaseRestoreMode.Override =>
                RestoreOverride(destinationPath, document, manifest),
            _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Unknown restore mode.")
        };
    }

    private HejDatabaseRestoreResult RestoreFromScratch(
        string destinationPath,
        HejDocument document,
        StructureTemplateManifest manifest)
    {
        if (Directory.Exists(destinationPath))
        {
            throw new IOException($"The from-scratch destination already exists: {destinationPath}");
        }

        string stagingDirectory = CreateSiblingPath(destinationPath, "restore");
        try
        {
            _templateMaterializer.Materialize(stagingDirectory);
            PopulateAndValidate(stagingDirectory, document, manifest);
            Directory.Move(stagingDirectory, destinationPath);
            return CreateResult(destinationPath, document, document.Warnings);
        }
        catch (Exception operationException)
        {
            CleanupAfterFailure(stagingDirectory, operationException);
            throw;
        }
    }

    private HejDatabaseRestoreResult RestoreExistingTemplate(
        string destinationPath,
        HejDocument document,
        StructureTemplateManifest manifest)
    {
        if (!Directory.Exists(destinationPath))
        {
            throw new DirectoryNotFoundException($"The restore template does not exist: {destinationPath}");
        }

        _templateMaterializer.ValidateDirectory(destinationPath);
        EnsureMappedTablesEmpty(destinationPath);
        string stagingDirectory = CreateSiblingPath(destinationPath, "restore");
        try
        {
            CopyDirectory(destinationPath, stagingDirectory);
            PopulateAndValidate(stagingDirectory, document, manifest);
            return PublishReplacingDirectory(stagingDirectory, destinationPath, document);
        }
        catch (Exception operationException)
        {
            CleanupAfterFailure(stagingDirectory, operationException);
            throw;
        }
    }

    private HejDatabaseRestoreResult RestoreOverride(
        string destinationPath,
        HejDocument document,
        StructureTemplateManifest manifest)
    {
        if (!Directory.Exists(destinationPath))
        {
            throw new DirectoryNotFoundException($"The override destination does not exist: {destinationPath}");
        }

        ValidateCompatibleDirectory(destinationPath, manifest);
        string candidateDirectory = CreateSiblingPath(destinationPath, "candidate");
        string stagingDirectory = CreateSiblingPath(destinationPath, "override");
        try
        {
            _templateMaterializer.Materialize(candidateDirectory);
            PopulateAndValidate(candidateDirectory, document, manifest);
            CopyDirectory(destinationPath, stagingDirectory);
            ReplaceMappedFamilies(candidateDirectory, stagingDirectory, manifest);
            ValidateCandidate(stagingDirectory, document, manifest, validateAncillaryBaseline: false);
            return PublishReplacingDirectory(stagingDirectory, destinationPath, document);
        }
        catch (Exception operationException)
        {
            CleanupAfterFailure(stagingDirectory, operationException);
            CleanupAfterFailure(candidateDirectory, operationException);
            throw;
        }
    }

    private void PopulateAndValidate(
        string directory,
        HejDocument document,
        StructureTemplateManifest manifest)
    {
        IReadOnlyList<ParadoxTableInventory> tables = _inventory.Read(directory);
        ValidateTemplateSchemas(tables, manifest, requireTemplateTableSet: true);
        Dictionary<string, IReadOnlyList<ParadoxField>> tableFields = tables.ToDictionary(
            static table => table.FileName,
            static table => table.Schema.Fields,
            StringComparer.OrdinalIgnoreCase);
        IReadOnlyDictionary<string, IReadOnlyList<IReadOnlyDictionary<string, string?>>> mapped =
            _recordMapper.Map(document, tableFields);

        ParadoxRecordWriter writer = new(_tableReader);
        ParadoxSecondaryIndexWriter indexWriter = new(_tableReader);
        foreach (string databaseFileName in MappedTableNames)
        {
            IReadOnlyList<IReadOnlyDictionary<string, string?>> records = mapped[databaseFileName];
            if (records.Count == 0)
            {
                continue;
            }

            string databasePath = FindFilePath(directory, databaseFileName);
            string primaryKey = databaseFileName.ToUpperInvariant() switch
            {
                "AWD.DB" => "Nummer",
                "MRG.DB" => "Numr",
                "ADP.DB" => "Nummer",
                "LOC.DB" => "Ort",
                "SOUR2.DB" => "N",
                _ => throw new InvalidOperationException($"No primary key mapping exists for {databaseFileName}.")
            };
            writer.AppendRecords(databasePath, primaryKey, records);
            indexWriter.RebuildForTable(databasePath);
        }

        ValidateCandidate(directory, document, manifest);
    }

    private void ValidateCandidate(
        string directory,
        HejDocument expectedDocument,
        StructureTemplateManifest manifest,
        bool validateAncillaryBaseline = true)
    {
        IReadOnlyList<ParadoxTableInventory> tables = _inventory.Read(directory);
        ValidateTemplateSchemas(tables, manifest, requireTemplateTableSet: false);
        Dictionary<string, ParadoxTableInventory> byName = tables.ToDictionary(
            static table => table.FileName,
            StringComparer.OrdinalIgnoreCase);
        Dictionary<string, int> expectedCounts = GetSectionCounts(expectedDocument);
        foreach (StructureTemplateTable expected in manifest.Tables)
        {
            ParadoxTableInventory actual = byName[expected.FileName];
            if (MappedTableNames.Contains(expected.FileName, StringComparer.OrdinalIgnoreCase))
            {
                int expectedCount = expectedCounts[GetSectionForTable(expected.FileName)];
                ParadoxTable table = _tableReader.Read(FindFilePath(directory, expected.FileName));
                if (table.Records.Count != expectedCount || actual.Schema.DeclaredRecordCount != expectedCount)
                {
                    throw new InvalidDataException(
                        $"Restored record count does not match the HEJ input for {expected.FileName}.");
                }

                ValidateSecondaryRecordCounts(directory, expected, expectedCount);
            }
            else if (validateAncillaryBaseline &&
                     actual.Schema.DeclaredRecordCount != expected.ExpectedRecords)
            {
                throw new InvalidDataException(
                    $"Restore changed the non-HEJ baseline row count in {expected.FileName}.");
            }
        }

        HejDocument actualDocument = _databaseExportService.Export(directory);
        CompareDocuments(expectedDocument, actualDocument);
    }

    private static void ValidateSecondaryRecordCounts(
        string directory,
        StructureTemplateTable table,
        int expectedRecordCount)
    {
        string stem = Path.GetFileNameWithoutExtension(table.FileName);
        string primaryPath = FindFilePath(directory, $"{stem}.PX");
        byte[] primary = File.ReadAllBytes(primaryPath);
        foreach (StructureTemplateIndex index in table.SecondaryIndexes)
        {
            string xgPath = FindFilePath(directory, index.XgFile);
            string ygPath = FindFilePath(directory, index.YgFile);
            byte[] xg = File.ReadAllBytes(xgPath);
            byte[] yg = File.ReadAllBytes(ygPath);
            int xgRecordCount = BinaryPrimitives.ReadInt32LittleEndian(xg.AsSpan(6, sizeof(int)));
            int ygRecordCount = BinaryPrimitives.ReadInt32LittleEndian(yg.AsSpan(6, sizeof(int)));
            if (xgRecordCount != expectedRecordCount ||
                (expectedRecordCount > 0 && ygRecordCount <= 0) ||
                (expectedRecordCount == 0 && ygRecordCount != 0) ||
                xg[0x2F] != primary[0x2C])
            {
                throw new InvalidDataException(
                    $"Secondary index count or revision is inconsistent: {Path.GetFileName(xgPath)}/{Path.GetFileName(ygPath)}.");
            }
        }
    }

    private void ValidateCompatibleDirectory(string directory, StructureTemplateManifest manifest)
    {
        IReadOnlyList<ParadoxTableInventory> tables = _inventory.Read(directory);
        ValidateTemplateSchemas(tables, manifest, requireTemplateTableSet: false);
        Dictionary<string, ParadoxTableInventory> byName = tables.ToDictionary(
            static table => table.FileName,
            StringComparer.OrdinalIgnoreCase);
        foreach (string tableName in MappedTableNames)
        {
            StructureTemplateTable expected = manifest.Tables.Single(table =>
                table.FileName.Equals(tableName, StringComparison.OrdinalIgnoreCase));
            ParadoxTableInventory actual = byName[expected.FileName];
            int[] expectedIndexes = expected.SecondaryIndexes.Select(static index => index.Number).Order().ToArray();
            if (!actual.SecondaryIndexes.Order().SequenceEqual(expectedIndexes))
            {
                throw new InvalidDataException(
                    $"The override destination has a different secondary-index family for {expected.FileName}.");
            }
        }
    }

    private static void ValidateTemplateSchemas(
        IReadOnlyList<ParadoxTableInventory> actualTables,
        StructureTemplateManifest manifest,
        bool requireTemplateTableSet)
    {
        Dictionary<string, ParadoxTableInventory> actualByName = actualTables.ToDictionary(
            static table => table.FileName,
            StringComparer.OrdinalIgnoreCase);
        if (requireTemplateTableSet && actualByName.Count != manifest.Tables.Count)
        {
            throw new InvalidDataException("The restore structure does not contain the complete template table set.");
        }

        foreach (StructureTemplateTable expected in manifest.Tables)
        {
            if (!actualByName.TryGetValue(expected.FileName, out ParadoxTableInventory? actual) ||
                actual.Schema.Version != expected.Version ||
                actual.Schema.Encrypted != expected.Encrypted ||
                actual.Schema.Fields.Count != expected.Fields.Count ||
                actual.Schema.Fields.Where((field, index) =>
                        !field.Name.Equals(expected.Fields[index].Name, StringComparison.Ordinal) ||
                        field.TypeCode != expected.Fields[index].TypeCode ||
                        field.Length != expected.Fields[index].Length)
                    .Any() ||
                !actual.HasPrimaryIndex ||
                actual.HasMemoFile != (expected.MemoFile is not null))
            {
                throw new InvalidDataException(
                    $"The restore destination schema does not match the approved template: {expected.FileName}.");
            }
        }
    }

    private void EnsureMappedTablesEmpty(string directory)
    {
        foreach (string tableName in MappedTableNames)
        {
            ParadoxTable table = _tableReader.Read(FindFilePath(directory, tableName));
            if (table.Records.Count != 0)
            {
                throw new InvalidDataException(
                    $"The existing-template restore requires empty mapped tables; {tableName} contains {table.Records.Count} records. Use --override explicitly.");
            }
        }
    }

    private void ReplaceMappedFamilies(
        string restoredTemplateDirectory,
        string stagingDirectory,
        StructureTemplateManifest manifest)
    {
        HashSet<string> replacementFiles = manifest.Assets
            .Select(static asset => asset.FileName)
            .Where(fileName => MappedTableNames.Any(tableName =>
                fileName.StartsWith(Path.GetFileNameWithoutExtension(tableName) + ".", StringComparison.OrdinalIgnoreCase)))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (string file in Directory.EnumerateFiles(stagingDirectory, "*", SearchOption.TopDirectoryOnly))
        {
            if (replacementFiles.Contains(Path.GetFileName(file)))
            {
                File.Delete(file);
            }
        }

        foreach (string fileName in replacementFiles)
        {
            string sourcePath = FindFilePath(restoredTemplateDirectory, fileName);
            string destinationPath = Path.Combine(stagingDirectory, fileName);
            File.Copy(sourcePath, destinationPath, overwrite: false);
        }
    }

    private HejDatabaseRestoreResult PublishReplacingDirectory(
        string stagingDirectory,
        string destinationDirectory,
        HejDocument document)
    {
        string backupDirectory = CreateSiblingPath(destinationDirectory, "previous");
        Directory.Move(destinationDirectory, backupDirectory);
        try
        {
            Directory.Move(stagingDirectory, destinationDirectory);
        }
        catch (Exception publishException)
        {
            try
            {
                Directory.Move(backupDirectory, destinationDirectory);
            }
            catch (Exception rollbackException)
            {
                throw new AggregateException(
                    $"Restore publication failed. The prior database was preserved at {backupDirectory}.",
                    publishException,
                    rollbackException);
            }

            throw;
        }

        List<string> warnings = document.Warnings.ToList();
        try
        {
            Directory.Delete(backupDirectory, recursive: true);
        }
        catch (Exception cleanupException) when (
            cleanupException is IOException or UnauthorizedAccessException)
        {
            warnings.Add($"Restore published, but the previous directory could not be removed: {backupDirectory} ({cleanupException.Message})");
        }

        return CreateResult(destinationDirectory, document, warnings);
    }

    private static void CompareDocuments(HejDocument expected, HejDocument actual)
    {
        foreach (HejSection section in Enum.GetValues<HejSection>())
        {
            IReadOnlyList<HejRecord> expectedRecords = expected.GetRecords(section);
            IReadOnlyList<HejRecord> actualRecords = actual.GetRecords(section);
            if (expectedRecords.Count != actualRecords.Count)
            {
                throw new InvalidDataException(
                    $"Restore validation failed for {section}: expected {expectedRecords.Count} records but read {actualRecords.Count}.");
            }

            for (int row = 0; row < expectedRecords.Count; row++)
            {
                for (int field = 0; field < expectedRecords[row].Fields.Count; field++)
                {
                    if (!NormalizeLineEndings(expectedRecords[row].Fields[field])
                            .Equals(NormalizeLineEndings(actualRecords[row].Fields[field]), StringComparison.Ordinal))
                    {
                        throw new InvalidDataException(
                            $"Restore validation differs at {section} record {row + 1}, field {field + 1}.");
                    }
                }
            }
        }
    }

    private static string NormalizeLineEndings(string value) =>
        value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n');

    private static Dictionary<string, int> GetSectionCounts(HejDocument document) => new(StringComparer.Ordinal)
    {
        ["Individuals"] = document.Individuals.Count,
        ["Marriages"] = document.Marriages.Count,
        ["Adoptions"] = document.Adoptions.Count,
        ["Places"] = document.Places.Count,
        ["Sources"] = document.Sources.Count
    };

    private static string GetSectionForTable(string tableName) => tableName.ToUpperInvariant() switch
    {
        "AWD.DB" => "Individuals",
        "MRG.DB" => "Marriages",
        "ADP.DB" => "Adoptions",
        "LOC.DB" => "Places",
        "SOUR2.DB" => "Sources",
        _ => throw new ArgumentOutOfRangeException(nameof(tableName), tableName, "Unknown HEJ-backed table.")
    };

    private static HejDatabaseRestoreResult CreateResult(
        string destinationDirectory,
        HejDocument document,
        IReadOnlyList<string> warnings) =>
        new(destinationDirectory, GetSectionCounts(document), warnings.ToArray());

    private static string CreateSiblingPath(string destinationPath, string purpose)
    {
        string parent = Path.GetDirectoryName(destinationPath)
            ?? throw new InvalidOperationException("The destination has no parent directory.");
        string name = Path.GetFileName(destinationPath);
        return Path.Combine(parent, $".{name}.ahwb-{purpose}-{Guid.NewGuid():N}.tmp");
    }

    private static string FindFilePath(string directory, string fileName)
    {
        string? path = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(candidate =>
                Path.GetFileName(candidate).Equals(fileName, StringComparison.OrdinalIgnoreCase));
        return path ?? throw new FileNotFoundException($"Required restore file was not found: {fileName}", fileName);
    }

    private static void CopyDirectory(string sourceDirectory, string destinationDirectory)
    {
        if (Directory.Exists(destinationDirectory) || File.Exists(destinationDirectory))
        {
            throw new IOException($"The restore staging path already exists: {destinationDirectory}");
        }

        Directory.CreateDirectory(destinationDirectory);
        Stack<(string Source, string Destination)> pending = new();
        pending.Push((sourceDirectory, destinationDirectory));
        while (pending.TryPop(out (string Source, string Destination) current))
        {
            foreach (string entry in Directory.EnumerateFileSystemEntries(current.Source))
            {
                FileAttributes attributes = File.GetAttributes(entry);
                if ((attributes & FileAttributes.ReparsePoint) != 0)
                {
                    throw new IOException($"Restore staging does not follow reparse points: {entry}");
                }

                string targetPath = Path.Combine(current.Destination, Path.GetFileName(entry));
                if ((attributes & FileAttributes.Directory) != 0)
                {
                    Directory.CreateDirectory(targetPath);
                    pending.Push((entry, targetPath));
                }
                else
                {
                    File.Copy(entry, targetPath, overwrite: false);
                }
            }
        }
    }

    private static void CleanupAfterFailure(string path, Exception operationException)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        try
        {
            Directory.Delete(path, recursive: true);
        }
        catch (Exception cleanupException) when (
            cleanupException is IOException or UnauthorizedAccessException)
        {
            throw new AggregateException(
                $"Restore failed and its staging directory could not be removed: {path}",
                operationException,
                cleanupException);
        }
    }

}
