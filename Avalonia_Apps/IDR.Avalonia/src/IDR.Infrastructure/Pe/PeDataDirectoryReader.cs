using IDR.Core.Models;
using IDR.Core.Services;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;

namespace IDR.Infrastructure.Pe;

internal sealed class PeDataDirectoryReader
{
    private const uint OrdinalFlag32 = 0x80000000;
    private const ulong OrdinalFlag64 = 0x8000000000000000;
    private const uint ResourceDirectoryFlag = 0x80000000;
    private const uint ResourceNameFlag = 0x80000000;
    private const uint StringResourceTypeId = 6;
    private const uint RDataResourceTypeId = 10;
    private const int MaximumTableEntries = 1_000_000;
    private const int MaximumResourceDirectoryEntries = 10000;
    private const int MaximumFormResourceBytes = 16 * 1024 * 1024;
    private const int MaximumFormCount = 512;
    private const int MaximumTotalFormResourceBytes = 64 * 1024 * 1024;

    private readonly ReadOnlyMemory<byte> _image;
    private readonly SectionHeader[] _sections;
    private readonly PEHeader _header;
    private readonly bool _is64Bit;

    public PeDataDirectoryReader(
        ReadOnlyMemory<byte> image,
        SectionHeader[] sections,
        PEHeader header)
    {
        _image = image;
        _sections = sections;
        _header = header;
        _is64Bit = header.Magic == PEMagic.PE32Plus;
    }

    public IReadOnlyList<PeImportModule> ReadImports()
    {
        DirectoryEntry directory = _header.ImportTableDirectory;
        if (directory.RelativeVirtualAddress == 0 || directory.Size == 0)
        {
            return [];
        }

        List<PeImportModule> modules = [];
        int descriptorCount = checked((int)(directory.Size / 20));
        for (int index = 0; index < descriptorCount; index++)
        {
            uint descriptorRva = checked((uint)directory.RelativeVirtualAddress + (uint)(index * 20));
            int descriptorOffset = MapRva(descriptorRva, 20);
            ReadOnlySpan<byte> descriptor = _image.Span.Slice(descriptorOffset, 20);
            uint originalFirstThunk = ReadUInt32(descriptor, 0);
            uint nameRva = ReadUInt32(descriptor, 12);
            uint firstThunk = ReadUInt32(descriptor, 16);
            if (originalFirstThunk == 0 && nameRva == 0 && firstThunk == 0)
            {
                return modules;
            }

            string moduleName = ReadAsciiString(nameRva);
            uint thunkRva = originalFirstThunk == 0 ? firstThunk : originalFirstThunk;
            modules.Add(new PeImportModule(moduleName, ReadImportSymbols(thunkRva)));
        }

        throw new InvalidDataException("The PE import directory has no terminating descriptor.");
    }

