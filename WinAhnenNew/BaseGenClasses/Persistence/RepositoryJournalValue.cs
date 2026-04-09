using System;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Represents a serializable repository snapshot for genealogy journal entries.
/// </summary>
public sealed class RepositoryJournalValue
{
    /// <summary>
    /// Gets or sets the repository name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Gets or sets the repository information text.
    /// </summary>
    public string? Info { get; init; }

    /// <summary>
    /// Gets or sets the repository URI.
    /// </summary>
    public string? Uri { get; init; }

    /// <summary>
    /// Gets or sets the number of linked sources.
    /// </summary>
    public int SourceCount { get; init; }

    /// <summary>
    /// Creates a journal snapshot from a genealogy repository.
    /// </summary>
    /// <param name="genRepository">The repository to snapshot.</param>
    /// <returns>The snapshot value, or <see langword="null"/> when no repository is available.</returns>
    public static RepositoryJournalValue? FromRepository(IGenRepository? genRepository)
    {
        if (genRepository is null)
        {
            return null;
        }

        return new RepositoryJournalValue
        {
            Name = genRepository.Name,
            Info = genRepository.Info,
            Uri = genRepository.Uri?.ToString(),
            SourceCount = genRepository.GenSources.Count
        };
    }
}
