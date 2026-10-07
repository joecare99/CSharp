namespace AhnWin52Backup.Core.Workflows;

/// <summary>Identifies one embedded file used to materialize the from-scratch structure.</summary>
public sealed record StructureTemplateAsset(
    string FileName,
    string ResourceName,
    long SizeBytes,
    string Sha256,
    string Kind);