    public IReadOnlyList<PeExport> ReadExports()
    {
        DirectoryEntry directory = _header.ExportTableDirectory;
        if (directory.RelativeVirtualAddress == 0 || directory.Size == 0)
        {
            return [];
        }

        int exportOffset = MapRva((uint)directory.RelativeVirtualAddress, 40);
        ReadOnlySpan<byte> exportDirectory = _image.Span.Slice(exportOffset, 40);
        uint ordinalBase = ReadUInt32(exportDirectory, 16);
        uint functionCount = ReadUInt32(exportDirectory, 20);
        uint nameCount = ReadUInt32(exportDirectory, 24);
        uint functionsRva = ReadUInt32(exportDirectory, 28);
        uint namesRva = ReadUInt32(exportDirectory, 32);
        uint ordinalsRva = ReadUInt32(exportDirectory, 36);
        if (functionCount > MaximumTableEntries || nameCount > functionCount)
        {
            throw new InvalidDataException("The PE export directory contains invalid table counts.");
        }

        Dictionary<uint, string> namesByIndex = [];
        if (nameCount > 0)
        {
            int namesOffset = MapRva(namesRva, checked((int)nameCount * sizeof(uint)));
            int ordinalsOffset = MapRva(ordinalsRva, checked((int)nameCount * sizeof(ushort)));
            for (uint index = 0; index < nameCount; index++)
            {
                uint nameRva = ReadUInt32(_image.Span.Slice(namesOffset), checked((int)index * sizeof(uint)));
                ushort functionIndex = ReadUInt16(_image.Span.Slice(ordinalsOffset), checked((int)index * sizeof(ushort)));
                if (functionIndex >= functionCount)
                {
                    throw new InvalidDataException("The PE export directory contains an invalid name ordinal.");
                }

                namesByIndex[functionIndex] = ReadAsciiString(nameRva);
            }
        }

        if (functionCount == 0)
        {
            return [];
        }

        int functionsOffset = MapRva(functionsRva, checked((int)functionCount * sizeof(uint)));
        List<PeExport> exports = new(checked((int)functionCount));
        for (uint index = 0; index < functionCount; index++)
        {
            uint addressRva = ReadUInt32(_image.Span.Slice(functionsOffset), checked((int)index * sizeof(uint)));
            if (addressRva == 0)
            {
                continue;
            }

            string? forwarder = addressRva >= directory.RelativeVirtualAddress
                && addressRva < checked((ulong)directory.RelativeVirtualAddress + (uint)directory.Size)
                    ? ReadAsciiString(addressRva)
                    : null;
            namesByIndex.TryGetValue(index, out string? name);
            exports.Add(new PeExport(name, checked(ordinalBase + index), addressRva, forwarder));
        }

        return exports;
    }

    public IReadOnlyList<PeResourceString> ReadStringResources()
    {
        DirectoryEntry directory = _header.ResourceTableDirectory;
        if (directory.RelativeVirtualAddress == 0 || directory.Size == 0)
        {
            return [];
        }

        if (directory.Size < 16)
        {
            throw new InvalidDataException("The PE resource directory is truncated.");
        }

        uint resourceBaseRva = checked((uint)directory.RelativeVirtualAddress);
        List<PeResourceString> strings = [];
        foreach ((uint typeId, uint typeOffset) in ReadResourceDirectory(
            resourceBaseRva,
            directory.Size,
            0))
        {
            if (typeId != StringResourceTypeId
                || (typeOffset & ResourceDirectoryFlag) == 0)
            {
                continue;
            }

            foreach ((uint blockId, uint blockOffset) in ReadResourceDirectory(
                resourceBaseRva,
                directory.Size,
                typeOffset & ~ResourceDirectoryFlag))
            {
                if (blockId == 0 || (blockOffset & ResourceDirectoryFlag) == 0)
                {
                    continue;
                }

                uint stringBlockBase = checked((blockId - 1) * 16);
                foreach ((uint languageId, uint dataOffset) in ReadResourceDirectory(
                    resourceBaseRva,
                    directory.Size,
                    blockOffset & ~ResourceDirectoryFlag))
                {
                    if (languageId > ushort.MaxValue
                        || (dataOffset & ResourceDirectoryFlag) != 0)
                    {
                        continue;
                    }

                    ReadResourceStringBlock(
                        checked(resourceBaseRva + (dataOffset & ~ResourceNameFlag)),
                        stringBlockBase,
                        (ushort)languageId,
                        strings);
                }
            }
        }

        return strings
            .OrderBy(resourceString => resourceString.Id)
            .ThenBy(resourceString => resourceString.LanguageId)
            .ToArray();
    }

