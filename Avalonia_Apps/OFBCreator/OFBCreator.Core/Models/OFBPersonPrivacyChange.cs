namespace OFBCreator.Core.Models;

/// <summary>Describes one person field changed by the configured export privacy rules.</summary>
/// <param name="Field">The stable field key used by the export-rule contract.</param>
/// <param name="OriginalValue">The value from the imported source record.</param>
/// <param name="ExportValue">The value remaining in the detached export record.</param>
public sealed record OFBPersonPrivacyChange(string Field, string? OriginalValue, string? ExportValue);
