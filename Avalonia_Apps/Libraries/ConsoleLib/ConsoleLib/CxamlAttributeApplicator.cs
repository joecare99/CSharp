using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;
using ConsoleLib.Interfaces;

namespace ConsoleLib;

/// <summary>
/// Applies CXAML attributes and bindings to controls created by <see cref="CxamlLoader"/>.
/// </summary>
internal static class CxamlAttributeApplicator
{
    /// <summary>
    /// Applies a single CXAML attribute, including binding and named-control registration rules.
    /// </summary>
    /// <param name="control">Control that receives the attribute value.</param>
    /// <param name="name">Attribute name as read from CXAML.</param>
    /// <param name="value">Raw attribute value.</param>
    /// <param name="context">Optional load context for binding resolution.</param>
    /// <param name="namedControls">Optional named-control lookup dictionary.</param>
    internal static void Apply(IControl control, string name, string value, CxamlLoadContext? context, IDictionary<string, IControl>? namedControls)
    {
        if (name == "Name")
        {
            if (namedControls is null || string.IsNullOrWhiteSpace(value) || !IsIdentifier(value) || namedControls.ContainsKey(value))
                throw new CxamlParseException("Invalid or duplicate CXAML control name: " + value);
            namedControls.Add(value, control);
            return;
        }

        if (TryGetBindingPath(value, out var bindingPath))
        {
            ApplyBinding(control, name, bindingPath, context);
            return;
        }

        switch (name)
        {
            case "Tag": control.Tag = value; break;
            case "Accelerator":
                if (value.Length != 1)
                    throw new CxamlParseException("Accelerator must contain exactly one character.");
                control.Accelerator = value[0];
                break;
            case "Shadow": control.Shadow = ParseBool(value, name); break;
            case "Text": control.Text = value; break;
            case "Visible": control.Visible = ParseBool(value, name); break;
            case "Enabled": control.Enabled = ParseBool(value, name); break;
            case "BackColor": control.BackColor = ParseColor(value, name); break;
            case "ForeColor": control.ForeColor = ParseColor(value, name); break;
            case "BorderStyle": SetBorderStyle(control, value); break;
            case "BorderColor": SetBorderColor(control, value); break;
            case "HLBackColor" when control is Button button: button.HLBackColor = ParseColor(value, name); break;
            case "IsChecked" when control is CheckBox checkBox: checkBox.IsChecked = ParseBool(value, name); break;
            case "Grid.Row": Grid.SetRow(control, ParseInt(value, name)); break;
            case "Grid.Column": Grid.SetColumn(control, ParseInt(value, name)); break;
            case "Grid.RowSpan": Grid.SetRowSpan(control, ParseInt(value, name)); break;
            case "Grid.ColumnSpan": Grid.SetColumnSpan(control, ParseInt(value, name)); break;
            case "RowDefinitions" when control is Grid grid: ParseCompactRows(grid, value); break;
            case "ColumnDefinitions" when control is Grid grid: ParseCompactColumns(grid, value); break;
            default: throw new CxamlParseException("Unsupported CXAML attribute '" + name + "' on " + control.GetType().Name);
        }
    }

