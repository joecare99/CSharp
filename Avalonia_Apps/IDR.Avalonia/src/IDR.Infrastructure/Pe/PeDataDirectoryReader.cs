using IDR.Core.Models;
using IDR.Core.Services;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;

namespace IDR.Infrastructure.Pe;

internal sealed class PeDataDirectoryReader
{
    private const uint OrdinalFlag32 = 0x80000000;
    private const ulong OrdinalFlag64 = 0x8000000000000000;
    private const int MaximumTableEntries = 1_000_000;

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
}
