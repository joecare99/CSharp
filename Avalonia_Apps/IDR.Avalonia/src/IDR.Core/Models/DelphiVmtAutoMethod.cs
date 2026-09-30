using System.Collections.Generic;

namespace IDR.Core.Models;

public sealed record DelphiVmtAutoMethod(
    int DispatchId,
    string Name,
    uint CodeAddress,
    int Flags,
    byte? ReturnType,
    IReadOnlyList<byte> ParameterTypes)
{
    public bool IsMethod => (Flags & 1) != 0;

    public bool IsVirtual => (Flags & 8) != 0;
}
