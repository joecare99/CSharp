using System.Text;
using Document.Base.Models.Interfaces;
using Document.Markdown.Model;

namespace Document.Markdown.Rendering;

public static class MarkdownRenderer
{
    public static string Render(MarkdownSection root)
    {
        StringBuilder sb = new();
        foreach (IDOMElement element in root.Nodes)
        {
            WriteElement(sb, element);
        }

        return sb.ToString();
    }

    private static void WriteElement(StringBuilder sb, IDOMElement element)
    {
        switch (element)
        {
            case MarkdownParagraph paragraph:
                WriteInlineContent(sb, paragraph);
                sb.AppendLine();
                break;
            case MarkdownHeadline headline:
                sb.Append(new string('#', headline.Level));
                sb.Append(' ');
                WriteInlineContent(sb, headline);
                sb.AppendLine();
                break;
            case MarkdownTOC toc:
                if (!string.IsNullOrWhiteSpace(toc.Name))
                {
                    sb.AppendLine($"## {EscapeMarkdown(toc.Name)}");
                    sb.AppendLine();
                }

                foreach (IDOMElement child in toc.Nodes)
                {
                    if (child is MarkdownParagraph paragraph)
                    {
                        sb.Append("- ");
                        WriteInlineContent(sb, paragraph);
                        sb.AppendLine();
                        continue;
                    }

                    WriteElement(sb, child);
                }
                break;
            case MarkdownSection section:
                foreach (IDOMElement child in section.Nodes)
                {
                    WriteElement(sb, child);
                }
                break;
            default:
                if (element is IDocContent content)
                {
                    sb.AppendLine(EscapeMarkdown(content.GetTextContent(true)));
                    sb.AppendLine();
                }
                break;
        }
    }

    private static void WriteInlineContent(StringBuilder sb, IDocContent content)
    {
        if (!string.IsNullOrEmpty(content.TextContent))
        {
            sb.Append(EscapeMarkdown(content.TextContent));
        }

        foreach (IDOMElement child in content.Nodes)
        {
            switch (child)
            {
                case MarkdownSpan span:
                    WriteSpan(sb, span);
                    break;
                case MarkdownLineBreak:
                    sb.Append("  ").AppendLine();
                    break;
                case MarkdownNbSpace:
                    sb.Append("&nbsp;");
                    break;
                case MarkdownTab:
                    sb.Append("    ");
                    break;
                case IDocContent childContent:
                    sb.Append(EscapeMarkdown(childContent.GetTextContent(true)));
                    break;
            }
        }
    }

    private static void WriteSpan(StringBuilder sb, MarkdownSpan span)
    {
        string inner = RenderInlineText(span);

        if (span.IsLink || !string.IsNullOrWhiteSpace(span.Href))
        {
            sb.Append('[').Append(inner).Append("](").Append(EscapeLinkDestination(span.Href ?? string.Empty)).Append(')');
            return;
        }

        if (span.FontStyle.Bold && span.FontStyle.Italic)
        {
            sb.Append("***").Append(inner).Append("***");
            return;
        }

        if (span.FontStyle.Bold)
        {
            sb.Append("**").Append(inner).Append("**");
            return;
        }

        if (span.FontStyle.Italic)
        {
            sb.Append('*').Append(inner).Append('*');
            return;
        }

        if (span.FontStyle.Underline)
        {
            sb.Append("<u>").Append(inner).Append("</u>");
            return;
        }

        if (span.FontStyle.Strikeout)
        {
            sb.Append("~~").Append(inner).Append("~~");
            return;
        }

        sb.Append(inner);
    }

    private static string RenderInlineText(IDocContent content)
    {
        StringBuilder sb = new();
        WriteInlineContent(sb, content);
        return sb.ToString();
    }

    private static string EscapeMarkdown(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("*", "\\*", StringComparison.Ordinal)
            .Replace("_", "\\_", StringComparison.Ordinal)
            .Replace("[", "\\[", StringComparison.Ordinal)
            .Replace("]", "\\]", StringComparison.Ordinal)
            .Replace("#", "\\#", StringComparison.Ordinal)
            .Replace("`", "\\`", StringComparison.Ordinal);
    }

    private static string EscapeLinkDestination(string value)
        => value.Replace(")", "%29", StringComparison.Ordinal);
}
