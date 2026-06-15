using System.Text;
using XamlDecompiler.Core.Models;

namespace XamlDecompiler.Core.Services;

internal sealed class XamlDocumentWriter
{
    private static readonly HashSet<string> ContentPropertyNames = new(StringComparer.Ordinal)
    {
        "Content"
    };

    public string Write(GeneratedSourceModel model)
    {
        StringBuilder builder = new();
        WriteElement(builder, model.RootElement, 0, model, isRoot: true);
        return builder.ToString();
    }

    private static void WriteElement(StringBuilder builder, DecompiledElement element, int indentLevel, GeneratedSourceModel model, bool isRoot)
    {
        string indent = new(' ', indentLevel * 4);
        builder.Append(indent);
        builder.Append('<');
        builder.Append(element.TypeName);

        Dictionary<string, string> attributes = new(element.Attributes, StringComparer.Ordinal);
        if (!string.IsNullOrWhiteSpace(element.XamlName))
        {
            attributes["x:Name"] = element.XamlName;
        }

        if (!string.IsNullOrWhiteSpace(element.ResourceKey))
        {
            attributes["x:Key"] = element.ResourceKey;
        }

        if (isRoot)
        {
            builder.AppendLine();
            builder.Append(indent);
            builder.Append("    x:Class=\"");
            builder.Append(EscapeAttribute($"{model.Namespace}.{model.ClassName}"));
            builder.Append('"');

            foreach ((string prefix, string ns) in model.XmlNamespaces)
            {
                builder.AppendLine();
                builder.Append(indent);
                builder.Append("    xmlns");
                if (!string.IsNullOrEmpty(prefix))
                {
                    builder.Append(':');
                    builder.Append(prefix);
                }

                builder.Append("=\"");
                builder.Append(EscapeAttribute(ns));
                builder.Append('"');
            }
        }

        foreach ((string propertyName, string value) in attributes.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            builder.AppendLine();
            builder.Append(indent);
            builder.Append("    ");
            builder.Append(propertyName);
            builder.Append("=\"");
            builder.Append(EscapeAttribute(value));
            builder.Append('"');
        }

        foreach ((string eventName, string handlerName) in element.Events.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            builder.AppendLine();
            builder.Append(indent);
            builder.Append("    ");
            builder.Append(eventName);
            builder.Append("=\"");
            builder.Append(EscapeAttribute(handlerName));
            builder.Append('"');
        }

        bool hasInnerContent = element.Content is not null || !string.IsNullOrWhiteSpace(element.TextValue) || element.Children.Count > 0 || element.PropertyElements.Count > 0 || element.Comments.Count > 0;
        if (!hasInnerContent)
        {
            builder.AppendLine(" />");
            return;
        }

        builder.AppendLine(">");

        foreach (string comment in element.Comments)
        {
            builder.Append(new string(' ', (indentLevel + 1) * 4));
            builder.Append("<!-- ");
            builder.Append(comment.Replace("--", "- -", StringComparison.Ordinal));
            builder.AppendLine(" -->");
        }

        if (element.Content is not null)
        {
            WriteElement(builder, element.Content, indentLevel + 1, model, isRoot: false);
        }

        if (!string.IsNullOrWhiteSpace(element.TextValue))
        {
            builder.Append(new string(' ', (indentLevel + 1) * 4));
            builder.AppendLine(EscapeAttribute(element.TextValue));
        }

        foreach (DecompiledElement child in element.Children)
        {
            WriteElement(builder, child, indentLevel + 1, model, isRoot: false);
        }

        foreach ((string propertyName, IList<DecompiledElement> propertyElements) in element.PropertyElements.OrderBy(static pair => pair.Key, StringComparer.Ordinal))
        {
            if (ContentPropertyNames.Contains(propertyName))
            {
                continue;
            }

            string propertyIndent = new(' ', (indentLevel + 1) * 4);
            builder.Append(propertyIndent);
            builder.Append('<');
            builder.Append(element.TypeName);
            builder.Append('.');
            builder.Append(propertyName);
            builder.AppendLine(">");

            foreach (DecompiledElement child in propertyElements)
            {
                WriteElement(builder, child, indentLevel + 2, model, isRoot: false);
            }

            builder.Append(propertyIndent);
            builder.Append("</");
            builder.Append(element.TypeName);
            builder.Append('.');
            builder.Append(propertyName);
            builder.AppendLine(">");
        }

        builder.Append(indent);
        builder.Append("</");
        builder.Append(element.TypeName);
        builder.AppendLine(">");
    }

    private static string EscapeAttribute(string value)
    {
        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
    }
}
