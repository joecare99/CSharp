namespace IDR.Core.Models;

public sealed record MemberAccessCandidate(
    string OwnerTypeName,
    int Offset,
    string FieldName,
    uint? TypeInfoAddress,
    string? TypeName);
