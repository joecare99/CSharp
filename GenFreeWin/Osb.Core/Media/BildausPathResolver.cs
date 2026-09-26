using System.IO;

namespace Osb.Core.Media;

/// <summary>
/// Resolves a picture record's stored path to an absolute filesystem path.
/// Legacy behaviour: a leading '#' marks a path relative to the configured picture base directory.
/// </summary>
public static class BildausPathResolver
{
    /// <summary>
    /// Resolve the full path to the picture file.
    /// </summary>
    /// <param name="entry">The picture entry.</param>
    /// <param name="pictureBaseDirectory">The legacy base directory (COND.Verz).</param>
    /// <returns>Full filesystem path to the picture file.
    /// The method never returns null; it returns a best-effort combined path.</returns>
    public static string ResolvePath(BildausEntry entry, string pictureBaseDirectory)
    {
        if (entry is null) return string.Empty;

        var rawPath = entry.Path ?? string.Empty;

        // If path starts with '#', treat it as relative to the configured base directory
        if (rawPath.StartsWith("#"))
        {
            var relative = rawPath.Substring(1).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            // Combine base directory, relative path and filename
            return Path.Combine(pictureBaseDirectory ?? string.Empty, relative ?? string.Empty, entry.FileName ?? string.Empty);
        }

        // Otherwise, combine the stored path and the filename.
        return Path.Combine(rawPath ?? string.Empty, entry.FileName ?? string.Empty);
    }
}
