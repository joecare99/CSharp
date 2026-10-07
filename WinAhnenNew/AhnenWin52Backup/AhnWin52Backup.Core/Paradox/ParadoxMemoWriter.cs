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
    private readonly string _memoFilePath;
    private readonly uint _encryption;
    private byte[]? _memoFile;
    private ushort _modificationCount;

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

        uint offset = Append(contents);
        BinaryPrimitives.WriteUInt32LittleEndian(field.AsSpan(leaderLength), offset | 0xFFu);
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
        int start = memoFile.Length;
        if (start % MemoBlockSize != 0)
        {
            throw new InvalidDataException($"The Paradox memo file is not aligned to {MemoBlockSize}-byte blocks.");
        }

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
        _modificationCount = unchecked((ushort)(_modificationCount + 1));
        if (_modificationCount == 0)
        {
            _modificationCount = 1;
        }

        BinaryPrimitives.WriteUInt16LittleEndian(memoFile.AsSpan(start + 7), _modificationCount);
        contents.CopyTo(memoFile, start + MemoBlockHeaderSize);
        _memoFile = memoFile;
        return checked((uint)start);
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
