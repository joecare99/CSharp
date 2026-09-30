namespace IDR.Core.Models;

public sealed record DelphiVmtMethod(
    string Name,
    uint? CodeAddress,
    bool IsExtended = false,
    ushort? Flags = null,
    ushort? VirtualIndex = null);
