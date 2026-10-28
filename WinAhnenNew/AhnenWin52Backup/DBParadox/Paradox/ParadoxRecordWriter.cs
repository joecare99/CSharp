using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Hej;

namespace AhnWin52Backup.Core.Paradox;

/// <summary>Writes restored records and rebuilt indexes into empty Paradox tables.</summary>
internal sealed class ParadoxRecordWriter
{
    private const int DataHeaderSize = 0x78;
    private const int DataBlockHeaderSize = 6;
    private const uint EncryptionMarker = 0xFF00FF00;
    private const byte AlphaType = 0x01;
    private const byte ShortType = 0x03;
    private const byte LongType = 0x04;
    private const byte MemoType = 0x0C;
    private const byte BlobType = 0x0D;
    private const byte FormattedMemoType = 0x0E;
    private const byte OleType = 0x0F;
    private const byte GraphicType = 0x10;
    private const byte AutoIncrementType = 0x16;

    private readonly IParadoxTableReader _tableReader;

    public ParadoxRecordWriter(IParadoxTableReader tableReader)
    {
        ArgumentNullException.ThrowIfNull(tableReader);
        _tableReader = tableReader;
    }

    public void AppendRecords(
        string databaseFilePath,
        string primaryKeyField,
        IReadOnlyList<IReadOnlyDictionary<string, string?>> records)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(primaryKeyField);
        ArgumentNullException.ThrowIfNull(records);
        if (records.Count == 0)
        {
            throw new ArgumentException("At least one record is required.", nameof(records));
        }

        string fullPath = Path.GetFullPath(databaseFilePath);
        ParadoxTable table = _tableReader.Read(fullPath);
        if (table.Records.Count != 0)
        {
            throw new InvalidOperationException("The experimental writer only accepts empty tables.");
        }

        byte[] database = File.ReadAllBytes(fullPath);
        TableHeader databaseHeader = ReadHeader(database, expectedFileType: 0, fullPath);
        if (databaseHeader.RecordCount != 0 || databaseHeader.PrimaryKeyFieldCount != 1)
        {
            throw new InvalidDataException("The empty table must declare zero records and one primary-key field.");
        }

        int primaryFieldIndex = FindField(table.Fields, primaryKeyField);
        ParadoxField keyField = table.Fields[primaryFieldIndex];
        IReadOnlyList<IReadOnlyDictionary<string, string?>> resolvedRecords =
            ResolveAutoIncrementRecords(table.Fields, database, records);
        ParadoxMemoWriter memoWriter = new(Path.ChangeExtension(fullPath, ".MB"), ReadEncryption(database));
        int keyOffset = table.Fields.Take(primaryFieldIndex).Sum(static field => field.Length);
        EncodedRecord[] sortedRecords = resolvedRecords
            .Select(record =>
            {
                byte[] data = EncodeRecord(table.Fields, record, memoWriter);
                return new EncodedRecord(data, data.AsSpan(keyOffset, keyField.Length).ToArray());
            })
            .OrderBy(static record => record.Key, ByteArrayComparer.Instance)
            .ToArray();
        for (int index = 1; index < sortedRecords.Length; index++)
        {
            if (sortedRecords[index - 1].Key.AsSpan().SequenceCompareTo(sortedRecords[index].Key) >= 0)
            {
                throw new ArgumentException("Primary-key values must be unique.", nameof(records));
            }
        }

        string primaryIndexPath = Path.ChangeExtension(fullPath, ".PX");
        byte[] primaryIndex = File.ReadAllBytes(primaryIndexPath);
        TableHeader primaryHeader = ReadHeader(primaryIndex, expectedFileType: 1, primaryIndexPath);
        if (primaryHeader.RecordCount != 0 ||
            primaryHeader.RecordSize != keyField.Length + DataBlockHeaderSize ||
            primaryHeader.FieldCount != 1 ||
            primaryIndex[0x58] != keyField.TypeCode ||
            primaryIndex[0x59] != keyField.Length)
        {
            throw new InvalidDataException(
                $"The primary index header does not match the empty table's key field " +
                $"(records {primaryHeader.RecordCount}, size {primaryHeader.RecordSize}, fields {primaryHeader.FieldCount}, " +
                $"type 0x{primaryIndex[0x58]:X2}/0x{keyField.TypeCode:X2}, length {primaryIndex[0x59]}/{keyField.Length}).");
        }