    public IReadOnlyList<DelphiForm> ReadForms(out IReadOnlyList<string> diagnostics)
    {
        DirectoryEntry directory = _header.ResourceTableDirectory;
        if (directory.RelativeVirtualAddress == 0 || directory.Size == 0)
        {
            diagnostics = [];
            return [];
        }

        uint resourceBaseRva = checked((uint)directory.RelativeVirtualAddress);
        List<DelphiForm> forms = [];
        List<string> formDiagnostics = [];
        long totalFormResourceBytes = 0;
        foreach ((uint typeId, uint typeOffset) in ReadResourceDirectory(
            resourceBaseRva,
            directory.Size,
            0))
        {
            if (typeId != RDataResourceTypeId
                || (typeOffset & ResourceDirectoryFlag) == 0)
            {
                continue;
            }

            foreach (ResourceEntry resource in ReadResourceEntries(
                resourceBaseRva,
                directory.Size,
                typeOffset & ~ResourceDirectoryFlag))
            {
                if ((resource.Offset & ResourceDirectoryFlag) == 0)
                {
                    continue;
                }

                foreach ((uint _, uint dataOffset) in ReadResourceDirectory(
                    resourceBaseRva,
                    directory.Size,
                    resource.Offset & ~ResourceDirectoryFlag))
                {
                    if ((dataOffset & ResourceDirectoryFlag) != 0)
                    {
                        continue;
                    }

                    string resourceName = resource.Name
                        ?? resource.Id?.ToString(System.Globalization.CultureInfo.InvariantCulture)
                        ?? string.Empty;
                    try
                    {
                        ReadOnlyMemory<byte>? resourceData = ReadFormResourceData(
                            checked(resourceBaseRva + dataOffset));
                        if (resourceData is not ReadOnlyMemory<byte> data)
                        {
                            continue;
                        }

                        totalFormResourceBytes = checked(totalFormResourceBytes + data.Length);
                        if (forms.Count >= MaximumFormCount
                            || totalFormResourceBytes > MaximumTotalFormResourceBytes)
                        {
                            throw new InvalidDataException("The PE image contains too many or too much form data.");
                        }

                        DelphiFormComponent root = new DelphiBinaryFormReader(data).Read();
                        forms.Add(new DelphiForm(resourceName, root));
                    }
                    catch (InvalidDataException exception)
                    {
                        formDiagnostics.Add(
                            $"Form resource '{resourceName}' was skipped: {exception.Message}");
                    }
                }
            }
        }

        diagnostics = formDiagnostics;
        return forms;
    }

    private IReadOnlyList<(uint Id, uint Offset)> ReadResourceDirectory(
        uint resourceBaseRva,
        int resourceDirectorySize,
        uint relativeOffset)
    {
        int headerOffset = MapResourceOffset(resourceBaseRva, resourceDirectorySize, relativeOffset, 16);
        ReadOnlySpan<byte> header = _image.Span.Slice(headerOffset, 16);
        int namedEntryCount = ReadUInt16(header, 12);
        int idEntryCount = ReadUInt16(header, 14);
        int entryCount = checked(namedEntryCount + idEntryCount);
        if (entryCount > MaximumResourceDirectoryEntries)
        {
            throw new InvalidDataException("The PE resource directory contains too many entries.");
        }

        int entriesOffset = MapResourceOffset(
            resourceBaseRva,
            resourceDirectorySize,
            checked(relativeOffset + 16),
            checked(entryCount * 8));
        List<(uint Id, uint Offset)> entries = new(entryCount);
        for (int index = 0; index < entryCount; index++)
        {
            ReadOnlySpan<byte> entry = _image.Span.Slice(entriesOffset + index * 8, 8);
            uint name = ReadUInt32(entry, 0);
            if ((name & ResourceNameFlag) != 0)
            {
                continue;
            }

            entries.Add((name, ReadUInt32(entry, 4)));
        }

        return entries;
    }

    private IReadOnlyList<ResourceEntry> ReadResourceEntries(
        uint resourceBaseRva,
        int resourceDirectorySize,
        uint relativeOffset)
    {
        int headerOffset = MapResourceOffset(resourceBaseRva, resourceDirectorySize, relativeOffset, 16);
        ReadOnlySpan<byte> header = _image.Span.Slice(headerOffset, 16);
        int namedEntryCount = ReadUInt16(header, 12);
        int idEntryCount = ReadUInt16(header, 14);
        int entryCount = checked(namedEntryCount + idEntryCount);
        if (entryCount > MaximumResourceDirectoryEntries)
        {
            throw new InvalidDataException("The PE resource directory contains too many entries.");
        }

        int entriesOffset = MapResourceOffset(
            resourceBaseRva,
            resourceDirectorySize,
            checked(relativeOffset + 16),
            checked(entryCount * 8));
        List<ResourceEntry> entries = new(entryCount);
        for (int index = 0; index < entryCount; index++)
        {
            ReadOnlySpan<byte> entry = _image.Span.Slice(entriesOffset + index * 8, 8);
            uint name = ReadUInt32(entry, 0);
            uint offset = ReadUInt32(entry, 4);
            if ((name & ResourceNameFlag) != 0)
            {
                entries.Add(new ResourceEntry(
                    null,
                    ReadResourceName(
                        resourceBaseRva,
                        resourceDirectorySize,
                        name & ~ResourceNameFlag),
                    offset));
            }
            else
            {
                entries.Add(new ResourceEntry(name, null, offset));
            }
        }

        return entries;
    }

