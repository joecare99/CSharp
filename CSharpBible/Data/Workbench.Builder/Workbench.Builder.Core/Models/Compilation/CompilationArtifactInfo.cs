namespace Workbench.Builder.Core.Models.Compilation;

/// <summary>
/// Represents a file artifact produced by a compilation attempt.
/// </summary>
public sealed class CompilationArtifactInfo
{
    /// <summary>
    /// Initializes a new instance of <see cref="CompilationArtifactInfo"/>.
    /// </summary>
    /// <param name="kind">The artifact kind.</param>
    /// <param name="filePath">The full path to the artifact.</param>
    /// <param name="exists">A value indicating whether the artifact exists on disk.</param>
    public CompilationArtifactInfo(CompilationArtifactKind kind, string filePath, bool exists)
    {
        Kind = kind;
        FilePath = filePath;
        Exists = exists;
    }

    /// <summary>
    /// Gets the artifact kind.
    /// </summary>
    public CompilationArtifactKind Kind { get; }

    /// <summary>
    /// Gets the full path to the artifact.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets a value indicating whether the artifact exists on disk.
    /// </summary>
    public bool Exists { get; }
}
