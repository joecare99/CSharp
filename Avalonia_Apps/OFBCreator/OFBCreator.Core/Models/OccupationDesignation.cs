namespace OFBCreator.Core.Models;

/// <summary>
/// A single occupation name and optional place or employer context parsed from a source label.
/// </summary>
public sealed record OccupationDesignation(string Name, string? Place, string? Employer);
