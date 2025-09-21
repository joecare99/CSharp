using System.Text;
using Document.Base.Models.Interfaces;
using Document.Xaml.Model;

namespace Document.Xaml.Serialization;

public static class XamlDocumentSerializer
{
    private const string NsPresentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
    private const string NsX = "http://schemas.microsoft.com/winfx/2006/xaml";

    public static string ToXamlString(XamlSection root)
    {
        var sb = new StringBuilder();
        sb.Append("<FlowDocument xmlns=\"").Append(NsPresentation).Append("\" xmlns:x=\"").Append(NsX).AppendLine("\" PagePadding=\"20\">");

        foreach (var child in root.Nodes)
        {
            WriteElement(sb, child);
        }

        sb.AppendLine("</FlowDocument>");
        return sb.ToString();
    }

    private static void WriteElement(StringBuilder sb, IDOMElement el)
    {
        switch (el)
        {
            case XamlParagraph p:
                WriteParagraph(sb, p);
                break;
            case XamlHeadline h:
                WriteHeadline(sb, h);
                break;
            case XamlTOC toc:
                WriteTOC(sb, toc);
                break;
            case XamlSpan s:
                WriteSpanInline(sb, s);
                break;
            case XamlLineBreak:
                sb.AppendLine("<LineBreak />");
                break;
            case XamlNbSpace:
                sb.Append("<Run xml:space=\"preserve\">").Append("&#160;").AppendLine("</Run>");
                break;
            case XamlTab:
                sb.Append("<Run xml:space=\"preserve\">").Append("\t").AppendLine("</Run>");
                break;
            case XamlSection sec:
                foreach (var n in sec.Nodes) WriteElement(sb, n);
                break;
        }
    }

    private static void WriteParagraph(StringBuilder sb, XamlContentBase p, string? extraAttributes = null)
    {
        sb.Append("<Paragraph xml:space=\"preserve\"");
        if (!string.IsNullOrEmpty(extraAttributes))
            sb.Append(' ').Append(extraAttributes);
        sb.Append('>');

        WriteContent(sb, p);

        sb.AppendLine("</Paragraph>");
    }

    private static void WriteParagraph(StringBuilder sb, XamlParagraph p)
        => WriteParagraph(sb, p, null);

    private static void WriteHeadline(StringBuilder sb, XamlHeadline h)
    {
        // Einfache Headline-Darstellung als Paragraph mit Attributen
        var attrs = new StringBuilder();
        attrs.Append("Tag=\"H").Append(h.Level).Append('"');
        // leichte Formatierung
        attrs.Append(" FontWeight=\"Bold\"");
        attrs.Append(" FontSize=\"").Append(28 - (h.Level - 1) * 3).Append('"');

        WriteParagraph(sb, h, attrs.ToString());
    }

    private static void WriteTOC(StringBuilder sb, XamlTOC toc)
    {
        // Ganz simpel: jede TOC-Zeile als Paragraph mit Hyperlink
        foreach (var line in toc.Nodes.OfType<XamlParagraph>())
        {
            WriteParagraph(sb, line);
        }
    }

    private static void WriteContent(StringBuilder sb, XamlContentBase node)
    {
        // eigener Text
        if (!string.IsNullOrEmpty(node.TextContent))
        {
            sb.Append("<Run>");
            AppendEscaped(sb, node.TextContent);
            sb.Append("</Run>");
        }

        foreach (var c in node.Nodes)
        {
            switch (c)
            {
                case XamlSpan s:
                    WriteSpanInline(sb, s);
                    break;
                case XamlLineBreak:
                    sb.Append("<LineBreak />");
                    break;
                case XamlNbSpace:
                    sb.Append("<Run xml:space=\"preserve\">&#160;</Run>");
                    break;
                case XamlTab:
                    sb.Append("<Run xml:space=\"preserve\">\t</Run>");
                    break;
                case XamlContentBase nested:
                    // falls verschachtelte Inhalte vorkommen
                    WriteContent(sb, nested);
                    break;
            }
        }
    }

    private static void WriteSpanInline(StringBuilder sb, XamlSpan s)
    {
        // ggf. Anker (x:Name) erzeugen, wenn Id vorhanden ist
        var hasAnchor = !string.IsNullOrEmpty(s.Id);
        var text = s.TextContent ?? string.Empty;
        var runOpenWritten = false;

        void OpenStyledRun()
        {
            sb.Append("<Run");
            var attrs = BuildRunAttributes(s);
            if (!string.IsNullOrEmpty(attrs)) { sb.Append(' ').Append(attrs); }
            sb.Append('>');
            runOpenWritten = true;
        }

        if (hasAnchor)
        {
            sb.Append("<Span x:Name=\"").Append(EscapeAttribute(s.Id!)).Append("\">");
        }

        if (s.IsLink || !string.IsNullOrEmpty(s.Href))
        {
            sb.Append("<Hyperlink");
            if (!string.IsNullOrEmpty(s.Href))
            {
                sb.Append(" NavigateUri=\"").Append(EscapeAttribute(s.Href!)).Append('"');
            }
            sb.Append('>');

            OpenStyledRun();
            AppendEscaped(sb, text);
            sb.Append("</Run>");
            sb.Append("</Hyperlink>");
        }
        else
        {
            OpenStyledRun();
            AppendEscaped(sb, text);
            sb.Append("</Run>");
        }

        if (hasAnchor)
        {
            sb.Append("</Span>");
        }
    }

    private static string BuildRunAttributes(XamlSpan s)
    {
        var parts = new List<string>();
        if (s.FontStyle is { } fs)
        {
            if (!string.IsNullOrEmpty(fs.Color)) parts.Add($"Foreground=\"{EscapeAttribute(fs.Color!)}\"");
            if (!string.IsNullOrEmpty(fs.FontFamily)) parts.Add($"FontFamily=\"{EscapeAttribute(fs.FontFamily!)}\"");
            if (fs.FontSizePt is double sizePt)
            {
                // WPF nutzt Device Independent Pixels (1/96 inch). 1pt = 1/72 inch => px = pt * 96/72 = pt * 1.333...
                var dip = sizePt * 96.0 / 72.0;
                parts.Add($"FontSize=\"{dip:0.#}\"");
            }
            if (fs.Strikeout) parts.Add("TextDecorations=\"Strikethrough\"");
        }
        // Fett/Kursiv/Underline als Container-Tags sind möglich, aber hier per Run-Attribute einfacher:
        if (s.FontStyle.Bold) parts.Add("FontWeight=\"Bold\"");
        if (s.FontStyle.Italic) parts.Add("FontStyle=\"Italic\"");
        if (s.FontStyle.Underline) parts.Add("TextDecorations=\"Underline\"");
        return string.Join(' ', parts);
    }

    private static void AppendEscaped(StringBuilder sb, string text)
    {
        foreach (var ch in text)
        {
            sb.Append(ch switch
            {
                '&' => "&amp;",
                '<' => "&lt;",
                '>' => "&gt;",
                '"' => "&quot;",
                '\'' => "&apos;",
                _ => ch
            });
        }
    }

    private static string EscapeAttribute(string s)
    {
        var sb = new StringBuilder(s.Length + 8);
        AppendEscaped(sb, s);
        return sb.ToString();
    }
}