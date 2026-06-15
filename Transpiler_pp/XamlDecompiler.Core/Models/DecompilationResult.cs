namespace XamlDecompiler.Core.Models;

/// <summary>
/// Contains the reconstructed artifacts for a generated source file.
/// </summary>
public sealed class DecompilationResult
{
    public required string ClassName { get; init; }

    public required string Namespace { get; init; }

    public required string RootTypeName { get; init; }

    public required string XamlFilePath { get; init; }

    public required string XamlText { get; init; }

    public required string CodeBehindText { get; init; }

    public required IReadOnlyList<string> Diagnostics { get; init; }
}
