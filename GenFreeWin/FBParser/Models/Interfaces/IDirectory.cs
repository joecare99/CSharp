namespace FBParser.Models.Interfaces;

/// <summary>
/// Provides an abstraction over <see cref="Directory"/> for testable directory access.
/// </summary>
public interface IDirectory
{
    /// <summary>
    /// Determines whether the specified directory exists.
    /// </summary>
    /// <param name="sPath">The directory path.</param>
    /// <returns><see langword="true"/> if the directory exists; otherwise <see langword="false"/>.</returns>
    bool Exists(string sPath);

    /// <summary>
    /// Creates the specified directory and all missing parent directories.
    /// </summary>
    /// <param name="sPath">The directory path.</param>
    void CreateDirectory(string sPath);
}
