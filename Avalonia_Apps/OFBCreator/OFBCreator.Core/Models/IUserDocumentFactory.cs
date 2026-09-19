using Document.Base.Models.Interfaces;

namespace OFBCreator.Core.Models;

/// <summary>
/// Factory for creating IUserDocument instances in specific output formats.
/// </summary>
public interface IUserDocumentFactory
{
    /// <summary>
    /// Creates a new document instance for the specified OFB output format.
    /// </summary>
    IUserDocument CreateDocument( OFBOutputFormat format );
}
