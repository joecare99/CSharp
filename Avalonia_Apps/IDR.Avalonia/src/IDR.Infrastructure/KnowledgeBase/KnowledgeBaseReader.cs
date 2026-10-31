using IDR.Core.Models;
using IDR.Core.Services;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IDR.Infrastructure.KnowledgeBase;

public sealed class KnowledgeBaseReader : IKnowledgeBaseProvider
{
    private const int HeaderSize = 24 + 1 + 4 + 4 + 256 + 4;
    private const int MaximumProcedureSignatureSize = 512;
    private const ushort MissingModuleId = ushort.MaxValue;

    public async Task<IKnowledgeBase> OpenAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("A Knowledge Base path is required.", nameof(filePath));
        }

        byte[] data = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
        return Parse(data);
    }

    public KnowledgeBaseData Parse(ReadOnlyMemory<byte> data)
    {
        try
        {
            using MemoryStream stream = new(data.ToArray(), writable: false);
            using BinaryReader reader = new(stream, Encoding.ASCII, leaveOpen: false);

            string signature = ReadFixedString(reader, 24);
            _ = reader.ReadBoolean();
            _ = reader.ReadInt32();
            _ = reader.ReadUInt32();
            _ = reader.ReadBytes(256);
            uint formatVersion = reader.ReadUInt32();

            if (signature is not ("IDD Knowledge Base File" or "IDR Knowledge Base File")
                || formatVersion is < 1 or > 2)
            {
                throw new InvalidDataException("Unsupported Knowledge Base header.");
            }

            if (data.Length < HeaderSize + sizeof(uint))
            {
                throw new InvalidDataException("The Knowledge Base is truncated.");
            }

            stream.Position = data.Length - sizeof(uint);
            int sectionsOffset = reader.ReadInt32();
            if (sectionsOffset < HeaderSize || sectionsOffset > data.Length - sizeof(uint))
            {
                throw new InvalidDataException("The Knowledge Base section offset is invalid.");
            }

            stream.Position = sectionsOffset;
            int moduleCount = reader.ReadInt32();
            _ = reader.ReadInt32();
            if (moduleCount < 0 || moduleCount > 1_000_000)
            {
                throw new InvalidDataException("The Knowledge Base module count is invalid.");
            }

            SectionOffset[] moduleOffsets = ReadOffsets(reader, moduleCount, data.Length);
            List<KnowledgeBaseModule> modules = moduleOffsets
                .Select(offset => ReadModule(data.Span, offset))
                .OrderBy(module => module.Id)
                .ToList();
            SkipSection(reader, data.Length);
            SectionOffset[] typeOffsets = ReadSection(reader, data.Length);
            List<KnowledgeBaseType> types = typeOffsets
                .Select(offset => ReadType(data.Span, offset, formatVersion))
                .ToList();
            SkipSection(reader, data.Length);
            SkipSection(reader, data.Length);
            SectionOffset[] procedureOffsets = ReadSection(reader, data.Length);
            List<KnowledgeBaseProcedure> procedures = procedureOffsets
                .Select(offset => ReadProcedure(data.Span, offset))
                .Where(procedure => procedure is not null)
                .Select(procedure => procedure!)
                .ToList();

            return new KnowledgeBaseData(formatVersion, modules, procedures, types);
        }
        catch (EndOfStreamException exception)
        {
            throw new InvalidDataException("The Knowledge Base is truncated.", exception);
        }
    }

    private static SectionOffset[] ReadOffsets(BinaryReader reader, int count, int dataLength)
    {
        if (reader.BaseStream.Position > dataLength
            || count > (dataLength - reader.BaseStream.Position) / 16)
        {
            throw new InvalidDataException("The Knowledge Base section index is truncated.");
        }

        SectionOffset[] offsets = new SectionOffset[count];
        for (int index = 0; index < count; index++)
        {
            int offset = reader.ReadInt32();
            int size = reader.ReadInt32();
            _ = reader.ReadInt32();
            _ = reader.ReadInt32();
            if (offset < 0 || size < 4 || offset > dataLength - size)
            {
                throw new InvalidDataException(
                    $"The Knowledge Base contains an invalid section offset ({offset}, {size}) for {dataLength} bytes.");
            }

            offsets[index] = new SectionOffset(offset, size);
        }

        return offsets;
    }

    private static void SkipSection(BinaryReader reader, int dataLength)
    {
        int count = reader.ReadInt32();
        if (count < 0 || count > 1_000_000)
        {
            throw new InvalidDataException("The Knowledge Base section count is invalid.");
        }

        _ = reader.ReadInt32();
        SkipOffsets(reader, count, dataLength);
    }

    private static SectionOffset[] ReadSection(BinaryReader reader, int dataLength)
    {
        int count = reader.ReadInt32();
        if (count < 0 || count > 1_000_000)
        {
            throw new InvalidDataException("The Knowledge Base section count is invalid.");
        }

        _ = reader.ReadInt32();
        return ReadOffsets(reader, count, dataLength);
    }

    private static void SkipOffsets(BinaryReader reader, int count, int dataLength)
    {
        if (count > (dataLength - reader.BaseStream.Position) / 16)
        {
            throw new InvalidDataException("The Knowledge Base section index is truncated.");
        }

        reader.BaseStream.Seek((long)count * 16, SeekOrigin.Current);
    }

    private static KnowledgeBaseModule ReadModule(ReadOnlySpan<byte> data, SectionOffset offset)
    {
        ReadOnlySpan<byte> record = data.Slice(offset.Offset, offset.Size);
        if (record.Length < 4)
        {
            throw new InvalidDataException("The Knowledge Base module record is truncated.");
        }

        ushort id = BitConverter.ToUInt16(record);
        int nameStart = 4;
        int nameLength = record[nameStart..].IndexOf((byte)0);
        if (nameLength < 0)
        {
            nameLength = record.Length - nameStart;
        }

        if (nameStart + nameLength > record.Length)
        {
            throw new InvalidDataException("The Knowledge Base contains an invalid module name.");
        }

        string name = Encoding.ASCII.GetString(record.Slice(nameStart, nameLength));
        return new KnowledgeBaseModule(id, name);
    }

    private static KnowledgeBaseType ReadType(
        ReadOnlySpan<byte> data,
        SectionOffset offset,
        uint formatVersion)
    {
        ReadOnlySpan<byte> record = data.Slice(offset.Offset, offset.Size);
        if (record.Length < sizeof(uint))
        {
            throw new InvalidDataException("The Knowledge Base type record is truncated.");
        }

        uint sizeBytes = formatVersion == 1
            ? checked((uint)offset.Size)
            : BinaryPrimitives.ReadUInt32LittleEndian(record);
        int position = sizeof(uint);
        if (!TryReadUInt16(record, ref position, out ushort moduleId)
            || !TryReadPascalCString(record, ref position, out string name)
            || name.Length == 0
            || position >= record.Length)
        {
            throw new InvalidDataException("The Knowledge Base contains an invalid type record.");
        }

        byte kind = record[position++];
        if (!TryReadUInt16(record, ref position, out ushort virtualMethodCount)
            || !TryReadPascalCString(record, ref position, out string declaration))
        {
            throw new InvalidDataException("The Knowledge Base contains an invalid type declaration.");
        }

        _ = ReadTypeSection(record, ref position);
        _ = ReadTypeSection(record, ref position);
        ReadOnlySpan<byte> propertySection = ReadTypeSection(record, ref position);
        if (propertySection.Length < sizeof(ushort))
        {
            throw new InvalidDataException("The Knowledge Base property section is truncated.");
        }

        int propertyPosition = 0;
        if (!TryReadUInt16(propertySection, ref propertyPosition, out ushort propertyCount))
        {
            throw new InvalidDataException("The Knowledge Base property section is truncated.");
        }

        List<KnowledgeBaseProperty> properties = new(propertyCount);
        for (int index = 0; index < propertyCount; index++)
        {
            if (propertyPosition >= propertySection.Length)
            {
                throw new InvalidDataException("The Knowledge Base contains a truncated type property.");
            }

            byte scope = propertySection[propertyPosition++];
            if (!TryReadUInt32(propertySection, ref propertyPosition, out uint propertyIndex)
                || !TryReadUInt32(propertySection, ref propertyPosition, out uint dispatchId)
                || !TryReadPascalCString(propertySection, ref propertyPosition, out string propertyName)
                || !TryReadPascalCString(propertySection, ref propertyPosition, out string typeDefinition)
                || !TryReadPascalCString(propertySection, ref propertyPosition, out string readName)
                || !TryReadPascalCString(propertySection, ref propertyPosition, out string writeName)
                || !TryReadPascalCString(propertySection, ref propertyPosition, out string storedName))
            {
                throw new InvalidDataException("The Knowledge Base contains an invalid type property.");
            }

            properties.Add(new KnowledgeBaseProperty(
                scope,
                propertyIndex,
                dispatchId,
                propertyName,
                typeDefinition,
                readName,
                writeName,
                storedName));
        }

        _ = ReadTypeSection(record, ref position);
        return new KnowledgeBaseType(
            moduleId,
            name,
            sizeBytes,
            kind,
            virtualMethodCount,
            declaration,
            properties);
    }

    private static ReadOnlySpan<byte> ReadTypeSection(ReadOnlySpan<byte> record, ref int position)
    {
        if (!TryReadUInt32(record, ref position, out uint sectionSize)
            || sectionSize > int.MaxValue
            || position > record.Length - (int)sectionSize)
        {
            throw new InvalidDataException("The Knowledge Base type section is invalid.");
        }

        ReadOnlySpan<byte> section = record.Slice(position, (int)sectionSize);
        position += (int)sectionSize;
        return section;
    }

    private static KnowledgeBaseProcedure? ReadProcedure(
        ReadOnlySpan<byte> data,
        SectionOffset offset)
    {
        ReadOnlySpan<byte> record = data.Slice(offset.Offset, offset.Size);
        int position = 0;
        if (!TryReadUInt16(record, ref position, out ushort moduleId)
            || !TryReadPascalCString(record, ref position, out string name)
            || name.Length == 0)
        {
            throw new InvalidDataException("The Knowledge Base contains an invalid procedure record.");
        }

        bool isRuntimeHandler = DelphiRuntimeHandlerSymbols.IsSupported(name);

        if (position > record.Length - 8)
        {
            return InvalidProcedure(isRuntimeHandler, "The Knowledge Base contains an invalid procedure record.");
        }

        position++;
        byte dumpType = record[position++];
        byte methodKind = record[position++];
        byte callingConvention = record[position++];
        if (position > record.Length - sizeof(uint))
        {
            return InvalidProcedure(isRuntimeHandler, "The Knowledge Base contains an invalid procedure record.");
        }

        position += sizeof(uint);
        if (!TryReadPascalCString(record, ref position, out string returnType)
            || !TryReadUInt32(record, ref position, out uint dumpTotal)
            || !TryReadUInt32(record, ref position, out uint dumpSize)
            || !TryReadUInt32(record, ref position, out _)
            || dumpSize > int.MaxValue
            || dumpTotal > int.MaxValue
            || (ulong)dumpSize * 2 > dumpTotal
            || (ulong)position + (ulong)dumpSize * 2 > (ulong)record.Length)
        {
            return InvalidProcedure(isRuntimeHandler, "The Knowledge Base contains an invalid procedure dump.");
        }

        int codeSize = (int)dumpSize;
        int codeOffset = position;
        int relocationOffset = checked(codeOffset + codeSize);
        if (isRuntimeHandler)
        {
            if (dumpType != (byte)'C' || codeSize is 0 or > MaximumProcedureSignatureSize)
            {
                return null;
            }

            return new KnowledgeBaseProcedure(
                moduleId,
                name,
                dumpType,
                record.Slice(codeOffset, codeSize).ToArray(),
                record.Slice(relocationOffset, codeSize).ToArray())
            {
                MethodKind = methodKind,
                CallingConvention = callingConvention,
                ReturnType = returnType
            };
        }

        if (dumpTotal > (uint)(record.Length - position))
        {
            return null;
        }

        position = checked(codeOffset + (int)dumpTotal);
        if (!TryReadUInt32(record, ref position, out uint argumentSectionSize)
            || argumentSectionSize is < sizeof(ushort) or > int.MaxValue
            || argumentSectionSize > record.Length - position)
        {
            return InvalidProcedure(isRuntimeHandler, "The Knowledge Base contains an invalid procedure argument section.");
        }

        int argumentSectionEnd = checked(position + (int)argumentSectionSize);
        if (!TryReadUInt16(record[..argumentSectionEnd], ref position, out ushort argumentCount)
            || argumentCount > 4096)
        {
            return InvalidProcedure(isRuntimeHandler, "The Knowledge Base contains an invalid procedure argument count.");
        }

        List<KnowledgeBaseArgument> arguments = new(argumentCount);
        for (int index = 0; index < argumentCount; index++)
        {
            if (position > argumentSectionEnd - 1)
            {
                return InvalidProcedure(isRuntimeHandler, "The Knowledge Base contains a truncated procedure argument.");
            }

            byte tag = record[position++];
            if (position > argumentSectionEnd - 9)
            {
                return InvalidProcedure(isRuntimeHandler, "The Knowledge Base contains a truncated procedure argument.");
            }

            bool isRegister = record[position++] != 0;
            int registerOrStackIndex = BinaryPrimitives.ReadInt32LittleEndian(record.Slice(position, sizeof(int)));
            position += sizeof(int);
            int sizeBytes = BinaryPrimitives.ReadInt32LittleEndian(record.Slice(position, sizeof(int)));
            position += sizeof(int);
            if (sizeBytes < 0
                || !TryReadPascalCString(record[..argumentSectionEnd], ref position, out string argumentName)
                || !TryReadPascalCString(record[..argumentSectionEnd], ref position, out string argumentType))
            {
                return InvalidProcedure(isRuntimeHandler, "The Knowledge Base contains an invalid procedure argument.");
            }

            arguments.Add(new KnowledgeBaseArgument(
                tag,
                isRegister,
                registerOrStackIndex,
                sizeBytes,
                argumentName,
                argumentType));
        }

        return new KnowledgeBaseProcedure(
            moduleId,
            name,
            dumpType,
            ReadOnlyMemory<byte>.Empty,
            ReadOnlyMemory<byte>.Empty)
        {
            MethodKind = methodKind,
            CallingConvention = callingConvention,
            ReturnType = returnType,
            Arguments = arguments
        };
    }

    private static KnowledgeBaseProcedure? InvalidProcedure(bool isRuntimeHandler, string message)
    {
        if (isRuntimeHandler)
        {
            throw new InvalidDataException(message);
        }

        return null;
    }

    private static bool TryReadPascalCString(
        ReadOnlySpan<byte> data,
        ref int position,
        out string value)
    {
        value = string.Empty;
        if (!TryReadUInt16(data, ref position, out ushort length)
            || position > data.Length - length - 1
            || data[position + length] != 0)
        {
            return false;
        }

        value = Encoding.ASCII.GetString(data.Slice(position, length));
        position += length + 1;
        return true;
    }

    private static bool TryReadUInt16(ReadOnlySpan<byte> data, ref int position, out ushort value)
    {
        if (position < 0 || position > data.Length - sizeof(ushort))
        {
            value = 0;
            return false;
        }

        value = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(position, sizeof(ushort)));
        position += sizeof(ushort);
        return true;
    }

    private static bool TryReadUInt32(ReadOnlySpan<byte> data, ref int position, out uint value)
    {
        if (position < 0 || position > data.Length - sizeof(uint))
        {
            value = 0;
            return false;
        }

        value = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(position, sizeof(uint)));
        position += sizeof(uint);
        return true;
    }

    private static string ReadFixedString(BinaryReader reader, int length)
    {
        byte[] bytes = reader.ReadBytes(length);
        int end = Array.IndexOf(bytes, (byte)0);
        return Encoding.ASCII.GetString(bytes, 0, end < 0 ? bytes.Length : end);
    }

    private readonly record struct SectionOffset(int Offset, int Size);
}

