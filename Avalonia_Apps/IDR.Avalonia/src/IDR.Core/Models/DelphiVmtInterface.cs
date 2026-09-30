using System;

namespace IDR.Core.Models;

public sealed record DelphiVmtInterface(
    Guid Id,
    uint? VTableAddress,
    int Offset,
    int? ImplementationGetter,
    uint? TypeInfoAddress = null);
