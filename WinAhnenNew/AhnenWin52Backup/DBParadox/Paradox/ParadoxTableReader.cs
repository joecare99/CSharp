using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using AhnWin52Backup.Core.Abstractions;
using AhnWin52Backup.Core.Hej;

namespace AhnWin52Backup.Core.Paradox;

/// <summary>Reads AhnWin Paradox 5/7 .DB tables and their supported field types.</summary>
public sealed class ParadoxTableReader : IParadoxTableReader
{
    private const int CommonHeaderSize = 0x58;
    private const int DataHeaderSize = 0x78;
    private const int DataBlockHeaderSize = 6;
    private const int MaximumFieldCount = 256;
    private const uint EncryptionMarker = 0xFF00FF00;
    private const byte AlphaType = 0x01;
    private const byte DateType = 0x02;
    private const byte ShortType = 0x03;
    private const byte LongType = 0x04;
    private const byte CurrencyType = 0x05;
    private const byte NumberType = 0x06;
    private const byte LogicalType = 0x09;
    private const byte MemoType = 0x0C;
    private const byte BlobType = 0x0D;
    private const byte FormattedMemoType = 0x0E;
    private const byte OleType = 0x0F;
    private const byte GraphicType = 0x10;
    private const byte TimeType = 0x14;
    private const byte TimestampType = 0x15;
    private const byte AutoIncrementType = 0x16;
    private const byte BcdType = 0x17;
    private const byte BytesType = 0x18;

    /// <inheritdoc />
    public ParadoxTable Read(string databaseFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseFilePath);

