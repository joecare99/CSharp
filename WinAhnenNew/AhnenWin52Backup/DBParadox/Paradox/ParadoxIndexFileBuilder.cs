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
        IReadOnlyList<IndexEntry> entries,
        int? maximumLeafEntriesPerBlock = null,
        int? targetLeafNodeCount = null)
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
        int physicalCapacity = (header.BlockSize - DataBlockHeaderSize) / header.RecordSize;
        int recordsPerLeaf = maximumLeafEntriesPerBlock ?? physicalCapacity;
        if (physicalCapacity <= 0 || recordsPerLeaf <= 0 || recordsPerLeaf > physicalCapacity)
        {
            throw new InvalidDataException("The index data block has an invalid entry capacity.");
        }

        int minimumLeafNodeCount = (entries.Count + recordsPerLeaf - 1) / recordsPerLeaf;
        int leafNodeCount = targetLeafNodeCount ?? minimumLeafNodeCount;
        if (leafNodeCount < minimumLeafNodeCount || leafNodeCount > entries.Count)
        {
            throw new InvalidDataException("The requested index leaf-node count is invalid.");
        }

        if (entries.Count > 1 && physicalCapacity < 2)
        {
            throw new NotSupportedException(
                "The Paradox index block cannot reduce the number of nodes at each tree level.");
        }

        List<List<IndexNode>> levels = new();
        List<IndexNode> currentLevel = new();
        int nextBlockNumber = 1;
        int indexRecordCount = 0;
        foreach (IndexEntry[] group in Partition(entries, leafNodeCount))
        {
            byte[][] recordsForNode = group
                .Select(entry => EncodeIndexRecord(entry.Key, entry.BlockNumber, entry.RecordCount))
                .ToArray();
            currentLevel.Add(new IndexNode(
                nextBlockNumber++,
                group[0].Key,
                group.Sum(static entry => entry.RecordCount),
                recordsForNode));
            indexRecordCount = checked(indexRecordCount + recordsForNode.Length);
        }

        levels.Add(currentLevel);
        while (currentLevel.Count > 1)
        {
            List<IndexNode> parentLevel = new();
            foreach (IndexNode[] childGroup in currentLevel.Chunk(physicalCapacity))
            {
                byte[][] recordsForNode = childGroup
                    .Select(child => EncodeIndexRecord(
                        child.FirstKey,
                        child.BlockNumber,
                        child.RecordCount))
                    .ToArray();
                parentLevel.Add(new IndexNode(
                    nextBlockNumber++,
                    childGroup[0].FirstKey,
                    childGroup.Sum(static child => child.RecordCount),
                    recordsForNode));
                indexRecordCount = checked(indexRecordCount + recordsForNode.Length);
            }

            levels.Add(parentLevel);
            currentLevel = parentLevel;
        }

        int blockCount = nextBlockNumber - 1;
        if (blockCount > ushort.MaxValue || levels.Count > byte.MaxValue)
        {
            throw new NotSupportedException("The Paradox index exceeds the supported block or tree-depth range.");
        }

        byte[][] blocks = new byte[blockCount][];
        foreach (List<IndexNode> level in levels)
        {
            for (int index = 0; index < level.Count; index++)
            {
                IndexNode node = level[index];
                blocks[node.BlockNumber - 1] = CreateDataBlock(
                    templateFile,
                    header,
                    node.BlockNumber,
                    blockCount,
                    node.Records);
            }
        }

        byte[] output = new byte[checked(header.HeaderSize + blocks.Length * header.BlockSize)];
        templateFile.AsSpan(0, header.HeaderSize).CopyTo(output);
        for (int index = 0; index < blocks.Length; index++)
        {
            blocks[index].CopyTo(output, header.HeaderSize + index * header.BlockSize);
        }

        ParadoxRecordWriter.UpdateMultipleBlockHeader(output, indexRecordCount, blocks.Length);
        BinaryPrimitives.WriteUInt16LittleEndian(
            output.AsSpan(0x1E, sizeof(ushort)),
            checked((ushort)currentLevel[0].BlockNumber));
        output[0x20] = checked((byte)levels.Count);
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

    private static IEnumerable<T[]> Partition<T>(IReadOnlyList<T> items, int groupCount)
    {
        int groupSize = items.Count / groupCount;
        int largerGroupCount = items.Count % groupCount;
        int offset = 0;
        for (int groupIndex = 0; groupIndex < groupCount; groupIndex++)
        {
            int currentGroupSize = groupSize + (groupIndex < largerGroupCount ? 1 : 0);
            T[] group = new T[currentGroupSize];
            for (int itemIndex = 0; itemIndex < currentGroupSize; itemIndex++)
            {
                group[itemIndex] = items[offset++];
            }

            yield return group;
        }
    }

    private static byte[] CreateDataBlock(
        byte[] templateFile,
        ParadoxRecordWriter.TableHeader header,
        int blockNumber,
        int blockCount,
        IEnumerable<byte[]> records)
    {
        byte[][] rows = records.ToArray();
        if (rows.Length == 0 || rows.Length > (header.BlockSize - DataBlockHeaderSize) / header.RecordSize)
        {
            throw new InvalidDataException("An index block has an invalid number of entries.");
        }

        byte[] block = new byte[header.BlockSize];
        BinaryPrimitives.WriteUInt16LittleEndian(
            block,
            blockNumber < blockCount ? checked((ushort)(blockNumber + 1)) : (ushort)0);
        BinaryPrimitives.WriteUInt16LittleEndian(
            block.AsSpan(2),
            checked((ushort)(blockNumber - 1)));
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

    private sealed record IndexNode(int BlockNumber, byte[] FirstKey, int RecordCount, byte[][] Records);
}
