namespace TranspilerLib.CSharp.VBLegacyReplace.Models;

/// <summary>Identifies the severity of a replacement diagnostic.</summary>
public enum RuleDiagnosticSeverity
{
    /// <summary>Informational diagnostic.</summary>
    Information,
    /// <summary>Potentially important condition that prevented a transformation.</summary>
    Warning,
    /// <summary>Invalid rule configuration.</summary>
    Error
}