        int recordsPerTableBlock = GetRecordsPerDataBlock(
            databaseHeader.BlockSize,
            databaseHeader.RecordSize);

        byte[][] encodedRecords = sortedRecords.Select(static record => record.Data).ToArray();
        List<byte[]> tableBlocks = CreateTableBlocks(databaseHeader, encodedRecords, recordsPerTableBlock);
        ParadoxIndexFileBuilder.IndexEntry[] indexEntries = sortedRecords
            .Chunk(recordsPerTableBlock)
            .Select((group, index) => new ParadoxIndexFileBuilder.IndexEntry(
                group[0].Key,
                index + 1,
                group.Length))
            .ToArray();

        byte[] outputDatabase = new byte[checked(databaseHeader.HeaderSize + tableBlocks.Count * databaseHeader.BlockSize)];
        database.AsSpan(0, databaseHeader.HeaderSize).CopyTo(outputDatabase);
        UpdateMultipleBlockHeader(outputDatabase, encodedRecords.Length, tableBlocks.Count);
        for (int index = 0; index < tableBlocks.Count; index++)
        {
            byte[] block = tableBlocks[index];
            uint encryption = ReadEncryption(outputDatabase);
            if (encryption != 0)
            {
                ParadoxBlockCipher.EncryptDatabaseBlock(block, encryption, index + 1);
            }

            block.CopyTo(outputDatabase, databaseHeader.HeaderSize + index * databaseHeader.BlockSize);
        }

        primaryIndex = ParadoxIndexFileBuilder.Build(primaryIndex, 1, indexEntries);

