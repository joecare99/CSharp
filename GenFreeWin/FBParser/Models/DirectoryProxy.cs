using FBParser.Models.Interfaces;

namespace FBParser.Models;

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
}
