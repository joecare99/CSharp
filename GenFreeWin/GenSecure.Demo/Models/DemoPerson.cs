namespace GenSecure.Demo.Models;

/// <summary>
/// Sample person record used in the GenSecure show-off demonstration.
/// </summary>
public sealed record DemoPerson
{
    /// <summary>Gets the first name.</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>Gets the last name.</summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>Gets the birth date as an ISO 8601 string.</summary>
    public string BirthDate { get; init; } = string.Empty;

    /// <summary>Gets the street address.</summary>
    public string Address { get; init; } = string.Empty;

    /// <summary>Gets an optional note.</summary>
    public string? Note { get; init; }
}
