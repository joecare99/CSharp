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

    /// <summary>
    /// A copied dependency artifact required by the emitted output.
    /// </summary>
    Dependency = 2,

    /// <summary>
    /// A runtime host or launcher artifact for executable output.
    /// </summary>
    RuntimeHost = 3,

    /// <summary>
    /// A runtime metadata artifact such as deps or runtimeconfig.
    /// </summary>
    RuntimeMetadata = 4,
}
