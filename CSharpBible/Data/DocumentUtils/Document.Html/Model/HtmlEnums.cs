namespace Document.Html.Model;

public enum HtmlElementType
{
    Section,
    Paragraph,
    Headline,   // use attribute "level" (1..6)
    TOC,        // use attribute "name" and "level"
    Span,
    Link,
    LineBreak,
    NbSpace,
    Tab,
    Bookmark
}

public static class HtmlAttributeKeys
{
    public const string Level = "level";
    public const string Name = "name";
    public const string Href = "href";
    public const string Id = "id";
    public const string StyleName = "styleName";
}
