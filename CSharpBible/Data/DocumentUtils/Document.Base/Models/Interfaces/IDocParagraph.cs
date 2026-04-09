namespace Document.Base.Models.Interfaces;

public interface IDocParagraph : IDocContent
{
    IDocSpan AddBookmark(string Id, IDocFontStyle docFontStyle);
}
