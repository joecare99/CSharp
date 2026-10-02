using Document.Base.Models.Interfaces;
using OFBCreator.Core.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Factory that creates IUserDocument instances.
/// Delegates document creation to the registered Document.Base providers.
/// </summary>
public class OFBDocumentFactory : IUserDocumentFactory
{
    private readonly UserDocumentFactoryImpl _factory = new();

    /// <inheritdoc />
    public IUserDocument CreateDocument(OFBOutputFormat format)
    {
        return _factory.CreateDocument(format);
    }
}
