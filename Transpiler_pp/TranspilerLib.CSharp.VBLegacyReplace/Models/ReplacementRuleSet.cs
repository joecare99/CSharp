using System.Collections.Generic;
namespace TranspilerLib.CSharp.VBLegacyReplace.Models;

/// <summary>Describes a versioned collection of externally configured replacement rules.</summary>
public sealed class ReplacementRuleSet
{
    /// <summary>Gets or initializes the schema version used to interpret this rule set.</summary>
    public int SchemaVersion { get; init; } = 1;

    /// <summary>Gets or initializes the ordered replacement rules in this rule set.</summary>
    public List<ReplacementRule> Rules { get; init; } = new();
}

