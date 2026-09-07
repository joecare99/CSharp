namespace ConsoleLib.Showcase.Models;

/// <summary>Describes one area in the ConsoleLib component gallery.</summary>
public sealed record ShowcaseSection(string Name, string Description)
{
    public override string ToString() => Name;
}
