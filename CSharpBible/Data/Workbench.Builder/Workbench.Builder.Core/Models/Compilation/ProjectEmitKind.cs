namespace Workbench.Builder.Core.Models.Compilation;

/// <summary>
/// Describes the primary artifact category a supported project should emit.
/// </summary>
public enum ProjectEmitKind
{
    /// <summary>
    /// The project should not be emitted in the current compilation slice.
    /// </summary>
    None = 0,

    /// <summary>
    /// The project should emit a library assembly.
    /// </summary>
    Library = 1,

    /// <summary>
    /// The project should emit an executable assembly.
    /// </summary>
    Executable = 2,
}
