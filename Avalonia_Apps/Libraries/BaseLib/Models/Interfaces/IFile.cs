using System.IO;
using System.Text;

namespace BaseLib.Models.Interfaces;

/// <summary>
/// Provides an abstraction over <see cref="File"/> for testable file system access.
/// </summary>
public interface IFile
{
    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <returns><see langword="true"/> if the file exists; otherwise <see langword="false"/>.</returns>
    bool Exists(string sPath);

    /// <summary>
    /// Opens a file for reading.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <returns>A readable stream.</returns>
    Stream OpenRead(string sPath);

    /// <summary>
    /// Opens an existing file for writing.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <returns>A writable stream.</returns>
    Stream OpenWrite(string sPath);

    /// <summary>
    /// Creates or overwrites a file.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <returns>A writable stream.</returns>
    Stream Create(string sPath);

    /// <summary>
    /// Reads all text from a file using UTF-8 encoding.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <returns>The file content.</returns>
    string ReadAllText(string sPath);

    /// <summary>
    /// Reads all text from a file using the specified encoding.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <param name="encoding">The text encoding.</param>
    /// <returns>The file content.</returns>
    string ReadAllText(string sPath, Encoding encoding);

    /// <summary>
    /// Writes text to a file using UTF-8 encoding.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <param name="sContents">The text content.</param>
    void WriteAllText(string sPath, string sContents);

    /// <summary>
    /// Writes text to a file using the specified encoding.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <param name="sContents">The text content.</param>
    /// <param name="encoding">The text encoding.</param>
    void WriteAllText(string sPath, string sContents, Encoding encoding);

    /// <summary>
    /// Reads all bytes from a file.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <returns>The file content as bytes.</returns>
    byte[] ReadAllBytes(string sPath);

    /// <summary>
    /// Writes all bytes to a file.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    /// <param name="rgBytes">The byte content.</param>
    void WriteAllBytes(string sPath, byte[] rgBytes);

    /// <summary>
    /// Deletes the specified file.
    /// </summary>
    /// <param name="sPath">The file path.</param>
    void Delete(string sPath);

    /// <summary>
    /// Copies a file to a new location.
    /// </summary>
    /// <param name="sSourceFileName">The source file path.</param>
    /// <param name="sDestFileName">The destination file path.</param>
    /// <param name="xOverwrite"><see langword="true"/> to overwrite an existing destination file.</param>
    void Copy(string sSourceFileName, string sDestFileName, bool xOverwrite);

    /// <summary>
    /// Moves a file to a new location.
    /// </summary>
    /// <param name="sSourceFileName">The source file path.</param>
    /// <param name="sDestFileName">The destination file path.</param>
    void Move(string sSourceFileName, string sDestFileName);
}
