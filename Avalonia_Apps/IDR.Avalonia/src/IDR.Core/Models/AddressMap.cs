using System;
using System.Collections.Generic;
using System.Linq;

namespace IDR.Core.Models;

public sealed class AddressMap
{
    private readonly IReadOnlyList<PeSection> _sections;

    public AddressMap(IEnumerable<PeSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);
        _sections = sections.ToArray();
        if (_sections.Count == 0)
        {
            throw new ArgumentException("At least one PE section is required.", nameof(sections));
        }
    }

    public bool TryRvaToRaw(uint rva, out int rawOffset)
    {
        foreach (PeSection section in _sections)
        {
            AddressRange virtualRange = new(section.VirtualAddress, section.VirtualSize);
            if (!virtualRange.Contains(rva))
            {
                continue;
            }

            uint offset = checked(rva - section.VirtualAddress);
            if (offset >= section.RawSize)
            {
                break;
            }

            rawOffset = checked((int)(section.RawAddress + offset));
            return true;
        }

        rawOffset = -1;
        return false;
    }

    public bool TryRawToRva(int rawOffset, out uint rva)
    {
        if (rawOffset < 0)
        {
            rva = 0;
            return false;
        }

        uint raw = checked((uint)rawOffset);
        foreach (PeSection section in _sections)
        {
            AddressRange rawRange = new(section.RawAddress, section.RawSize);
            if (!rawRange.Contains(raw))
            {
                continue;
            }

            rva = checked(section.VirtualAddress + raw - section.RawAddress);
            return true;
        }

        rva = 0;
        return false;
    }
}
