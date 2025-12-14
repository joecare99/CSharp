using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Trnsp.Show.Lfm.Models.Components;
using Trnsp.Show.Lfm.Services.Interfaces;

namespace Trnsp.Show.Lfm.Services;

/// <summary>
/// Exports LFM components to XAML Window markup.
/// </summary>
public partial class XamlExporter : IXamlExporter
{
    private readonly StringBuilder _sb = new();
    private int _indentLevel;
    private const string IndentString = "    ";

    /// <inheritdoc/>
    public string ExportToXaml(LfmComponentBase component)
    {
        _sb.Clear();
        _indentLevel = 0;

        // Write XML declaration
        AppendLine("<!-- Generated from LFM file -->");

        if (component is TForm form)
        {
            ExportWindow(form);
        }
        else
        {
            // Export as UserControl if not a form
            ExportUserControl(component);
        }

        return _sb.ToString();
    }

    /// <inheritdoc/>
    public void ExportToFile(LfmComponentBase component, string filePath)
    {
        // First export XAML (in-memory) as before
        var xaml = ExportToXaml(component);

        // Then export image resources next to the XAML file
        try
        {
            var baseDirectory = Path.GetDirectoryName(filePath) ?? string.Empty;
            var imagesDirectory = Path.Combine(baseDirectory, "Images");

            ExportImagesRecursive(component, imagesDirectory);
        }
        catch
        {
            // Ignore image export failures for now, XAML export should still work
        }

        File.WriteAllText(filePath, xaml, Encoding.UTF8);
    }

    private void ExportWindow(TForm form) => ExportWindow(form, null);

