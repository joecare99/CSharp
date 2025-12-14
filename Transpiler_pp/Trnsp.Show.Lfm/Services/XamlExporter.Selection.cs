using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trnsp.Show.Lfm.Models.Components;

namespace Trnsp.Show.Lfm.Services;

public partial class XamlExporter
{
    private void ExportCheckBox(TCheckBox checkBox)
    {
        var attrs = new List<string>
        {
            $"Content=\"{EscapeXml(checkBox.Caption)}\"",
            GetPositionAttributes(checkBox),
            GetSizeAttributes(checkBox)
        };

        if (checkBox.State == CheckBoxState.Checked)
            attrs.Add("IsChecked=\"True\"");
        else if (checkBox.State == CheckBoxState.Grayed)
            attrs.Add("IsChecked=\"{x:Null}\"");

        if (checkBox.AllowGrayed)
            attrs.Add("IsThreeState=\"True\"");

        AppendSingleElement("CheckBox", attrs, checkBox.Name);
    }

    private void ExportRadioButton(TRadioButton radioBtn)
    {
        var attrs = new List<string>
        {
            $"Content=\"{EscapeXml(radioBtn.Caption)}\"",
            GetPositionAttributes(radioBtn),
            GetSizeAttributes(radioBtn)
        };

        if (radioBtn.Checked)
            attrs.Add("IsChecked=\"True\"");

        AppendSingleElement("RadioButton", attrs, radioBtn.Name);
    }

    private void ExportComboBox(TComboBox comboBox)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(comboBox),
            GetSizeAttributes(comboBox)
        };

        if (comboBox.Style != ComboBoxStyle.DropDownList)
            attrs.Add("IsEditable=\"True\"");
        if (comboBox.ItemIndex >= 0)
            attrs.Add($"SelectedIndex=\"{comboBox.ItemIndex}\"");

        if (comboBox.Items.Count == 0)
        {
            AppendSingleElement("ComboBox", attrs, comboBox.Name);
        }
        else
        {
            AppendLine($"<ComboBox {string.Join(" ", attrs)} x:Name=\"{SanitizeName(comboBox.Name)}\">");
            _indentLevel++;
            foreach (var item in comboBox.Items)
            {
                AppendLine($"<ComboBoxItem Content=\"{EscapeXml(item)}\" />");
            }
            _indentLevel--;
            AppendLine("</ComboBox>");
        }
    }

    private void ExportListBox(TListBox listBox)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(listBox),
            GetSizeAttributes(listBox)
        };

        if (listBox.MultiSelect)
            attrs.Add("SelectionMode=\"Multiple\"");

        if (listBox.Items.Count == 0)
        {
            AppendSingleElement("ListBox", attrs, listBox.Name);
        }
        else
        {
            AppendLine($"<ListBox {string.Join(" ", attrs)} x:Name=\"{SanitizeName(listBox.Name)}\">");
            _indentLevel++;
            foreach (var item in listBox.Items)
            {
                AppendLine($"<ListBoxItem Content=\"{EscapeXml(item)}\" />");
            }
            _indentLevel--;
            AppendLine("</ListBox>");
        }
    }
}
