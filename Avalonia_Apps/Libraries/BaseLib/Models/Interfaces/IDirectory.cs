using System.Collections.Generic;
using System.IO;

namespace BaseLib.Models.Interfaces;

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

    /// <summary>
    /// Gets the files directly contained in the specified directory.
    /// </summary>
    /// <param name="sPath">The directory path.</param>
    /// <param name="sSearchPattern">The search pattern.</param>
    /// <returns>Metadata abstractions for the matching files.</returns>
    IReadOnlyList<IFileInfo> GetFiles(string sPath, string sSearchPattern);
}