public sealed class KnowledgeBaseData : IKnowledgeBase
{
    public KnowledgeBaseData(
        uint formatVersion,
        IReadOnlyList<KnowledgeBaseModule> modules,
        IReadOnlyList<KnowledgeBaseProcedure>? procedures = null,
        IReadOnlyList<KnowledgeBaseType>? types = null)
    {
        FormatVersion = formatVersion;
        Modules = modules;
        Procedures = procedures ?? Array.Empty<KnowledgeBaseProcedure>();
        Types = types ?? Array.Empty<KnowledgeBaseType>();
    }

    public uint FormatVersion { get; }

    public IReadOnlyList<KnowledgeBaseModule> Modules { get; }

    public IReadOnlyList<KnowledgeBaseType> Types { get; }

    public IReadOnlyList<KnowledgeBaseProcedure> Procedures { get; }

    public ushort GetModuleId(string moduleName)
    {
        if (string.IsNullOrWhiteSpace(moduleName))
        {
            return ushort.MaxValue;
        }

        KnowledgeBaseModule? module = Modules.FirstOrDefault(
            candidate => string.Equals(candidate.Name, moduleName, StringComparison.OrdinalIgnoreCase));
        return module?.Id ?? ushort.MaxValue;
    }

    public string? GetModuleName(ushort moduleId)
    {
        return Modules.FirstOrDefault(module => module.Id == moduleId)?.Name;
    }
}
