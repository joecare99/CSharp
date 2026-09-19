/// <summary>
/// Abstract contract for output format providers.
/// Used to validate whether a specific document format is supported by the runtime.
/// </summary>
namespace OFBCreator.Abstractions.Interfaces;

public interface IUserDocumentFormatProvider
{
    /// <summary>
    /// Returns true if this provider supports creating documents in the requested output format.
    /// The format name should be compared case-insensitively (e.g., "docx", "odt").
    /// </summary>
    /// <param name="format">The output format identifier (e.g., "docx", "odt").</param>
    /// <returns>True if the format is supported; otherwise, false.</returns>
    bool IsFormatSupported( string format );
}
