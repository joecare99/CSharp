using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AhnWin52Backup.Core.Paradox;

internal static class ParadoxIndexFileBuilder
{
    private const int DataBlockHeaderSize = 6;
    private const int DataHeaderSize = 0x78;

    public static byte[] Build(
        byte[] templateFile,
        byte expectedFileType,
        IReadOnlyList<IndexEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(templateFile);
        ArgumentNullException.ThrowIfNull(entries);
        if (entries.Count == 0)
        {
            throw new ArgumentException("An index requires at least one data-block entry.", nameof(entries));
        }

        ParadoxRecordWriter.TableHeader header =
            ParadoxRecordWriter.ReadHeader(templateFile, expectedFileType, "index template");
        if (header.RecordSize <= DataBlockHeaderSize || header.FieldCount <= 0)
        {
            throw new InvalidDataException("The empty index schema is invalid.");
        }

        int keyLength = header.RecordSize - DataBlockHeaderSize;
        int recordsPerBlock = (header.BlockSize - DataBlockHeaderSize) / header.RecordSize;
        if (recordsPerBlock <= 0)
        {
            throw new InvalidDataException("The index data block cannot contain an entry.");
        }

        List<IndexEntry[]> leafGroups = entries
            .Chunk(recordsPerBlock)
            .Select(static group => group.ToArray())
            .ToList();
        bool hasSecondLevel = leafGroups.Count > 1;
        if (hasSecondLevel && leafGroups.Count > recordsPerBlock)
        {
            throw new NotSupportedException(
                "The Paradox index requires more than two index levels; this database exceeds the supported B-tree depth.");
        }

        List<byte[]> blocks = new();
        if (!hasSecondLevel)
        {
            blocks.Add(CreateDataBlock(
                templateFile,
                header,
                1,
                0,
                0,
                leafGroups[0].Select(entry => EncodeIndexRecord(entry.Key, entry.BlockNumber, entry.RecordCount))));
        }
        else
        {
            IndexEntry[] rootEntries = new IndexEntry[leafGroups.Count];
            for (int index = 0; index < leafGroups.Count; index++)
            {
                IndexEntry[] group = leafGroups[index];
                int childBlockNumber = index + 2;
                rootEntries[index] = new IndexEntry(
                    group[0].Key,
                    childBlockNumber,
                    group.Sum(static entry => entry.RecordCount));
                blocks.Add(Array.Empty<byte>());
            }

            blocks.Insert(0, CreateDataBlock(
                templateFile,
                header,
                1,
                0,
                0,
                rootEntries.Select(entry => EncodeIndexRecord(
                    entry.Key,
                    entry.BlockNumber,
                    entry.RecordCount))));

            for (int index = 0; index < leafGroups.Count; index++)
            {
                int blockNumber = index + 2;
                blocks[blockNumber - 1] = CreateDataBlock(
                    templateFile,
                    header,
                    blockNumber,
                    blockNumber - 1,
                    blockNumber == leafGroups.Count + 1 ? 0 : blockNumber + 1,
                    leafGroups[index].Select(entry =>
                        EncodeIndexRecord(entry.Key, entry.BlockNumber, entry.RecordCount)));
            }
        }

        byte[] output = new byte[checked(header.HeaderSize + blocks.Count * header.BlockSize)];
        templateFile.AsSpan(0, header.HeaderSize).CopyTo(output);
        for (int index = 0; index < blocks.Count; index++)
        {
            blocks[index].CopyTo(output, header.HeaderSize + index * header.BlockSize);
        }

        int indexRecordCount = entries.Count + (hasSecondLevel ? leafGroups.Count : 0);
        ParadoxRecordWriter.UpdateMultipleBlockHeader(output, indexRecordCount, blocks.Count);
        BinaryPrimitives.WriteUInt16LittleEndian(output.AsSpan(0x1E, sizeof(ushort)), 1);
        output[0x20] = hasSecondLevel ? (byte)2 : (byte)1;
        return output;

        byte[] EncodeIndexRecord(byte[] key, int targetBlock, int recordCount)
        {
            if (key.Length != keyLength || targetBlock <= 0 || targetBlock > ushort.MaxValue ||
                recordCount <= 0 || recordCount > short.MaxValue)
            {
                throw new InvalidDataException("An index entry exceeds the supported key or block range.");
            }

            byte[] record = new byte[header.RecordSize];
            key.CopyTo(record, 0);
            Span<byte> link = record.AsSpan(keyLength, DataBlockHeaderSize);
            ParadoxRecordWriter.WriteParadoxShort(link, checked((short)targetBlock));
            ParadoxRecordWriter.WriteParadoxShort(link[2..], checked((short)recordCount));
            ParadoxRecordWriter.WriteParadoxShort(link[4..], 0);
            return record;
        }
    }

    private static byte[] CreateDataBlock(
        byte[] templateFile,
        ParadoxRecordWriter.TableHeader header,
        int blockNumber,
        int previousBlock,
        int nextBlock,
        IEnumerable<byte[]> records)
    {
        byte[][] rows = records.ToArray();
        if (rows.Length == 0 || rows.Length > (header.BlockSize - DataBlockHeaderSize) / header.RecordSize)
        {
            throw new InvalidDataException("An index block has an invalid number of entries.");
        }

        byte[] block = new byte[header.BlockSize];
        BinaryPrimitives.WriteUInt16LittleEndian(block, checked((ushort)nextBlock));
        BinaryPrimitives.WriteUInt16LittleEndian(block.AsSpan(2), checked((ushort)previousBlock));
        BinaryPrimitives.WriteInt16LittleEndian(
            block.AsSpan(4),
            checked((short)((rows.Length - 1) * header.RecordSize)));
        int offset = DataBlockHeaderSize;
        foreach (byte[] row in rows)
        {
            if (row.Length != header.RecordSize)
            {
                throw new InvalidDataException("An index entry has an unexpected physical size.");
            }

            row.CopyTo(block, offset);
            offset += row.Length;
        }

        uint encryption = ParadoxRecordWriter.ReadEncryption(templateFile);
        if (encryption != 0)
        {
            ParadoxBlockCipher.EncryptDatabaseBlock(block, encryption, blockNumber);
        }

        return block;
    }

    internal sealed record IndexEntry(byte[] Key, int BlockNumber, int RecordCount);
}