    private string ReadResourceName(uint resourceBaseRva, int resourceDirectorySize, uint relativeOffset)
    {
        int lengthOffset = MapResourceOffset(resourceBaseRva, resourceDirectorySize, relativeOffset, sizeof(ushort));
        ushort characterCount = ReadUInt16(_image.Span.Slice(lengthOffset), 0);
        int byteCount = checked(characterCount * sizeof(char));
        int textOffset = MapResourceOffset(
            resourceBaseRva,
            resourceDirectorySize,
            checked(relativeOffset + sizeof(ushort)),
            byteCount);
        return Encoding.Unicode.GetString(_image.Span.Slice(textOffset, byteCount));
    }

    private ReadOnlyMemory<byte>? ReadFormResourceData(uint dataOffset)
    {
        int entryOffset = MapRva(dataOffset, 16);
        ReadOnlySpan<byte> entry = _image.Span.Slice(entryOffset, 16);
        uint dataRva = ReadUInt32(entry, 0);
        uint dataSize = ReadUInt32(entry, 4);
        if (dataSize < 4)
        {
            return null;
        }

        int signatureOffset = MapRva(dataRva, 4);
        if (!_image.Span.Slice(signatureOffset, 4).SequenceEqual("TPF0"u8))
        {
            return null;
        }

        if (dataSize > MaximumFormResourceBytes)
        {
            throw new InvalidDataException("A PE form resource exceeds the size limit.");
        }

        int rawOffset = MapRva(dataRva, checked((int)dataSize));
        return _image.Slice(rawOffset, (int)dataSize);
    }

    private void ReadResourceStringBlock(
        uint dataEntryRva,
        uint stringIdBase,
        ushort languageId,
        List<PeResourceString> strings)
    {
        int dataEntryOffset = MapRva(dataEntryRva, 16);
        ReadOnlySpan<byte> dataEntry = _image.Span.Slice(dataEntryOffset, 16);
        uint dataRva = ReadUInt32(dataEntry, 0);
        uint dataSize = ReadUInt32(dataEntry, 4);
        if (dataSize > int.MaxValue || (dataSize & 1) != 0)
        {
            throw new InvalidDataException("A PE string resource block has an invalid size.");
        }

        int dataOffset = MapRva(dataRva, (int)dataSize);
        ReadOnlySpan<byte> block = _image.Span.Slice(dataOffset, (int)dataSize);
        int cursor = 0;
        for (uint index = 0; index < 16; index++)
        {
            if (cursor > block.Length - sizeof(ushort))
            {
                throw new InvalidDataException("A PE string resource table is truncated.");
            }

            ushort characterCount = ReadUInt16(block, cursor);
            cursor += sizeof(ushort);
            int characterBytes = checked(characterCount * sizeof(char));
            if (characterBytes > block.Length - cursor)
            {
                throw new InvalidDataException("A PE string resource is truncated.");
            }

            if (characterCount > 0)
            {
                uint stringId = checked(stringIdBase + index);
                strings.Add(new PeResourceString(
                    stringId,
                    languageId,
                    Encoding.Unicode.GetString(block.Slice(cursor, characterBytes))));
            }

            cursor += characterBytes;
        }
    }

