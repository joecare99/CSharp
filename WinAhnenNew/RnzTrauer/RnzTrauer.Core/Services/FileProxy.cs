using RnzTrauer.Core.Services.Interfaces;
using System.IO;

namespace RnzTrauer.Core;

/// <summary>
/// Delegates file-system operations to <see cref="File"/>.
/// </summary>
public sealed class FileProxy : IFile
{
    /// <inheritdoc />
    public bool Exists(string sPath) => File.Exists(sPath);

    /// <inheritdoc />
    public string ReadAllText(string sPath) => File.ReadAllText(sPath);

    /// <inheritdoc />
    public byte[] ReadAllBytes(string sPath) => File.ReadAllBytes(sPath);

    /// <inheritdoc />
    public void WriteAllText(string sPath, string sContents) => File.WriteAllText(sPath, sContents);

    /// <inheritdoc />
    public void WriteAllBytes(string sPath, byte[] arrBytes) => File.WriteAllBytes(sPath, arrBytes);
}
