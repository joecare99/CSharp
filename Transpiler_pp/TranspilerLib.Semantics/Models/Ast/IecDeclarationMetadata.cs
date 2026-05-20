namespace TranspilerLib.IEC.Models.Ast;

/// <summary>
/// Represents shared language-independent metadata for a semantic declaration.
/// </summary>
public sealed class IecDeclarationMetadata : IecMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IecDeclarationMetadata"/> class.
    /// </summary>
    /// <param name="section">The declaration section that contains the variable.</param>
    /// <param name="initializerText">The raw initializer text when one exists in the declaration.</param>
    public IecDeclarationMetadata(
        IecDeclarationSection section = IecDeclarationSection.Unknown,
        string? initializerText = null)
    {
        Section = section;
        InitializerText = initializerText;
    }

    /// <summary>
    /// Gets the declaration section that contains the variable.
    /// </summary>
    public IecDeclarationSection Section { get; }

    /// <summary>
    /// Gets the raw initializer text when one exists in the declaration.
    /// </summary>
    public string? InitializerText { get; }
}