    private int MapResourceOffset(
        uint resourceBaseRva,
        int resourceDirectorySize,
        uint relativeOffset,
        int size)
    {
        if (relativeOffset > resourceDirectorySize
            || size < 0
            || (ulong)size > (ulong)resourceDirectorySize - relativeOffset)
        {
            throw new InvalidDataException("A PE resource-directory entry extends beyond its directory.");
        }

        return MapRva(checked(resourceBaseRva + relativeOffset), size);
    }

    private IReadOnlyList<PeImportSymbol> ReadImportSymbols(uint thunkRva)
    {
        List<PeImportSymbol> symbols = [];
        int entrySize = _is64Bit ? sizeof(ulong) : sizeof(uint);
        int maximumEntries = GetRemainingRawBytes(thunkRva) / entrySize;
        maximumEntries = Math.Min(maximumEntries, MaximumTableEntries);

        for (int index = 0; index < maximumEntries; index++)
        {
            uint entryRva = checked(thunkRva + (uint)(index * entrySize));
            int entryOffset = MapRva(entryRva, entrySize);
            ulong thunk = _is64Bit
                ? ReadUInt64(_image.Span.Slice(entryOffset, entrySize), 0)
                : ReadUInt32(_image.Span.Slice(entryOffset, entrySize), 0);
            if (thunk == 0)
            {
                return symbols;
            }

            bool isOrdinal = _is64Bit
                ? (thunk & OrdinalFlag64) != 0
                : (thunk & OrdinalFlag32) != 0;
            if (isOrdinal)
            {
                symbols.Add(new PeImportSymbol(null, checked((ushort)(thunk & 0xffff)), entryRva));
            }
            else
            {
                uint importNameRva = checked((uint)thunk);
                int hintOffset = MapRva(importNameRva, sizeof(ushort));
                string name = ReadAsciiString(checked(importNameRva + (uint)sizeof(ushort)));
                _ = ReadUInt16(_image.Span.Slice(hintOffset), 0);
                symbols.Add(new PeImportSymbol(name, null, entryRva));
            }
        }

        throw new InvalidDataException("A PE import thunk table has no terminating entry.");
    }

    private string ReadAsciiString(uint rva)
    {
        int offset = MapRva(rva, 1);
        int maximumLength = GetRemainingRawBytes(rva);
        ReadOnlySpan<byte> data = _image.Span.Slice(offset, maximumLength);
        int terminator = data.IndexOf((byte)0);
        if (terminator < 0)
        {
            throw new InvalidDataException("A PE data-directory string is not terminated.");
        }

        return Encoding.ASCII.GetString(data[..terminator]);
    }

    private int MapRva(uint rva, int size)
    {
        foreach (SectionHeader section in _sections)
        {
            uint delta = rva >= section.VirtualAddress ? rva - (uint)section.VirtualAddress : uint.MaxValue;
            if (delta <= section.SizeOfRawData && size <= section.SizeOfRawData - delta)
            {
                long rawOffset = (long)section.PointerToRawData + delta;
                if (rawOffset >= 0 && rawOffset <= _image.Length - size)
                {
                    return checked((int)rawOffset);
                }
            }
        }

        throw new InvalidDataException($"PE RVA 0x{rva:X8} does not map to {size} bytes of file data.");
    }

    private int GetRemainingRawBytes(uint rva)
    {
        foreach (SectionHeader section in _sections)
        {
            uint delta = rva >= section.VirtualAddress ? rva - (uint)section.VirtualAddress : uint.MaxValue;
            if (delta < section.SizeOfRawData)
            {
                return checked((int)(section.SizeOfRawData - delta));
            }
        }

        throw new InvalidDataException($"PE RVA 0x{rva:X8} does not map to file data.");
    }

    private static uint ReadUInt32(ReadOnlySpan<byte> data, int offset) =>
        BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, sizeof(uint)));

    private static ushort ReadUInt16(ReadOnlySpan<byte> data, int offset) =>
        BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, sizeof(ushort)));

    private static ulong ReadUInt64(ReadOnlySpan<byte> data, int offset) =>
        BinaryPrimitives.ReadUInt64LittleEndian(data.Slice(offset, sizeof(ulong)));

    private readonly record struct ResourceEntry(uint? Id, string? Name, uint Offset);
}
