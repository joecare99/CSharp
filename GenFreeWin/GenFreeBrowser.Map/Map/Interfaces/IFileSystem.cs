using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GenFreeBrowser.Map.Interfaces;

/// <summary>
/// Abstraction of file / directory operations used by the tile cache to simplify testing.
/// </summary>
public interface IFileSystem
{
    void EnsureDirectory(string path);
    bool FileExists(string path);
    Task<byte[]?> ReadAllBytesAsync(string path);
    Task WriteAllBytesAsync(string path, byte[] data);
    bool DirectoryExists(string path);
    IEnumerable<string> EnumerateFiles(string root, string pattern, bool recursive);
    FileMetadata GetFileMetadata(string path);
    bool TryDeleteFile(string path);
    IEnumerable<string> EnumerateDirectories(string root, bool recursive);
    int GetEntryCount(string directory);
    void TryDeleteDirectory(string path);
}

/// <summary>
/// Simple immutable metadata snapshot for a file.
/// </summary>
public readonly record struct FileMetadata(long Length, DateTime LastWriteUtc);
