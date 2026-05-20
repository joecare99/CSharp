#if NET8_0_OR_GREATER
namespace TranspilerLib.CSharp.Models.Generation;

/// <summary>
/// Defines output-shaping options for the AST-based C# backend.
/// The initial option set stays intentionally small and focuses on backend-specific wrapper structure.
/// </summary>
public sealed class CSharpBackendOptions
{
    /// <summary>
    /// Gets or sets the optional namespace for the generated C# source.
    /// </summary>
    public string? NamespaceName { get; set; }

    /// <summary>
    /// Gets or sets the containing type name for the generated C# source.
    /// </summary>
    public string TypeName { get; set; } = "GeneratedIecProgram";
}
#endif
