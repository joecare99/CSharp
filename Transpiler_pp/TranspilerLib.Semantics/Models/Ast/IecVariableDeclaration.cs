namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents a typed IEC variable declaration.
/// The first AST slice keeps the declaration model intentionally small and focuses on the
/// identifier and declared IEC type name so later binding work can extend it incrementally.
/// </summary>
public sealed class IecVariableDeclaration : IecAstNode
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecVariableDeclaration"/> class.
    /// </summary>
    /// <param name="identifier">The declared variable name.</param>
    /// <param name="typeName">The declared IEC type name.</param>
    /// <param name="section">The declaration section that contains the variable.</param>
    /// <param name="initializerText">The raw initializer text when it exists.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecVariableDeclaration(string identifier, string? typeName = null, IecDeclarationSection section = IecDeclarationSection.Unknown, string? initializerText = null, int sourcePos = -1)
        : this(identifier, typeName, new IecDeclarationMetadata(section, initializerText), sourcePos)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IecVariableDeclaration"/> class with explicit declaration metadata.
    /// </summary>
    /// <param name="identifier">The declared variable name.</param>
    /// <param name="typeName">The declared IEC type name.</param>
    /// <param name="declarationMetadata">The shared declaration metadata.</param>
    /// <param name="sourcePos">Zero-based source position or a negative value when unknown.</param>
    public IecVariableDeclaration(string identifier, string? typeName, IecDeclarationMetadata? declarationMetadata, int sourcePos = -1)
        : base(sourcePos)
    {
        Identifier = identifier;
        TypeName = typeName;
        DeclarationMetadata = declarationMetadata ?? new IecDeclarationMetadata();
    }

    /// <summary>
    /// Gets the declared variable name.
    /// </summary>
    public string Identifier { get; }

    /// <summary>
    /// Gets the declared IEC type name when it is known.
    /// </summary>
    public string? TypeName { get; }

    /// <summary>
    /// Gets the shared declaration metadata.
    /// </summary>
    public IecDeclarationMetadata DeclarationMetadata { get; }

    /// <summary>
    /// Gets the declaration section that contains the variable.
    /// </summary>
    public IecDeclarationSection Section => DeclarationMetadata.Section;

    /// <summary>
    /// Gets the raw initializer text when one exists in the declaration.
    /// </summary>
    public string? InitializerText => DeclarationMetadata.InitializerText;
}
