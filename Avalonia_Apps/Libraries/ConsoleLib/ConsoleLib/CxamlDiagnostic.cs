namespace ConsoleLib;

public enum CxamlDiagnosticSeverity
{
    Warning,
    Error
}

public sealed class CxamlDiagnostic
{
    public CxamlDiagnostic(CxamlDiagnosticSeverity severity, string message)
    {
        Severity = severity;
        Message = message;
    }

    public CxamlDiagnosticSeverity Severity { get; }
    public string Message { get; }
}
