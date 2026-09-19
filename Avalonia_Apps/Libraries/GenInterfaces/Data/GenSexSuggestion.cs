namespace GenInterfaces.Data;

/// <summary>
/// Represents a suggested sex classification for a given name.
/// </summary>
public sealed class GenSexSuggestion
{
    /// <summary>
    /// Gets the suggested sex marker.
    /// </summary>
    public char Sex { get; set; }

    /// <summary>
    /// Gets the confidence score in the range from 0 to 1.
    /// </summary>
    public double Confidence { get; set; }

    /// <summary>
    /// Gets the originating authority or dataset name.
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Gets optional explanatory details for the suggestion.
    /// </summary>
    public string? Detail { get; set; }
}
