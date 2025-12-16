using System.Xml.Linq;
using Document.Base.Models.Interfaces;
using Document.Odf.Models;

namespace Document.Odf.Writing;

/// <summary>
/// Generiert ODF-XML-Dokumente aus dem Dokumentmodell.
/// </summary>
public static class OdfXmlWriter
{
    /// <summary>
    /// Erstellt ein vollständiges content.xml für ein ODF-Textdokument.
    /// </summary>
    public static XDocument CreateContentXml(OdfSection root)
    {
        var officeText = new XElement(OdfNamespaces.Office + "text");
        
        foreach (var node in root.Nodes)
        {
            var el = ConvertNode(node);
            if (el != null)
                officeText.Add(el);
        }

        var doc = new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement(OdfNamespaces.Office + "document-content",
                new XAttribute(XNamespace.Xmlns + "office", OdfNamespaces.Office),
                new XAttribute(XNamespace.Xmlns + "text", OdfNamespaces.Text),
                new XAttribute(XNamespace.Xmlns + "style", OdfNamespaces.Style),
                new XAttribute(XNamespace.Xmlns + "fo", OdfNamespaces.Fo),
                new XAttribute(XNamespace.Xmlns + "xlink", OdfNamespaces.XLink),
                new XAttribute(XNamespace.Xmlns + "draw", OdfNamespaces.Draw),
                new XAttribute(XNamespace.Xmlns + "table", OdfNamespaces.Table),
                new XAttribute(XNamespace.Xmlns + "svg", OdfNamespaces.Svg),
                new XAttribute(OdfNamespaces.Office + "version", "1.3"),
                CreateAutomaticStyles(root),
                new XElement(OdfNamespaces.Office + "body", officeText)
            )
        );

