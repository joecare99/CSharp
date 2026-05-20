namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents shared language-independent metadata for a semantic compilation unit artifact.
/// </summary>
public sealed class IecArtifactMetadata : IecMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecArtifactMetadata"/> class.
    /// </summary>
    /// <param name="artifactKind">The language-independent artifact kind.</param>
    /// <param name="accessibility">The shared artifact accessibility.</param>
    /// <param name="isStatic">The wrapper static intent.</param>
    /// <param name="isPartial">The wrapper partial intent.</param>
    public IecArtifactMetadata(
        IecArtifactKind artifactKind = IecArtifactKind.Function,
        IecAccessibility accessibility = IecAccessibility.Public,
        bool? isStatic = null,
        bool isPartial = false)
    {
        ArtifactKind = artifactKind;
        Accessibility = accessibility;
        IsStatic = isStatic ?? artifactKind == IecArtifactKind.Function;
        IsPartial = isPartial;
    }

    /// <summary>
    /// Gets the language-independent artifact kind.
    /// </summary>
    public IecArtifactKind ArtifactKind { get; }

    /// <summary>
    /// Gets the shared artifact accessibility.
    /// </summary>
    public IecAccessibility Accessibility { get; }

    /// <summary>
    /// Gets a value indicating whether the generated wrapper has static intent.
    /// </summary>
    public bool IsStatic { get; }

    /// <summary>
    /// Gets a value indicating whether the generated wrapper has partial intent.
    /// </summary>
    public bool IsPartial { get; }
}
