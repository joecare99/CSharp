using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace RnzTrauer.Media;

/// <summary>
/// Parses the DOCUMENT/PAGE/TEXT/IMAGE structure used by the legacy PDF viewer.
/// </summary>
public sealed class PdfXmlDocumentParser
{
    /// <summary>Extracts line-oriented text and positioned image candidates.</summary>
    public (string Text, IReadOnlyList<PdfImageCandidate> Images) Parse(string xml)
    {
        ArgumentNullException.ThrowIfNull(xml);
        var document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
        var textLines = new List<string>();
        var images = new List<PdfImageCandidate>();

        foreach (var page in document.Descendants().Where(IsNamed("PAGE")))
        {
            foreach (var textElement in page.Elements().Where(IsNamed("TEXT")))
            {
                var text = string.Concat(textElement.DescendantNodes().OfType<XText>()
                    .Select(node => node.Value));
                text = NormalizeText(text);
                if (text.Length > 0)
                    textLines.Add(text);
            }

            foreach (var imageElement in page.Elements().Where(IsNamed("IMAGE")))
                images.Add(new PdfImageCandidate(
                    ReadNumber(imageElement, "x"),
                    ReadNumber(imageElement, "y"),
                    ReadNumber(imageElement, "width"),
                    ReadNumber(imageElement, "height"),
                    ReadAttribute(imageElement, "src")
                        ?? ReadAttribute(imageElement, "file")
                        ?? ReadAttribute(imageElement, "name")));
        }

        return (string.Join(Environment.NewLine, textLines), images);
    }

    private static Func<XElement, bool> IsNamed(string name)
    {
        return element => string.Equals(element.Name.LocalName, name, StringComparison.OrdinalIgnoreCase);
    }

    private static double ReadNumber(XElement element, string attributeName)
    {
        var value = ReadAttribute(element, attributeName);
        return value is not null &&
            double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number)
            ? number
            : 0;
    }

    private static string? ReadAttribute(XElement element, string attributeName)
    {
        return element.Attributes()
            .FirstOrDefault(attribute =>
                string.Equals(attribute.Name.LocalName, attributeName, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    private static string NormalizeText(string value)
    {
        return string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
    }
}
