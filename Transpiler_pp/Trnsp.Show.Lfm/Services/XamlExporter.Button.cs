using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trnsp.Show.Lfm.Models.Components;

namespace Trnsp.Show.Lfm.Services;

public partial class XamlExporter
{
    private void ExportButton(TButton button)
    {
        var attrs = new List<string>
        {
            $"Content=\"{EscapeXml(button.EffectiveCaption)}\"",
            GetPositionAttributes(button),
            GetSizeAttributes(button)
        };

        if (button.Default)
            attrs.Add("IsDefault=\"True\"");

        AppendSingleElement("Button", attrs, button.Name);
    }

    private void ExportBitBtn(TBitBtn bitBtn)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(bitBtn),
            GetSizeAttributes(bitBtn)
        };

        AppendLine($"<Button {string.Join(" ", attrs)} x:Name=\"{SanitizeName(bitBtn.Name)}\">");
        _indentLevel++;

        var orientation = bitBtn.Layout == ButtonLayout.Top || bitBtn.Layout == ButtonLayout.Bottom
            ? "Vertical" : "Horizontal";

        AppendLine($"<StackPanel Orientation=\"{orientation}\">");
        _indentLevel++;

        // Icon placeholder
        var icon = GetBitBtnIconContent(bitBtn.Kind);
        if (!string.IsNullOrEmpty(icon))
        {
            AppendLine($"<TextBlock Text=\"{icon}\" Margin=\"0,0,4,0\" />");
        }

        AppendLine($"<TextBlock Text=\"{EscapeXml(bitBtn.EffectiveCaption)}\" />");

        _indentLevel--;
        AppendLine("</StackPanel>");
        _indentLevel--;
        AppendLine("</Button>");
    }

    private void ExportSpeedButton(TSpeedButton speedBtn)
    {
        var attrs = new List<string>
        {
            $"Content=\"{EscapeXml(speedBtn.EffectiveCaption)}\"",
            GetPositionAttributes(speedBtn),
            GetSizeAttributes(speedBtn)
        };

        if (speedBtn.Down)
            attrs.Add("IsChecked=\"True\"");

        AppendSingleElement("ToggleButton", attrs, speedBtn.Name);
    }

}
