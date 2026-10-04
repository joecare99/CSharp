namespace Genealogy.Drivers;

/// <summary>
/// Common output options; provider-specific options are supplied by provider implementations.
/// </summary>
public sealed class GenealogyOutputOptions
{
    public string? TargetVersion { get; set; }

    /// <summary>
    /// Allows a provider to emit best-effort output from an import that required recovery.
    /// The output may omit or alter source data and must be accompanied by diagnostics.
    /// </summary>
    public bool AllowRecovery { get; set; }
}
