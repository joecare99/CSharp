using GenFreeBrowser.Map.Interfaces;
using System.IO;

namespace GenFreeBrowser.Map;

public sealed class LocalFileSystem : IFileSystem
{
    public void EnsureDirectory(string path) => Directory.CreateDirectory(path);
    public bool FileExists(string path) => File.Exists(path);
    public async Task<byte[]?> ReadAllBytesAsync(string path) => await File.ReadAllBytesAsync(path).ConfigureAwait(false);
    public Task WriteAllBytesAsync(string path, byte[] data) => File.WriteAllBytesAsync(path, data);
    public bool DirectoryExists(string path) => Directory.Exists(path);
    public IEnumerable<string> EnumerateFiles(string root, string pattern, bool recursive) =>
        Directory.EnumerateFiles(root, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    public FileMetadata GetFileMetadata(string path)
    {
        var fi = new FileInfo(path);
        return new FileMetadata(fi.Length, fi.LastWriteTimeUtc);
    }
    public bool TryDeleteFile(string path)
    {
        try { File.Delete(path); return true; } catch { return false; }
    }
    public IEnumerable<string> EnumerateDirectories(string root, bool recursive) =>
        Directory.EnumerateDirectories(root, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    public int GetEntryCount(string directory)
    {
        try { return Directory.GetFileSystemEntries(directory).Length; } catch { return -1; }
    }
    public void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path)) Directory.Delete(path, false);
        }
        catch { }
    }
}