    private void ExportWindow(TForm form, string? imagesDirectory)
    {
        AppendLine("<Window");
        _indentLevel++;
        AppendLine("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
        AppendLine("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
        AppendLine($"x:Class=\"GeneratedNamespace.{SanitizeName(form.Name)}\"");
        AppendLine($"Title=\"{EscapeXml(form.Caption)}\"");
        AppendLine($"Width=\"{form.Width}\"");
        AppendLine($"Height=\"{form.Height}\"");
        
        if (form.Color != Colors.Transparent)
        {
            AppendLine($"Background=\"{ColorToString(form.Color)}\"");
        }

        AppendLine("WindowStartupLocation=\"CenterScreen\">");
        
        // Check for menu
        var mainMenu = form.Children.OfType<TMainMenu>().FirstOrDefault();
        
        if (mainMenu != null)
        {
            // Use DockPanel for menu support
            AppendLine("<DockPanel>");
            _indentLevel++;
            
            ExportMainMenu(mainMenu, imagesDirectory);
            
            // Canvas for other controls
            AppendLine("<Canvas>");
            _indentLevel++;
            
            foreach (var child in form.Children.Where(c => c is not TMainMenu))
            {
                ExportComponent(child, imagesDirectory);
            }
            
            _indentLevel--;
            AppendLine("</Canvas>");
            _indentLevel--;
            AppendLine("</DockPanel>");
        }
        else
        {
            // Simple Canvas layout
            AppendLine("<Canvas>");
            _indentLevel++;
            
            foreach (var child in form.Children)
            {
                ExportComponent(child, imagesDirectory);
            }
            
            _indentLevel--;
            AppendLine("</Canvas>");
        }
        
        _indentLevel--;
        AppendLine("</Window>");
    }

    private void ExportUserControl(LfmComponentBase component) => ExportUserControl(component, null);

    private void ExportUserControl(LfmComponentBase component, string? imagesDirectory)
    {
        AppendLine("<UserControl");
        _indentLevel++;
        AppendLine("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"");
        AppendLine("xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"");
        AppendLine($"x:Class=\"GeneratedNamespace.{SanitizeName(component.Name)}\">");
        _indentLevel--;
        
        AppendLine("<Canvas>");
        _indentLevel++;
        ExportComponent(component, imagesDirectory);
        _indentLevel--;
        AppendLine("</Canvas>");
        
        AppendLine("</UserControl>");
    }

    private void ExportComponent(LfmComponentBase component)
        => ExportComponent(component, null);

    private void ExportComponent(LfmComponentBase component, string? imagesDirectory)
    {
        switch (component)
        {
            // More specific types first (inheritance hierarchy)
            case TStaticText staticText:
                ExportStaticText(staticText);
                break;
            case TLabel label:
                ExportLabel(label);
                break;
            case TLabeledEdit labeledEdit:
                ExportLabeledEdit(labeledEdit);
                break;
            case TMemo memo:
                ExportMemo(memo);
                break;
            case TEdit edit:
                ExportEdit(edit);
                break;
            case TBitBtn bitBtn:
                ExportBitBtn(bitBtn);
                break;
            case TButton button:
                ExportButton(button);
                break;
            case TSpeedButton speedBtn:
                ExportSpeedButton(speedBtn);
                break;
            case TCheckBox checkBox:
                ExportCheckBox(checkBox);
                break;
            case TRadioButton radioBtn:
                ExportRadioButton(radioBtn);
                break;
            case TComboBox comboBox:
                ExportComboBox(comboBox);
                break;
            case TListBox listBox:
                ExportListBox(listBox);
                break;
            case TPageControl pageControl:
                ExportPageControl(pageControl);
                break;
            case TTabSheet tabSheet:
                ExportTabSheet(tabSheet);
                break;
            case TRadioGroup radioGroup:
                ExportRadioGroup(radioGroup);
                break;
            case TCheckGroup checkGroup:
                ExportCheckGroup(checkGroup);
                break;
            case TGroupBox groupBox:
                ExportGroupBox(groupBox);
                break;
            case TScrollBox scrollBox:
                ExportScrollBox(scrollBox);
                break;
            case TPanel panel:
                ExportPanel(panel);
                break;
            case TImage image:
                ExportImage(image);
                break;
            case TShape shape:
                ExportShape(shape);
                break;
            case TProgressBar progressBar:
                ExportProgressBar(progressBar);
                break;
            case TTrackBar trackBar:
                ExportTrackBar(trackBar);
                break;
            case TSpinEdit spinEdit:
                ExportSpinEdit(spinEdit);
                break;
            case TDrawGrid drawGrid:
                ExportDataGrid(drawGrid);
                break;
            case TStatusBar statusBar:
                ExportStatusBar(statusBar);
                break;
            case TToolBar toolBar:
                ExportToolBar(toolBar);
                break;
            case TPopupMenu popupMenu:
                // Non-visual - add as comment
                ExportNonVisualComment(popupMenu);
                break;
            case TMainMenu mainMenu:
                // Non-visual when standalone - add as comment
                ExportNonVisualComment(mainMenu);
                break;
            case TActionList:
            case TAction:
            case TImageList:
            case TTimer:
                // Non-visual components - add as comment
                ExportNonVisualComment(component);
                break;
            default:
                ExportUnknownComment(component);
                break;
        }
    }

    #region Container Components

    private void ExportPanel(TPanel panel)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(panel),
            GetSizeAttributes(panel)
        };

        if (panel.Color != Colors.Transparent)
            attrs.Add($"Background=\"{ColorToString(panel.Color)}\"");
        if (panel.BevelOuter != PanelBevelStyle.None)
            attrs.Add("BorderBrush=\"Gray\" BorderThickness=\"1\"");

        AppendLine($"<Border {string.Join(" ", attrs)} x:Name=\"{SanitizeName(panel.Name)}\">");
        _indentLevel++;
        AppendLine("<Canvas>");
        _indentLevel++;

        foreach (var child in panel.Children)
        {
            ExportComponent(child);
        }

        _indentLevel--;
        AppendLine("</Canvas>");
        _indentLevel--;
        AppendLine("</Border>");
    }

    private void ExportScrollBox(TScrollBox scrollBox)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(scrollBox),
            GetSizeAttributes(scrollBox),
            "HorizontalScrollBarVisibility=\"Auto\"",
            "VerticalScrollBarVisibility=\"Auto\""
        };

        AppendLine($"<ScrollViewer {string.Join(" ", attrs)} x:Name=\"{SanitizeName(scrollBox.Name)}\">");
        _indentLevel++;
        AppendLine("<Canvas>");
        _indentLevel++;

        foreach (var child in scrollBox.Children)
        {
            ExportComponent(child);
        }

        _indentLevel--;
        AppendLine("</Canvas>");
        _indentLevel--;
        AppendLine("</ScrollViewer>");
    }

    private void ExportGroupBox(TGroupBox groupBox)
    {
        var attrs = new List<string>
        {
            $"Header=\"{EscapeXml(groupBox.Caption)}\"",
            GetPositionAttributes(groupBox),
            GetSizeAttributes(groupBox)
        };

        AppendLine($"<GroupBox {string.Join(" ", attrs)} x:Name=\"{SanitizeName(groupBox.Name)}\">");
        _indentLevel++;
        AppendLine("<Canvas>");
        _indentLevel++;

        foreach (var child in groupBox.Children)
        {
            ExportComponent(child);
        }

        _indentLevel--;
        AppendLine("</Canvas>");
        _indentLevel--;
        AppendLine("</GroupBox>");
    }

