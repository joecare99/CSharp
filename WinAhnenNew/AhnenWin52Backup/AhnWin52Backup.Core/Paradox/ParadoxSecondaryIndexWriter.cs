using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AhnWin52Backup.Core.Abstractions;

namespace AhnWin52Backup.Core.Paradox;

/// <summary>Rebuilds the supported single-block Paradox XG/YG secondary-index pairs.</summary>
internal sealed class ParadoxSecondaryIndexWriter
{
    private const int CommonHeaderSize = 0x58;
    private const int DataHeaderSize = 0x78;
    private const int DataBlockHeaderSize = 6;
    private const byte AlphaType = 0x01;
    private const byte ShortType = 0x03;
    private const byte BcdType = 0x17;
    private const byte SecondaryDataFileType = 8;
    private const byte SecondaryIndexFileType = 7;

    private readonly IParadoxTableReader _tableReader;

    public ParadoxSecondaryIndexWriter(IParadoxTableReader tableReader)
    {
        ArgumentNullException.ThrowIfNull(tableReader);
        _tableReader = tableReader;
    }

    public void RebuildForTable(string databaseFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseFilePath);

        string fullDatabasePath = Path.GetFullPath(databaseFilePath);
        ParadoxTable table = _tableReader.Read(fullDatabasePath);
        byte[] database = File.ReadAllBytes(fullDatabasePath);
        ParadoxRecordWriter.TableHeader databaseHeader =
            ParadoxRecordWriter.ReadHeader(database, expectedFileType: 0, fullDatabasePath);
        if (databaseHeader.BlockCount <= 0 ||
            databaseHeader.FirstBlock <= 0 ||
            databaseHeader.LastBlock <= 0 ||
            table.Records.Count == 0)
        {
            throw new NotSupportedException(
                "The secondary-index writer requires a non-empty table with valid data blocks.");
        }

        string directory = Path.GetDirectoryName(fullDatabasePath)
            ?? throw new InvalidOperationException("The database file has no parent directory.");
        string tableName = Path.GetFileNameWithoutExtension(fullDatabasePath);
        string primaryIndexPath = Path.ChangeExtension(fullDatabasePath, ".PX");
        byte[] primaryIndex = File.ReadAllBytes(primaryIndexPath);
        _ = ParadoxRecordWriter.ReadHeader(primaryIndex, expectedFileType: 1, primaryIndexPath);
        byte primaryRevision = primaryIndex[0x2C];
        string sidecarPrefix = $"{tableName}.XG";
        string[] secondaryDataPaths = Directory.EnumerateFiles(directory)
            .Where(path => IsSecondaryDataFile(Path.GetFileName(path), sidecarPrefix))
            .OrderBy(path => GetIndexNumber(Path.GetFileName(path), sidecarPrefix))
            .ToArray();
        if (secondaryDataPaths.Length == 0)
        {
            throw new FileNotFoundException($"No XG secondary-index files were found for {fullDatabasePath}.");
        }

