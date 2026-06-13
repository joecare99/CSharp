namespace XamlDecompiler.Core.Models;

internal sealed class GeneratedSourceModel
{
    public required string Namespace { get; init; }

    public required string ClassName { get; init; }

    public required string RootTypeName { get; init; }

    public required string XamlFilePath { get; init; }

    public required string UsingBlock { get; init; }

    public required string UserMembersBlock { get; init; }

    public required DecompiledElement RootElement { get; init; }

    public required IReadOnlyList<(string Prefix, string Namespace)> XmlNamespaces { get; init; }

    public required IReadOnlyList<string> Diagnostics { get; init; }
}
