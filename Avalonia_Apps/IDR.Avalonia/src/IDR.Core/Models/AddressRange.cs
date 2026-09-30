namespace IDR.Core.Models;

public readonly record struct AddressRange(uint Start, uint Size)
{
    public uint EndExclusive => checked(Start + Size);

    public bool Contains(uint address)
    {
        return address >= Start && address < EndExclusive;
    }
}
