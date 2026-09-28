namespace TranspilerLib.CSharp.VBLegacyReplace.Models;

/// <summary>Reports a rule application, skip, or configuration error.</summary>
public sealed class RuleDiagnostic
{
    /// <summary>Initializes a diagnostic with its identifying metadata.</summary>
    /// <param name="kind">The outcome category.</param>
    /// <param name="severity">The diagnostic severity.</param>
    /// <param name="message">A human-readable explanation.</param>
    /// <param name="ruleId">The associated rule ID, when available.</param>
    /// <param name="position">The zero-based source offset, when available.</param>
    public RuleDiagnostic(RuleDiagnosticKind kind, RuleDiagnosticSeverity severity, string message, string? ruleId = null, int? position = null)
    {
        Kind = kind;
        Severity = severity;
        Message = message;
        RuleId = ruleId;
        Position = position;
    }

    /// <summary>Gets the diagnostic category.</summary>
    public RuleDiagnosticKind Kind { get; }

    /// <summary>Gets the diagnostic severity.</summary>
    public RuleDiagnosticSeverity Severity { get; }

    /// <summary>Gets the explanatory message.</summary>
    public string Message { get; }

    /// <summary>Gets the associated rule identifier, if known.</summary>
    public string? RuleId { get; }

    /// <summary>Gets the zero-based source offset, if applicable.</summary>
    public int? Position { get; }
}
