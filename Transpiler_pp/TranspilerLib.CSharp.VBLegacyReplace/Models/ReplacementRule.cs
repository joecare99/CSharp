using System.Collections.Generic;
namespace TranspilerLib.CSharp.VBLegacyReplace.Models;

/// <summary>Defines one token-template replacement rule.</summary>
public sealed class ReplacementRule
{
    /// <summary>Gets or initializes the stable identifier for this rule.</summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>Gets or initializes the source token template to find.</summary>
    public string SourceTemplate { get; init; } = string.Empty;

    /// <summary>Gets or initializes the replacement token template to emit.</summary>
    public string? ReplacementTemplate { get; init; }

    /// <summary>Gets or initializes namespaces required by replacement text.</summary>
    public List<string> RequiredUsings { get; init; } = new();

    /// <summary>Gets or initializes precedence; higher values are considered first.</summary>
    public int Priority { get; init; }

    /// <summary>Gets or initializes whether this rule participates in matching.</summary>
    public bool Enabled { get; init; } = true;

    /// <summary>Gets or initializes optional explanatory documentation.</summary>
    public string? Documentation { get; init; }
}


