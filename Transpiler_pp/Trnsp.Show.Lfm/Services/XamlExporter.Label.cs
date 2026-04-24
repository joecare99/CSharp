using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Trnsp.Show.Lfm.Models.Components;

namespace Trnsp.Show.Lfm.Services;

public partial class XamlExporter
{

    private void ExportLabel(TLabel label)
    {
        var attrs = new List<string>
        {
            $"Content=\"{EscapeXml(label.Caption)}\"",
            GetPositionAttributes(label),
            GetSizeAttributes(label)
        };

        if (label.FontSize != 12)
            attrs.Add($"FontSize=\"{label.FontSize}\"");
        if (!string.IsNullOrEmpty(label.FontName) && label.FontName != "Segoe UI")
            attrs.Add($"FontFamily=\"{label.FontName}\"");
        if (label.FontColor != Colors.Black)
            attrs.Add($"Foreground=\"{ColorToString(label.FontColor)}\"");
        if (!label.Transparent && label.Color != Colors.Transparent)
            attrs.Add($"Background=\"{ColorToString(label.Color)}\"");

        AppendSingleElement("Label", attrs, label.Name);
    }

    private void ExportStaticText(TStaticText staticText)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(staticText),
            GetSizeAttributes(staticText)
        };

        if (staticText.StaticBorderStyleKind != EStaticBorderStyle.None)
        {
            attrs.Add("BorderBrush=\"Gray\"");
            attrs.Add("BorderThickness=\"1\"");
        }

        AppendLine($"<Border {string.Join(" ", attrs)}>");
        _indentLevel++;
        AppendLine($"<TextBlock Text=\"{EscapeXml(staticText.Caption)}\" VerticalAlignment=\"Center\" Padding=\"2\" />");
        _indentLevel--;
        AppendLine("</Border>");
    }
}
