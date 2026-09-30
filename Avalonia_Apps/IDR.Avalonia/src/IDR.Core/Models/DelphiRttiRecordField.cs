namespace IDR.Core.Models;

public sealed record DelphiRttiRecordField(
    string Name,
    int Offset,
    uint? TypeInfoAddress);
