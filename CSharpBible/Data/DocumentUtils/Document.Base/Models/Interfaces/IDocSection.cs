namespace Document.Base.Models.Interfaces;

/// <summary>
/// Interface IDocSection  
/// Extends the <see cref="Document.Base.Models.Interfaces.IDocElement" />
/// A section is a container for other document elements like paragraphs, headlines, tables of contents, etc.
/// </summary>
/// <seealso cref="Document.Base.Models.Interfaces.IDocElement" />
public interface IDocSection : IDocElement
{
    IDocParagraph AddParagraph(string ATextStyleName);
    IDocContent AddHeadline(int aLevel);
    IDocContent AddTOC(string aName, int aLevel);
}
