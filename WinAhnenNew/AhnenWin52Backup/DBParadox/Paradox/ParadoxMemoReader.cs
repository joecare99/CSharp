using System;
using System.Buffers.Binary;
using System.IO;

namespace AhnWin52Backup.Core.Paradox;

internal sealed class ParadoxMemoReader
{
    private readonly string _memoFilePath;
    private readonly uint _encryption;
    private byte[]? _memoFile;

    public ParadoxMemoReader(string memoFilePath, uint encryption)
    {
        _memoFilePath = memoFilePath;
        _encryption = encryption;
    }

    public byte[]? Read(ReadOnlySpan<byte> fieldData, bool graphic)
    {
        if (fieldData.Length < 10)
        {
            throw new InvalidDataException("A Paradox blob reference must contain at least 10 bytes.");
        }

        int leaderLength = fieldData.Length - 10;
        uint rawSize = BinaryPrimitives.ReadUInt32LittleEndian(fieldData.Slice(leaderLength + 4, 4));
        if (rawSize == 0)
        {
            return null;
        }

        int blobSize = GetBlobSize(rawSize, graphic);
        uint location = BinaryPrimitives.ReadUInt32LittleEndian(fieldData.Slice(leaderLength, 4));
        int index = (int)(location & 0xFF);
        uint offset = location & 0xFFFFFF00;

        if (blobSize <= leaderLength)
        {
            return fieldData[..blobSize].ToArray();
        }

        if (offset == 0)
        {
            throw new InvalidDataException("A Paradox blob reference has data but no memo-file offset.");
        }

        byte blockType = ReadRange(offset, 1)[0];
        if (blockType == 2)
        {
            return ReadSingleBlock(offset, index, rawSize, blobSize, graphic);
        }

        if (blockType == 3)
        {
            return ReadSuballocatedBlock(offset, index, rawSize, blobSize);
        }

        throw new InvalidDataException($"Unsupported Paradox memo block type {blockType} at offset {offset}.");
    }

    private byte[] ReadSingleBlock(uint offset, int index, uint rawSize, int blobSize, bool graphic)
    {
        if (index != 0xFF)
        {
            throw new InvalidDataException("A standalone Paradox memo block has an invalid blob index.");
        }

        int headerSize = graphic ? 17 : 9;
        byte[] blockHeader = ReadRange(checked(offset + 3), headerSize - 3);
        uint storedSize = BinaryPrimitives.ReadUInt32LittleEndian(blockHeader);
        if (storedSize != rawSize)
        {
            throw new InvalidDataException(
                $"Paradox memo size mismatch: the record references {rawSize} bytes but the block stores {storedSize}.");
        }

        return ReadRange(checked(offset + (uint)headerSize), blobSize);
    }

    private byte[] ReadSuballocatedBlock(uint offset, int index, uint rawSize, int blobSize)
    {
        _ = ReadRange(offset, 12);
        byte[] pointer = ReadRange(checked(offset + 12 + (uint)(index * 5)), 5);
        if (pointer[1] == 0)
        {
            throw new InvalidDataException($"Paradox memo block at offset {offset} has an empty blob slot.");
        }

        uint storedSize = ((uint)pointer[1] - 1) * 16 + pointer[4];
        if (storedSize != rawSize)
        {
            throw new InvalidDataException(
                $"Paradox memo size mismatch: the record references {rawSize} bytes but the block stores {storedSize}.");
        }

        int dataOffset = checked((int)offset + pointer[0] * 16);
        return ReadRange((uint)dataOffset, blobSize);
    }

    private byte[] ReadRange(uint offset, int length)
    {
        if (length < 0 || offset > int.MaxValue)
        {
            throw new InvalidDataException("A Paradox memo reference is outside the supported file range.");
        }

        byte[] data = GetMemoFile();
        int start = (int)offset;
        if (start > data.Length - length)
        {
            throw new InvalidDataException("A Paradox memo reference extends beyond the .MB file.");
        }

        return data.AsSpan(start, length).ToArray();
    }

    private byte[] GetMemoFile()
    {
        if (_memoFile is not null)
        {
            return _memoFile;
        }

        byte[] data = File.ReadAllBytes(_memoFilePath);
        if (_encryption != 0)
        {
            if (data.Length == 0 || data.Length % 256 != 0)
            {
                throw new InvalidDataException("An encrypted Paradox .MB file must contain complete 256-byte cipher blocks.");
            }

            ParadoxBlockCipher.DecryptMemoBlock(data, _encryption);
        }

        _memoFile = data;
        return data;
    }

    private static int GetBlobSize(uint rawSize, bool graphic)
    {
        if (graphic && rawSize < 8)
        {
            throw new InvalidDataException("A Paradox graphic blob has an invalid size.");
        }

        uint blobSize = graphic ? rawSize - 8 : rawSize;
        if (blobSize > int.MaxValue)
        {
            throw new InvalidDataException("A Paradox blob exceeds the supported size.");
        }

        return (int)blobSize;
    }
}
