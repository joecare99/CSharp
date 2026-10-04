namespace OFBCreator.Core.Models;

/// <summary>Explains a stale decision or a grouping candidate that cannot be persisted safely.</summary>
public sealed record OFBGroupingDiagnostic(string Code, string? DecisionId, string Message);
