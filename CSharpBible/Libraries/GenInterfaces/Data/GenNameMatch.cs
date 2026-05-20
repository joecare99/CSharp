namespace GenInterfaces.Data;

/// <summary>
/// Represents a name suggestion or lookup hit returned by a name authority.
/// </summary>
public sealed class GenNameMatch
{
    /// <summary>
    /// Gets the display name or canonical name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets a normalized name representation when available.
    /// </summary>
    public string? NormalizedName { get; set; }

    /// <summary>
    /// Gets the suggested sex marker when available.
    /// </summary>
    public char? Sex { get; set; }

    /// <summary>
    /// Gets the culture or language tag associated with the name.
    /// </summary>
    public string? CultureName { get; set; }

    /// <summary>
    /// Gets the confidence score in the range from 0 to 1.
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Gets the originating authority or dataset name.
    /// </summary>
    public string? Source { get; set; }
}
