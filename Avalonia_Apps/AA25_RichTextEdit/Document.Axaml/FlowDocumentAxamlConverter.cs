using System.Globalization;
using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace Document.Axaml;

/// <summary>
/// Converts WPF FlowDocument XAML into Avalonia-friendly preview AXAML.
/// </summary>
public sealed class FlowDocumentAxamlConverter
{
    private const string AvaloniaXmlNamespace = "https://github.com/avaloniaui";
    private static readonly XNamespace AvaloniaNamespace = AvaloniaXmlNamespace;
    private static readonly XNamespace XmlNamespace = XNamespace.Xml;
 //   private static readonly XName DefaultNamespaceAttribute = XNamespace.Xmlns + string.Empty;

    /// <summary>
    /// Converts WPF FlowDocument XAML into a read-only Avalonia AXAML control tree.
    /// </summary>
    /// <param name="flowDocumentXaml">The source WPF FlowDocument XAML.</param>
    /// <returns>Avalonia AXAML for runtime loading.</returns>
    public string ConvertToPreviewAxaml(string? flowDocumentXaml)
    {
        if (string.IsNullOrWhiteSpace(flowDocumentXaml))
        {
            return CreateFallbackPreviewAxaml(string.Empty);
        }

        if (!TryParseDocument(flowDocumentXaml, out var sourceDocument))
        {
            return CreateFallbackPreviewAxaml(flowDocumentXaml);
        }

        var root = new XElement(
            AvaloniaNamespace + "StackPanel",
    //        new XAttribute(DefaultNamespaceAttribute, AvaloniaXmlNamespace),
            new XAttribute("Spacing", "8"));

        foreach (var block in sourceDocument.Root?.Elements() ?? Enumerable.Empty<XElement>())
        {
            var previewBlock = ConvertBlock(block);
            if (previewBlock is not null)
            {
                root.Add(previewBlock);
            }
        }

        if (!root.Elements().Any())
        {
            root.Add(CreateTextBlock(Array.Empty<object>(), null));
        }

        return root.ToString(SaveOptions.DisableFormatting);
    }

    /// <summary>
    /// Creates a preview control from WPF FlowDocument XAML.
    /// </summary>
    /// <param name="flowDocumentXaml">The source WPF FlowDocument XAML.</param>
    /// <returns>A parsed Avalonia control.</returns>
    public Control CreatePreviewControl(string? flowDocumentXaml)
    {
        if (string.IsNullOrWhiteSpace(flowDocumentXaml))
        {
            return new TextBlock
            {
                TextWrapping = TextWrapping.Wrap,
            };
        }

        if (!TryParseDocument(flowDocumentXaml, out var sourceDocument))
        {
            return new TextBlock
            {
                Text = flowDocumentXaml,
                TextWrapping = TextWrapping.Wrap,
            };
        }

        var panel = new StackPanel
        {
            Spacing = 8,
        };

        foreach (var block in sourceDocument.Root?.Elements() ?? Enumerable.Empty<XElement>())
        {
            var control = CreateBlockControl(block);
            if (control is not null)
            {
                panel.Children.Add(control);
            }
        }

        if (panel.Children.Count == 0)
        {
            panel.Children.Add(new TextBlock { TextWrapping = TextWrapping.Wrap });
        }

        return panel;
    }

    /// <summary>
    /// Extracts readable plain text from WPF FlowDocument XAML.
    /// </summary>
    /// <param name="flowDocumentXaml">The source WPF FlowDocument XAML.</param>
    /// <returns>The extracted plain text.</returns>
    public string ExtractPlainText(string? flowDocumentXaml)
    {
        if (string.IsNullOrWhiteSpace(flowDocumentXaml))
        {
            return string.Empty;
        }

        if (!TryParseDocument(flowDocumentXaml, out var sourceDocument))
        {
            return flowDocumentXaml;
        }

        var paragraphs = new List<string>();
        foreach (var block in sourceDocument.Root?.Elements() ?? Enumerable.Empty<XElement>())
        {
            var paragraphText = ExtractText(block).TrimEnd('\r', '\n');
            if (!string.IsNullOrWhiteSpace(paragraphText))
            {
                paragraphs.Add(paragraphText);
            }
        }

        return string.Join(Environment.NewLine + Environment.NewLine, paragraphs);
    }

    private static XElement? ConvertBlock(XElement block)
        => block.Name.LocalName switch
        {
            "Paragraph" => ConvertParagraph(block),
            "Section" => ConvertSection(block),
            _ => ConvertParagraph(block),
        };

