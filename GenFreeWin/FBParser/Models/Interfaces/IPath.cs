namespace FBParser.Models.Interfaces;

/// <summary>
/// Provides an abstraction over <see cref="Path"/> for testable path handling.
/// </summary>
public interface IPath
{
    /// <summary>
    /// Returns the absolute path for the specified input path.
    /// </summary>
    /// <param name="sPath">The source path.</param>
    /// <returns>The absolute path.</returns>
    string GetFullPath(string sPath);

    /// <summary>
    /// Returns the directory component of the specified path.
    /// </summary>
    /// <param name="sPath">The source path.</param>
    /// <returns>The directory component, or <see langword="null"/> when unavailable.</returns>
    string? GetDirectoryName(string sPath);

    /// <summary>
    /// Returns a path to a uniquely named temporary file.
    /// </summary>
    /// <returns>The temporary file path.</returns>
    string GetTempFileName();
}
