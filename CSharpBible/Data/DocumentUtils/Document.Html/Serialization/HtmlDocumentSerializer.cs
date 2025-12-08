using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using Document.Base.Models.Interfaces;
using Document.Html.Model;

namespace Document.Html.Serialization;

public static class HtmlDocumentSerializer
{
    public static async Task<HtmlSection> FromHtmlStringAsync(string html)
    {
        var config = Configuration.Default.WithDefaultLoader();
        using var context = BrowsingContext.New(config);
        var doc = await context.OpenAsync(req => req.Content(html));

        var root = new HtmlSection();
        var body = doc.Body ?? doc.DocumentElement;

        if (body is null) return root;

        foreach (var child in body.Children)
        {
            MapElement(child, root);
        }

        return root;
    }

    private static void MapElement(IElement element, HtmlSection rootOrContainer)
    {
        switch (element.TagName.ToUpperInvariant())
        {
            case "P":
            {
                var p = rootOrContainer.AddParagraph(ATextStyleName: null!);
                MapInline(element, p);
                break;
            }
            case "H1":
            case "H2":
            case "H3":
            case "H4":
            case "H5":
            case "H6":
            {
                var level = int.Parse(element.TagName[1].ToString());
                var h = (HtmlHeadline)rootOrContainer.AddHeadline(level, element.Id);
                MapInline(element, h);
                break;
            }
            case "DIV":
            case "SECTION":
            {
                var sec = (HtmlSection)rootOrContainer.AppendDocElement(HtmlElementType.Section);
                foreach (var ch in element.Children)
                    MapElement(ch, sec);
                break;
            }
            case "UL":
            case "OL":
            {
                // Mappe Listeneinträge als einzelne Paragraphen
                foreach (var li in element.Children.Where(c => c.TagName.Equals("LI", StringComparison.OrdinalIgnoreCase)))
                {
                    var p = rootOrContainer.AddParagraph("List");
                    MapInline(li, p);
                }
                break;
            }
            default:
            {
                // Fallback: Alles als Paragraph
                var p = rootOrContainer.AddParagraph(null!);
                MapInline(element, p);
                break;
            }
        }
    }

    private static void MapInline(IElement element, IDocContent container)
    {
        foreach (var node in element.ChildNodes)
        {
            switch (node)
            {
                case IText text:
                    container.AppendText(text.Data);
                    break;
                case IElement el:
                {
                    var tag = el.TagName.ToUpperInvariant();
                    if (tag is "BR")
                    {
                        container.AddLineBreak();
                    }
                    else if (tag is "SPAN" or "EM" or "STRONG" or "B" or "I" or "U")
                    {
                        var span = (HtmlSpan)container.AddSpan(HtmlFontStyle.Default);
                        if (tag is "B" or "STRONG") span.Attributes["weight"] = "bold";
                        if (tag is "I" or "EM") span.Attributes["style"] = "italic";
                        if (tag is "U") span.Attributes["decoration"] = "underline";
                        MapInline(el, span);
                    }
                    else if (tag is "A")
                    {
                        var link = (HtmlSpan)container.AddLink(el.GetAttribute("href"), HtmlFontStyle.Default);
                        MapInline(el, link);
                    }
                    else if (tag is "NBSP")
                    {
                        container.AddNBSpace(HtmlFontStyle.Default);
                    }
                    else
                    {
                        // Unbekannte Inline-Elemente: Inline weiter abbilden
                        MapInline(el, container);
                    }
                    break;
                }
            }
        }
    }

    public static string ToHtmlString(HtmlSection root)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<!DOCTYPE html>");
        sb.AppendLine("<html>");
        sb.AppendLine("<head><meta charset=\"utf-8\" /></head>");
        sb.AppendLine("<body>");
        foreach (var child in (root as HtmlNodeBase).Nodes)
        {
            WriteElement(sb, child );
        }
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");
        return sb.ToString();
    }

    private static void WriteElement(StringBuilder sb, IDOMElement element)
    {
        switch (element)
        {
            case HtmlParagraph p:
                sb.Append("<p>");
                WriteInline(sb, p);
                sb.AppendLine("</p>");
                break;

            case HtmlHeadline h:
                var lvl = h.Level;
                sb.Append($"<h{lvl}>");
                WriteInline(sb, h);
                sb.AppendLine($"</h{lvl}>");
                break;

            case HtmlTOC toc:
                // Einfache Darstellung als ungeordnete Liste
                sb.AppendLine("<ul>");
                foreach (var entry in toc.Nodes.OfType<HtmlParagraph>())
                {
                    sb.Append("<li>");
                    WriteInline(sb, entry);
                    sb.AppendLine("</li>");
                }
                sb.AppendLine("</ul>");
                break;

            case HtmlSection sec:
                sb.AppendLine("<section>");
                foreach (var c in sec.Nodes)
                {
                    WriteElement(sb, c);
                }
                sb.AppendLine("</section>");
                break;
        }
    }

    private static void WriteInline(StringBuilder sb, HtmlContentBase content)
    {
        // Eigener Text
        var text = content.TextContent;
        if (!string.IsNullOrEmpty(text))
            sb.Append(System.Net.WebUtility.HtmlEncode(text));

        foreach (var c in content.Nodes)
        {
            switch (c)
            {
                case HtmlSpan span when span.IsLink:
                    var href = span.Href ?? "#";
                    sb.Append($"<a href=\"{System.Net.WebUtility.HtmlEncode(href)}\">");
                    WriteInline(sb, span);
                    sb.Append("</a>");
                    break;

                case HtmlSpan span:
                    sb.Append("<span");
                    if (!string.IsNullOrEmpty(span.Id))
                        sb.Append($" id=\"{System.Net.WebUtility.HtmlEncode(span.Id)}\"");
                    sb.Append(">");
                    if (span.FontStyle.Bold)
                        sb.Append("<strong>");
                    if (span.FontStyle.Italic)
                        sb.Append("<em>");
                    WriteInline(sb, span);
                    if (span.FontStyle.Italic)
                        sb.Append("</em>");
                    if (span.FontStyle.Bold)
                        sb.Append("</strong>");
                    sb.Append("</span>");
                    break;

                case HtmlLineBreak:
                    sb.Append("<br />");
                    break;

                case HtmlNbSpace:
                    sb.Append("&nbsp;");
                    break;

                case HtmlTab:
                    sb.Append("\t");
                    break;

                case HtmlContentBase inner:
                    WriteInline(sb, inner);
                    break;
            }
        }
    }
}
