namespace Genealogy.Models;

/// <summary>
/// A structured issue reported while importing, interpreting, or exporting genealogy data.
/// </summary>
public sealed class GenealogyDiagnostic
{
    public GenealogyDiagnosticSeverity Severity { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public int? LineNumber { get; set; }

    public string? Subject { get; set; }
}
