using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AhnWin52Backup.Core.Workflows;

namespace AhnWin52Backup.Core.Paradox;

/// <summary>Creates empty Paradox table and index files from declarative schemas.</summary>
internal static class ParadoxStructureWriter
{
    private const int CommonHeaderSize = 0x58;
    private const int DataHeaderSize = 0x78;
    private const int DataBlockHeaderSize = 6;
    private const int HeaderAlignment = 0x800;
    private const int MemoBlockSize = 4096;
    private const byte BcdType = 0x17;
    private const byte SecondaryDataFileType = 8;
    private const byte SecondaryIndexFileType = 7;
    private const uint EncryptionMarker = 0xFF00FF00;

    public static bool IsParadoxFileName(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        return extension.Equals(".DB", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".PX", StringComparison.OrdinalIgnoreCase) ||
               extension.Equals(".MB", StringComparison.OrdinalIgnoreCase) ||
               (extension.Length == 4 &&
                (extension.StartsWith(".XG", StringComparison.OrdinalIgnoreCase) ||
                 extension.StartsWith(".YG", StringComparison.OrdinalIgnoreCase)) &&
                Uri.IsHexDigit(extension[3]));
    }

    public static void WriteTableFamily(
        string directory,
        StructureTemplateTable table,
        uint templateEncryptionKey,
        int indexMaximumTableSize)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(directory);
        ArgumentNullException.ThrowIfNull(table);

        uint encryptionKey = table.Encrypted ? templateEncryptionKey : 0;
        if (table.Encrypted != (encryptionKey != 0))
        {
            throw new InvalidDataException($"The Paradox encryption metadata is inconsistent: {table.FileName}");
        }

        ParadoxField[] fields = table.Fields
            .Select(static field => new ParadoxField(field.Name, field.TypeCode, field.Length))
            .ToArray();
        if (fields.Sum(static field => field.Length) != table.RecordSize)
        {
            throw new InvalidDataException($"The declared fields do not match the record width for {table.FileName}.");
        }

        ParadoxField primaryField = fields[0];
        int initialTableBlocks = table.ExpectedRecords == 0 ? table.ExpectedBlocks : 0;
        WriteDatabase(
            Path.Combine(directory, table.FileName),
            table.FileName,
            table.Version,
            fields,
            table.RecordSize,
            table.MaximumTableSize,
            initialTableBlocks,
            encryptionKey,
            table.DatabaseFieldNumbers,
            table.DatabaseSortOrder,
            table.DatabaseHeaderMetadata);
        WritePrimaryIndex(
            Path.Combine(directory, table.PrimaryIndexFile),
            table.PrimaryIndexFile,
            table.Version,
            primaryField,
            indexMaximumTableSize,
            table.ExpectedPrimaryIndexBlocks,
            table.ExpectedRecords == 0 ? table.PrimaryIndexEmptyBlockRecordHex : null,
            encryptionKey,
            table.PrimaryIndexHeaderMetadata);

        if (table.MemoFile is not null)
        {
            File.WriteAllBytes(Path.Combine(directory, table.MemoFile), CreateMemoFile(encryptionKey));
        }