        return doc;
    }

    /// <summary>
    /// Erstellt ein minimales styles.xml.
    /// </summary>
    public static XDocument CreateStylesXml()
    {
        return new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement(OdfNamespaces.Office + "document-styles",
                new XAttribute(XNamespace.Xmlns + "office", OdfNamespaces.Office),
                new XAttribute(XNamespace.Xmlns + "style", OdfNamespaces.Style),
                new XAttribute(XNamespace.Xmlns + "fo", OdfNamespaces.Fo),
                new XAttribute(XNamespace.Xmlns + "text", OdfNamespaces.Text),
                new XAttribute(OdfNamespaces.Office + "version", "1.3"),
                new XElement(OdfNamespaces.Office + "styles",
                    CreateDefaultParagraphStyle(),
                    CreateHeadingStyles()
                )
            )
        );
    }

    /// <summary>
    /// Erstellt ein minimales meta.xml.
    /// </summary>
    public static XDocument CreateMetaXml(string? title = null, string? creator = null)
    {
        var meta = new XElement(OdfNamespaces.Office + "meta");
        
        if (!string.IsNullOrEmpty(title))
            meta.Add(new XElement(OdfNamespaces.Dc + "title", title));
        
        if (!string.IsNullOrEmpty(creator))
            meta.Add(new XElement(OdfNamespaces.Dc + "creator", creator));
        
        meta.Add(new XElement(OdfNamespaces.Meta + "creation-date", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")));
        meta.Add(new XElement(OdfNamespaces.Meta + "generator", "Document.Odf"));

        return new XDocument(
            new XDeclaration("1.0", "UTF-8", null),
            new XElement(OdfNamespaces.Office + "document-meta",
                new XAttribute(XNamespace.Xmlns + "office", OdfNamespaces.Office),
                new XAttribute(XNamespace.Xmlns + "dc", OdfNamespaces.Dc),
                new XAttribute(XNamespace.Xmlns + "meta", OdfNamespaces.Meta),
                new XAttribute(OdfNamespaces.Office + "version", "1.3"),
                meta
            )
        );
    }

    private static XElement CreateAutomaticStyles(OdfSection root)
    {
        var autoStyles = new XElement(OdfNamespaces.Office + "automatic-styles");
        var usedStyles = new HashSet<string>();

        CollectUsedStyles(root, usedStyles);

        foreach (var styleName in usedStyles)
        {
            var style = CreateAutomaticStyle(styleName);
            if (style != null)
                autoStyles.Add(style);
        }

        return autoStyles;
    }

    private static void CollectUsedStyles(IDOMElement element, HashSet<string> styles)
    {
        if (element is OdfSpan span && span.FontStyle != null)
        {
            var styleName = GetAutoStyleName(span.FontStyle);
            if (!string.IsNullOrEmpty(styleName))
                styles.Add(styleName);
        }

        foreach (var child in element.Nodes)
        {
            if (child is IDOMElement childEl)
                CollectUsedStyles(childEl, styles);
        }
    }

    private static XElement? CreateAutomaticStyle(string styleName)
    {
        var textProps = new XElement(OdfNamespaces.Style + "text-properties");
        bool hasProps = false;

        if (styleName.Contains("Bold"))
        {
            textProps.Add(new XAttribute(OdfNamespaces.Fo + "font-weight", "bold"));
            hasProps = true;
        }
        if (styleName.Contains("Italic"))
        {
            textProps.Add(new XAttribute(OdfNamespaces.Fo + "font-style", "italic"));
            hasProps = true;
        }
        if (styleName.Contains("Underline"))
        {
            textProps.Add(new XAttribute(OdfNamespaces.Style + "text-underline-style", "solid"));
            textProps.Add(new XAttribute(OdfNamespaces.Style + "text-underline-width", "auto"));
            hasProps = true;
        }
        if (styleName.Contains("Strikeout"))
        {
            textProps.Add(new XAttribute(OdfNamespaces.Style + "text-line-through-style", "solid"));
            hasProps = true;
        }

        if (!hasProps) return null;

        return new XElement(OdfNamespaces.Style + "style",
            new XAttribute(OdfNamespaces.Style + "name", styleName),
            new XAttribute(OdfNamespaces.Style + "family", "text"),
            textProps
        );
    }

    private static string? GetAutoStyleName(IDocFontStyle fontStyle)
    {
        var parts = new List<string>();
        if (fontStyle.Bold) parts.Add("Bold");
        if (fontStyle.Italic) parts.Add("Italic");
        if (fontStyle.Underline) parts.Add("Underline");
        if (fontStyle.Strikeout) parts.Add("Strikeout");
        
        return parts.Count > 0 ? "T_" + string.Join("_", parts) : null;
    }

    private static XElement CreateDefaultParagraphStyle()
    {
        return new XElement(OdfNamespaces.Style + "default-style",
            new XAttribute(OdfNamespaces.Style + "family", "paragraph"),
            new XElement(OdfNamespaces.Style + "paragraph-properties",
                new XAttribute(OdfNamespaces.Fo + "margin-top", "0cm"),
                new XAttribute(OdfNamespaces.Fo + "margin-bottom", "0.212cm")
            ),
            new XElement(OdfNamespaces.Style + "text-properties",
                new XAttribute(OdfNamespaces.Style + "font-name", "Liberation Serif"),
                new XAttribute(OdfNamespaces.Fo + "font-size", "12pt")
            )
        );
    }

    private static IEnumerable<XElement> CreateHeadingStyles()
    {
        for (int level = 1; level <= 6; level++)
        {
            var fontSize = level switch
            {
                1 => "24pt",
                2 => "18pt",
                3 => "14pt",
                4 => "12pt",
                5 => "10pt",
                _ => "10pt"
            };

            yield return new XElement(OdfNamespaces.Style + "style",
                new XAttribute(OdfNamespaces.Style + "name", $"Heading_{level}"),
                new XAttribute(OdfNamespaces.Style + "family", "paragraph"),
                new XAttribute(OdfNamespaces.Style + "parent-style-name", "Heading"),
                new XAttribute(OdfNamespaces.Style + "default-outline-level", level.ToString()),
                new XElement(OdfNamespaces.Style + "text-properties",
                    new XAttribute(OdfNamespaces.Fo + "font-size", fontSize),
                    new XAttribute(OdfNamespaces.Fo + "font-weight", "bold")
                )
            );
        }
    }

    private static XElement? ConvertNode(IDOMElement node)
    {
        return node switch
        {
            OdfParagraph p => ConvertParagraph(p),
            OdfHeadline h => ConvertHeadline(h),
            OdfTOC toc => ConvertTOC(toc),
            _ => null
        };
    }

    private static XElement ConvertParagraph(OdfParagraph p)
    {
        var el = new XElement(OdfNamespaces.Text + "p");
        
        if (!string.IsNullOrEmpty(p.StyleName))
            el.Add(new XAttribute(OdfNamespaces.Text + "style-name", p.StyleName));

        AddInlineContent(el, p);
        return el;
    }

    private static XElement ConvertHeadline(OdfHeadline h)
    {
        var el = new XElement(OdfNamespaces.Text + "h",
            new XAttribute(OdfNamespaces.Text + "outline-level", h.Level.ToString()),
            new XAttribute(OdfNamespaces.Text + "style-name", $"Heading_{h.Level}")
        );

        if (!string.IsNullOrEmpty(h.Id))
            el.Add(new XAttribute(OdfNamespaces.Text + "name", h.Id));

        AddInlineContent(el, h);
        return el;
    }

    private static XElement ConvertTOC(OdfTOC toc)
    {
        var tocSource = new XElement(OdfNamespaces.Text + "table-of-content-source",
            new XAttribute(OdfNamespaces.Text + "outline-level", toc.Level.ToString())
        );

        var tocBody = new XElement(OdfNamespaces.Text + "index-body");
        
        foreach (var child in toc.Nodes)
        {
            if (child is OdfParagraph p)
            {
                tocBody.Add(ConvertParagraph(p));
            }
        }

        return new XElement(OdfNamespaces.Text + "table-of-content",
            new XAttribute(OdfNamespaces.Text + "name", toc.Name),
            tocSource,
            tocBody
        );
    }

    private static void AddInlineContent(XElement parent, OdfContentBase content)
    {
        if (!string.IsNullOrEmpty(content.TextContent))
            parent.Add(new XText(content.TextContent));

        foreach (var child in content.Nodes)
        {
            var el = ConvertInlineNode(child);
            if (el != null)
                parent.Add(el);
        }
    }

    private static object? ConvertInlineNode(IDOMElement node)
    {
        return node switch
        {
            OdfSpan span => ConvertSpan(span),
            OdfLineBreak => new XElement(OdfNamespaces.Text + "line-break"),
            OdfNbSpace => new XElement(OdfNamespaces.Text + "s"),
            OdfTab => new XElement(OdfNamespaces.Text + "tab"),
            _ => null
        };
    }

    private static XElement ConvertSpan(OdfSpan span)
    {
        XElement el;

        if (span.IsLink && !string.IsNullOrEmpty(span.Href))
        {
            el = new XElement(OdfNamespaces.Text + "a",
                new XAttribute(OdfNamespaces.XLink + "href", span.Href),
                new XAttribute(OdfNamespaces.XLink + "type", "simple")
            );
        }
        else
        {
            el = new XElement(OdfNamespaces.Text + "span");
            
            var styleName = GetAutoStyleName(span.FontStyle);
            if (!string.IsNullOrEmpty(styleName))
                el.Add(new XAttribute(OdfNamespaces.Text + "style-name", styleName));
        }

        if (!string.IsNullOrEmpty(span.Id))
        {
            el.AddFirst(new XElement(OdfNamespaces.Text + "bookmark-start",
                new XAttribute(OdfNamespaces.Text + "name", span.Id)));
        }

        if (!string.IsNullOrEmpty(span.TextContent))
            el.Add(new XText(span.TextContent));

        foreach (var child in span.Nodes)
        {
            var childEl = ConvertInlineNode(child);
            if (childEl != null)
                el.Add(childEl);
        }

        if (!string.IsNullOrEmpty(span.Id))
        {
            el.Add(new XElement(OdfNamespaces.Text + "bookmark-end",
                new XAttribute(OdfNamespaces.Text + "name", span.Id)));
        }

        return el;
    }
}
