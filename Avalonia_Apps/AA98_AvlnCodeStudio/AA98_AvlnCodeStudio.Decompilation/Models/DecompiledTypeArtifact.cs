using Code.Navigation;

namespace AA98_AvlnCodeStudio.Decompilation;

/// <summary>A generated, persisted decompilation artifact and its source-navigation location.</summary>
public sealed record DecompiledTypeArtifact(
    string AssemblyPath,
    string TypeFullName,
    string ArtifactPath,
    string Source,
    CodeLocation NavigationLocation);
