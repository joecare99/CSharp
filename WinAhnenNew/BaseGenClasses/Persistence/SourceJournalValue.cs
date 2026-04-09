using System;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Represents a serializable source snapshot for genealogy journal entries.
/// </summary>
public sealed class SourceJournalValue
{
    /// <summary>
    /// Gets or sets the short description of the source.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets or sets the source URI.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Gets or sets the source payload text.
    /// </summary>
    public string? Data { get; init; }

    /// <summary>
    /// Gets or sets the number of linked media items.
    /// </summary>
    public int MediaCount { get; init; }

    /// <summary>
    /// Creates a journal snapshot from a genealogy source.
    /// </summary>
    /// <param name="genSource">The source to snapshot.</param>
    /// <returns>The snapshot value, or <see langword="null"/> when no source is available.</returns>
    public static SourceJournalValue? FromSource(IGenSource? genSource)
    {
        if (genSource is null)
        {
            return null;
        }

        return new SourceJournalValue
        {
            Description = genSource.Description,
            Url = genSource.Url?.ToString(),
            Data = genSource.Data,
            MediaCount = genSource.Medias.Count
        };
    }
}
