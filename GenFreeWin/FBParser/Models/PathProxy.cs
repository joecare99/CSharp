using FBParser.Models.Interfaces;

namespace FBParser.Models;

/// <summary>
/// Implements <see cref="IPath"/> by delegating to <see cref="Path"/>.
/// </summary>
public sealed class PathProxy : IPath
{
    /// <inheritdoc />
    public string GetFullPath(string sPath)
        => Path.GetFullPath(sPath);

    /// <inheritdoc />
    public string? GetDirectoryName(string sPath)
        => Path.GetDirectoryName(sPath);

    /// <inheritdoc />
    public string GetTempFileName()
        => Path.GetTempFileName();
}
