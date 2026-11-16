using Avalonia.Styling;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace AA98_AvlnCodeStudio.UI.Controls;

internal static class CSharpThemeHighlighting
{
    private static readonly Lazy<IHighlightingDefinition> Dark = new(() => Create(true));
    private static readonly Lazy<IHighlightingDefinition> Light = new(() => Create(false));

    public static IHighlightingDefinition Get(ThemeVariant variant) =>
        variant == ThemeVariant.Dark ? Dark.Value : Light.Value;

    private static IHighlightingDefinition Create(bool dark)
    {
        HighlightingManager manager = new();
        manager.RegisterHighlighting("XmlDoc", Array.Empty<string>(), Load("XmlDoc", dark, manager));
        return Load("CSharp-Mode", dark, manager);
    }

    private static IHighlightingDefinition Load(string resourceName, bool dark, HighlightingManager manager)
    {
        string resource = $"AvaloniaEdit.Highlighting.Resources.{resourceName}.xshd";
        using Stream stream = typeof(HighlightingManager).Assembly.GetManifestResourceStream(resource)
            ?? throw new InvalidOperationException($"Missing AvaloniaEdit syntax resource: {resource}");
        using XmlReader reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit });
        XDocument document = XDocument.Load(reader);
        foreach (XAttribute foreground in document.Descendants().Attributes("foreground"))
        {
            foreground.Value = MapColor(foreground.Value, dark);
        }

        using StringReader mappedText = new(document.ToString());
        using XmlReader mappedReader = XmlReader.Create(mappedText, new XmlReaderSettings { DtdProcessing = DtdProcessing.Prohibit });
        return HighlightingLoader.Load(mappedReader, manager);
    }

    private static string MapColor(string color, bool dark) => (color.ToUpperInvariant(), dark) switch
    {
        ("GREEN", true) => "#A4D692",
        ("GREEN", false) => "#276027",
        ("BLUE" or "NAVY" or "MIDNIGHTBLUE" or "DARKBLUE", true) => "#8AB4F8",
        ("BLUE" or "NAVY" or "MIDNIGHTBLUE" or "DARKBLUE", false) => "#003DAD",
        ("RED", true) => "#F29D9D",
        ("RED", false) => "#A12622",
        ("MAGENTA" or "DEEPPINK" or "PINK", true) => "#E9ADD9",
        ("MAGENTA" or "DEEPPINK" or "PINK", false) => "#90175B",
        ("TEAL" or "DARKCYAN", true) => "#80D1C4",
        ("TEAL" or "DARKCYAN", false) => "#00645B",
        ("BROWN" or "SADDLEBROWN" or "OLIVE" or "#E0E000", true) => "#E8C780",
        ("BROWN" or "SADDLEBROWN" or "OLIVE" or "#E0E000", false) => "#765000",
        ("GRAY" or "DARKGRAY" or "SILVER", true) => "#BFBFBF",
        ("GRAY" or "DARKGRAY" or "SILVER", false) => "#555555",
        (_, true) => "#E5E5E5",
        _ => "#202020",
    };
}
