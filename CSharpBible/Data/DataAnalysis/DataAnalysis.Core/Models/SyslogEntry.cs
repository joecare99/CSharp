namespace DataAnalysis.Core.Models;

public sealed record SyslogEntry(
 DateTimeOffset? Timestamp,
 SyslogSeverity Severity,
 string SeverityText,
 string Source,
 string Message
)
{
 // Zusätzliche, dynamische Attribute je nach Mapping/Parser
 public IReadOnlyDictionary<string, string?> Attributes { get; init; } = new Dictionary<string, string?>();
}
