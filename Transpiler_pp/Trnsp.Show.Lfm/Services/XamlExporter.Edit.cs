using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trnsp.Show.Lfm.Models.Components;

namespace Trnsp.Show.Lfm.Services;

public partial class XamlExporter
{
    private void ExportEdit(TEdit edit)
    {
        var attrs = new List<string>
        {
            $"Text=\"{EscapeXml(edit.Caption)}\"",
            GetPositionAttributes(edit),
            GetSizeAttributes(edit)
        };

        if (edit.ReadOnly)
            attrs.Add("IsReadOnly=\"True\"");
        if (edit.MaxLength > 0)
            attrs.Add($"MaxLength=\"{edit.MaxLength}\"");

        AppendSingleElement("TextBox", attrs, edit.Name);
    }

    private void ExportLabeledEdit(TLabeledEdit labeledEdit)
    {
        // Export as StackPanel with Label and TextBox
        var attrs = new List<string>
        {
            GetPositionAttributes(labeledEdit),
            GetSizeAttributes(labeledEdit),
            $"Orientation=\"{(labeledEdit.LabelPosKind == ELabelPosition.Left || labeledEdit.LabelPosKind == ELabelPosition.Right ? "Horizontal" : "Vertical")}\""
        };

        AppendLine($"<StackPanel {string.Join(" ", attrs)}>");
        _indentLevel++;

        if (labeledEdit.LabelPosKind == ELabelPosition.Above || labeledEdit.LabelPosKind == ELabelPosition.Left)
        {
            AppendLine($"<Label Content=\"{EscapeXml(labeledEdit.LabelCaption)}\" />");
            AppendLine($"<TextBox Text=\"{EscapeXml(labeledEdit.Caption)}\" />");
        }
        else
        {
            AppendLine($"<TextBox Text=\"{EscapeXml(labeledEdit.Caption)}\" />");
            AppendLine($"<Label Content=\"{EscapeXml(labeledEdit.LabelCaption)}\" />");
        }

        _indentLevel--;
        AppendLine("</StackPanel>");
    }

    private void ExportMemo(TMemo memo)
    {
        var attrs = new List<string>
        {
            $"Text=\"{EscapeXml(memo.Caption)}\"",
            GetPositionAttributes(memo),
            GetSizeAttributes(memo),
            "AcceptsReturn=\"True\"",
            "AcceptsTab=\"True\""
        };

        if (memo.WordWrap)
            attrs.Add("TextWrapping=\"Wrap\"");
        if (memo.ScrollBars == ScrollStyle.Vertical || memo.ScrollBars == ScrollStyle.Both)
            attrs.Add("VerticalScrollBarVisibility=\"Auto\"");
        if (memo.ScrollBars == ScrollStyle.Horizontal || memo.ScrollBars == ScrollStyle.Both)
            attrs.Add("HorizontalScrollBarVisibility=\"Auto\"");

        AppendSingleElement("TextBox", attrs, memo.Name);
    }

}
