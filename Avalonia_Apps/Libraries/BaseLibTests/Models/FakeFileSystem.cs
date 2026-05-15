using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BaseLib.Models.Interfaces;

namespace BaseLib.Models;

public sealed class FakeFileSystem : IFile, IDirectory, IPath
{
    private readonly Dictionary<string, byte[]> _files = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, FakeFileInfo> _fileInfos = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _directories = new(StringComparer.OrdinalIgnoreCase);
    private int _tempFileCounter;

    public bool Exists(string sPath)
        => _files.ContainsKey(sPath) || _directories.Contains(sPath);

    public IFileInfo GetFileInfo(string sPath)
        => _fileInfos.TryGetValue(sPath, out var fileInfo)
            ? fileInfo
            : new FakeFileInfo(sPath, Exists(sPath), _files.TryGetValue(sPath, out var bytes) ? bytes.LongLength : 0);

    public System.IO.Stream OpenRead(string sPath)
        => new System.IO.MemoryStream(ReadAllBytes(sPath), writable: false);

    public System.IO.Stream OpenWrite(string sPath)
        => throw new NotSupportedException();

    public System.IO.Stream Create(string sPath)
        => throw new NotSupportedException();

    public string ReadAllText(string sPath)
        => ReadAllText(sPath, Encoding.UTF8);

    public string ReadAllText(string sPath, Encoding encoding)
        => encoding.GetString(ReadAllBytes(sPath));

    public void WriteAllText(string sPath, string sContents)
        => WriteAllText(sPath, sContents, Encoding.UTF8);

    public void WriteAllText(string sPath, string sContents, Encoding encoding)
    {
        var rgBytes = encoding.GetBytes(sContents);
        _files[sPath] = rgBytes;
        _fileInfos[sPath] = new FakeFileInfo(sPath, true, rgBytes.LongLength);
    }

    public byte[] ReadAllBytes(string sPath)
        => _files.TryGetValue(sPath, out var bytes) ? bytes : throw new FileNotFoundException(sPath);

    public void WriteAllBytes(string sPath, byte[] rgBytes)
    {
        _files[sPath] = rgBytes;
        _fileInfos[sPath] = new FakeFileInfo(sPath, true, rgBytes.LongLength);
    }

    public void Delete(string sPath)
    {
        _files.Remove(sPath);
        _directories.Remove(sPath);
        _fileInfos.Remove(sPath);
    }

    public void Copy(string sSourceFileName, string sDestFileName, bool xOverwrite)
    {
        if (!xOverwrite && _files.ContainsKey(sDestFileName))
        {
            throw new IOException("Destination exists.");
        }

        _files[sDestFileName] = ReadAllBytes(sSourceFileName).ToArray();
        if (_fileInfos.TryGetValue(sSourceFileName, out var sourceInfo))
        {
            _fileInfos[sDestFileName] = sourceInfo.WithPath(sDestFileName);
        }
    }

    public void Move(string sSourceFileName, string sDestFileName)
    {
        _files[sDestFileName] = ReadAllBytes(sSourceFileName).ToArray();
        _files.Remove(sSourceFileName);
        if (_fileInfos.TryGetValue(sSourceFileName, out var sourceInfo))
        {
            _fileInfos[sDestFileName] = sourceInfo.WithPath(sDestFileName);
            _fileInfos.Remove(sSourceFileName);
        }
    }

    public void CreateDirectory(string sPath)
        => _directories.Add(sPath);

    public string GetFullPath(string sPath)
        => sPath.StartsWith("ROOT:", StringComparison.OrdinalIgnoreCase) ? sPath : "ROOT:" + sPath;

    public string? GetDirectoryName(string sPath)
    {
        var normalized = sPath.Replace('/', '\\');
        var index = normalized.LastIndexOf('\\');
        return index < 0 ? null : normalized[..index];
    }

    public string GetTempFileName()
    {
        _tempFileCounter++;
        var path = $"ROOT_temp{_tempFileCounter}.tmp";
        _files[path] = [];
        _fileInfos[path] = new FakeFileInfo(path, true, 0);
        return path;
    }

    public System.IO.StreamWriter CreateStreamWriter(string sPath)
        => new(CreateWriteStream(sPath), new UTF8Encoding(false),bufferSize: -1, leaveOpen: false);

    private System.IO.Stream CreateWriteStream(string sPath)
        => new FakeWriteBackStream(this, sPath);

    private sealed class FakeWriteBackStream : System.IO.MemoryStream
    {
        private readonly FakeFileSystem _fileSystem;
        private readonly string _path;

        public FakeWriteBackStream(FakeFileSystem fileSystem, string path)
        {
            _fileSystem = fileSystem;
            _path = path;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                var rgBytes = ToArray();
                _fileSystem._files[_path] = rgBytes;
                _fileSystem._fileInfos[_path] = new FakeFileInfo(_path, true, rgBytes.LongLength);
            }

            base.Dispose(disposing);
        }
    }

    private sealed class FakeFileInfo : IFileInfo
    {
        public FakeFileInfo(string sPath, bool xExists, long fLength)
        {
            FullName = sPath;
            DirectoryName = Path.GetDirectoryName(sPath);
            Name = Path.GetFileName(sPath);
            Extension = Path.GetExtension(sPath);
            Exists = xExists;
            Length = fLength;
        }

        public string FullName { get; }

        public string? DirectoryName { get; }

        public string Name { get; }

        public string Extension { get; }

        public long Length { get; }

        public bool Exists { get; }

        public FakeFileInfo WithPath(string sPath)
            => new(sPath, Exists, Length);
    }
}
