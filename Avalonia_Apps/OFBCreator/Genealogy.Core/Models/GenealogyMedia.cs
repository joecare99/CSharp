namespace Genealogy.Models;

/// <summary>
/// A media record describing a linked image, document, or other artifact.
/// </summary>
public sealed class GenealogyMedia : GenealogyRecord
{
    public GenealogyMedia()
    {
        Kind = "Media";
        TypeCode = "OBJE";
    }
}
