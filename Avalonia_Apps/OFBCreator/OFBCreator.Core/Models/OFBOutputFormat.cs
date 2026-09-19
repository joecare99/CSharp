using GenInterfaces.Interfaces.Genealogic;

namespace OFBCreator.Core.Models;

/// <summary>
/// Defines supported output document formats for the OFB (Ortsfamilienbuch) export.
/// </summary>
public enum OFBOutputFormat
{
    /// <summary>
    /// Microsoft Word DOCX format (default, uses Xceed.Document.NET).
    /// </summary>
    Docx,

    /// <summary>
    /// OpenDocument Text (.odt) format.
    /// </summary>
    Odt
}