    private static Control? CreateBlockControl(XElement block)
        => block.Name.LocalName switch
        {
            "Paragraph" => CreateParagraphControl(block),
            "Section" => CreateSectionControl(block),
            _ => CreateParagraphControl(block),
        };

    private static XElement? ConvertSection(XElement section)
    {
        var childBlocks = section.Elements().Select(ConvertBlock).Where(element => element is not null).ToArray();
        if (childBlocks.Length == 0)
        {
            return null;
        }

        return new XElement(
            AvaloniaNamespace + "StackPanel",
            new XAttribute("Spacing", "8"),
            childBlocks!);
    }

    private static Control? CreateSectionControl(XElement section)
    {
        var panel = new StackPanel
        {
            Spacing = 8,
        };

        foreach (var child in section.Elements())
        {
            var childControl = CreateBlockControl(child);
            if (childControl is not null)
            {
                panel.Children.Add(childControl);
            }
        }

        return panel.Children.Count > 0 ? panel : null;
    }

    private static XElement ConvertParagraph(XElement paragraph)
    {
        var inlineStyle = ReadInlineStyle(paragraph, InlineStyle.Empty);
        var inlineElements = new List<object>();

        foreach (var node in paragraph.Nodes())
        {
            AppendInlineElements(inlineElements, node, inlineStyle);
        }

        if (inlineElements.Count == 0)
        {
            inlineElements.Add(CreateRun(string.Empty, inlineStyle));
        }

        return CreateTextBlock(inlineElements, paragraph);
    }

    private static Control CreateParagraphControl(XElement paragraph)
    {
        var textBlock = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
        };
        var inlines = textBlock.Inlines ?? throw new InvalidOperationException("TextBlock.Inlines must be available.");

        ApplyTextBlockAttributes(textBlock, paragraph);
        var inlineStyle = ReadInlineStyle(paragraph, InlineStyle.Empty);
        foreach (var node in paragraph.Nodes())
        {
            AppendInlineControls(inlines, node, inlineStyle);
        }

        if (inlines.Count == 0)
        {
            inlines.Add(CreateRunControl(string.Empty, inlineStyle));
        }

