namespace IDR.Core.Models;

public sealed record DelphiVmtField(
    string Name,
    int Offset,
    uint? TypeInfoAddress,
    bool IsExtended = false,
    byte? Flags = null);
