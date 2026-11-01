using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// Parses AXAML into a bounded safe preview tree without loading or instantiating user types.
/// </summary>
public sealed class AxamlPreviewService
{
    private const int MaximumDocumentCharacters = 1_000_000;
    private const int MaximumTreeDepth = 64;
    private const int MaximumControls = 2_000;
    private static readonly HashSet<string> s_supportedControls = new(StringComparer.Ordinal)
    {
        "Border",
        "Button",
        "CheckBox",
        "ComboBox",
        "ContentControl",
        "DockPanel",
        "Grid",
        "Label",
        "ListBox",
        "ProgressBar",
        "RadioButton",
        "ScrollViewer",
        "Separator",
        "Slider",
        "StackPanel",
        "TextBlock",
        "TextBox",
        "UserControl",
        "Window",
        "WrapPanel",
    };

    /// <summary>
    /// Parses an AXAML document and returns its preview tree.
    /// </summary>
    public AxamlPreviewNode Parse(string axaml)
    {
        ArgumentNullException.ThrowIfNull(axaml);
        if (axaml.Length > MaximumDocumentCharacters)
        {
            throw new InvalidDataException($"AXAML preview is limited to {MaximumDocumentCharacters} characters.");
        }

        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersInDocument = MaximumDocumentCharacters,
            MaxCharactersFromEntities = 0,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
        };
        using StringReader textReader = new(axaml);
        using XmlReader xmlReader = XmlReader.Create(textReader, settings);
        XDocument document = XDocument.Load(xmlReader, LoadOptions.SetLineInfo);
        XElement root = document.Root
            ?? throw new InvalidDataException("The AXAML document has no root element.");
        int controlCount = 0;
        return ParseElement(root, axaml, depth: 0, ref controlCount);
    }

    public bool CanEditText(AxamlPreviewNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        string? propertyName = GetEditablePropertyName(node.TypeName);
        return propertyName is not null
            && node.Children.Count == 0
            && !node.HasInlineText
            && CanEditProperty(node, propertyName);
    }

    public bool CanEditProperty(AxamlPreviewNode node, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        return node.IsSupported
            && GetEditableProperties(node).Contains(propertyName, StringComparer.Ordinal)
            && node.SourceStartOffset >= 0
            && (!node.Properties.TryGetValue(propertyName, out string? value)
                || !value.TrimStart().StartsWith('{'))
            && !(propertyName == GetEditablePropertyName(node.TypeName) && node.HasInlineText)
            && !node.PropertyElementNames.Contains($"{node.TypeName}.{propertyName}", StringComparer.Ordinal);
    }

    public IReadOnlyList<string> GetEditableProperties(AxamlPreviewNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        string[] properties = node.TypeName switch
        {
            "Border" => ["Background", "BorderThickness", "Height", "Margin", "Padding", "Width"],
            "Button" => ["Content", "Height", "HorizontalAlignment", "Margin", "VerticalAlignment", "Width"],
            "CheckBox" or "RadioButton" or "Label" => ["Content", "Height", "HorizontalAlignment", "Margin", "VerticalAlignment", "Width"],
            "Grid" => ["Height", "Margin", "Width"],
            "StackPanel" => ["Height", "Margin", "Orientation", "Spacing", "Width"],
            "WrapPanel" => ["Height", "Margin", "Orientation", "Width"],
            "TextBlock" => ["FontSize", "FontWeight", "Height", "HorizontalAlignment", "Margin", "Text", "VerticalAlignment", "Width"],
            "TextBox" => ["Height", "Margin", "PlaceholderText", "Text", "Width"],
            _ => [],
        };
        return properties;
    }

    public string UpdateTextAttribute(string axaml, AxamlPreviewNode node, string value)
    {
        ArgumentNullException.ThrowIfNull(axaml);
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(value);
        if (!CanEditText(node))
        {
            throw new InvalidOperationException($"The '{node.TypeName}' preview element cannot be edited.");
        }

        string propertyName = GetEditablePropertyName(node.TypeName)!;
        return UpdateAttribute(axaml, node, propertyName, value);
    }

    public string UpdateAttribute(string axaml, AxamlPreviewNode node, string propertyName, string value)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(axaml);
        ArgumentNullException.ThrowIfNull(value);
        if (!CanEditProperty(node, propertyName))
        {
            throw new InvalidOperationException($"The '{propertyName}' property on '{node.TypeName}' cannot be edited.");
        }

        int tagStart = node.SourceStartOffset;
        if (tagStart >= axaml.Length || axaml[tagStart] != '<')
        {
            throw new InvalidDataException("The AXAML source changed; select the preview element again.");
        }

        int tagEnd = FindStartTagEnd(axaml, tagStart);
        if (node.SourceStartTag is null
            || !axaml.AsSpan(tagStart, tagEnd - tagStart + 1).SequenceEqual(node.SourceStartTag.AsSpan()))
        {
            throw new InvalidDataException("The AXAML source changed; select the preview element again.");
        }

        int elementNameStart = tagStart + 1;
        int elementNameEnd = elementNameStart;
        while (elementNameEnd < tagEnd
            && !char.IsWhiteSpace(axaml[elementNameEnd])
            && axaml[elementNameEnd] is not '/' and not '>')
        {
            elementNameEnd++;
        }

        string sourceTypeName = axaml[elementNameStart..elementNameEnd];
        int prefixSeparator = sourceTypeName.LastIndexOf(':');
        if (prefixSeparator >= 0)
        {
            sourceTypeName = sourceTypeName[(prefixSeparator + 1)..];
        }

        if (!string.Equals(sourceTypeName, node.TypeName, StringComparison.Ordinal))
        {
            throw new InvalidDataException("The AXAML source changed; select the preview element again.");
        }

        int attributeStart = FindAttributeValueStart(axaml, tagStart, tagEnd, propertyName);
        string escapedValue = EscapeAttributeValue(value);
        if (attributeStart >= 0)
        {
            char quote = axaml[attributeStart - 1];
            int attributeEnd = axaml.IndexOf(quote, attributeStart);
            if (attributeEnd < 0 || attributeEnd > tagEnd)
            {
                throw new InvalidDataException("The AXAML text attribute is malformed.");
            }

            return string.Concat(axaml.AsSpan(0, attributeStart), escapedValue, axaml.AsSpan(attributeEnd));
        }

        int insertionOffset = tagEnd;
        while (insertionOffset > tagStart && char.IsWhiteSpace(axaml[insertionOffset - 1]))
        {
            insertionOffset--;
        }

        if (insertionOffset > tagStart && axaml[insertionOffset - 1] == '/')
        {
            insertionOffset--;
        }

        string separator = insertionOffset > tagStart && char.IsWhiteSpace(axaml[insertionOffset - 1])
            ? string.Empty
            : " ";
        return string.Concat(
            axaml.AsSpan(0, insertionOffset),
            $"{separator}{propertyName}=\"{escapedValue}\"",
            axaml.AsSpan(insertionOffset));
    }

    private static AxamlPreviewNode ParseElement(XElement element, string source, int depth, ref int controlCount)
    {
        if (depth > MaximumTreeDepth)
        {
            throw new InvalidDataException($"AXAML preview supports at most {MaximumTreeDepth} nested controls.");
        }

        if (++controlCount > MaximumControls)
        {
            throw new InvalidDataException($"AXAML preview supports at most {MaximumControls} controls.");
        }

        string typeName = element.Name.LocalName;
        Dictionary<string, string> properties = element.Attributes()
            .Where(static attribute => !attribute.IsNamespaceDeclaration && attribute.Name.NamespaceName.Length == 0)
            .Where(static attribute => !attribute.Name.LocalName.StartsWith("x:", StringComparison.Ordinal))
            .ToDictionary(
                static attribute => attribute.Name.LocalName,
                static attribute => attribute.Value,
                StringComparer.Ordinal);
        List<AxamlPreviewNode> children = [];
        List<string> rowDefinitions = GetDefinitionValues(properties, "RowDefinitions");
        List<string> columnDefinitions = GetDefinitionValues(properties, "ColumnDefinitions");
        string? textPropertyName = GetEditablePropertyName(typeName);
        bool hasTextPropertyElement = false;
        List<string> propertyElementNames = [];

        foreach (XElement child in element.Elements())
        {
            string childName = child.Name.LocalName;
            if (childName.StartsWith($"{typeName}.", StringComparison.Ordinal))
            {
                propertyElementNames.Add(childName);
            }

            if (textPropertyName is not null && childName == $"{typeName}.{textPropertyName}")
            {
                hasTextPropertyElement = true;
            }
            else if (childName == $"{typeName}.RowDefinitions")
            {
                rowDefinitions.AddRange(child.Elements().Select(static definition =>
                    (string?)definition.Attribute("Height") ?? "Auto"));
            }
            else if (childName == $"{typeName}.ColumnDefinitions")
            {
                columnDefinitions.AddRange(child.Elements().Select(static definition =>
                    (string?)definition.Attribute("Width") ?? "Auto"));
            }
            else if (childName.Contains('.', StringComparison.Ordinal))
            {
                foreach (XElement propertyChild in child.Elements())
                {
                    if (LooksLikeControl(propertyChild))
                    {
                        children.Add(ParseElement(propertyChild, source, depth + 1, ref controlCount));
                    }
                }
            }
            else if (LooksLikeControl(child))
            {
                children.Add(ParseElement(child, source, depth + 1, ref controlCount));
            }
        }

        if (IsContentRoot(typeName) && children.Count == 1)
        {
            return children[0];
        }

        string? text = GetDisplayText(typeName, properties, element);
        int sourceStartOffset = GetSourceStartOffset(element, source);
        return new AxamlPreviewNode
        {
            TypeName = typeName,
            Text = text,
            IsSupported = s_supportedControls.Contains(typeName),
            SourceStartOffset = sourceStartOffset,
            SourceStartTag = sourceStartOffset >= 0
                ? source[sourceStartOffset..(FindStartTagEnd(source, sourceStartOffset) + 1)]
                : null,
            HasTextPropertyElement = hasTextPropertyElement,
            HasInlineText = element.Nodes().OfType<XText>().Any(static value => !string.IsNullOrWhiteSpace(value.Value)),
            PropertyElementNames = propertyElementNames,
            Properties = properties,
            Children = children,
            RowDefinitions = rowDefinitions,
            ColumnDefinitions = columnDefinitions,
        };
    }

    private static string? GetEditablePropertyName(string typeName)
    {
        return typeName switch
        {
            "TextBlock" or "TextBox" => "Text",
            "Button" or "CheckBox" or "RadioButton" or "Label" => "Content",
            _ => null,
        };
    }

    private static int GetSourceStartOffset(XElement element, string source)
    {
        if (element is not IXmlLineInfo lineInfo || !lineInfo.HasLineInfo())
        {
            return -1;
        }

        int line = 1;
        int lineStart = 0;
        for (int index = 0; index < source.Length && line < lineInfo.LineNumber; index++)
        {
            if (source[index] == '\n')
            {
                line++;
                lineStart = index + 1;
            }
        }

        if (line != lineInfo.LineNumber)
        {
            return -1;
        }

        int reportedOffset = lineStart + lineInfo.LinePosition - 1;
        int tagStart = source.LastIndexOf('<', Math.Min(reportedOffset, source.Length - 1));
        return tagStart >= lineStart ? tagStart : -1;
    }

    private static int FindStartTagEnd(string source, int tagStart)
    {
        char quote = '\0';
        for (int index = tagStart + 1; index < source.Length; index++)
        {
            char current = source[index];
            if (quote != '\0')
            {
                if (current == quote)
                {
                    quote = '\0';
                }
            }
            else if (current is '"' or '\'')
            {
                quote = current;
            }
            else if (current == '>')
            {
                return index;
            }
        }

        throw new InvalidDataException("The AXAML start tag is incomplete.");
    }

    private static int FindAttributeValueStart(string source, int tagStart, int tagEnd, string propertyName)
    {
        int index = tagStart + 1;
        while (index < tagEnd && !char.IsWhiteSpace(source[index]) && source[index] is not '/' and not '>')
        {
            index++;
        }

        while (index < tagEnd)
        {
            while (index < tagEnd && char.IsWhiteSpace(source[index]))
            {
                index++;
            }

            if (index >= tagEnd || source[index] == '/')
            {
                break;
            }

            int nameStart = index;
            while (index < tagEnd && !char.IsWhiteSpace(source[index]) && source[index] is not '=' and not '/' and not '>')
            {
                index++;
            }

            string attributeName = source[nameStart..index];
            while (index < tagEnd && char.IsWhiteSpace(source[index]))
            {
                index++;
            }

            if (index >= tagEnd || source[index] != '=')
            {
                continue;
            }

            index++;
            while (index < tagEnd && char.IsWhiteSpace(source[index]))
            {
                index++;
            }

            if (index >= tagEnd || source[index] is not ('"' or '\''))
            {
                continue;
            }

            char quote = source[index++];
            if (attributeName == propertyName)
            {
                return index;
            }

            while (index < tagEnd && source[index] != quote)
            {
                index++;
            }

            index++;
        }

        return -1;
    }

    private static string EscapeAttributeValue(string value)
    {
        return value
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace("\r", "&#xD;", StringComparison.Ordinal)
            .Replace("\n", "&#xA;", StringComparison.Ordinal)
            .Replace("\t", "&#x9;", StringComparison.Ordinal);
    }

    private static bool LooksLikeControl(XElement element)
    {
        string name = element.Name.LocalName;
        return !name.Contains('.', StringComparison.Ordinal)
            && (s_supportedControls.Contains(name) || name.Length > 0 && char.IsUpper(name[0]));
    }

    private static bool IsContentRoot(string typeName)
    {
        return typeName is "Window" or "UserControl";
    }

    private static string? GetDisplayText(
        string typeName,
        IReadOnlyDictionary<string, string> properties,
        XElement element)
    {
        string? textProperty = typeName switch
        {
            "TextBlock" or "TextBox" => "Text",
            "Button" or "CheckBox" or "RadioButton" or "Label" => "Content",
            _ => null,
        };
        if (textProperty is not null && properties.TryGetValue(textProperty, out string? text))
        {
            return FormatMarkupValue(text);
        }

        if (typeName is "TextBlock" or "Label")
        {
            string? nodeText = element.Nodes()
                .OfType<XText>()
                .Select(static value => value.Value.Trim())
                .FirstOrDefault(static value => value.Length > 0);
            return nodeText is null ? null : FormatMarkupValue(nodeText);
        }

        return null;
    }

    private static string FormatMarkupValue(string value)
    {
        if (value.StartsWith("{Binding", StringComparison.Ordinal)
            || value.StartsWith("{StaticResource", StringComparison.Ordinal)
            || value.StartsWith("{DynamicResource", StringComparison.Ordinal))
        {
            return $"[{value.Trim('{', '}')}]";
        }

        return value;
    }

    private static List<string> GetDefinitionValues(
        IReadOnlyDictionary<string, string> properties,
        string propertyName)
    {
        if (!properties.TryGetValue(propertyName, out string? definitions)
            || string.IsNullOrWhiteSpace(definitions))
        {
            return [];
        }

        return definitions.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}
