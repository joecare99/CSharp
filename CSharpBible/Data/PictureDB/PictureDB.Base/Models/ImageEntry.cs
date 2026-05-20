using System;
using System.Collections.Generic;

namespace PictureDB.Base.Models;

/// <summary>
/// Represents an analyzed image in the database.
/// </summary>
public sealed class ImageEntry
{
    /// <summary>
    /// Gets or sets the unique identifier of the image.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the absolute file path to the image on disk.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name, typically inferred from the file name.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the AI-generated or user-provided description of the image.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the date and time the image was taken or created (EXIF data).
    /// </summary>
    public DateTimeOffset? CapturedAt { get; set; }

    /// <summary>
    /// Gets or sets the list of tags associated with this image.
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Gets or sets a dictionary of additional basic metadata such as dimensions or camera model.
    /// </summary>
    public Dictionary<string, string> BasicMetadata { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}