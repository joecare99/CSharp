using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using AhnWin52Backup.Core.Paradox;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Validates and materializes the embedded, hash-pinned AhnWin structure template.</summary>
public sealed class StructureTemplateMaterializer
{
    private const string ManifestResourceName = "AhnWin52Backup.Core.StructureTemplate.manifest.json";
    private const string AssetArchiveResourceName = "AhnWin52Backup.Core.StructureTemplate.assets.zip";
    private const string AssetResourcePrefix = "AhnWin52Backup.Core.StructureTemplate.Files.";
    private const string NamesdayResourceName = "AhnWin52Backup.Core.namesdays.json";
    private const int SupportedSchemaVersion = 1;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        MaxDepth = 32
    };

    private readonly Assembly _assembly;
    private readonly ParadoxDirectoryInventory _inventory;
    private readonly ParadoxTableReader _tableReader;

    public StructureTemplateMaterializer()
        : this(typeof(StructureTemplateMaterializer).Assembly, new ParadoxDirectoryInventory(), new ParadoxTableReader())
    {
    }

    internal StructureTemplateMaterializer(
        Assembly assembly,
        ParadoxDirectoryInventory inventory,
        ParadoxTableReader tableReader)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(inventory);
        ArgumentNullException.ThrowIfNull(tableReader);
        _assembly = assembly;
        _inventory = inventory;
        _tableReader = tableReader;
    }

    /// <summary>Loads and validates the embedded JSON manifest and each declared embedded asset.</summary>
    public StructureTemplateManifest ReadManifest()
    {
        using Stream manifestStream = _assembly.GetManifestResourceStream(ManifestResourceName)
            ?? throw new InvalidDataException("The embedded structure-template manifest is missing.");
        StructureTemplateManifest manifest = JsonSerializer.Deserialize<StructureTemplateManifest>(
            manifestStream,
            JsonOptions) ?? throw new InvalidDataException("The embedded structure-template manifest is empty.");
        ValidateManifest(manifest);
        ValidateAssetArchive(manifest);
        foreach (StructureTemplateAsset asset in manifest.Assets)
        {
            _ = ReadAndValidateAsset(asset);
        }

        return manifest;
    }

    /// <summary>Creates, verifies, and atomically publishes the template in a new directory.</summary>
    public StructureTemplateManifest Materialize(string destinationDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);
        string fullDestination = Path.GetFullPath(destinationDirectory);
        if (Directory.Exists(fullDestination) || File.Exists(fullDestination))
        {
            throw new IOException($"The template destination already exists: {fullDestination}");
        }

        string parentDirectory = Path.GetDirectoryName(fullDestination)
            ?? throw new ArgumentException("The destination must have a parent directory.", nameof(destinationDirectory));
        if (!Directory.Exists(parentDirectory))
        {
            throw new DirectoryNotFoundException($"The destination parent directory does not exist: {parentDirectory}");
        }

        StructureTemplateManifest manifest = ReadManifest();
        string stagingDirectory = Path.Combine(parentDirectory, $".ahwb-template-{Guid.NewGuid():N}.tmp");
        Directory.CreateDirectory(stagingDirectory);
        try
        {
            foreach (StructureTemplateAsset asset in manifest.Assets)
            {
                byte[] contents = ReadAndValidateAsset(asset);
                string assetPath = Path.Combine(stagingDirectory, asset.FileName);
                using FileStream destination = new(
                    assetPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None);
                destination.Write(contents);
                destination.Flush(flushToDisk: true);
            }

            ValidateDirectory(stagingDirectory, manifest);
            Directory.Move(stagingDirectory, fullDestination);
            return manifest;
        }
        catch (Exception operationException)
        {
            try
            {
                Directory.Delete(stagingDirectory, recursive: true);
            }
            catch (Exception cleanupException) when (
                cleanupException is IOException or UnauthorizedAccessException)
            {
                throw new AggregateException(
                    "Structure-template materialization failed and its staging directory could not be removed.",
                    operationException,
                    cleanupException);
            }

            throw;
        }
    }

    /// <summary>Creates the Paradox files from the manifest and copies only non-Paradox assets.</summary>
    public StructureTemplateManifest MaterializeGenerated(string destinationDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationDirectory);
        string fullDestination = Path.GetFullPath(destinationDirectory);
        if (Directory.Exists(fullDestination) || File.Exists(fullDestination))
        {
            throw new IOException($"The generated-template destination already exists: {fullDestination}");
        }

        string parentDirectory = Path.GetDirectoryName(fullDestination)
            ?? throw new ArgumentException("The destination must have a parent directory.", nameof(destinationDirectory));
        if (!Directory.Exists(parentDirectory))
        {
            throw new DirectoryNotFoundException($"The destination parent directory does not exist: {parentDirectory}");
        }

        StructureTemplateManifest manifest = ReadManifest();
        string stagingDirectory = Path.Combine(parentDirectory, $".ahwb-generated-template-{Guid.NewGuid():N}.tmp");
        Directory.CreateDirectory(stagingDirectory);
        try
        {
            foreach (StructureTemplateAsset asset in manifest.Assets.Where(static asset =>
                         !ParadoxStructureWriter.IsParadoxFileName(asset.FileName)))
            {
                byte[] contents = ReadAndValidateAsset(asset);
                string assetPath = Path.Combine(stagingDirectory, asset.FileName);
                using FileStream destination = new(assetPath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                destination.Write(contents);
                destination.Flush(flushToDisk: true);
            }

            foreach (StructureTemplateTable table in manifest.Tables)
            {
                ParadoxStructureWriter.WriteTableFamily(
                    stagingDirectory,
                    table,
                    manifest.EncryptionKey,
                    manifest.IndexMaximumTableSize);
            }

            foreach (StructureTemplateTable table in manifest.Tables.Where(static table =>
                         table.BaselineData is not null))
            {
                IReadOnlyList<IReadOnlyDictionary<string, string?>> records = NamesdayDefaults.Read();
                string databasePath = Path.Combine(stagingDirectory, table.FileName);
                ParadoxRecordWriter writer = new(_tableReader);
                writer.AppendRecords(databasePath, table.Fields[0].Name, records);
                if (table.SecondaryIndexes.Count > 0)
                {
                    new ParadoxSecondaryIndexWriter(_tableReader).RebuildForTable(databasePath);
                }

                ParadoxStructureWriter.WriteHeaderMetadata(databasePath, table.DatabaseHeaderMetadata);
                ParadoxStructureWriter.WriteHeaderMetadata(
                    Path.Combine(stagingDirectory, table.PrimaryIndexFile),
                    table.PrimaryIndexHeaderMetadata);
                foreach (StructureTemplateIndex index in table.SecondaryIndexes)
                {
                    ParadoxStructureWriter.WriteHeaderMetadata(
                        Path.Combine(stagingDirectory, index.XgFile),
                        index.XgHeaderMetadata);
                    ParadoxStructureWriter.WriteHeaderMetadata(
                        Path.Combine(stagingDirectory, index.YgFile),
                        index.YgHeaderMetadata);
                }
            }

            ValidateDirectory(stagingDirectory, manifest, generatedParadoxFiles: true);
            Directory.Move(stagingDirectory, fullDestination);
            return manifest;
        }
        catch (Exception operationException)
        {
            try
            {
                Directory.Delete(stagingDirectory, recursive: true);
            }
            catch (Exception cleanupException) when (
                cleanupException is IOException or UnauthorizedAccessException)
            {
                throw new AggregateException(
                    "Generated-template materialization failed and its staging directory could not be removed.",
                    operationException,
                    cleanupException);
            }

            throw;
        }
    }

    /// <summary>Verifies asset hashes, table schemas, baseline rows, and index counts in a materialized directory.</summary>
    public void ValidateDirectory(string directory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        string fullDirectory = Path.GetFullPath(directory);
        if (!Directory.Exists(fullDirectory))
        {
            throw new DirectoryNotFoundException($"The structure-template directory does not exist: {fullDirectory}");
        }

        ValidateDirectory(fullDirectory, ReadManifest(), generatedParadoxFiles: false);
    }

    private void ValidateDirectory(
        string directory,
        StructureTemplateManifest manifest,
        bool generatedParadoxFiles = false)
    {
        string[] materializedFiles = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)
            .Select(static path => Path.GetFileName(path)
                ?? throw new InvalidDataException("A materialized template file has no name."))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        string[] expectedFiles = manifest.Assets
            .Select(static asset => asset.FileName)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (!materializedFiles.SequenceEqual(expectedFiles, StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("The materialized template file list does not match its manifest.");
        }

        foreach (StructureTemplateAsset asset in manifest.Assets)
        {
            if (generatedParadoxFiles && ParadoxStructureWriter.IsParadoxFileName(asset.FileName))
            {
                continue;
            }

            string path = Path.Combine(directory, asset.FileName);
            using FileStream stream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            if (stream.Length != asset.SizeBytes)
            {
                throw new InvalidDataException($"The materialized template asset has an invalid size: {asset.FileName}");
            }

            string hash = Convert.ToHexString(SHA256.HashData(stream));
            if (!hash.Equals(asset.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException($"The materialized template asset hash is invalid: {asset.FileName}");
            }
        }

        IReadOnlyList<ParadoxTableInventory> actualTables = _inventory.Read(directory);
        Dictionary<string, ParadoxTableInventory> tablesByName = actualTables.ToDictionary(
            static table => table.FileName,
            StringComparer.OrdinalIgnoreCase);
        if (tablesByName.Count != manifest.Tables.Count)
        {
            throw new InvalidDataException("The materialized Paradox table count does not match the template manifest.");
        }

        foreach (StructureTemplateTable expected in manifest.Tables)
        {
            if (!tablesByName.TryGetValue(expected.FileName, out ParadoxTableInventory? actual))
            {
                throw new InvalidDataException($"A structure-template table is missing: {expected.FileName}");
            }

            ValidateTable(
                expected,
                actual,
                directory,
                generatedParadoxFiles,
                manifest.EncryptionKey,
                manifest.IndexMaximumTableSize);
        }
    }

    private void ValidateTable(
        StructureTemplateTable expected,
        ParadoxTableInventory actual,
        string directory,
        bool generatedParadoxFiles,
        uint templateEncryptionKey,
        int indexMaximumTableSize)
    {
        ParadoxTableSchema schema = actual.Schema;
        uint expectedEncryption = expected.Encrypted ? templateEncryptionKey : 0;
        if (schema.Version != expected.Version ||
            (generatedParadoxFiles &&
             !schema.Name.Equals(expected.FileName, StringComparison.OrdinalIgnoreCase)) ||
            schema.DeclaredRecordCount != expected.ExpectedRecords ||
            schema.DeclaredBlockCount != expected.ExpectedBlocks ||
            schema.Encrypted != expected.Encrypted ||
            !actual.HasPrimaryIndex ||
            actual.HasMemoFile != (expected.MemoFile is not null) ||
            !Path.GetFileNameWithoutExtension(expected.PrimaryIndexFile)
                .Equals(Path.GetFileNameWithoutExtension(expected.FileName), StringComparison.OrdinalIgnoreCase) ||
            !File.Exists(Path.Combine(directory, expected.PrimaryIndexFile)) ||
            schema.Fields.Count != expected.Fields.Count ||
            schema.Fields.Where((field, index) =>
                    !field.Name.Equals(expected.Fields[index].Name, StringComparison.Ordinal) ||
                    field.TypeCode != expected.Fields[index].TypeCode ||
                    field.Length != expected.Fields[index].Length)
                .Any())
        {
            throw new InvalidDataException($"The table schema or baseline count does not match the template: {expected.FileName}");
        }

        byte[] primaryIndex = File.ReadAllBytes(Path.Combine(directory, expected.PrimaryIndexFile));
        if (ParadoxRecordWriter.ReadEncryption(primaryIndex) != expectedEncryption ||
            primaryIndex[5] != indexMaximumTableSize)
        {
            throw new InvalidDataException($"The primary-index encryption does not match the table: {expected.PrimaryIndexFile}");
        }
        ParadoxRecordWriter.TableHeader primaryIndexHeader =
            ParadoxRecordWriter.ReadHeader(primaryIndex, 1, expected.PrimaryIndexFile);
        if (primaryIndexHeader.PrimaryKeyFieldCount != 0 ||
            primaryIndexHeader.BlockCount != expected.ExpectedPrimaryIndexBlocks)
        {
            throw new InvalidDataException($"The primary-index structure is invalid: {expected.PrimaryIndexFile}");
        }

        if (expected.MemoFile is not null &&
            !File.Exists(Path.Combine(directory, expected.MemoFile)))
        {
            throw new InvalidDataException($"A required memo file is missing: {expected.MemoFile}");
        }

        byte[] databaseFile = File.ReadAllBytes(Path.Combine(directory, expected.FileName));
        if (ParadoxRecordWriter.ReadEncryption(databaseFile) != expectedEncryption ||
            databaseFile[5] != expected.MaximumTableSize)
        {
            throw new InvalidDataException($"The table encryption does not match its manifest: {expected.FileName}");
        }

        int[] expectedIndexes = expected.SecondaryIndexes.Select(static index => index.Number).Order().ToArray();
        if (!actual.SecondaryIndexes.Order().SequenceEqual(expectedIndexes))
        {
            throw new InvalidDataException($"Secondary-index pairs do not match the template: {expected.FileName}");
        }

        foreach (StructureTemplateIndex index in expected.SecondaryIndexes)
        {
            byte[] xg = File.ReadAllBytes(Path.Combine(directory, index.XgFile));
            byte[] yg = File.ReadAllBytes(Path.Combine(directory, index.YgFile));
            ParadoxRecordWriter.TableHeader xgHeader =
                ParadoxRecordWriter.ReadHeader(xg, 8, index.XgFile);
            ParadoxRecordWriter.TableHeader ygHeader =
                ParadoxRecordWriter.ReadHeader(yg, 7, index.YgFile);
            if (xg.Length < 10 || yg.Length < 10 ||
                xg[4] != 8 || yg[4] != 7 ||
                xg[5] != expected.MaximumTableSize ||
                yg[5] != indexMaximumTableSize ||
                ygHeader.PrimaryKeyFieldCount != 0 ||
                xgHeader.BlockCount != index.ExpectedXgBlocks ||
                ygHeader.BlockCount != index.ExpectedYgBlocks ||
                ParadoxRecordWriter.ReadEncryption(xg) != expectedEncryption ||
                ParadoxRecordWriter.ReadEncryption(yg) != expectedEncryption ||
                BinaryPrimitives.ReadInt32LittleEndian(xg.AsSpan(6, sizeof(int))) != index.ExpectedXgRecords ||
                BinaryPrimitives.ReadInt32LittleEndian(yg.AsSpan(6, sizeof(int))) != index.ExpectedYgRecords ||
                index.ExpectedXgRecords != expected.ExpectedRecords)
            {
                throw new InvalidDataException($"A secondary-index baseline is invalid: {index.XgFile}/{index.YgFile}");
            }
        }

        if (expected.BaselineData is not null)
        {
            ValidateBaselineRows(expected, schema, directory);
        }
    }

    private void ValidateBaselineRows(
        StructureTemplateTable table,
        ParadoxTableSchema schema,
        string directory)
    {
        StructureTemplateBaseline baseline = table.BaselineData
            ?? throw new InvalidOperationException("The baseline data is missing.");
        if (!baseline.ResourceName.Equals(NamesdayResourceName, StringComparison.Ordinal) ||
            baseline.ExpectedRecords != table.ExpectedRecords)
        {
            throw new InvalidDataException($"The baseline data reference is invalid: {table.FileName}");
        }

        NamesdayDefaults.ValidateSchema(schema);
        IReadOnlyList<IReadOnlyDictionary<string, string?>> expectedRows = NamesdayDefaults.Read();
        if (expectedRows.Count != baseline.ExpectedRecords)
        {
            throw new InvalidDataException($"The baseline row count is invalid: {table.FileName}");
        }

        ParadoxTable actualTable = _tableReader.Read(Path.Combine(directory, table.FileName));
        if (actualTable.Records.Count != expectedRows.Count)
        {
            throw new InvalidDataException($"The materialized baseline row count is invalid: {table.FileName}");
        }

        for (int row = 0; row < expectedRows.Count; row++)
        {
            for (int field = 0; field < schema.Fields.Count; field++)
            {
                string fieldName = schema.Fields[field].Name;
                if (!expectedRows[row].TryGetValue(fieldName, out string? expectedValue) ||
                    !string.Equals(actualTable.Records[row][field], expectedValue, StringComparison.Ordinal))
                {
                    throw new InvalidDataException(
                        $"The materialized baseline data differs at {table.FileName}, row {row + 1}, field {fieldName}.");
                }
            }
        }
    }

    private byte[] ReadAndValidateAsset(StructureTemplateAsset asset)
    {
        using Stream archiveResource = _assembly.GetManifestResourceStream(AssetArchiveResourceName)
            ?? throw new InvalidDataException("The embedded structure-template archive is missing.");
        using ZipArchive archive = new(archiveResource, ZipArchiveMode.Read);
        ZipArchiveEntry entry = archive.GetEntry(asset.FileName)
            ?? throw new InvalidDataException($"An archived structure-template asset is missing: {asset.FileName}");
        if (!entry.FullName.Equals(asset.FileName, StringComparison.Ordinal) ||
            entry.Length != asset.SizeBytes)
        {
            throw new InvalidDataException($"An archived structure-template asset has an invalid path or size: {asset.FileName}");
        }

        using MemoryStream contents = new();
        using (Stream resource = entry.Open())
        {
            resource.CopyTo(contents);
        }

        byte[] bytes = contents.ToArray();
        string hash = Convert.ToHexString(SHA256.HashData(bytes));
        if (!hash.Equals(asset.Sha256, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException($"An embedded structure-template asset hash is invalid: {asset.FileName}");
        }

        return bytes;
    }

    private void ValidateAssetArchive(StructureTemplateManifest manifest)
    {
        using Stream archiveResource = _assembly.GetManifestResourceStream(AssetArchiveResourceName)
            ?? throw new InvalidDataException("The embedded structure-template archive is missing.");
        using ZipArchive archive = new(archiveResource, ZipArchiveMode.Read);
        HashSet<string> expectedNames = manifest.Assets
            .Select(static asset => asset.FileName)
            .ToHashSet(StringComparer.Ordinal);
        HashSet<string> actualNames = new(StringComparer.Ordinal);
        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            if (entry.FullName.Length == 0 ||
                entry.FullName.IndexOfAny(['\\', '/']) >= 0 ||
                !actualNames.Add(entry.FullName) ||
                !expectedNames.Contains(entry.FullName))
            {
                throw new InvalidDataException($"The structure-template archive contains an invalid entry: {entry.FullName}");
            }
        }

        if (!actualNames.SetEquals(expectedNames))
        {
            throw new InvalidDataException("The structure-template archive entries do not match the manifest.");
        }
    }

    private static void ValidateManifest(StructureTemplateManifest manifest)
    {
        if (manifest.SchemaVersion != SupportedSchemaVersion ||
            string.IsNullOrWhiteSpace(manifest.TemplateId) ||
            string.IsNullOrWhiteSpace(manifest.SourceDescription) ||
            !manifest.Encoding.Equals("Windows-1252", StringComparison.OrdinalIgnoreCase) ||
            manifest.Assets.Count == 0 ||
            manifest.Tables.Count == 0 ||
            manifest.IndexMaximumTableSize is < 1 or > 32)
        {
            throw new InvalidDataException("The structure-template manifest metadata is invalid.");
        }

        HashSet<string> fileNames = new(StringComparer.OrdinalIgnoreCase);
        HashSet<string> resourceNames = new(StringComparer.Ordinal);
        foreach (StructureTemplateAsset asset in manifest.Assets)
        {
            if (string.IsNullOrWhiteSpace(asset.FileName) ||
                asset.FileName is "." or ".." ||
                asset.FileName.IndexOfAny(['\\', '/', ':']) >= 0 ||
                !fileNames.Add(asset.FileName) ||
                !resourceNames.Add(asset.ResourceName) ||
                !asset.ResourceName.Equals(AssetResourcePrefix + asset.FileName, StringComparison.Ordinal) ||
                asset.SizeBytes < 0 ||
                asset.Sha256.Length != 64 ||
                asset.Sha256.Any(static character => !Uri.IsHexDigit(character)) ||
                string.IsNullOrWhiteSpace(asset.Kind))
            {
                throw new InvalidDataException($"A structure-template asset entry is invalid: {asset.FileName}");
            }
        }

        HashSet<string> tableNames = new(StringComparer.OrdinalIgnoreCase);
        if (manifest.Tables.Any(static table => table.Encrypted) && manifest.EncryptionKey == 0)
        {
            throw new InvalidDataException("The encrypted structure-template tables have no encryption key.");
        }

        foreach (StructureTemplateTable table in manifest.Tables)
        {
            if (string.IsNullOrWhiteSpace(table.FileName) ||
                !table.FileName.EndsWith(".DB", StringComparison.OrdinalIgnoreCase) ||
                !tableNames.Add(table.FileName) ||
                !fileNames.Contains(table.FileName) ||
                !fileNames.Contains(table.PrimaryIndexFile) ||
                table.ExpectedRecords < 0 ||
                table.ExpectedBlocks < 0 ||
                table.ExpectedPrimaryIndexBlocks < 0 ||
                table.RecordSize <= 0 ||
                table.MaximumTableSize is < 1 or > 32 ||
                table.Fields.Count == 0 ||
                table.Fields.Sum(static field => field.Length) != table.RecordSize ||
                table.SecondaryIndexes.Select(static index => index.Number).Distinct().Count() !=
                table.SecondaryIndexes.Count)
            {
                throw new InvalidDataException($"A structure-template table entry is invalid: {table.FileName}");
            }

            if (table.MemoFile is not null && !fileNames.Contains(table.MemoFile))
            {
                throw new InvalidDataException($"A structure-template memo asset is missing: {table.MemoFile}");
            }

            foreach (StructureTemplateIndex index in table.SecondaryIndexes)
            {
                if (index.Number is < 0 or > 15 ||
                    !fileNames.Contains(index.XgFile) ||
                    !fileNames.Contains(index.YgFile) ||
                    index.KeyFields.Count == 0 ||
                    index.Fields.Count != index.KeyFields.Count + 1 ||
                    index.ExpectedXgRecords < 0 ||
                    index.ExpectedYgRecords < 0 ||
                    index.ExpectedXgBlocks < 0 ||
                    index.ExpectedYgBlocks < 0 ||
                    index.ExpectedXgRecords != table.ExpectedRecords)
                {
                    throw new InvalidDataException($"A structure-template index entry is invalid: {table.FileName}");
                }
            }

            if (table.BaselineData is not null &&
                (!table.BaselineData.ResourceName.Equals(NamesdayResourceName, StringComparison.Ordinal) ||
                 table.BaselineData.ExpectedRecords != table.ExpectedRecords))
            {
                throw new InvalidDataException($"A structure-template baseline entry is invalid: {table.FileName}");
            }
        }

        string[] databaseAssets = fileNames
            .Where(static fileName => Path.GetExtension(fileName).Equals(".DB", StringComparison.OrdinalIgnoreCase))
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (!databaseAssets.SequenceEqual(tableNames.Order(StringComparer.OrdinalIgnoreCase), StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("The structure-template table entries do not cover every DB asset.");
        }
    }
}
