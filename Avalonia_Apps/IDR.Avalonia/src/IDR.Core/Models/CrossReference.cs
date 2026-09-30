namespace IDR.Core.Models;

public sealed record CrossReference(
    uint SourceAddress,
    uint TargetAddress,
    CrossReferenceKind Kind,
    string? TargetName = null);

public enum CrossReferenceKind
{
    Unknown,
    Call,
    Jump,
    Constant
}
