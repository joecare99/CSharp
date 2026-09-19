namespace GenInterfaces.Data;

/// <summary>
/// Represents a place search query for a place authority.
/// </summary>
public sealed class GenPlaceQuery
{
    /// <summary>
    /// Gets the search text.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets an optional country or region filter.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Gets the maximum number of results to return.
    /// </summary>
    public int MaxResults { get; set; } = 20;
}