        return textBlock;
    }

    private static XElement CreateTextBlock(IEnumerable<object> inlineElements, XElement? sourceParagraph)
    {
        var textBlock = new XElement(
            AvaloniaNamespace + "TextBlock",
            new XAttribute("TextWrapping", "Wrap"));

        CopyAttribute(sourceParagraph, textBlock, "TextAlignment");
        CopyAttribute(sourceParagraph, textBlock, "FontWeight");
        CopyAttribute(sourceParagraph, textBlock, "FontStyle");
        CopyAttribute(sourceParagraph, textBlock, "FontSize");
        CopyAttribute(sourceParagraph, textBlock, "Margin");

        foreach (var inlineElement in inlineElements)
        {
            textBlock.Add(inlineElement);
        }

        return textBlock;
    }

    private static void AppendInlineElements(ICollection<object> target, XNode node, InlineStyle style)
    {
        switch (node)
        {
            case XText textNode when !string.IsNullOrEmpty(textNode.Value):
                target.Add(CreateRun(textNode.Value, style));
                break;
            case XCData cdataNode when !string.IsNullOrEmpty(cdataNode.Value):
                target.Add(CreateRun(cdataNode.Value, style));
                break;
            case XElement element:
                AppendInlineElements(target, element, style);
                break;
        }
    }

    private static void AppendInlineControls(InlineCollection target, XNode node, InlineStyle style)
    {
        switch (node)
        {
            case XText textNode when !string.IsNullOrEmpty(textNode.Value):
                target.Add(CreateRunControl(textNode.Value, style));
                break;
            case XElement element:
                AppendInlineControls(target, element, style);
                break;
        }
    }

    private static void AppendInlineElements(ICollection<object> target, XElement element, InlineStyle inheritedStyle)
    {
        if (IsPropertyElement(element))
        {
            return;
        }

        var style = ReadInlineStyle(element, inheritedStyle);
        switch (element.Name.LocalName)
        {
            case "LineBreak":
                target.Add(new XElement(AvaloniaNamespace + "LineBreak"));
                return;
            case "Run":
                AppendRunElements(target, element, style);
                return;
            case "Bold":
                style = style.ApplyAttributes("Bold", null, null, null);
                break;
            case "Italic":
                style = style.ApplyAttributes(null, "Italic", null, null);
                break;
            case "Underline":
                style = style.ApplyAttributes(null, null, null, "Underline");
                break;
        }

        var textAttribute = element.Attribute("Text")?.Value;
        if (!string.IsNullOrEmpty(textAttribute) && !element.Nodes().OfType<XText>().Any())
        {
            target.Add(CreateRun(textAttribute, style));
        }

        foreach (var childNode in element.Nodes())
        {
            AppendInlineElements(target, childNode, style);
        }
    }

    private static void AppendInlineControls(InlineCollection target, XElement element, InlineStyle inheritedStyle)
    {
        if (IsPropertyElement(element))
        {
            return;
        }

        var style = ReadInlineStyle(element, inheritedStyle);
        switch (element.Name.LocalName)
        {
            case "LineBreak":
                target.Add(new LineBreak());
                return;
            case "Run":
                AppendRunControls(target, element, style);
                return;
            case "Bold":
                style = style.ApplyAttributes("Bold", null, null, null);
                break;
            case "Italic":
                style = style.ApplyAttributes(null, "Italic", null, null);
                break;
            case "Underline":
                style = style.ApplyAttributes(null, null, null, "Underline");
                break;
        }

        var textAttribute = element.Attribute("Text")?.Value;
        if (!string.IsNullOrEmpty(textAttribute) && !element.Nodes().OfType<XText>().Any())
        {
            target.Add(CreateRunControl(textAttribute, style));
        }

        foreach (var childNode in element.Nodes())
        {
            AppendInlineControls(target, childNode, style);
        }
    }

    private static void AppendRunElements(ICollection<object> target, XElement runElement, InlineStyle style)
    {
        var textAttribute = runElement.Attribute("Text")?.Value;
        var appendedFromNodes = false;
        foreach (var childNode in runElement.Nodes())
        {
            if (childNode is XElement childElement && IsPropertyElement(childElement))
            {
                continue;
            }

            appendedFromNodes = true;
            AppendInlineElements(target, childNode, style);
        }

        if (!appendedFromNodes)
        {
            target.Add(CreateRun(textAttribute ?? runElement.Value, style));
        }
    }

    private static void AppendRunControls(InlineCollection target, XElement runElement, InlineStyle style)
    {
        var textAttribute = runElement.Attribute("Text")?.Value;
        var appendedFromNodes = false;
        foreach (var childNode in runElement.Nodes())
        {
            if (childNode is XElement childElement && IsPropertyElement(childElement))
            {
                continue;
            }

            appendedFromNodes = true;
            AppendInlineControls(target, childNode, style);
        }

        if (!appendedFromNodes)
        {
            target.Add(CreateRunControl(textAttribute ?? runElement.Value, style));
        }
    }

    private static XElement CreateRun(string text, InlineStyle style)
    {
        var run = new XElement(
            AvaloniaNamespace + "Run",
            new XAttribute(XmlNamespace + "space", "preserve"),
            new XAttribute("Text", text));

        AddAttributeIfSet(run, "FontWeight", style.FontWeight);
        AddAttributeIfSet(run, "FontStyle", style.FontStyle);
        AddAttributeIfSet(run, "FontSize", style.FontSize);
        AddAttributeIfSet(run, "TextDecorations", style.TextDecorations);
        return run;
    }

    private static Run CreateRunControl(string text, InlineStyle style)
    {
        var run = new Run(text);
        ApplyRunAttributes(run, style);
        return run;
    }

    private static InlineStyle ReadInlineStyle(XElement element, InlineStyle inheritedStyle)
    {
        var textDecorations = ReadTextDecorations(element);
        return inheritedStyle.ApplyAttributes(
            element.Attribute("FontWeight")?.Value,
            element.Attribute("FontStyle")?.Value,
            element.Attribute("FontSize")?.Value,
            textDecorations);
    }

    private static string? ReadTextDecorations(XElement element)
    {
        var directTextDecorations = element.Attribute("TextDecorations")?.Value;
        if (!string.IsNullOrWhiteSpace(directTextDecorations))
        {
            return directTextDecorations;
        }

        foreach (var propertyElement in element.Elements().Where(IsPropertyElement))
        {
            var hasUnderline = propertyElement.Elements().Any(child =>
                string.Equals(child.Name.LocalName, "TextDecoration", StringComparison.OrdinalIgnoreCase)
                && string.Equals(child.Attribute("Location")?.Value, "Underline", StringComparison.OrdinalIgnoreCase));
            if (hasUnderline)
            {
                return "Underline";
            }
        }

        return null;
    }

    private static void ApplyTextBlockAttributes(TextBlock textBlock, XElement sourceParagraph)
    {
        if (Enum.TryParse<TextAlignment>(sourceParagraph.Attribute("TextAlignment")?.Value, true, out var textAlignment))
        {
            textBlock.TextAlignment = textAlignment;
        }

        if (TryParseFontWeight(sourceParagraph.Attribute("FontWeight")?.Value, out var fontWeight))
        {
            textBlock.FontWeight = fontWeight;
        }

        if (TryParseFontStyle(sourceParagraph.Attribute("FontStyle")?.Value, out var fontStyle))
        {
            textBlock.FontStyle = fontStyle;
        }

        if (double.TryParse(sourceParagraph.Attribute("FontSize")?.Value,CultureInfo.InvariantCulture, out var fontSize))
        {
            textBlock.FontSize = fontSize;
        }

        if (TryParseThickness(sourceParagraph.Attribute("Margin")?.Value, out var margin))
        {
            textBlock.Margin = margin;
        }
    }

    private static void ApplyRunAttributes(Run run, InlineStyle style)
    {
        if (TryParseFontWeight(style.FontWeight, out var fontWeight))
        {
            run.FontWeight = fontWeight;
        }

        if (TryParseFontStyle(style.FontStyle, out var fontStyle))
        {
            run.FontStyle = fontStyle;
        }

        if (double.TryParse(style.FontSize,CultureInfo.InvariantCulture, out var fontSize))
        {
            run.FontSize = fontSize;
        }

        if (TryParseTextDecorations(style.TextDecorations, out var textDecorations))
        {
            run.TextDecorations = textDecorations;
        }
    }

    private static bool TryParseFontWeight(string? value, out FontWeight fontWeight)
    {
        fontWeight = FontWeight.Normal;
        return !string.IsNullOrWhiteSpace(value) && FontWeight.TryParse(value, out fontWeight);
    }

    private static bool TryParseFontStyle(string? value, out FontStyle fontStyle)
    {
        fontStyle = FontStyle.Normal;
        return Enum.TryParse(value, true, out fontStyle);
    }

    private static bool TryParseTextDecorations(string? value, out TextDecorationCollection? textDecorations)
    {
        textDecorations = null;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (string.Equals(value, "Underline", StringComparison.OrdinalIgnoreCase))
        {
            textDecorations = TextDecorations.Underline;
            return true;
        }

        return false;
    }

    private static bool TryParseThickness(string? value, out Thickness thickness)
    {
        thickness = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        try
        {
            thickness = Thickness.Parse(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string ExtractText(XElement element)
    {
        if (IsPropertyElement(element))
        {
            return string.Empty;
        }

        if (string.Equals(element.Name.LocalName, "LineBreak", StringComparison.Ordinal))
        {
            return Environment.NewLine;
        }

        var text = string.Empty;
        var textAttribute = element.Attribute("Text")?.Value;
        if (!string.IsNullOrEmpty(textAttribute) && !element.Nodes().OfType<XText>().Any())
        {
            text += textAttribute;
        }

        foreach (var childNode in element.Nodes())
        {
            text += childNode switch
            {
                XText childText => childText.Value,
                XElement childElement => ExtractText(childElement),
                _ => string.Empty,
            };
        }

        if (string.Equals(element.Name.LocalName, "Paragraph", StringComparison.Ordinal))
        {
            text += Environment.NewLine + Environment.NewLine;
        }

        return text;
    }

    private static bool TryParseDocument(string flowDocumentXaml, out XDocument sourceDocument)
    {
        try
        {
            sourceDocument = XDocument.Parse(flowDocumentXaml, LoadOptions.PreserveWhitespace);
            return true;
        }
        catch
        {
            sourceDocument = new XDocument();
            return false;
        }
    }

    private static bool IsPropertyElement(XElement element)
        => element.Name.LocalName.Contains('.', StringComparison.Ordinal);

    private static void CopyAttribute(XElement? source, XElement target, string attributeName)
    {
        var value = source?.Attribute(attributeName)?.Value;
        AddAttributeIfSet(target, attributeName, value);
    }

    private static void AddAttributeIfSet(XElement target, string attributeName, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            target.Add(new XAttribute(attributeName, value));
        }
    }

    private static string CreateFallbackPreviewAxaml(string text)
    {
        var root = new XElement(
            AvaloniaNamespace + "TextBlock",
            //new XAttribute(DefaultNamespaceAttribute, AvaloniaXmlNamespace),
            new XAttribute("TextWrapping", "Wrap"),
            new XAttribute("Text", text));
        return root.ToString(SaveOptions.DisableFormatting);
    }
}
