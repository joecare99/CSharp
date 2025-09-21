using System.Xml.Linq;
using Document.Base.Models.Interfaces;
using Document.Odf.Models;

namespace Document.Odf.Parsing;

internal static class OdfBlockParser
{
    public static void Populate(IUserDocument doc, XDocument contentXml, IDocFontStyle? defaultStyle = null)
    {
        defaultStyle ??= OdfDefaultFontStyle.Instance;

        var officeText = contentXml.Root?
            .Element(OdfNamespaces.Office + "body")?
            .Element(OdfNamespaces.Office + "text");
        if (officeText is null) return;

        foreach (var el in officeText.Elements())
        {
            if (el.Name == OdfNamespaces.Text + "p")
            {
                var p = doc.AddParagraph(string.Empty);
                AppendInlines(el, p, defaultStyle);
            }
            else if (el.Name == OdfNamespaces.Text + "h")
            {
                int level = 1;
                var lvlAttr = (string?)el.Attribute(OdfNamespaces.Text + "outline-level");
                if (!int.TryParse(lvlAttr, out level)) level = 1;

                var id = (string?)el.Attribute("id")
                         ?? (string?)el.Attribute(OdfNamespaces.Text + "name");

                var h = doc.AddHeadline(level, id);
                AppendInlines(el, h, defaultStyle);
            }
            // TODO: lists (text:list), tables (table:table), images (draw:frame/draw:image)
        }
    }

    private static void AppendInlines(XElement src, IDocContent target, IDocFontStyle defaultStyle)
    {
        foreach (var node in src.Nodes())
        {
            switch (node)
            {
                case XText t:
                    if (!string.IsNullOrEmpty(t.Value))
                        target.AppendText(t.Value);
                    break;

                case XElement el:
                    if (el.Name == OdfNamespaces.Text + "span")
                    {
                        var span = target.AddSpan(defaultStyle);
                        AppendInlines(el, span, defaultStyle);
                    }
                    else if (el.Name == OdfNamespaces.Text + "s") // non-breaking spaces
                    {
                        var countAttr = (string?)el.Attribute("c");
                        var count = 1;
                        _ = int.TryParse(countAttr, out count);
                        for (int i = 0; i < Math.Max(1, count); i++)
                            target.AddNBSpace(defaultStyle);
                    }
                    else if (el.Name == OdfNamespaces.Text + "tab")
                    {
                        target.AddTab(defaultStyle);
                    }
                    else if (el.Name == OdfNamespaces.Text + "line-break")
                    {
                        target.AddLineBreak();
                    }
                    else if (el.Name == OdfNamespaces.Text + "a")
                    {
                        var href = (string?)el.Attribute(OdfNamespaces.XLink + "href") ?? string.Empty;
                        var linkSpan = target.AddLink(href, defaultStyle);
                        AppendInlines(el, linkSpan, defaultStyle);
                    }
                    else
                    {
                        // fallback recursion
                        AppendInlines(el, target, defaultStyle);
                    }
                    break;
            }
        }
    }
}