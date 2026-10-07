namespace OFBCreator.Console.Models;

/// <summary>
/// Persistent family-book settings, independent of any one GEDCOM input file.
/// </summary>
public sealed class OFBProject
{
    public int SchemaVersion { get; set; } = 1;

    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string EntryTemplate { get; set; } = "gc";
}
