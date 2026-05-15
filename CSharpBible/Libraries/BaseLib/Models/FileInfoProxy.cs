using System.IO;
using BaseLib.Models.Interfaces;

namespace BaseLib.Models;

/// <summary>
/// Wraps <see cref="FileInfo"/> behind <see cref="IFileInfo"/> for testable metadata access.
/// </summary>
public sealed class FileInfoProxy : IFileInfo
{
    private readonly FileInfo _fileInfo;

    public FileInfoProxy(string sPath)
    {
        _fileInfo = new FileInfo(sPath);
    }

    public string FullName => _fileInfo.FullName;

    public string? DirectoryName => _fileInfo.DirectoryName;

    public string Name => _fileInfo.Name;

    public string Extension => _fileInfo.Extension;

    public long Length => _fileInfo.Length;

    public bool Exists => _fileInfo.Exists;
}