    private void ExportRadioGroup(TRadioGroup radioGroup)
    {
        var attrs = new List<string>
        {
            $"Header=\"{EscapeXml(radioGroup.Caption)}\"",
            GetPositionAttributes(radioGroup),
            GetSizeAttributes(radioGroup)
        };

        AppendLine($"<GroupBox {string.Join(" ", attrs)} x:Name=\"{SanitizeName(radioGroup.Name)}\">");
        _indentLevel++;
        AppendLine("<StackPanel>");
        _indentLevel++;

        for (int i = 0; i < radioGroup.Items.Count; i++)
        {
            var isChecked = i == radioGroup.ItemIndex ? " IsChecked=\"True\"" : "";
            AppendLine($"<RadioButton Content=\"{EscapeXml(radioGroup.Items[i])}\"{isChecked} Margin=\"2\" />");
        }

        _indentLevel--;
        AppendLine("</StackPanel>");
        _indentLevel--;
        AppendLine("</GroupBox>");
    }

    private void ExportCheckGroup(TCheckGroup checkGroup)
    {
        var attrs = new List<string>
        {
            $"Header=\"{EscapeXml(checkGroup.Caption)}\"",
            GetPositionAttributes(checkGroup),
            GetSizeAttributes(checkGroup)
        };

        AppendLine($"<GroupBox {string.Join(" ", attrs)} x:Name=\"{SanitizeName(checkGroup.Name)}\">");
        _indentLevel++;
        AppendLine("<StackPanel>");
        _indentLevel++;

        for (int i = 0; i < checkGroup.Items.Count; i++)
        {
            var isChecked = i < checkGroup.Checked.Count && checkGroup.Checked[i] ? " IsChecked=\"True\"" : "";
            AppendLine($"<CheckBox Content=\"{EscapeXml(checkGroup.Items[i])}\"{isChecked} Margin=\"2\" />");
        }

        _indentLevel--;
        AppendLine("</StackPanel>");
        _indentLevel--;
        AppendLine("</GroupBox>");
    }