        memoWriter.Save();
        File.WriteAllBytes(fullPath, outputDatabase);
        File.WriteAllBytes(primaryIndexPath, primaryIndex);
    }

    private static List<byte[]> CreateTableBlocks(
        TableHeader header,
        IReadOnlyList<byte[]> records,
        int recordsPerBlock)
    {
        int blockCount = checked((records.Count + recordsPerBlock - 1) / recordsPerBlock);
        if (blockCount > ushort.MaxValue)
        {
            throw new NotSupportedException("The table requires more physical data blocks than Paradox can address.");
        }

        List<byte[]> blocks = new(blockCount);
        for (int start = 0; start < records.Count; start += recordsPerBlock)
        {
            int blockNumber = blocks.Count + 1;
            byte[][] blockRecords = records.Skip(start).Take(recordsPerBlock).ToArray();
            byte[] block = new byte[header.BlockSize];
            BinaryPrimitives.WriteUInt16LittleEndian(
                block,
                blockNumber < blockCount ? checked((ushort)(blockNumber + 1)) : (ushort)0);
            BinaryPrimitives.WriteUInt16LittleEndian(block.AsSpan(2), checked((ushort)(blockNumber - 1)));
            BinaryPrimitives.WriteInt16LittleEndian(
                block.AsSpan(4),
                checked((short)((blockRecords.Length - 1) * header.RecordSize)));
            int offset = DataBlockHeaderSize;
            foreach (byte[] record in blockRecords)
            {
                record.CopyTo(block, offset);
                offset += record.Length;
            }

            blocks.Add(block);
        }

        return blocks;
    }

    internal static byte[] PrepareEmptyBlock(byte[] file, TableHeader header, string path)
    {
        int blockSize = header.BlockSize;
        int blockNumber = header.FirstBlock;
        byte[] block;
        if (header.BlockCount == 0)
        {
            if (header.FirstBlock != 0 || header.LastBlock != 0 || file.Length != header.HeaderSize)
            {
                throw new InvalidDataException($"The empty table has inconsistent block metadata: {path}");
            }

            blockNumber = 1;
            block = new byte[blockSize];
            BinaryPrimitives.WriteInt16LittleEndian(
                block.AsSpan(4, sizeof(short)),
                checked((short)-header.RecordSize));
        }
        else if (header.BlockCount == 1 &&
                 header.FirstBlock == 1 &&
                 header.LastBlock == 1 &&
                 file.Length == header.HeaderSize + blockSize)
        {
            block = file.AsSpan(header.HeaderSize, blockSize).ToArray();
            uint encryption = ReadEncryption(file);
            if (encryption != 0)
            {
                ParadoxBlockCipher.DecryptDatabaseBlock(block, encryption, blockNumber);
            }

            short dataSize = BinaryPrimitives.ReadInt16LittleEndian(block.AsSpan(4, sizeof(short)));
            if (dataSize != -header.RecordSize)
            {
                throw new InvalidDataException($"The existing block is not an empty Paradox data block: {path}");
            }
        }
        else
        {
            throw new NotSupportedException("The experimental writer supports only zero or one empty data block.");
        }

        return block;
    }

    internal static void UpdateSingleBlockHeader(byte[] file, int recordCount)
    {
        UpdateMultipleBlockHeader(file, recordCount, 1);
        byte fileType = file[4];
        if (fileType is 1 or 7)
        {
            BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x1E, sizeof(ushort)), 1);
            file[0x20] = 1;
        }
    }

    internal static void UpdateMultipleBlockHeader(byte[] file, int recordCount, int blockCount)
    {
        if (file.Length < 0x4D || recordCount < 0 || blockCount <= 0 || blockCount > ushort.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(blockCount), "Paradox header dimensions exceed the supported range.");
        }

        byte fileType = file[4];
        BinaryPrimitives.WriteInt32LittleEndian(file.AsSpan(6, sizeof(int)), recordCount);
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x0A, sizeof(ushort)), checked((ushort)blockCount));
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x0C, sizeof(ushort)), checked((ushort)blockCount));
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x0E, sizeof(ushort)), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x10, sizeof(ushort)), checked((ushort)blockCount));
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x3A, sizeof(ushort)), checked((ushort)blockCount));
        if (fileType is 0 or 6 or 8)
        {
            file[0x2D] = unchecked((byte)(file[0x2D] + 1));
        }

        if (fileType is 1 or 6 or 7 or 8)
        {
            uint metadata = BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(0x49, sizeof(uint)));
            BinaryPrimitives.WriteUInt32LittleEndian(file.AsSpan(0x49, sizeof(uint)), unchecked(metadata + 1));
        }

        if (fileType is 1 or 7)
        {
            // The native fixtures advance this marker once for a rewritten index.
            file[0x2C] = unchecked((byte)(file[0x2C] + 1));
        }
    }

    internal static byte[] CommitBlock(byte[] file, byte[] block, TableHeader header, int blockNumber)
    {
        int offset = checked(header.HeaderSize + (blockNumber - 1) * header.BlockSize);
        if (offset == file.Length)
        {
            Array.Resize(ref file, checked(file.Length + header.BlockSize));
        }
        else if (offset > file.Length - header.BlockSize)
        {
            throw new InvalidDataException("The output data block would leave a gap in the Paradox file.");
        }

        if (ReadEncryption(file) is uint encryption and not 0)
        {
            ParadoxBlockCipher.EncryptDatabaseBlock(block, encryption, blockNumber);
        }

        block.CopyTo(file, offset);
        return file;
    }

    internal static byte[] EncodeRecord(
        IReadOnlyList<ParadoxField> fields,
        IReadOnlyDictionary<string, string?> values)
        => EncodeRecord(fields, values, null);

    private static byte[] EncodeRecord(
        IReadOnlyList<ParadoxField> fields,
        IReadOnlyDictionary<string, string?> values,
        ParadoxMemoWriter? memoWriter)
    {
        ArgumentNullException.ThrowIfNull(values);
        foreach (string fieldName in values.Keys)
        {
            if (fields.All(field => !string.Equals(field.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"The table has no field named \"{fieldName}\".", nameof(values));
            }
        }

        return EncodeRecord(fields, fields.Select(field => FindValue(values, field.Name)).ToArray(), memoWriter);
    }

    internal static byte[] EncodeRecord(
        IReadOnlyList<ParadoxField> fields,
        IReadOnlyList<string?> values)
        => EncodeRecord(fields, values, null);

    private static byte[] EncodeRecord(
        IReadOnlyList<ParadoxField> fields,
        IReadOnlyList<string?> values,
        ParadoxMemoWriter? memoWriter)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (fields.Count != values.Count)
        {
            throw new ArgumentException("The value count must match the field count.", nameof(values));
        }

        byte[] record = new byte[fields.Sum(static field => field.Length)];
        int offset = 0;
        for (int fieldIndex = 0; fieldIndex < fields.Count; fieldIndex++)
        {
            ParadoxField field = fields[fieldIndex];
            string? value = values[fieldIndex];
            Span<byte> destination = record.AsSpan(offset, field.Length);
            switch (field.TypeCode)
            {
                case AlphaType:
                destination.Clear();
                    if (!string.IsNullOrEmpty(value))
                    {
                        using MemoryStream encoded = new();
                        Windows1252.Encode(value.AsSpan(), encoded);
                        if (encoded.Length > field.Length)
                        {
                            throw new ArgumentException(
                                $"Value for field \"{field.Name}\" exceeds its {field.Length}-byte Paradox width.");
                        }

                        encoded.ToArray().CopyTo(destination);
                    }

                    break;
                case ShortType:
                    EncodeInteger(value, destination, 2, field.Name);
                    break;
                case LongType:
                case AutoIncrementType:
                    EncodeInteger(value, destination, 4, field.Name);
                    break;
                case MemoType:
                case FormattedMemoType:
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (memoWriter is null)
                        {
                            throw new NotSupportedException(
                                $"A Paradox memo writer is required for field \"{field.Name}\".");
                        }

                        memoWriter.EncodeText(value, field.Length, field.Name).CopyTo(destination);
                    }

                    break;
                case BlobType:
                case OleType:
                case GraphicType:
                    if (!string.IsNullOrEmpty(value))
                    {
                        throw new NotSupportedException(
                            $"The experimental writer does not encode non-empty memo/blob field \"{field.Name}\".");
                    }

                    break;
                default:
                    throw new NotSupportedException(
                        $"The experimental writer does not support field type 0x{field.TypeCode:X2} in \"{field.Name}\".");
            }

            offset += field.Length;
        }

        return record;
    }

    private static void EncodeInteger(string? value, Span<byte> destination, int expectedLength, string fieldName)
    {
        if (destination.Length != expectedLength)
        {
            throw new InvalidDataException($"Integer field \"{fieldName}\" has an unsupported physical size.");
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long number) ||
            number < 0 ||
            (expectedLength == 2 && number > short.MaxValue) ||
            (expectedLength == 4 && number > int.MaxValue))
        {
            throw new ArgumentException($"Value for integer field \"{fieldName}\" is invalid or out of range.");
        }

        if (number == 0)
        {
            return;
        }

        if (expectedLength == 2)
        {
            WriteParadoxShort(destination, (short)number);
        }
        else
        {
            BinaryPrimitives.WriteInt32BigEndian(destination, (int)number);
            destination[0] |= 0x80;
        }
    }

    internal static void WriteParadoxShort(Span<byte> destination, short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(destination, value);
        destination[0] |= 0x80;
    }

    internal static int GetRecordsPerDataBlock(int blockSize, int recordSize)
    {
        int physicalCapacity = (blockSize - DataBlockHeaderSize) / recordSize;
        if (physicalCapacity <= 0)
        {
            throw new InvalidDataException("A Paradox data block cannot contain one record.");
        }

        return Math.Max(1, physicalCapacity - 1);
    }

    internal static IReadOnlyList<IReadOnlyDictionary<string, string?>> ResolveAutoIncrementRecords(
        IReadOnlyList<ParadoxField> fields,
        byte[] database,
        IReadOnlyList<IReadOnlyDictionary<string, string?>> records)
    {
        ParadoxField[] autoFields = fields.Where(static field => field.TypeCode == AutoIncrementType).ToArray();
        if (autoFields.Length == 0)
        {
            return records;
        }

        if (autoFields.Length != 1 || database.Length < 0x4D)
        {
            throw new NotSupportedException("Only one auto-increment field per table is supported.");
        }

        ParadoxField autoField = autoFields[0];
        uint lastIssued = BinaryPrimitives.ReadUInt32LittleEndian(database.AsSpan(0x49, sizeof(uint)));
        List<IReadOnlyDictionary<string, string?>> resolved = new(records.Count);
        foreach (IReadOnlyDictionary<string, string?> record in records)
        {
            string? value = FindValue(record, autoField.Name);
            if (string.IsNullOrWhiteSpace(value))
            {
                if (lastIssued >= int.MaxValue)
                {
                    throw new InvalidDataException($"Auto-increment field \"{autoField.Name}\" has exhausted its range.");
                }

                lastIssued++;
                Dictionary<string, string?> generated = new(record, StringComparer.OrdinalIgnoreCase)
                {
                    [autoField.Name] = lastIssued.ToString(CultureInfo.InvariantCulture)
                };
                resolved.Add(generated);
            }
            else
            {
                if (!int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int explicitId) ||
                    explicitId <= 0)
                {
                    throw new ArgumentException($"Auto-increment field \"{autoField.Name}\" requires a positive integer.");
                }

                resolved.Add(record);
            }
        }

        BinaryPrimitives.WriteUInt32LittleEndian(database.AsSpan(0x49, sizeof(uint)), lastIssued);
        return resolved;
    }

    private static string? FindValue(IReadOnlyDictionary<string, string?> values, string fieldName)
    {
        foreach ((string key, string? value) in values)
        {
            if (string.Equals(key, fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return value;
            }
        }

        return null;
    }

    private static int FindField(IReadOnlyList<ParadoxField> fields, string fieldName)
    {
        for (int index = 0; index < fields.Count; index++)
        {
            if (string.Equals(fields[index].Name, fieldName, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        throw new InvalidDataException($"The table does not contain its configured primary key field \"{fieldName}\".");
    }

    internal static TableHeader ReadHeader(byte[] file, byte expectedFileType, string path)
    {
        if (file.Length < DataHeaderSize || file[4] != expectedFileType)
        {
            throw new InvalidDataException($"The Paradox file header is invalid: {path}");
        }

        int recordSize = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0, sizeof(ushort)));
        int recordCount = BinaryPrimitives.ReadInt32LittleEndian(file.AsSpan(6, sizeof(int)));
        int blockCount = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0x0C, sizeof(ushort)));
        int firstBlock = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0x0E, sizeof(ushort)));
        int lastBlock = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0x10, sizeof(ushort)));
        int maximumTableSize = file[5];
        int fieldCount = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0x21, sizeof(ushort)));
        int keyFieldCount = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(0x23, sizeof(ushort)));
        int headerSize = BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(2, sizeof(ushort)));
        if (recordSize <= 0 ||
            recordCount < 0 ||
            blockCount < 0 ||
            maximumTableSize is < 1 or > 32 ||
            fieldCount <= 0 ||
            headerSize < (expectedFileType is 0 or 2 or 6 or 8 ? DataHeaderSize : 0x58) ||
            headerSize > file.Length)
        {
            throw new InvalidDataException($"The Paradox file dimensions are invalid: {path}");
        }

        int blockSize = checked(maximumTableSize * 1024);
        if (blockCount > (file.Length - headerSize) / blockSize)
        {
            throw new InvalidDataException($"The Paradox file does not contain all declared blocks: {path}");
        }

        return new TableHeader(
            recordSize,
            recordCount,
            blockCount,
            firstBlock,
            lastBlock,
            maximumTableSize,
            fieldCount,
            keyFieldCount,
            headerSize);
    }

    internal static uint ReadEncryption(byte[] file)
    {
        uint encryption = BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(0x25, sizeof(uint)));
        return encryption == EncryptionMarker
            ? BinaryPrimitives.ReadUInt32LittleEndian(file.AsSpan(0x5C, sizeof(uint)))
            : encryption;
    }

    internal sealed record TableHeader(
        int RecordSize,
        int RecordCount,
        int BlockCount,
        int FirstBlock,
        int LastBlock,
        int MaximumTableSize,
        int FieldCount,
        int PrimaryKeyFieldCount,
        int HeaderSize)
    {
        public int BlockSize => checked(MaximumTableSize * 1024);
    }

    private sealed record EncodedRecord(byte[] Data, byte[] Key);

    private sealed class ByteArrayComparer : IComparer<byte[]>
    {
        public static ByteArrayComparer Instance { get; } = new();

        public int Compare(byte[]? left, byte[]? right) =>
            left is null
                ? right is null ? 0 : -1
                : right is null ? 1 : left.AsSpan().SequenceCompareTo(right);
    }
}
