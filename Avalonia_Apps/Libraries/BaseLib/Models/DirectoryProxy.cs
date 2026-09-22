using BaseLib.Models.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BaseLib.Models;

/// <summary>
/// Implements <see cref="IDirectory"/> by delegating to <see cref="Directory"/>.
/// </summary>
public sealed class DirectoryProxy : IDirectory
{
    /// <inheritdoc />
    public bool Exists(string sPath)
        => Directory.Exists(sPath);

    /// <inheritdoc />
    public void CreateDirectory(string sPath)
        => Directory.CreateDirectory(sPath);

    /// <inheritdoc />
    public IReadOnlyList<IFileInfo> GetFiles(string sPath, string sSearchPattern)
        => Directory.GetFiles(sPath, sSearchPattern)
            .Select(filePath => (IFileInfo)new FileInfoProxy(filePath))
            .ToArray();
}
