namespace IDR.Core.Models;

public sealed record PeSection(
    string Name,
    uint VirtualAddress,
    uint VirtualSize,
    uint RawAddress,
    uint RawSize,
    bool ContainsCode);