    /// <summary>
    /// Parses an invariant integer CXAML value.
    /// </summary>
    /// <param name="value">Raw scalar value.</param>
    /// <param name="name">Attribute name for diagnostics.</param>
    /// <returns>Parsed integer value.</returns>
    internal static int ParseInt(string value, string name) => int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result) ? result : throw new CxamlParseException("Invalid integer value for " + name + ": " + value);

    /// <summary>
    /// Determines whether an attribute name is valid for validation diagnostics.
    /// </summary>
    /// <param name="name">Attribute name.</param>
    /// <param name="controlType">Resolved control type, when available.</param>
    /// <returns><c>true</c> when the attribute is supported for the given control type.</returns>
    internal static bool IsSupported(string name, Type? controlType) =>
        name is "Name" or "Text" or "Width" or "Height" or "X" or "Y" or "Visible" or "Enabled" or "BackColor" or "ForeColor" or "Tag" or "Accelerator" or "Shadow" or "Command" or "ItemsSource"
        || name is "Grid.Row" or "Grid.Column" or "Grid.RowSpan" or "Grid.ColumnSpan"
        || name is "RowDefinitions" or "ColumnDefinitions" && controlType is not null && typeof(Grid).IsAssignableFrom(controlType)
        || name is ("BorderStyle" or "BorderColor") && controlType is not null && (typeof(IHasBorder).IsAssignableFrom(controlType) || typeof(Terminal).IsAssignableFrom(controlType))
        || name == "HLBackColor" && controlType == typeof(Button)
        || name == "IsChecked" && controlType == typeof(CheckBox);

    /// <summary>
    /// Parses a compact grid length token (<c>Auto</c>, star, or pixel value).
    /// </summary>
    /// <param name="value">Raw grid length token.</param>
    /// <returns>Parsed <see cref="GridLength"/> value.</returns>
    internal static GridLength ParseGridLength(string value)
    {
        if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase)) return GridLength.Auto;
        if (value.EndsWith("*", StringComparison.Ordinal))
        {
            var starValue = value[..^1];
            var weight = starValue.Length == 0 ? 1 : double.TryParse(starValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) ? parsed : throw new CxamlParseException("Invalid Grid star length: " + value);
            return new GridLength(weight, GridUnitType.Star);
        }
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var pixels) ? new GridLength(pixels, GridUnitType.Pixel) : throw new CxamlParseException("Invalid Grid length: " + value);
    }

    private static bool ParseBool(string value, string name) => bool.TryParse(value, out var result) ? result : throw new CxamlParseException("Invalid boolean value for " + name + ": " + value);
    private static ConsoleColor ParseColor(string value, string name) => Enum.TryParse<ConsoleColor>(value, true, out var result) ? result : throw new CxamlParseException("Invalid console color value for " + name + ": " + value);
    private static void ParseCompactRows(Grid grid, string value) { grid.RowDefinitions.Clear(); foreach (var item in SplitDefinitionList(value)) grid.RowDefinitions.Add(new RowDefinition { Height = ParseGridLength(item) }); }
    private static void ParseCompactColumns(Grid grid, string value) { grid.ColumnDefinitions.Clear(); foreach (var item in SplitDefinitionList(value)) grid.ColumnDefinitions.Add(new ColumnDefinition { Width = ParseGridLength(item) }); }
    private static IEnumerable<string> SplitDefinitionList(string value) => value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries).Select(item => item.Trim()).Where(item => item.Length != 0);
    private static void SetBorderStyle(IControl control, string value) { if (!Enum.TryParse<BorderStyle>(value, true, out var style)) throw new CxamlParseException("Invalid border style value: " + value); if (control is Panel panel) panel.BorderStyle = style; else if (control is Terminal terminal) terminal.BorderStyle = style; else if (control is IHasBorder border) border.BorderDefinition.Style = style; else throw new CxamlParseException("BorderStyle is not supported on " + control.GetType().Name); }
    private static void SetBorderColor(IControl control, string value) { var color = ParseColor(value, "BorderColor"); if (control is Panel panel) panel.BorderColor = color; else if (control is Terminal terminal) terminal.BorderColor = color; else if (control is IHasBorder border) border.BorderDefinition.BorderColor = color; else throw new CxamlParseException("BorderColor is not supported on " + control.GetType().Name); }
    private static bool TryGetBindingPath(string value, out string path) { const string prefix = "{Binding "; path = string.Empty; if (!value.StartsWith(prefix, StringComparison.Ordinal) || !value.EndsWith("}", StringComparison.Ordinal)) return false; path = value.Substring(prefix.Length, value.Length - prefix.Length - 1).Trim(); return path.Length != 0; }
    private static void ApplyBinding(IControl control, string attribute, string path, CxamlLoadContext? context) { if (context?.DataContext is not INotifyPropertyChanged notifyingContext) { if (context?.AllowUnresolvedBindings == true) { ApplyDesignBindingPlaceholder(control, attribute, path); return; } throw new CxamlParseException("CXAML binding '" + path + "' requires an INotifyPropertyChanged data context."); } var property = context.DataContext.GetType().GetProperty(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase); if (property is null) { if (context.AllowUnresolvedBindings) { ApplyDesignBindingPlaceholder(control, attribute, path); return; } throw new CxamlParseException("CXAML binding target was not found: " + path); } if (attribute == "Command" && control is CommandControl commandControl) { if (!typeof(ICommand).IsAssignableFrom(property.PropertyType)) throw new CxamlParseException("CXAML command binding is not an ICommand: " + path); commandControl.Command = property.GetValue(context.DataContext) as ICommand; return; } if (attribute == "Text") { control.Text = property.GetValue(context.DataContext)?.ToString() ?? string.Empty; control.Binding = (notifyingContext, property.Name); return; } if (attribute == "ItemsSource" && control is ListBox listBox) { listBox.ItemsSource = property.GetValue(context.DataContext) as System.Collections.IList ?? throw new CxamlParseException("CXAML ItemsSource binding is not an IList: " + path); return; } throw new CxamlParseException("CXAML binding is not supported for " + attribute + " on " + control.GetType().Name); }
    private static void ApplyDesignBindingPlaceholder(IControl control, string attribute, string path) { if (attribute == "Text") control.Text = "[" + path + "]"; }
    private static bool IsIdentifier(string value) => (char.IsLetter(value[0]) || value[0] == '_') && value.All(character => char.IsLetterOrDigit(character) || character == '_');
}
