namespace TranspilerLib.CSharp.StatEqualCheck;

/// <summary>Identifies the severity of a comparison finding.</summary>
public enum StatEqualFindingSeverity
{
    /// <summary>The finding limits the proof but does not establish a mismatch.</summary>
    Warning,
    /// <summary>The finding establishes observably different modeled behavior.</summary>
    Error
}
