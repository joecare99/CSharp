namespace OFBCreator.Core.Models;

/// <summary>Describes a project rule that was stale, unsafe to apply, or otherwise noteworthy.</summary>
public sealed record OFBExportRuleDiagnostic(
    string Code,
    string RuleId,
    string Message,
    bool IsError);
