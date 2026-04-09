using System;
using GenInterfaces.Data;
using GenInterfaces.Interfaces.Genealogic;

namespace BaseGenClasses.Persistence;

/// <summary>
/// Represents a serializable media snapshot for genealogy journal entries.
/// </summary>
public sealed class MediaJournalValue
{
    /// <summary>
    /// Gets or sets the media type.
    /// </summary>
    public EMediaType eMediaType { get; init; }

    /// <summary>
    /// Gets or sets the media URI.
    /// </summary>
    public string? MediaUri { get; init; }

    /// <summary>
    /// Gets or sets the display name of the media.
    /// </summary>
    public string? MediaName { get; init; }

    /// <summary>
    /// Gets or sets the description of the media.
    /// </summary>
    public string? MediaDescription { get; init; }

    /// <summary>
    /// Creates a journal snapshot from a genealogy media item.
    /// </summary>
    /// <param name="genMedia">The media item to snapshot.</param>
    /// <returns>The snapshot value, or <see langword="null"/> when no media item is available.</returns>
    public static MediaJournalValue? FromMedia(IGenMedia? genMedia)
    {
        if (genMedia is null)
        {
            return null;
        }

        return new MediaJournalValue
        {
            eMediaType = genMedia.eMediaType,
            MediaUri = genMedia.MediaUri?.ToString(),
            MediaName = genMedia.MediaName,
            MediaDescription = genMedia.MediaDescription
        };
    }
}
