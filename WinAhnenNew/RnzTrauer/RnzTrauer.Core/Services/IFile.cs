namespace RnzTrauer.Core;

/// <summary>
/// Provides an abstraction over <see cref="File"/> for testable file-system access.
/// </summary>
public interface IFile
{
    /// <summary>
    /// Determines whether the specified file exists.
    /// </summary>
    bool Exists(string sPath);

    /// <summary>
    /// Reads all text from the specified file.
    /// </summary>
    string ReadAllText(string sPath);

    /// <summary>
    /// Reads all bytes from the specified file.
    /// </summary>
    byte[] ReadAllBytes(string sPath);

    /// <summary>
    /// Writes all text to the specified file.
    /// </summary>
    void WriteAllText(string sPath, string sContents);

    /// <summary>
    /// Writes all bytes to the specified file.
    /// </summary>
    void WriteAllBytes(string sPath, byte[] arrBytes);
}
