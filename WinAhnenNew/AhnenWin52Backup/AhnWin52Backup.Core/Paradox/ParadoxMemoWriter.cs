using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;
using AhnWin52Backup.Core.Hej;

namespace AhnWin52Backup.Core.Paradox;

internal sealed class ParadoxMemoWriter
{
    private const int MemoBlockSize = 4096;
    private const int MemoBlockHeaderSize = 9;
    private const int InlineReferenceSize = 10;
    private const int SuballocatedBlockHeaderSize = 12;
    private const int SuballocatedSlotSize = 5;
    private const int SuballocatedSlotCount = 64;
    private const int SuballocatedDataStartUnit = 21;
    private const int SuballocatedDataUnitSize = 16;
    private const int MaximumSuballocatedBlobSize = 2048;
    private readonly string _memoFilePath;
    private readonly uint _encryption;
    private byte[]? _memoFile;
    private ushort _modificationCount;
    private int _suballocatedBlockOffset;
    private int _suballocatedBlobCount;
    private int _suballocatedUsedUnits;

    public ParadoxMemoWriter(string memoFilePath, uint encryption)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(memoFilePath);
        _memoFilePath = Path.GetFullPath(memoFilePath);
        _encryption = encryption;
    }

    public byte[] EncodeText(string value, int fieldLength, string fieldName)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentException.ThrowIfNullOrWhiteSpace(fieldName);
        if (fieldLength < InlineReferenceSize)
        {
            throw new InvalidDataException($"Memo field \"{fieldName}\" is shorter than its reference header.");
        }

        string memoText = NormalizeLineEndings(value);
        using MemoryStream encoded = new();
        Windows1252.Encode(memoText.AsSpan(), encoded);
        byte[] contents = encoded.ToArray();
        byte[] field = new byte[fieldLength];
        int leaderLength = fieldLength - InlineReferenceSize;
        if (contents.Length <= leaderLength)
        {
            contents.CopyTo(field, 0);
            BinaryPrimitives.WriteUInt32LittleEndian(field.AsSpan(leaderLength + 4), checked((uint)contents.Length));
            return field;
        }

        uint location = Append(contents);
        BinaryPrimitives.WriteUInt32LittleEndian(field.AsSpan(leaderLength), location);
        BinaryPrimitives.WriteUInt32LittleEndian(field.AsSpan(leaderLength + 4), checked((uint)contents.Length));
        BinaryPrimitives.WriteUInt16LittleEndian(field.AsSpan(leaderLength + 8), _modificationCount);
        return field;
    }

    public void Save()
    {
        if (_memoFile is null)
        {
            return;
        }

        if (_encryption != 0)
        {
            ParadoxBlockCipher.EncryptMemoBlock(_memoFile, _encryption);
        }

        File.WriteAllBytes(_memoFilePath, _memoFile);
    }

    private uint Append(byte[] contents)
    {
        byte[] memoFile = GetMemoFile();
        if (contents.Length <= MaximumSuballocatedBlobSize)
        {
            return AppendSuballocated(memoFile, contents);
        }

        int start = memoFile.Length;
        ValidateBlockAlignedOffset(start);
        if ((uint)start > 0xFFFFFF00u)
        {
            throw new NotSupportedException("The Paradox memo file has exhausted its addressable offset range.");
        }

        int blockCount = checked((contents.Length + MemoBlockHeaderSize + MemoBlockSize - 1) / MemoBlockSize);
        if (blockCount is < 1 or > ushort.MaxValue)
        {
            throw new NotSupportedException("A memo value exceeds the supported Paradox block-count range.");
        }

        int reservedLength = checked(blockCount * MemoBlockSize);
        Array.Resize(ref memoFile, checked(start + reservedLength));
        memoFile[start] = 2;
        BinaryPrimitives.WriteUInt16LittleEndian(memoFile.AsSpan(start + 1), checked((ushort)blockCount));
        BinaryPrimitives.WriteUInt32LittleEndian(memoFile.AsSpan(start + 3), checked((uint)contents.Length));
        AdvanceModificationCount();
        BinaryPrimitives.WriteUInt16LittleEndian(memoFile.AsSpan(start + 7), _modificationCount);
        contents.CopyTo(memoFile, start + MemoBlockHeaderSize);
        _memoFile = memoFile;
        return checked((uint)start | 0xFFu);
    }

    private uint AppendSuballocated(byte[] memoFile, byte[] contents)
    {
        int requiredUnits = checked((contents.Length + SuballocatedDataUnitSize - 1) / SuballocatedDataUnitSize);
        if (_suballocatedBlockOffset == 0 ||
            _suballocatedBlobCount >= SuballocatedSlotCount ||
            _suballocatedUsedUnits + requiredUnits > MemoBlockSize / SuballocatedDataUnitSize)
        {
            int start = memoFile.Length;
            ValidateBlockAlignedOffset(start);
            if ((uint)start > 0xFFFFFF00u)
            {
                throw new NotSupportedException("The Paradox memo file has exhausted its addressable offset range.");
            }

            Array.Resize(ref memoFile, checked(start + MemoBlockSize));
            memoFile[start] = 3;
            BinaryPrimitives.WriteUInt16LittleEndian(memoFile.AsSpan(start + 1), 1);
            _suballocatedBlockOffset = start;
            _suballocatedBlobCount = 0;
            _suballocatedUsedUnits = SuballocatedDataStartUnit;
        }

        int slot = SuballocatedSlotCount - 1 - _suballocatedBlobCount;
        int dataUnitOffset = _suballocatedUsedUnits;
        int tableOffset = checked(
            _suballocatedBlockOffset + SuballocatedBlockHeaderSize + slot * SuballocatedSlotSize);
        memoFile[tableOffset] = checked((byte)dataUnitOffset);
        memoFile[tableOffset + 1] = checked((byte)requiredUnits);
        AdvanceModificationCount();
        BinaryPrimitives.WriteUInt16LittleEndian(memoFile.AsSpan(tableOffset + 2), _modificationCount);
        int finalUnitLength = contents.Length % SuballocatedDataUnitSize;
        memoFile[tableOffset + 4] = checked((byte)(finalUnitLength == 0
            ? SuballocatedDataUnitSize
            : finalUnitLength));
        contents.CopyTo(memoFile, checked(_suballocatedBlockOffset + dataUnitOffset * SuballocatedDataUnitSize));

        _suballocatedBlobCount++;
        _suballocatedUsedUnits += requiredUnits;
        _memoFile = memoFile;
        return checked((uint)_suballocatedBlockOffset | (uint)slot);
    }

    private static void ValidateBlockAlignedOffset(int offset)
    {
        if (offset % MemoBlockSize != 0)
        {
            throw new InvalidDataException($"The Paradox memo file is not aligned to {MemoBlockSize}-byte blocks.");
        }
    }

    private void AdvanceModificationCount()
    {
        _modificationCount = unchecked((ushort)(_modificationCount + 1));
        if (_modificationCount == 0)
        {
            _modificationCount = 1;
        }
    }

    private byte[] GetMemoFile()
    {
        if (_memoFile is not null)
        {
            return _memoFile;
        }

        if (!File.Exists(_memoFilePath))
        {
            throw new FileNotFoundException("The structure template is missing a required memo file.", _memoFilePath);
        }

        byte[] memoFile = File.ReadAllBytes(_memoFilePath);
        if (memoFile.Length == 0 || memoFile.Length % 256 != 0)
        {
            throw new InvalidDataException("An encrypted Paradox memo file must contain complete 256-byte cipher blocks.");
        }

        if (_encryption != 0)
        {
            ParadoxBlockCipher.DecryptMemoBlock(memoFile, _encryption);
        }

        if (memoFile.Length < MemoBlockSize || memoFile.Length % MemoBlockSize != 0)
        {
            throw new InvalidDataException("The Paradox memo file is not aligned to complete 4096-byte blocks.");
        }

        if (memoFile.Length >= 5)
        {
            _modificationCount = BinaryPrimitives.ReadUInt16LittleEndian(memoFile.AsSpan(3));
        }

        _memoFile = memoFile;
        return memoFile;
    }

    private static string NormalizeLineEndings(string value)
    {
        StringBuilder normalized = new(value.Length);
        for (int index = 0; index < value.Length; index++)
        {
            char character = value[index];
            if (character == '\r')
            {
                normalized.Append("\r\n");
                if (index + 1 < value.Length && value[index + 1] == '\n')
                {
                    index++;
                }
            }
            else if (character == '\n')
            {
                normalized.Append("\r\n");
            }
            else
            {
                normalized.Append(character);
            }
        }

        return normalized.ToString();
    }
}
