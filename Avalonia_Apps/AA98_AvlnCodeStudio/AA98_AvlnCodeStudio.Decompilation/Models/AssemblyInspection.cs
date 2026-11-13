using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Decompilation;

/// <summary>Metadata-only summary of a managed assembly.</summary>
public sealed record AssemblyInspection(
    string AssemblyPath,
    string Name,
    string Version,
    IReadOnlyList<AssemblyTypeInfo> Types);

/// <summary>Metadata names exposed for a single type definition.</summary>
public sealed record AssemblyTypeInfo(
    string FullName,
    string Name,
    int MetadataToken,
    bool IsPublic,
    IReadOnlyList<string> Methods,
    IReadOnlyList<string> Fields,
    IReadOnlyList<string> Properties);