        string fullPath = Path.GetFullPath(databaseFilePath);
        byte[] database = File.ReadAllBytes(fullPath);
        try
        {
            TableHeader header = ReadHeader(database, database.Length);
            List<IReadOnlyList<string?>> records = ReadRecords(database, header, fullPath);

            return new ParadoxTable(header.Name, header.Fields, records);
        }
        catch (OverflowException exception)
        {
            throw new InvalidDataException("Paradox table dimensions exceed the supported file size.", exception);
        }
    }

    public ParadoxTableSchema ReadSchema(string databaseFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseFilePath);
        using FileStream stream = new(databaseFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        Span<byte> prefix = stackalloc byte[CommonHeaderSize];
        stream.ReadExactly(prefix);
        int headerSize = BinaryPrimitives.ReadUInt16LittleEndian(prefix[2..]);
        if (headerSize < DataHeaderSize || headerSize > stream.Length)
        {
            throw new InvalidDataException("The Paradox header size is invalid.");
        }

        byte[] bytes = new byte[headerSize];
        stream.Position = 0;
        stream.ReadExactly(bytes);
        try
        {
            TableHeader header = ReadHeader(bytes, stream.Length);
            return new ParadoxTableSchema(header.Name, header.Fields, header.RecordCount,
                header.BlockCount, bytes[0x39], header.Encryption != 0);
        }
        catch (OverflowException exception)
        {
            throw new InvalidDataException("Paradox table dimensions exceed the supported file size.", exception);
        }
    }

    private static TableHeader ReadHeader(byte[] database, long fileLength)
    {
        if (database.Length < CommonHeaderSize)
        {
            throw new InvalidDataException("The file is too small to contain a Paradox table header.");
        }

        int recordSize = ReadUInt16(database, 0);
        int headerSize = ReadUInt16(database, 2);
        byte fileType = database[4];
        int maximumTableSize = database[5];
        int recordCount = ReadInt32(database, 6);
        int blockCount = ReadUInt16(database, 0x0C);
        int firstBlock = ReadUInt16(database, 0x0E);
        byte version = database[0x39];
        int fieldCount = ReadUInt16(database, 0x21);

        if (fileType is not 0 and not 2)
        {
            throw new InvalidDataException($"Paradox file type {fileType} is not a .DB table.");
        }

        if (version is not 0x0B and not 0x0C)
        {
            throw new NotSupportedException($"Paradox file version 0x{version:X2} is not supported; expected version 5 or 7.");
        }

        if (recordSize <= 0 || headerSize < CommonHeaderSize || headerSize > database.Length)
        {
            throw new InvalidDataException("The Paradox header contains an invalid record or header size.");
        }

        if (maximumTableSize is < 1 or > 32 || recordCount < 0 || blockCount < 0 ||
            fieldCount is < 1 or > MaximumFieldCount)
        {
            throw new InvalidDataException("The Paradox header contains invalid table dimensions.");
        }

        int dataHeaderOffset = version >= 0x05 ? DataHeaderSize : CommonHeaderSize;
        if (dataHeaderOffset > headerSize)
        {
            throw new InvalidDataException("The Paradox data header is truncated.");
        }

        uint encryption = ReadUInt32(database, 0x25);
        if (encryption == EncryptionMarker)
        {
            if (database.Length < DataHeaderSize)
            {
                throw new InvalidDataException("The encrypted Paradox data header is truncated.");
            }

            encryption = ReadUInt32(database, 0x5C);
        }

        IReadOnlyList<ParadoxField> fields = ReadFields(database, dataHeaderOffset, headerSize, fieldCount, version);
        int calculatedRecordSize = fields.Sum(static field => field.Length);
        if (calculatedRecordSize != recordSize)
        {
            throw new InvalidDataException(
                $"Paradox fields occupy {calculatedRecordSize} bytes but the header declares {recordSize} bytes per record.");
        }

        string name = ReadTableName(database, dataHeaderOffset, headerSize, fieldCount, version);
        int tableBlockSize = checked(maximumTableSize * 1024);
        long availableDataBlocks = (fileLength - headerSize) / tableBlockSize;
        if (blockCount > availableDataBlocks)
        {
            throw new InvalidDataException("The Paradox header declares data blocks beyond the end of the file.");
        }

        long recordsPerBlock = (tableBlockSize - DataBlockHeaderSize) / recordSize;
        if (recordCount > recordsPerBlock * blockCount)
        {
            throw new InvalidDataException("The Paradox header declares more records than its data blocks can contain.");
        }

        if (recordCount > 0 && (blockCount == 0 || firstBlock == 0))
        {
            throw new InvalidDataException("The Paradox table declares records but has no first data block.");
        }

        return new TableHeader(
            name,
            fields,
            recordSize,
            headerSize,
            maximumTableSize,
            recordCount,
            blockCount,
            firstBlock,
            encryption);
    }

    private static IReadOnlyList<ParadoxField> ReadFields(
        byte[] database,
        int dataHeaderOffset,
        int headerSize,
        int fieldCount,
        byte version)
    {
        int descriptorsEnd = checked(dataHeaderOffset + fieldCount * 2);
        int fieldNamePointersEnd = checked(descriptorsEnd + 4 + fieldCount * 4);
        int tableNameLength = version == 0x0C ? 261 : 79;
        int fieldNamesOffset = checked(fieldNamePointersEnd + tableNameLength);
        if (fieldNamesOffset > headerSize)
        {
            throw new InvalidDataException("The Paradox field-name table exceeds the header.");
        }

        List<ParadoxField> fields = new(fieldCount);
        HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);
        int nameOffset = fieldNamesOffset;
        for (int index = 0; index < fieldCount; index++)
        {
            int descriptorOffset = dataHeaderOffset + index * 2;
            byte type = database[descriptorOffset];
            byte storedLength = database[descriptorOffset + 1];
            int length = type == BcdType ? 17 : storedLength;
            string name = ReadNullTerminatedAscii(database, ref nameOffset, headerSize);
            if (name.Length == 0 || !names.Add(name))
            {
                throw new InvalidDataException("The Paradox table contains an empty or duplicate field name.");
            }

            if (length <= 0)
            {
                throw new InvalidDataException($"Paradox field \"{name}\" has an invalid field length.");
            }

            fields.Add(new ParadoxField(name, type, length));
        }

        return fields;
    }

    private static string ReadTableName(
        byte[] database,
        int dataHeaderOffset,
        int headerSize,
        int fieldCount,
        byte version)
    {
        int tableNameOffset = checked(dataHeaderOffset + fieldCount * 2 + 4 + fieldCount * 4);
        int tableNameLength = version == 0x0C ? 261 : 79;
        if (tableNameOffset > headerSize - tableNameLength)
        {
            throw new InvalidDataException("The Paradox table name exceeds the header.");
        }

        int terminator = Array.IndexOf(database, (byte)0, tableNameOffset, tableNameLength);
        int length = terminator < 0 ? tableNameLength : terminator - tableNameOffset;
        return Encoding.ASCII.GetString(database, tableNameOffset, length);
    }

    private static List<IReadOnlyList<string?>> ReadRecords(
        byte[] database,
        TableHeader header,
        string databaseFilePath)
    {
        List<IReadOnlyList<string?>> records = new(header.RecordCount);
        if (header.RecordCount == 0)
        {
            return records;
        }

        int blockSize = checked(header.MaximumTableSize * 1024);
        int blockNumber = header.FirstBlock;
        HashSet<int> visitedBlocks = new();
        ParadoxMemoReader? memoReader = null;
        string memoFilePath = Path.ChangeExtension(databaseFilePath, ".MB");

        while (blockNumber != 0 && records.Count < header.RecordCount)
        {
            if (blockNumber > header.BlockCount || !visitedBlocks.Add(blockNumber))
            {
                throw new InvalidDataException("The Paradox data-block chain contains an invalid or repeated block number.");
            }

            int blockOffset = checked(header.HeaderSize + (blockNumber - 1) * blockSize);
            byte[] block = database.AsSpan(blockOffset, blockSize).ToArray();
            if (header.Encryption != 0)
            {
                ParadoxBlockCipher.DecryptDatabaseBlock(block, header.Encryption, blockNumber);
            }

            int nextBlock = ReadUInt16(block, 0);
            int dataSize = ReadInt16(block, 4);
            int maximumRecordData = blockSize - DataBlockHeaderSize;
            if (dataSize < -header.RecordSize || dataSize > maximumRecordData - header.RecordSize ||
                dataSize < 0 && dataSize != -header.RecordSize ||
                dataSize >= 0 && dataSize % header.RecordSize != 0)
            {
                throw new InvalidDataException($"Paradox data block {blockNumber} has an invalid record-data size.");
            }

            int recordsInBlock = dataSize < 0 ? 0 : dataSize / header.RecordSize + 1;
            if (recordsInBlock > (maximumRecordData / header.RecordSize))
            {
                throw new InvalidDataException($"Paradox data block {blockNumber} declares too many records.");
            }

            for (int index = 0; index < recordsInBlock && records.Count < header.RecordCount; index++)
            {
                int recordOffset = DataBlockHeaderSize + index * header.RecordSize;
                records.Add(ReadRecord(
                    block.AsSpan(recordOffset, header.RecordSize),
                    header.Fields,
                    memoFilePath,
                    header.Encryption,
                    ref memoReader));
            }

            blockNumber = nextBlock;
        }

        if (records.Count != header.RecordCount)
        {
            throw new InvalidDataException(
                $"The Paradox table header declares {header.RecordCount} records but only {records.Count} active records were found.");
        }

        return records;
    }

    private static IReadOnlyList<string?> ReadRecord(
        ReadOnlySpan<byte> record,
        IReadOnlyList<ParadoxField> fields,
        string memoFilePath,
        uint encryption,
        ref ParadoxMemoReader? memoReader)
    {
        string?[] values = new string?[fields.Count];
        int fieldOffset = 0;
        for (int index = 0; index < fields.Count; index++)
        {
            ParadoxField field = fields[index];
            ReadOnlySpan<byte> fieldData = record.Slice(fieldOffset, field.Length);
            values[index] = ReadField(field, fieldData, memoFilePath, encryption, ref memoReader);
            fieldOffset += field.Length;
        }

        return values;
    }

    private static string? ReadField(
        ParadoxField field,
        ReadOnlySpan<byte> data,
        string memoFilePath,
        uint encryption,
        ref ParadoxMemoReader? memoReader)
    {
        switch (field.TypeCode)
        {
            case AlphaType:
                return DecodeAlpha(data);
            case ShortType:
                return DecodeInteger(data, 2).ToString(CultureInfo.InvariantCulture);
            case LongType:
            case DateType:
            case TimeType:
            case AutoIncrementType:
                return DecodeInteger(data, 4).ToString(CultureInfo.InvariantCulture);
            case LogicalType:
                return DecodeLogical(data);
            case MemoType:
            case BlobType:
            case FormattedMemoType:
            case OleType:
            case GraphicType:
            {
                memoReader ??= new ParadoxMemoReader(memoFilePath, encryption);
                byte[]? blob = memoReader.Read(data, field.TypeCode == GraphicType);
                if (blob is null)
                {
                    return null;
                }

                return field.TypeCode == GraphicType || field.TypeCode is BlobType or OleType
                    ? Convert.ToBase64String(blob)
                    : Windows1252.Decode(blob).TrimEnd('\0');
            }
            case BytesType:
                return Convert.ToHexString(data);
            case CurrencyType:
            case NumberType:
            case TimestampType:
            case BcdType:
                throw new NotSupportedException(
                    $"Paradox field type 0x{field.TypeCode:X2} in field \"{field.Name}\" is not supported by this adapter.");
            default:
                throw new NotSupportedException(
                    $"Unknown Paradox field type 0x{field.TypeCode:X2} in field \"{field.Name}\".");
        }
    }

    private static string? DecodeAlpha(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty || data[0] == 0)
        {
            return null;
        }

        int terminator = data.IndexOf((byte)0);
        ReadOnlySpan<byte> text = terminator < 0 ? data : data[..terminator];
        return Windows1252.Decode(text).TrimEnd(' ');
    }

    private static long DecodeInteger(ReadOnlySpan<byte> data, int expectedLength)
    {
        if (data.Length != expectedLength || expectedLength is not 2 and not 4)
        {
            throw new InvalidDataException("A Paradox integer field has an unexpected physical length.");
        }

        Span<byte> normalized = stackalloc byte[4];
        data.CopyTo(normalized);
        if ((normalized[0] & 0x80) != 0)
        {
            normalized[0] &= 0x7F;
        }
        else
        {
            bool hasValue = false;
            foreach (byte value in data)
            {
                hasValue |= value != 0;
            }

            if (!hasValue)
            {
                return 0;
            }

            normalized[0] |= 0x80;
        }

        return expectedLength == 2
            ? BinaryPrimitives.ReadInt16BigEndian(normalized)
            : BinaryPrimitives.ReadInt32BigEndian(normalized);
    }

    private static string? DecodeLogical(ReadOnlySpan<byte> data)
    {
        if (data.Length != 1 || data[0] == 0)
        {
            return null;
        }

        byte value = (byte)(data[0] & 0x7F);
        return value switch
        {
            1 => "True",
            0 => "False",
            _ => value.ToString(CultureInfo.InvariantCulture)
        };
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
            throw new InvalidDataException("A Paradox field name is not terminated within the table header.");
        }

        string value = Encoding.ASCII.GetString(data, start, offset - start);
        offset++;
        return value;
    }

    private static int ReadUInt16(byte[] data, int offset) =>
        BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset, sizeof(ushort)));

    private static int ReadInt16(byte[] data, int offset) =>
        BinaryPrimitives.ReadInt16LittleEndian(data.AsSpan(offset, sizeof(short)));

    private static int ReadInt32(byte[] data, int offset) =>
        BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(offset, sizeof(int)));

    private static uint ReadUInt32(byte[] data, int offset) =>
        BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, sizeof(uint)));

    private sealed record TableHeader(
        string Name,
        IReadOnlyList<ParadoxField> Fields,
        int RecordSize,
        int HeaderSize,
        int MaximumTableSize,
        int RecordCount,
        int BlockCount,
        int FirstBlock,
        uint Encryption);
}
