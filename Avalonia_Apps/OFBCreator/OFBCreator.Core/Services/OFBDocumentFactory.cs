using Document.Base.Models.Interfaces;
using OFBCreator.Core.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Factory that creates IUserDocument instances.
/// TODO: Implement actual document creation when CSharpBible.DocumentUtils references are resolvable.
/// Currently returns stub documents for Odt and Docx formats.
/// </summary>
public class OFBDocumentFactory : IUserDocumentFactory
{
    /// <inheritdoc />
    public IUserDocument CreateDocument(OFBOutputFormat format)
    {
        // TODO: Implement with CSharpBible.DocumentUtils when references are resolvable
        throw new System.NotImplementedException($"Document creation for format '{format}' requires CSharpBible.DocumentUtils project references which could not be resolved in this environment.");
    }
}
