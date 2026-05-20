using System;

namespace BaseLib.Models.Interfaces;

/// <summary>
/// Provides an abstraction over <see cref="System.IO.FileInfo"/> for testable file metadata access.
/// </summary>
public interface IFileInfo
{
    /// <summary>
    /// Gets the full file path.
    /// </summary>
    string FullName { get; }

    /// <summary>
    /// Gets the directory path.
    /// </summary>
    string? DirectoryName { get; }

    /// <summary>
    /// Gets the file name including extension.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the file extension.
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// Gets the file length in bytes.
    /// </summary>
    long Length { get; }

    /// <summary>
    /// Gets whether the file exists.
    /// </summary>
    bool Exists { get; }
}