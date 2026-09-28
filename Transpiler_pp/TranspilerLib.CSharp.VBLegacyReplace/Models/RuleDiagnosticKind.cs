namespace TranspilerLib.CSharp.VBLegacyReplace.Models;

/// <summary>Identifies the outcome category of a replacement diagnostic.</summary>
public enum RuleDiagnosticKind
{
    /// <summary>The rule was applied successfully.</summary>
    Applied,
    /// <summary>The rule was intentionally not applied.</summary>
    Skipped,
    /// <summary>The rule definition or rule set was malformed.</summary>
    Malformed
}