    private void ExportPageControl(TPageControl pageControl)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(pageControl),
            GetSizeAttributes(pageControl)
        };

        if (pageControl.TabPosition != TabPosition.Top)
        {
            var placement = pageControl.TabPosition switch
            {
                TabPosition.Bottom => "Bottom",
                TabPosition.Left => "Left",
                TabPosition.Right => "Right",
                _ => "Top"
            };
            attrs.Add($"TabStripPlacement=\"{placement}\"");
        }

        AppendLine($"<TabControl {string.Join(" ", attrs)} x:Name=\"{SanitizeName(pageControl.Name)}\">");
        _indentLevel++;

        foreach (var child in pageControl.Children.OfType<TTabSheet>())
        {
            ExportTabItem(child);
        }

        _indentLevel--;
        AppendLine("</TabControl>");
    }

    private void ExportTabItem(TTabSheet tabSheet)
    {
        AppendLine($"<TabItem Header=\"{EscapeXml(tabSheet.Caption)}\" x:Name=\"{SanitizeName(tabSheet.Name)}\">");
        _indentLevel++;
        AppendLine("<Canvas>");
        _indentLevel++;

        foreach (var child in tabSheet.Children)
        {
            ExportComponent(child);
        }

        _indentLevel--;
        AppendLine("</Canvas>");
        _indentLevel--;
        AppendLine("</TabItem>");
    }

    private void ExportTabSheet(TTabSheet tabSheet)
    {
        // Standalone TabSheet - export as Border with Canvas
        var attrs = new List<string>
        {
            GetPositionAttributes(tabSheet),
            GetSizeAttributes(tabSheet),
            "BorderBrush=\"Gray\"",
            "BorderThickness=\"1\""
        };

        AppendLine($"<Border {string.Join(" ", attrs)} x:Name=\"{SanitizeName(tabSheet.Name)}\">");
        _indentLevel++;
        AppendLine("<Canvas>");
        _indentLevel++;

        foreach (var child in tabSheet.Children)
        {
            ExportComponent(child);
        }

        _indentLevel--;
        AppendLine("</Canvas>");
        _indentLevel--;
        AppendLine("</Border>");
    }

    #endregion

    #region Other Visual Components

    private void ExportImage(TImage image)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(image),
            GetSizeAttributes(image)
        };

        if (image.Stretch)
            attrs.Add("Stretch=\"Fill\"");
        else if (image.Proportional)
            attrs.Add("Stretch=\"Uniform\"");
        else
            attrs.Add("Stretch=\"None\"");

        // Source would need to be set separately (step 2 will adjust this)
        attrs.Add("Source=\"{Binding ImageSource}\" <!-- Set image source -->");

        AppendSingleElement("Image", attrs, image.Name);
    }

    private void ExportShape(TShape shape)
    {
        var elementName = shape.ShapeKind switch
        {
            EShapeType.Ellipse or EShapeType.Circle => "Ellipse",
            _ => "Rectangle"
        };

        var attrs = new List<string>
        {
            GetPositionAttributes(shape),
            GetSizeAttributes(shape)
        };

        if (shape.BrushStyleKind != EBrushStyle.Clear)
            attrs.Add($"Fill=\"{ColorToString(shape.BrushColor)}\"");
        if (shape.PenStyleKind != EPenStyle.Clear)
        {
            attrs.Add($"Stroke=\"{ColorToString(shape.PenColor)}\"");
            attrs.Add($"StrokeThickness=\"{shape.PenWidth}\"");
        }

        if (shape.ShapeKind == EShapeType.RoundRect || shape.ShapeKind == EShapeType.RoundSquare)
        {
            attrs.Add("RadiusX=\"5\" RadiusY=\"5\"");
        }

        AppendSingleElement(elementName, attrs, shape.Name);
    }

    private void ExportProgressBar(TProgressBar progressBar)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(progressBar),
            GetSizeAttributes(progressBar),
            $"Minimum=\"{progressBar.Min}\"",
            $"Maximum=\"{progressBar.Max}\"",
            $"Value=\"{progressBar.Position}\""
        };

        if (progressBar.Orientation == ProgressBarOrientation.Vertical)
            attrs.Add("Orientation=\"Vertical\"");

        AppendSingleElement("ProgressBar", attrs, progressBar.Name);
    }

    private void ExportTrackBar(TTrackBar trackBar)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(trackBar),
            GetSizeAttributes(trackBar),
            $"Minimum=\"{trackBar.Min}\"",
            $"Maximum=\"{trackBar.Max}\"",
            $"Value=\"{trackBar.Position}\""
        };

        if (trackBar.Orientation == TrackBarOrientation.Vertical)
            attrs.Add("Orientation=\"Vertical\"");

        AppendSingleElement("Slider", attrs, trackBar.Name);
    }

    private void ExportSpinEdit(TSpinEdit spinEdit)
    {
        // WPF doesn't have a native SpinEdit, use TextBox with comment
        var attrs = new List<string>
        {
            $"Text=\"{spinEdit.Value}\"",
            GetPositionAttributes(spinEdit),
            GetSizeAttributes(spinEdit)
        };

        AppendLine($"<!-- SpinEdit: Min={spinEdit.MinValue}, Max={spinEdit.MaxValue} -->");
        AppendSingleElement("TextBox", attrs, spinEdit.Name);
    }

    private void ExportDataGrid(TDrawGrid drawGrid)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(drawGrid),
            GetSizeAttributes(drawGrid)
        };

        AppendLine($"<!-- Grid: {drawGrid.ColCount} columns x {drawGrid.RowCount} rows -->");
        AppendSingleElement("DataGrid", attrs, drawGrid.Name);
    }

    #endregion

    #region Menu and Toolbar

    private void ExportMainMenu(TMainMenu mainMenu, string? imagesDirectory)
    {
        AppendLine("<Menu DockPanel.Dock=\"Top\">");
        _indentLevel++;

        foreach (var child in mainMenu.Children.OfType<TMenuItem>())
        {
            ExportMenuItem(child);
        }

        _indentLevel--;
        AppendLine("</Menu>");
    }

    private void ExportMenuItem(TMenuItem menuItem)
    {
        if (menuItem.IsSeparator || menuItem.EffectiveCaption == "-")
        {
            AppendLine("<Separator />");
            return;
        }

        var header = EscapeXml(menuItem.EffectiveCaption).Replace("&", "_");
        var hasChildren = menuItem.Children.OfType<TMenuItem>().Any() || menuItem.SubItems.Any();

        if (!hasChildren)
        {
            var attrs = new List<string> { $"Header=\"{header}\"" };
            if (!string.IsNullOrEmpty(menuItem.EffectiveShortCut))
            {
                attrs.Add($"InputGestureText=\"{FormatShortcut(menuItem.EffectiveShortCut)}\"");
            }
            AppendSingleElement("MenuItem", attrs, menuItem.Name);
        }
        else
        {
            AppendLine($"<MenuItem Header=\"{header}\" x:Name=\"{SanitizeName(menuItem.Name)}\">");
            _indentLevel++;

            foreach (var child in menuItem.Children.OfType<TMenuItem>())
            {
                ExportMenuItem(child);
            }
            foreach (var subItem in menuItem.SubItems)
            {
                ExportMenuItem(subItem);
            }

            _indentLevel--;
            AppendLine("</MenuItem>");
        }
    }

    private void ExportStatusBar(TStatusBar statusBar)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(statusBar),
            GetSizeAttributes(statusBar)
        };

        AppendLine($"<StatusBar {string.Join(" ", attrs)} x:Name=\"{SanitizeName(statusBar.Name)}\">");
        _indentLevel++;

        if (statusBar.SimplePanel)
        {
            AppendLine($"<StatusBarItem Content=\"{EscapeXml(statusBar.SimpleText)}\" />");
        }
        else
        {
            foreach (var panel in statusBar.Panels)
            {
                AppendLine($"<StatusBarItem Content=\"{EscapeXml(panel.Text)}\" Width=\"{panel.Width}\" />");
            }
        }

        _indentLevel--;
        AppendLine("</StatusBar>");
    }

    private void ExportToolBar(TToolBar toolBar)
    {
        var attrs = new List<string>
        {
            GetPositionAttributes(toolBar),
            GetSizeAttributes(toolBar)
        };

        AppendLine($"<ToolBar {string.Join(" ", attrs)} x:Name=\"{SanitizeName(toolBar.Name)}\">");
        _indentLevel++;

        foreach (var child in toolBar.Children)
        {
            if (child is TToolButton toolBtn)
            {
                ExportToolButton(toolBtn);
            }
            else
            {
                ExportComponent(child);
            }
        }

        _indentLevel--;
        AppendLine("</ToolBar>");
    }

    private void ExportToolButton(TToolButton toolBtn)
    {
        if (toolBtn.Style == ToolButtonStyle.Separator)
        {
            AppendLine("<Separator />");
            return;
        }

        var attrs = new List<string>();

        if (!string.IsNullOrEmpty(toolBtn.EffectiveHint))
            attrs.Add($"ToolTip=\"{EscapeXml(toolBtn.EffectiveHint)}\"");

        // Add image index as comment
        if (toolBtn.EffectiveImageIndex >= 0)
        {
            AppendLine($"<!-- ImageIndex: {toolBtn.EffectiveImageIndex} -->");
        }

        AppendSingleElement("Button", attrs, toolBtn.Name);
    }

    #endregion


    #region Image Export Helpers

    private static void ExportImagesRecursive(LfmComponentBase component, string imagesDirectory)
    {
        if (string.IsNullOrEmpty(imagesDirectory))
            return;

        // Export image for TImage/TPaintBox
        if (component is TImage image && image.ImageSource is not null)
        {
            Directory.CreateDirectory(imagesDirectory);
            var fileName = SanitizeName(string.IsNullOrEmpty(image.Name) ? "Image" : image.Name) + ".png";
            var fullPath = Path.Combine(imagesDirectory, fileName);
            SaveImageSourceAsPng(image.ImageSource, fullPath);
        }

        // Export combined bitmap for TImageList (the strip); individual images could be added later if needed
        if (component is TImageList imageList && imageList.Bitmap?.ImageSource is not null)
        {
            Directory.CreateDirectory(imagesDirectory);
            var fileName = SanitizeName(string.IsNullOrEmpty(imageList.Name) ? "ImageList" : imageList.Name) + ".png";
            var fullPath = Path.Combine(imagesDirectory, fileName);
            SaveImageSourceAsPng(imageList.Bitmap.ImageSource, fullPath);
        }

        // Recurse into children for containers/forms
        switch (component)
        {
            case TForm form:
                foreach (var child in form.Children)
                    ExportImagesRecursive(child, imagesDirectory);
                break;
            case TScrollBox scrollBox:
                foreach (var child in scrollBox.Children)
                    ExportImagesRecursive(child, imagesDirectory);
                break;
            case TPanel panel:
                foreach (var child in panel.Children)
                    ExportImagesRecursive(child, imagesDirectory);
                break;
            case TGroupBox groupBox:
                foreach (var child in groupBox.Children)
                    ExportImagesRecursive(child, imagesDirectory);
                break;
            case TPageControl pageControl:
                foreach (var child in pageControl.Children)
                    ExportImagesRecursive(child, imagesDirectory);
                break;
            case TTabSheet tabSheet:
                foreach (var child in tabSheet.Children)
                    ExportImagesRecursive(child, imagesDirectory);
                break;
            case TToolBar toolBar:
                foreach (var child in toolBar.Children)
                    ExportImagesRecursive(child, imagesDirectory);
                break;
        }
    }

    private static void SaveImageSourceAsPng(ImageSource source, string fullPath)
    {
        if (source is not System.Windows.Media.Imaging.BitmapSource bitmapSource)
            return;

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmapSource));

        using var stream = File.Create(fullPath);
        encoder.Save(stream);
    }

    #endregion

    #region Non-Visual Components

    private void ExportNonVisualComment(LfmComponentBase component) => AppendLine($"<!-- Non-visual: {component.TypeName} Name=\"{component.Name}\" -->");

    private void ExportUnknownComment(LfmComponentBase component) => AppendLine($"<!-- Unknown component: {component.TypeName} Name=\"{component.Name}\" -->");

    #endregion

    #region Helper Methods

    private void AppendLine(string line)
    {
        _sb.Append(new string(' ', _indentLevel * IndentString.Length));
        _sb.AppendLine(line);
    }

    private void AppendSingleElement(string elementName, List<string> attributes, string name)
    {
        var nameAttr = !string.IsNullOrEmpty(name) ? $" x:Name=\"{SanitizeName(name)}\"" : "";
        var attrString = attributes.Count > 0 ? " " + string.Join(" ", attributes) : "";
        AppendLine($"<{elementName}{attrString}{nameAttr} />");
    }

    private static string GetPositionAttributes(LfmComponentBase component) => $"Canvas.Left=\"{component.Left}\" Canvas.Top=\"{component.Top}\"";

    private static string GetSizeAttributes(LfmComponentBase component) => $"Width=\"{component.Width}\" Height=\"{component.Height}\"";

    private static string ColorToString(Color color)
    {
        if (color == Colors.Transparent)
            return "Transparent";
        if (color.A == 255)
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    private static string EscapeXml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }

    private static string SanitizeName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "unnamed";

        // Remove invalid characters for XAML names
        var result = new StringBuilder();
        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
                result.Append(c);
        }

        var sanitized = result.ToString();
        if (sanitized.Length == 0 || char.IsDigit(sanitized[0]))
            sanitized = "_" + sanitized;

        return sanitized;
    }

    private static string GetBitBtnIconContent(BitBtnKind kind) => kind switch
    {
        BitBtnKind.OK => "?",
        BitBtnKind.Cancel => "?",
        BitBtnKind.Help => "?",
        BitBtnKind.Yes => "?",
        BitBtnKind.No => "?",
        BitBtnKind.Close => "?",
        BitBtnKind.Abort => "?",
        BitBtnKind.Retry => "??",
        BitBtnKind.Ignore => "?",
        BitBtnKind.All => "?",
        _ => ""
    };

    private static string FormatShortcut(string shortcut)
    {
        if (string.IsNullOrEmpty(shortcut))
            return string.Empty;

        if (int.TryParse(shortcut, out int keyCode))
        {
            var modifiers = new StringBuilder();
            if ((keyCode & 0x4000) != 0) modifiers.Append("Ctrl+");
            if ((keyCode & 0x2000) != 0) modifiers.Append("Shift+");
            if ((keyCode & 0x8000) != 0) modifiers.Append("Alt+");

            int key = keyCode & 0xFF;
            string keyName = key switch
            {
                >= 65 and <= 90 => ((char)key).ToString(),
                >= 48 and <= 57 => ((char)key).ToString(),
                112 => "F1", 113 => "F2", 114 => "F3", 115 => "F4",
                116 => "F5", 117 => "F6", 118 => "F7", 119 => "F8",
                120 => "F9", 121 => "F10", 122 => "F11", 123 => "F12",
                _ => $"Key{key}"
            };

            return modifiers.ToString() + keyName;
        }

        return shortcut;
    }

    #endregion
}
