using System.Collections.Generic;

namespace AhnWin52Backup.Core.Workflows;

/// <summary>Describes one secondary XG/YG index pair and its key layout.</summary>
public sealed record StructureTemplateIndex(
    int Number,
    string XgFile,
    string YgFile,
    string SortOrder,
    string Label,
    IReadOnlyList<string> KeyFields,
    IReadOnlyList<StructureTemplateField> Fields,
    int ExpectedXgRecords,
    int ExpectedYgRecords);
