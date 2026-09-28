using System;
namespace TranspilerLib.CSharp.StatEqualCheck;

/// <summary>A diagnostic produced while parsing, modeling, or comparing an input.</summary>
public sealed class StatEqualFinding
{
    /// <summary>Creates a finding with optional locations in either input.</summary>
    public StatEqualFinding(StatEqualFindingSeverity severity, string code, string message,
        StatEqualSourceSpan? leftSpan = null, StatEqualSourceSpan? rightSpan = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        Severity = severity;
        Code = code;
        Message = message;
        LeftSpan = leftSpan;
        RightSpan = rightSpan;
    }

    /// <summary>Gets the diagnostic severity.</summary>
    public StatEqualFindingSeverity Severity { get; }
    /// <summary>Gets the stable diagnostic identifier.</summary>
    public string Code { get; }
    /// <summary>Gets the human-readable diagnostic detail.</summary>
    public string Message { get; }
    /// <summary>Gets the location in the left input, when available.</summary>
    public StatEqualSourceSpan? LeftSpan { get; }
    /// <summary>Gets the location in the right input, when available.</summary>
    public StatEqualSourceSpan? RightSpan { get; }
}

