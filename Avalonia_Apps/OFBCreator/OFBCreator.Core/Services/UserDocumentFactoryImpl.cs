using Document.Base.Factories;
using Document.Base.Models.Interfaces;
using OFBCreator.Core.Models;

namespace OFBCreator.Core.Services;

/// <summary>
/// Factory implementation for creating OFB (Ortsfamilienbuch) documents via Document.Base.
/// </summary>
public sealed class UserDocumentFactoryImpl : IUserDocumentFactory
{
    /// <inheritdoc />
    public IUserDocument CreateDocument( OFBOutputFormat format )
    {
        return format switch
        {
            OFBOutputFormat.Docx => UserDocumentFactory.Create( ".docx" ),
            OFBOutputFormat.Odt  => UserDocumentFactory.Create( ".odt" ),
            _ => throw new ArgumentException( $"Unknown output format: {format}", nameof( format ) )
        };
    }
}
