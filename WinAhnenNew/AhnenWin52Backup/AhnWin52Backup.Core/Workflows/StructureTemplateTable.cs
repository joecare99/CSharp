using System.Collections.Generic;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Describes a Paradox table family and its required initial row count.</summary>
public sealed record StructureTemplateTable(
    string FileName,
    byte Version,
    int RecordSize,
    int MaximumTableSize,
    string DatabaseSortOrder,
    IReadOnlyList<int> DatabaseFieldNumbers,
    int ExpectedRecords,
    int ExpectedBlocks,
    int ExpectedPrimaryIndexBlocks,
    string? PrimaryIndexEmptyBlockRecordHex,
    bool Encrypted,
    string PrimaryIndexFile,
    string? MemoFile,
    IReadOnlyList<StructureTemplateField> Fields,
    IReadOnlyList<StructureTemplateIndex> SecondaryIndexes,
    StructureTemplateBaseline? BaselineData,
    StructureTemplateHeaderMetadata DatabaseHeaderMetadata,
    StructureTemplateHeaderMetadata PrimaryIndexHeaderMetadata);
