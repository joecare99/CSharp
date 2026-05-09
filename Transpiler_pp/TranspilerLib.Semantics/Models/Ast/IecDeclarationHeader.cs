namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents exported declaration-header information extracted from IEC declaration text.
/// </summary>
public sealed class IecDeclarationHeader : IecAstNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecDeclarationHeader"/> class.
    /// </summary>
    /// <param name="artifactName">The exported artifact name when available.</param>
    /// <param name="returnTypeName">The declared return type name when available.</param>
    /// <param name="artifactMetadata">The shared artifact metadata.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecDeclarationHeader(string? artifactName, string? returnTypeName, IecArtifactMetadata? artifactMetadata = null, int sourcePos = -1)
        : base(sourcePos)
    {
        ArtifactName = artifactName;
        ReturnTypeName = returnTypeName;
        ArtifactMetadata = artifactMetadata ?? new IecArtifactMetadata();
    }

    /// <summary>
    /// Gets the exported artifact name when it is available.
    /// </summary>
    public string? ArtifactName { get; }

    /// <summary>
    /// Gets the declared return type name when it is available.
    /// </summary>
    public string? ReturnTypeName { get; }

    /// <summary>
    /// Gets the shared artifact metadata derived from the declaration header.
    /// </summary>
    public IecArtifactMetadata ArtifactMetadata { get; }
}