        foreach (StructureTemplateIndex index in table.SecondaryIndexes)
        {
            ParadoxField[] indexFields = index.Fields
                .Select(static field => new ParadoxField(field.Name, field.TypeCode, field.Length))
                .ToArray();
            ParadoxField[] keyFields = indexFields[..^1];
            int keyLength = keyFields.Sum(static field => field.Length);
            if (keyFields.Length != index.KeyFields.Count ||
                !keyFields.Select(static field => field.Name)
                    .SequenceEqual(index.KeyFields, StringComparer.OrdinalIgnoreCase) ||
                !keyFields[^1].Name.Equals(primaryField.Name, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    $"The declared index fields or primary-key suffix are invalid: {index.XgFile}");
            }

            int[] fieldMap = keyFields
                .Select((field, index) => index == keyFields.Length - 1
                    ? keyFields.Length
                    : FindFieldOrdinal(fields, field.Name) + 1)
                .Append(indexFields.Length)
                .ToArray();
            WriteSecondaryDataIndex(
                Path.Combine(directory, index.XgFile),
                index.XgFile,
                table.Version,
                indexFields,
                keyFields.Length,
                fieldMap,
                table.MaximumTableSize,
                index.SortOrder,
                index.Label,
                index.ExpectedXgBlocks,
                index.ExpectedXgRecords == 0 ? index.XgEmptyBlockRecordHex : null,
                encryptionKey,
                index.XgHeaderMetadata);
            WriteSecondaryPrimaryIndex(
                Path.Combine(directory, index.YgFile),
                index.YgFile,
                table.Version,
                keyFields,
                keyLength,
                indexMaximumTableSize,
                index.ExpectedYgBlocks,
                index.ExpectedYgRecords == 0 ? index.YgEmptyBlockRecordHex : null,
                encryptionKey,
                index.YgHeaderMetadata);
        }
    }

    public static void WriteHeaderMetadata(string path, StructureTemplateHeaderMetadata headerMetadata)
    {
        byte[] file = File.ReadAllBytes(path);
        ApplyHeaderMetadata(file, headerMetadata);
        File.WriteAllBytes(path, file);
    }

    private static void WriteDatabase(
        string path,
        string tableName,
        byte version,
        IReadOnlyList<ParadoxField> fields,
        int recordSize,
        int maximumTableSize,
        int blockCount,
        uint encryptionKey,
        IReadOnlyList<int> fieldNumbers,
        string sortOrder,
        StructureTemplateHeaderMetadata headerMetadata)
    {
        byte[] file = CreateHeader(
            path,
            fileType: 0,
            version,
            recordSize,
            fields,
            tableName,
            primaryKeyFields: 1,
            maximumTableSize,
            blockCount,
            includeDataHeader: true,
            includeFieldNames: true,
            fieldMap: fieldNumbers,
            sortOrder,
            indexLabel: null,
            dataHeaderKind: 1,
            encryptionKey,
            headerMetadata);
        WriteEmptyBlocks(
            file,
            ReadHeaderSize(file),
            recordSize,
            maximumTableSize,
            blockCount,
            null,
            encryptionKey);
        File.WriteAllBytes(path, file);
    }

    private static void WritePrimaryIndex(
        string path,
        string tableName,
        byte version,
        ParadoxField keyField,
        int maximumTableSize,
        int blockCount,
        string? emptyBlockRecordHex,
        uint encryptionKey,
        StructureTemplateHeaderMetadata headerMetadata)
    {
        int recordSize = checked(keyField.Length + DataBlockHeaderSize);
        byte[] file = CreateHeader(
            path,
            fileType: 1,
            version,
            recordSize,
            [keyField],
            tableName,
            primaryKeyFields: 0,
            maximumTableSize,
            blockCount,
            includeDataHeader: false,
            includeFieldNames: false,
            fieldMap: [],
            sortOrder: null,
            indexLabel: null,
            dataHeaderKind: 0,
            encryptionKey,
            headerMetadata);
        WriteEmptyBlocks(
            file,
            ReadHeaderSize(file),
            recordSize,
            maximumTableSize,
            blockCount,
            emptyBlockRecordHex,
            encryptionKey);
        File.WriteAllBytes(path, file);
    }

    private static void WriteSecondaryDataIndex(
        string path,
        string tableName,
        byte version,
        IReadOnlyList<ParadoxField> fields,
        int keyFieldCount,
        IReadOnlyList<int> fieldMap,
        int maximumTableSize,
        string sortOrder,
        string label,
        int blockCount,
        string? emptyBlockRecordHex,
        uint encryptionKey,
        StructureTemplateHeaderMetadata headerMetadata)
    {
        int recordSize = fields.Sum(static field => field.Length);
        byte[] file = CreateHeader(
            path,
            SecondaryDataFileType,
            version,
            recordSize,
            fields,
            tableName,
            keyFieldCount,
            maximumTableSize,
            blockCount,
            includeDataHeader: true,
            includeFieldNames: true,
            fieldMap,
            sortOrder,
            label,
            dataHeaderKind: 2,
            encryptionKey,
            headerMetadata);
        WriteEmptyBlocks(
            file,
            ReadHeaderSize(file),
            recordSize,
            maximumTableSize,
            blockCount,
            emptyBlockRecordHex,
            encryptionKey);
        File.WriteAllBytes(path, file);
    }

    private static void WriteSecondaryPrimaryIndex(
        string path,
        string tableName,
        byte version,
        IReadOnlyList<ParadoxField> keyFields,
        int keyLength,
        int maximumTableSize,
        int blockCount,
        string? emptyBlockRecordHex,
        uint encryptionKey,
        StructureTemplateHeaderMetadata headerMetadata)
    {
        int recordSize = checked(keyLength + DataBlockHeaderSize);
        byte[] file = CreateHeader(
            path,
            SecondaryIndexFileType,
            version,
            recordSize,
            keyFields,
            tableName,
            primaryKeyFields: 0,
            maximumTableSize,
            blockCount,
            includeDataHeader: false,
            includeFieldNames: false,
            fieldMap: [],
            sortOrder: null,
            indexLabel: null,
            dataHeaderKind: 0,
            encryptionKey,
            headerMetadata);
        WriteEmptyBlocks(
            file,
            ReadHeaderSize(file),
            recordSize,
            maximumTableSize,
            blockCount,
            emptyBlockRecordHex,
            encryptionKey);
        File.WriteAllBytes(path, file);
    }

    private static byte[] CreateHeader(
        string path,
        byte fileType,
        byte version,
        int recordSize,
        IReadOnlyList<ParadoxField> fields,
        string tableName,
        int primaryKeyFields,
        int maximumTableSize,
        int blockCount,
        bool includeDataHeader,
        bool includeFieldNames,
        IReadOnlyList<int> fieldMap,
        string? sortOrder,
        string? indexLabel,
        int dataHeaderKind,
        uint encryptionKey,
        StructureTemplateHeaderMetadata headerMetadata)
    {
        if (version is not 0x0B and not 0x0C ||
            fields.Count is < 1 or > ushort.MaxValue ||
            recordSize is <= 0 or > ushort.MaxValue ||
            maximumTableSize is < 1 or > 32 ||
            fields.Sum(static field => field.Length) +
            (fileType is 1 or SecondaryIndexFileType ? DataBlockHeaderSize : 0) != recordSize ||
            primaryKeyFields is < 0 or > ushort.MaxValue ||
            blockCount is < 0 or > ushort.MaxValue ||
            headerMetadata is null)
        {
            throw new InvalidDataException($"The Paradox header dimensions are invalid: {path}");
        }

        int tableNameLength = version == 0x0C ? 261 : 79;
        byte[] encodedTableName = Encoding.ASCII.GetBytes(tableName);
        byte[] encodedFieldNames = includeFieldNames
            ? fields.Select(static field => Encoding.ASCII.GetBytes(field.Name + "\0")).SelectMany(static bytes => bytes).ToArray()
            : [];
        byte[] encodedSortOrder = sortOrder is null ? [] : Encoding.ASCII.GetBytes(sortOrder + "\0");
        byte[] encodedIndexLabel = indexLabel is null ? [] : Encoding.ASCII.GetBytes(indexLabel + "\0");
        int dataOffset = includeDataHeader ? DataHeaderSize : CommonHeaderSize;
        int tableNameOffset = checked(
            dataOffset + fields.Count * 2 + sizeof(uint) + (includeFieldNames ? fields.Count * sizeof(uint) : 0));
        int fieldNamesOffset = checked(tableNameOffset + tableNameLength);
        int fieldMapOffset = checked(fieldNamesOffset + encodedFieldNames.Length);
        int metadataEnd = checked(fieldMapOffset +
            (includeFieldNames ? fieldMap.Count * sizeof(ushort) + encodedSortOrder.Length + encodedIndexLabel.Length : 0));
        int headerSize = Math.Max(HeaderAlignment, RoundUp(metadataEnd, HeaderAlignment));
        byte[] header = new byte[checked(headerSize + blockCount * maximumTableSize * 1024)];

        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0), checked((ushort)recordSize));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(2), checked((ushort)headerSize));
        header[4] = fileType;
        header[5] = checked((byte)maximumTableSize);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x0A), checked((ushort)blockCount));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x0C), checked((ushort)blockCount));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x0E), blockCount == 0 ? (ushort)0 : (ushort)1);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x10), checked((ushort)blockCount));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x12), headerMetadata.Word12);
        if (fileType == SecondaryDataFileType)
        {
            if (fieldMap.Count == 0 || fieldMap[0] is < 1 or > byte.MaxValue)
            {
                throw new InvalidDataException($"The XG leading-field map is invalid: {path}");
            }

            header[0x15] = checked((byte)fieldMap[0]);
        }

        BinaryPrimitives.WriteUInt16LittleEndian(
            header.AsSpan(0x1E),
            fileType is 1 or SecondaryIndexFileType ? (ushort)1 : (ushort)0);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x21), checked((ushort)fields.Count));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x23), checked((ushort)primaryKeyFields));
        if (includeDataHeader && encryptionKey != 0)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x25), EncryptionMarker);
        }
        else if (!includeDataHeader)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x25), encryptionKey);
        }

        if (fileType == 0)
        {
            header[0x3E] = 0x1F;
            header[0x3F] = 0x0F;
            header[0x56] = 0x20;
        }

        BinaryPrimitives.WriteUInt32LittleEndian(
            header.AsSpan(0x30),
            checked((uint)(dataOffset + fields.Count * 2 + sizeof(uint))));
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x34), checked((uint)dataOffset));
        header[0x39] = version;
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x3A), checked((ushort)blockCount));
        int realHeaderSize = fileType is 1 or SecondaryIndexFileType
            ? dataOffset + fields.Count * 2 + sizeof(uint) + tableNameLength
            : dataOffset + fields.Count * 8 + sizeof(uint) + tableNameLength + encodedFieldNames.Length + 9;
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x51), checked((ushort)realHeaderSize));

        if (fileType is 0 or SecondaryDataFileType)
        {
            WriteDataHeader(
                header,
                version,
                fields,
                tableNameLength,
                encodedFieldNames.Length,
                dataHeaderKind,
                encryptionKey);
        }

        int descriptorOffset = dataOffset;
        for (int index = 0; index < fields.Count; index++)
        {
            ParadoxField field = fields[index];
            header[descriptorOffset + index * 2] = field.TypeCode;
            header[descriptorOffset + index * 2 + 1] =
                field.TypeCode == BcdType ? (byte)0 : checked((byte)field.Length);
        }

        BinaryPrimitives.WriteUInt32LittleEndian(
            header.AsSpan(descriptorOffset + fields.Count * 2),
            checked((uint)tableNameOffset));
        if (includeFieldNames)
        {
            int pointerOffset = descriptorOffset + fields.Count * 2 + sizeof(uint);
            int nameOffset = fieldNamesOffset;
            for (int index = 0; index < fields.Count; index++)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(
                    header.AsSpan(pointerOffset + index * sizeof(uint)),
                    checked((uint)nameOffset));
                nameOffset += Encoding.ASCII.GetByteCount(fields[index].Name) + 1;
            }
        }

        encodedTableName.CopyTo(header, tableNameOffset);
        encodedFieldNames.CopyTo(header, fieldNamesOffset);
        if (includeFieldNames)
        {
            int mapOffset = fieldMapOffset;
            foreach (int fieldNumber in fieldMap)
            {
                if (fieldNumber is < 0 or > ushort.MaxValue)
                {
                    throw new InvalidDataException($"An XG field-map number is invalid: {path}");
                }

                BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(mapOffset), checked((ushort)fieldNumber));
                mapOffset += sizeof(ushort);
            }

            encodedSortOrder.CopyTo(header, mapOffset);
            mapOffset += encodedSortOrder.Length;
            encodedIndexLabel.CopyTo(header, mapOffset);
        }

        ApplyHeaderMetadata(header, headerMetadata);
        return header;
    }

    private static void ApplyHeaderMetadata(byte[] header, StructureTemplateHeaderMetadata headerMetadata)
    {
        if (headerMetadata is null)
        {
            throw new InvalidDataException("Paradox header metadata is missing.");
        }

        byte[] revisionBytes;
        byte[] headerBytes16To1D;
        byte[] headerBytes30To48;
        byte[] headerBytes38To3F;
        byte[] headerBytes4DTo57;
        byte[] extendedHeaderBytes58To77;
        try
        {
            revisionBytes = Convert.FromHexString(headerMetadata.RevisionBytesHex);
            headerBytes16To1D = Convert.FromHexString(headerMetadata.HeaderBytes16To1DHex);
            headerBytes30To48 = Convert.FromHexString(headerMetadata.HeaderBytes30To48Hex);
            headerBytes38To3F = Convert.FromHexString(headerMetadata.HeaderBytes38To3FHex);
            headerBytes4DTo57 = Convert.FromHexString(headerMetadata.HeaderBytes4DTo57Hex);
            extendedHeaderBytes58To77 = Convert.FromHexString(headerMetadata.ExtendedHeaderBytes58To77Hex);
        }
        catch (FormatException exception)
        {
            throw new InvalidDataException("The Paradox header metadata is not valid hexadecimal.", exception);
        }

        if (revisionBytes.Length != 4 ||
            headerBytes16To1D.Length != 8 ||
            headerBytes30To48.Length != 0x19 ||
            headerBytes38To3F.Length != 8 ||
            headerBytes4DTo57.Length != 11 ||
            extendedHeaderBytes58To77.Length != 0x20 ||
            header.Length < 0x58)
        {
            throw new InvalidDataException("The Paradox header metadata has an invalid size.");
        }

        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x12), headerMetadata.Word12);
        headerBytes16To1D.CopyTo(header, 0x16);
        header[0x29] = headerMetadata.SortOrderCode;
        revisionBytes.CopyTo(header, 0x2C);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x49), headerMetadata.AutoIncrementValue);
        headerBytes30To48.CopyTo(header, 0x30);
        headerBytes38To3F.CopyTo(header, 0x38);
        headerBytes4DTo57.CopyTo(header, 0x4D);
        extendedHeaderBytes58To77.CopyTo(header, 0x58);
    }

    private static void WriteDataHeader(
        byte[] header,
        byte version,
        IReadOnlyList<ParadoxField> fields,
        int tableNameLength,
        int fieldNamesLength,
        int dataHeaderKind,
        uint encryptionKey)
    {
        ushort versionId = checked((ushort)(0x0100 | version));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x58), versionId);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x5A), versionId);
        BinaryPrimitives.WriteUInt32LittleEndian(header.AsSpan(0x5C), encryptionKey);
        BinaryPrimitives.WriteUInt32LittleEndian(
            header.AsSpan(0x60),
            checked((uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds()));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x64), checked((ushort)(fields.Count + 1)));
        int fieldNamesTotal = fields.Sum(static field => Encoding.ASCII.GetByteCount(field.Name) + 1);
        BinaryPrimitives.WriteUInt16LittleEndian(
            header.AsSpan(0x66),
            checked((ushort)(0x20 + fields.Count * 6 + tableNameLength + fieldNamesTotal)));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x68), checked((ushort)fields.Count));
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x6A), 1252);
        if (dataHeaderKind is 1 or 2)
        {
            header[0x6C] = 1;
            header[0x6D] = 1;
        }

        int sortOrderOffset = checked(
            0x18 + fields.Count * 8 + sizeof(uint) + tableNameLength + fieldNamesLength + 8);
        BinaryPrimitives.WriteUInt16LittleEndian(header.AsSpan(0x6E), checked((ushort)sortOrderOffset));
    }

    private static void WriteEmptyBlocks(
        byte[] file,
        int headerSize,
        int recordSize,
        int tableSize,
        int blockCount,
        string? emptyBlockRecordHex,
        uint encryptionKey)
    {
        int blockSize = checked(tableSize * 1024);
        byte[]? emptyBlockRecord = emptyBlockRecordHex is null
            ? null
            : Convert.FromHexString(emptyBlockRecordHex);
        if ((emptyBlockRecord is not null && emptyBlockRecord.Length != recordSize) ||
            (emptyBlockRecord is not null && blockCount == 0))
        {
            throw new InvalidDataException("The declared empty index record does not match its initial block.");
        }

        for (int blockNumber = 1; blockNumber <= blockCount; blockNumber++)
        {
            int offset = checked(headerSize + (blockNumber - 1) * blockSize);
            BinaryPrimitives.WriteUInt16LittleEndian(
                file.AsSpan(offset),
                blockNumber == blockCount ? (ushort)0 : checked((ushort)(blockNumber + 1)));
            BinaryPrimitives.WriteUInt16LittleEndian(
                file.AsSpan(offset + 2),
                blockNumber == 1 ? (ushort)0 : checked((ushort)(blockNumber - 1)));
            BinaryPrimitives.WriteInt16LittleEndian(file.AsSpan(offset + 4), checked((short)-recordSize));
            if (blockNumber == 1 && emptyBlockRecord is not null)
            {
                emptyBlockRecord.CopyTo(file, offset + DataBlockHeaderSize);
            }

            if (encryptionKey != 0)
            {
                ParadoxBlockCipher.EncryptDatabaseBlock(
                    file.AsSpan(offset, blockSize),
                    encryptionKey,
                    blockNumber);
            }
        }
    }

    private static byte[] CreateMemoFile(uint encryptionKey)
    {
        byte[] file = new byte[MemoBlockSize];
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(1), 1);
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(3), 1);
        file[5] = 0x82;
        file[6] = 0x73;
        file[7] = 0x02;
        file[9] = 0x29;
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x0B), 0x1000);
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x0D), 0x1000);
        file[0x10] = 0x10;
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x11), 0x0040);
        BinaryPrimitives.WriteUInt16LittleEndian(file.AsSpan(0x13), 0x0800);
        if (encryptionKey != 0)
        {
            ParadoxBlockCipher.EncryptMemoBlock(file, encryptionKey);
        }

        return file;
    }

    private static int FindFieldOrdinal(IReadOnlyList<ParadoxField> fields, string name)
    {
        for (int index = 0; index < fields.Count; index++)
        {
            if (fields[index].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        throw new InvalidDataException($"The secondary index references an unknown field: {name}");
    }

    private static int ReadHeaderSize(byte[] file) =>
        BinaryPrimitives.ReadUInt16LittleEndian(file.AsSpan(2));

    private static int RoundUp(int value, int alignment) =>
        checked((value + alignment - 1) / alignment * alignment);
}
