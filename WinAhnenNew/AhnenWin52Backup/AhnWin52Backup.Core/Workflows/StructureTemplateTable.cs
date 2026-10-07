using System.Collections.Generic;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Describes a Paradox table family and its required initial row count.</summary>
public sealed record StructureTemplateTable(
    string FileName,
    byte Version,
    int RecordSize,
    int ExpectedRecords,
    int ExpectedBlocks,
    bool Encrypted,
    string PrimaryIndexFile,
    string? MemoFile,
    IReadOnlyList<StructureTemplateField> Fields,
    IReadOnlyList<StructureTemplateIndex> SecondaryIndexes,
    StructureTemplateBaseline? BaselineData);