        foreach (string secondaryDataPath in secondaryDataPaths)
        {
            int indexNumber = GetIndexNumber(Path.GetFileName(secondaryDataPath), sidecarPrefix);
            string secondaryIndexPath = Path.Combine(directory, $"{tableName}.YG{indexNumber}");
            WriteIndexPair(table, secondaryDataPath, secondaryIndexPath, primaryRevision);
        }
    }

    private static void WriteIndexPair(
        ParadoxTable table,
        string secondaryDataPath,
        string secondaryIndexPath,
        byte primaryRevision)
    {
        byte[] secondaryDataFile = File.ReadAllBytes(secondaryDataPath);
        ParadoxRecordWriter.TableHeader secondaryDataHeader =
            ParadoxRecordWriter.ReadHeader(secondaryDataFile, SecondaryDataFileType, secondaryDataPath);
        SecondaryDataSchema secondarySchema = ReadSecondaryDataSchema(secondaryDataFile, secondaryDataHeader);
        IReadOnlyList<ParadoxField> secondaryFields = secondarySchema.Fields;
        int keyFieldCount = secondaryDataHeader.PrimaryKeyFieldCount;
        if (secondaryDataHeader.RecordCount != 0 ||
            keyFieldCount <= 0 ||
            keyFieldCount != secondaryFields.Count - 1 ||
            !IsHintField(secondaryFields[^1]) ||
            secondaryFields[^1].TypeCode != ShortType ||
            secondaryFields[^1].Length != sizeof(short))
        {
            throw new InvalidDataException($"The empty XG index header is inconsistent: {secondaryDataPath}");
        }

        SynchronizePrimaryRevision(secondaryDataFile, primaryRevision);

        int keyLength = secondaryFields.Take(keyFieldCount).Sum(static field => field.Length);
        List<EncodedIndexRecord> encodedRecords = new(table.Records.Count);
        List<string?> encodedValues = new(keyFieldCount + 1);
        Dictionary<string, string?> sourceValues = new(StringComparer.OrdinalIgnoreCase);
        byte indexFieldNumber = secondaryDataFile[0x15];
        foreach (IReadOnlyList<string?> sourceRecord in table.Records)
        {
            encodedValues.Clear();
            sourceValues.Clear();
            for (int fieldIndex = 0; fieldIndex < keyFieldCount; fieldIndex++)
            {
                ParadoxField indexField = secondaryFields[fieldIndex];
                int sourceFieldIndex = FindSourceFieldIndex(
                    table.Fields,
                    indexField,
                    fieldIndex,
                    indexFieldNumber,
                    secondaryDataPath);
                ParadoxField sourceField = table.Fields[sourceFieldIndex];
                if (sourceField.TypeCode != indexField.TypeCode || sourceField.Length != indexField.Length)
                {
                    throw new InvalidDataException(
                        $"Index field \"{indexField.Name}\" does not match the source table schema in {secondaryDataPath}.");
                }

                string? value = sourceRecord[sourceFieldIndex];
                if (sourceValues.TryGetValue(indexField.Name, out string? previousValue) &&
                    !string.Equals(previousValue, value, StringComparison.Ordinal))
                {
                    throw new InvalidDataException(
                        $"Duplicate index field name \"{indexField.Name}\" maps to different source values.");
                }

                sourceValues[indexField.Name] = value;
                if (value is not null &&
                    ShouldUppercaseIndexField(
                        secondarySchema.SortOrder,
                        secondarySchema.IndexLabel,
                        indexField.Name,
                        fieldIndex))
                {
                    value = value.ToUpperInvariant();
                }

                encodedValues.Add(value);
            }

            encodedValues.Add("1");
            byte[] record = ParadoxRecordWriter.EncodeRecord(secondaryFields, encodedValues);
            encodedRecords.Add(new EncodedIndexRecord(record, record.AsSpan(0, keyLength).ToArray()));
        }

        encodedRecords.Sort(static (left, right) =>
            left.Key.AsSpan().SequenceCompareTo(right.Key));

        int recordsPerSecondaryBlock = (secondaryDataHeader.BlockSize - DataBlockHeaderSize) /
                                       secondaryDataHeader.RecordSize;
        if (recordsPerSecondaryBlock <= 0)
        {
            throw new InvalidDataException($"An XG block cannot contain an index entry: {secondaryDataPath}");
        }

        byte[] secondaryIndexFile = File.ReadAllBytes(secondaryIndexPath);
        ParadoxRecordWriter.TableHeader secondaryIndexHeader =
            ParadoxRecordWriter.ReadHeader(secondaryIndexFile, SecondaryIndexFileType, secondaryIndexPath);
        IReadOnlyList<ParadoxField> secondaryIndexFields =
            ReadSecondaryIndexFields(secondaryIndexFile, secondaryIndexHeader);
        if (secondaryIndexHeader.RecordCount != 0 ||
            secondaryIndexFields.Count != keyFieldCount ||
            secondaryIndexHeader.RecordSize != keyLength + DataBlockHeaderSize ||
            secondaryIndexFields.Where((field, index) =>
                    field.TypeCode != secondaryFields[index].TypeCode ||
                    field.Length != secondaryFields[index].Length)
                .Any())
        {
            throw new InvalidDataException($"The empty YG index header does not match its XG key: {secondaryIndexPath}");
        }

        EncodedIndexRecord[][] secondaryGroups = encodedRecords
            .Chunk(recordsPerSecondaryBlock)
            .Select(static group => group.ToArray())
            .ToArray();
        ParadoxIndexFileBuilder.IndexEntry[] indexEntries = secondaryGroups
            .Select((group, index) => new ParadoxIndexFileBuilder.IndexEntry(
                group[0].Key,
                index + 1,
                group.Length))
            .ToArray();

        WriteSecondaryDataFile(secondaryDataFile, secondaryDataHeader, secondaryDataPath, encodedRecords);
        secondaryIndexFile = ParadoxIndexFileBuilder.Build(secondaryIndexFile, SecondaryIndexFileType, indexEntries);
        File.WriteAllBytes(secondaryIndexPath, secondaryIndexFile);
    }

    internal static void SynchronizePrimaryRevision(byte[] secondaryDataFile, byte primaryRevision)
    {
        ArgumentNullException.ThrowIfNull(secondaryDataFile);
        if (secondaryDataFile.Length <= 0x2F ||
            secondaryDataFile[0x2F] != unchecked((byte)(primaryRevision - 1)))
        {
            throw new InvalidDataException("The empty XG revision does not match the preceding primary-index revision.");
        }

        secondaryDataFile[0x2F] = primaryRevision;
    }

    private static void WriteSecondaryDataFile(
        byte[] file,
        ParadoxRecordWriter.TableHeader header,
        string path,
        IReadOnlyList<EncodedIndexRecord> records)
    {
        int recordsPerBlock = (header.BlockSize - DataBlockHeaderSize) / header.RecordSize;
        int blockCount = checked((records.Count + recordsPerBlock - 1) / recordsPerBlock);
        if (blockCount > ushort.MaxValue)
        {
            throw new NotSupportedException($"The XG file requires too many blocks: {path}");
        }

        byte[] output = new byte[checked(header.HeaderSize + blockCount * header.BlockSize)];
        file.AsSpan(0, header.HeaderSize).CopyTo(output);
        ParadoxRecordWriter.UpdateMultipleBlockHeader(output, records.Count, blockCount);
        for (int index = 0; index < blockCount; index++)
        {
            EncodedIndexRecord[] group = records.Skip(index * recordsPerBlock).Take(recordsPerBlock).ToArray();
            byte[] block = new byte[header.BlockSize];
            BinaryPrimitives.WriteUInt16LittleEndian(
                block,
                index + 1 < blockCount ? checked((ushort)(index + 2)) : (ushort)0);
            BinaryPrimitives.WriteUInt16LittleEndian(block.AsSpan(2), checked((ushort)index));
            BinaryPrimitives.WriteInt16LittleEndian(
                block.AsSpan(4),
                checked((short)((group.Length - 1) * header.RecordSize)));
            int offset = DataBlockHeaderSize;
            foreach (EncodedIndexRecord record in group)
            {
                record.Data.CopyTo(block, offset);
                offset += record.Data.Length;
            }

            uint encryption = ParadoxRecordWriter.ReadEncryption(output);
            if (encryption != 0)
            {
                ParadoxBlockCipher.EncryptDatabaseBlock(block, encryption, index + 1);
            }

            block.CopyTo(output, header.HeaderSize + index * header.BlockSize);
        }

        File.WriteAllBytes(path, output);
    }

    private static void WriteSecondaryIndexBlock(
        byte[] file,
        ParadoxRecordWriter.TableHeader header,
        string path,
        byte[] firstKey,
        int secondaryRecordCount)
    {
        byte[] block = ParadoxRecordWriter.PrepareEmptyBlock(file, header, path);
        BinaryPrimitives.WriteInt16LittleEndian(block.AsSpan(4, sizeof(short)), 0);
        firstKey.CopyTo(block, DataBlockHeaderSize);
        Span<byte> reference = block.AsSpan(
            DataBlockHeaderSize + firstKey.Length,
            DataBlockHeaderSize);
        ParadoxRecordWriter.WriteParadoxShort(reference, 1);
        ParadoxRecordWriter.WriteParadoxShort(reference[2..], checked((short)secondaryRecordCount));
        ParadoxRecordWriter.WriteParadoxShort(reference[4..], 0);

        ParadoxRecordWriter.UpdateSingleBlockHeader(file, 1);
        file = ParadoxRecordWriter.CommitBlock(file, block, header, 1);
        File.WriteAllBytes(path, file);
    }

    private static SecondaryDataSchema ReadSecondaryDataSchema(
        byte[] file,
        ParadoxRecordWriter.TableHeader header)
    {
        byte version = file[0x39];
        if (version is not 0x0B and not 0x0C)
        {
            throw new NotSupportedException($"XG file version 0x{version:X2} is not supported.");
        }

        int tableNameLength = version == 0x0C ? 261 : 79;
        int namesOffset = checked(DataHeaderSize + header.FieldCount * 2 + 4 + header.FieldCount * 4 + tableNameLength);
        if (namesOffset >= header.HeaderSize)
        {
            throw new InvalidDataException("The XG field-name table exceeds its header.");
        }

        List<ParadoxField> fields = new(header.FieldCount);
        int nameOffset = namesOffset;
        for (int index = 0; index < header.FieldCount; index++)
        {
            int descriptorOffset = DataHeaderSize + index * 2;
            byte typeCode = file[descriptorOffset];
            int length = typeCode == BcdType ? 17 : file[descriptorOffset + 1];
            string name = ReadNullTerminatedAscii(file, ref nameOffset, header.HeaderSize);
            if (string.IsNullOrWhiteSpace(name) || length <= 0)
            {
                throw new InvalidDataException($"The XG field definition at position {index} is invalid.");
            }

            fields.Add(new ParadoxField(name, typeCode, length));
        }

        if (fields.Sum(static field => field.Length) != header.RecordSize)
        {
            throw new InvalidDataException("The XG field lengths do not match its declared record size.");
        }

        int fieldMapEnd = checked(nameOffset + header.FieldCount * sizeof(ushort));
        string sortOrder = ReadNullTerminatedAscii(file, ref fieldMapEnd, header.HeaderSize);
        string indexLabel = ReadNullTerminatedAscii(file, ref fieldMapEnd, header.HeaderSize);
        if (string.IsNullOrWhiteSpace(sortOrder) || string.IsNullOrWhiteSpace(indexLabel))
        {
            throw new InvalidDataException("The XG sort order or index label is missing.");
        }

        return new SecondaryDataSchema(fields, sortOrder, indexLabel);
    }

    private static IReadOnlyList<ParadoxField> ReadSecondaryIndexFields(
        byte[] file,
        ParadoxRecordWriter.TableHeader header)
    {
        if (header.HeaderSize < CommonHeaderSize + header.FieldCount * 2)
        {
            throw new InvalidDataException("The YG field definitions exceed its header.");
        }

        List<ParadoxField> fields = new(header.FieldCount);
        for (int index = 0; index < header.FieldCount; index++)
        {
            int descriptorOffset = CommonHeaderSize + index * 2;
            byte typeCode = file[descriptorOffset];
            int length = typeCode == BcdType ? 17 : file[descriptorOffset + 1];
            if (length <= 0)
            {
                throw new InvalidDataException($"The YG field definition at position {index} is invalid.");
            }

            fields.Add(new ParadoxField($"Key{index}", typeCode, length));
        }

        return fields;
    }

    private static int FindSourceFieldIndex(
        IReadOnlyList<ParadoxField> sourceFields,
        ParadoxField indexField,
        int indexFieldPosition,
        byte indexFieldNumber,
        string indexPath)
    {
        if (string.Equals(indexField.Name, "Sec Key", StringComparison.OrdinalIgnoreCase))
        {
            int sourceIndex = indexFieldNumber - 1;
            if (indexFieldPosition != 0 || sourceIndex < 0 || sourceIndex >= sourceFields.Count)
            {
                throw new InvalidDataException($"The XG Sec Key field mapping is invalid: {indexPath}");
            }

            return sourceIndex;
        }

        for (int index = 0; index < sourceFields.Count; index++)
        {
            if (string.Equals(sourceFields[index].Name, indexField.Name, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        throw new InvalidDataException(
            $"Index field \"{indexField.Name}\" does not exist in its source table: {indexPath}");
    }

    private static bool IsHintField(ParadoxField field) =>
        string.Equals(field.Name, "Hint", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(field.Name, "Blk Num", StringComparison.OrdinalIgnoreCase);

    private static bool ShouldUppercaseIndexField(
        string sortOrder,
        string indexLabel,
        string fieldName,
        int fieldPosition)
    {
        if (string.Equals(indexLabel, "Ort_", StringComparison.OrdinalIgnoreCase) &&
            fieldPosition == 0 &&
            string.Equals(fieldName, "Ort", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (!sortOrder.Contains("intl", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return indexLabel.ToLowerInvariant() switch
        {
            "namgeb" or "geba" =>
                string.Equals(fieldName, "Name", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(fieldName, "Vornamen", StringComparison.OrdinalIgnoreCase),
            "gebo" => string.Equals(fieldName, "Gebort", StringComparison.OrdinalIgnoreCase),
            "tit" => string.Equals(fieldName, "Titel", StringComparison.OrdinalIgnoreCase),
            _ => false
        };
    }

    private static bool IsSecondaryDataFile(string fileName, string prefix)
    {
        if (!fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        string suffix = fileName[prefix.Length..];
        return suffix.Length > 0 && suffix.All(char.IsAsciiDigit);
    }

    private static int GetIndexNumber(string fileName, string prefix)
    {
        string suffix = fileName[prefix.Length..];
        return int.TryParse(suffix, out int indexNumber)
            ? indexNumber
            : throw new InvalidDataException($"The secondary-index suffix is invalid: {fileName}");
    }

    private static string ReadNullTerminatedAscii(byte[] data, ref int offset, int limit)
    {
        int start = offset;
        while (offset < limit && data[offset] != 0)
        {
            offset++;
        }

        if (offset >= limit)
        {
            throw new InvalidDataException("An XG field name is not terminated within the file header.");
        }

        string value = Encoding.ASCII.GetString(data, start, offset - start);
        offset++;
        return value;
    }

    private sealed record EncodedIndexRecord(byte[] Data, byte[] Key);

    private sealed record SecondaryDataSchema(
        IReadOnlyList<ParadoxField> Fields,
        string SortOrder,
        string IndexLabel);
}
