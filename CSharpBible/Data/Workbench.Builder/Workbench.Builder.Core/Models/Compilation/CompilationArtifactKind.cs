namespace Workbench.Builder.Core.Models.Compilation;

/// <summary>
/// Describes a generated artifact that belongs to a compilation result.
/// </summary>
public enum CompilationArtifactKind
{
    /// <summary>
    /// The primary emitted assembly or executable artifact.
    /// </summary>
    PrimaryOutput = 0,

    /// <summary>
    /// The emitted portable debug-symbol artifact.
    /// </summary>
    DebugSymbols = 1,
}
