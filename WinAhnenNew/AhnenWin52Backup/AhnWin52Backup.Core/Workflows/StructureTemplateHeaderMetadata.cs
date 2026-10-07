namespace AhnWin52Backup.Core.Workflows;

/// <summary>Declares initial Paradox header state that varies between template files.</summary>
public sealed record StructureTemplateHeaderMetadata(
    ushort Word12,
    byte SortOrderCode,
    string RevisionBytesHex,
    uint AutoIncrementValue,
    string HeaderBytes38To3FHex,
    string HeaderBytes4DTo57Hex,
    string? DataHeaderMetadataHex);
