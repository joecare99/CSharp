using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using ConsoleLib.Interfaces;
using ConsoleLib.CommonControls;
using ConsoleLib.Data;

namespace ConsoleLib;

/// <summary>Minimal reflection-based CXAML runtime loader for ConsoleLib controls.</summary>
public sealed class CxamlLoader : ICxamlLoader, ICxamlValidator
{
    public IReadOnlyList<CxamlDiagnostic> Validate(TextReader markup)
    {
        if (markup is null)
            throw new ArgumentNullException(nameof(markup));

        var diagnostics = new List<CxamlDiagnostic>();
        try
        {
            using var reader = XmlReader.Create(markup, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true });
            reader.MoveToContent();
            ValidateControl(reader, diagnostics);
        }
        catch (XmlException error)
        {
            diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error, error.Message));
        }
        return diagnostics;
    }

    public IControl Load(TextReader markup)
        => LoadCore(markup, null, new Dictionary<string, IControl>(StringComparer.Ordinal));

    public CxamlLoadResult Load(TextReader markup, CxamlLoadContext context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var namedControls = new Dictionary<string, IControl>(StringComparer.Ordinal);
        return new CxamlLoadResult(LoadCore(markup, context, namedControls), namedControls);
    }

    private static IControl LoadCore(TextReader markup, CxamlLoadContext? context, IDictionary<string, IControl>? namedControls)
    {
        if (markup is null)
            throw new ArgumentNullException(nameof(markup));

        try
        {
            using var reader = XmlReader.Create(markup, new XmlReaderSettings { IgnoreComments = true, IgnoreWhitespace = true });
            if (reader.MoveToContent() == XmlNodeType.None)
                throw new CxamlParseException("CXAML markup does not contain a root control.");
            var control = ReadControl(reader, context, namedControls);
            if (reader.Read() && reader.MoveToContent() == XmlNodeType.Element)
                throw new CxamlParseException("CXAML markup contains more than one root control.");
            return control;
        }
        catch (XmlException error)
        {
            throw new CxamlParseException("Invalid CXAML markup: " + error.Message, error);
        }
    }

    private static void ValidateControl(XmlReader reader, ICollection<CxamlDiagnostic> diagnostics)
    {
        var controlName = reader.LocalName;
        var type = Type.GetType("ConsoleLib.CommonControls." + controlName + ", ConsoleLib", throwOnError: false);
        if (type is null || !typeof(IControl).IsAssignableFrom(type))
            diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error, "Unsupported CXAML control: " + controlName));

        if (reader.HasAttributes)
        {
            while (reader.MoveToNextAttribute())
            {
                if (!IsSupportedAttribute(reader.Name, type))
                    diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error,
                        "Unsupported CXAML attribute '" + reader.LocalName + "' on " + controlName));
            }
            reader.MoveToElement();
        }

        if (reader.IsEmptyElement)
            return;

        var depth = reader.Depth;
        reader.Read();
        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (type == typeof(Grid) && IsGridDefinitionElement(reader.Name))
                    ValidateGridDefinitions(reader, diagnostics);
                else
                    ValidateControl(reader, diagnostics);
            }
            reader.Read();
        }
    }

    private static void ValidateGridDefinitions(XmlReader reader, ICollection<CxamlDiagnostic> diagnostics)
    {
        var isRows = reader.Name == "Grid.RowDefinitions";
        if (reader.IsEmptyElement)
            return;

        var depth = reader.Depth;
        reader.Read();
        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                var expected = isRows ? "RowDefinition" : "ColumnDefinition";
                if (reader.LocalName != expected)
                    diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error,
                        "Unsupported Grid definition element: " + reader.Name));
                else
                {
                    var attribute = isRows ? "Height" : "Width";
                    if (reader.HasAttributes)
                    {
                        while (reader.MoveToNextAttribute())
                            if (reader.LocalName != attribute)
                                diagnostics.Add(new CxamlDiagnostic(CxamlDiagnosticSeverity.Error,
                                    "Unsupported Grid definition attribute '" + reader.Name + "'."));
                        reader.MoveToElement();
                    }
                }
            }
            reader.Read();
        }
    }

    private static IControl ReadControl(XmlReader reader, CxamlLoadContext? context, IDictionary<string, IControl>? namedControls)
    {
        var control = CreateControl(reader.LocalName);
        int? width = null;
        int? height = null;
        int? x = null;
        int? y = null;
        if (reader.HasAttributes)
        {
            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "Width":
                        width = ParseInt(reader.Value, reader.LocalName);
                        break;
                    case "Height":
                        height = ParseInt(reader.Value, reader.LocalName);
                        break;
                    case "X":
                        x = ParseInt(reader.Value, reader.LocalName);
                        break;
                    case "Y":
                        y = ParseInt(reader.Value, reader.LocalName);
                        break;
                    default:
                        ApplyAttribute(control, reader.Name, reader.Value, context, namedControls);
                        break;
                }
            }
            reader.MoveToElement();
        }
        if (width.HasValue || height.HasValue || x.HasValue || y.HasValue)
        {
            control.Dimension = new Rectangle(
                x ?? control.Position.X,
                y ?? control.Position.Y,
                width ?? control.size.Width,
                height ?? control.size.Height);
        }

        if (reader.IsEmptyElement)
            return control;

        var depth = reader.Depth;
        reader.Read();
        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                if (control is Grid grid && IsGridDefinitionElement(reader.Name))
                    ReadGridDefinitions(reader, grid);
                else
                    control.Add(ReadControl(reader, context, namedControls));
            }
            else if (reader.NodeType == XmlNodeType.Text && !string.IsNullOrWhiteSpace(reader.Value))
            {
                control.Text = reader.Value;
            }
            reader.Read();
        }
        return control;
    }

    private static IControl CreateControl(string name)
    {
        var type = Type.GetType("ConsoleLib.CommonControls." + name + ", ConsoleLib", throwOnError: false);
        if (type is null || !typeof(IControl).IsAssignableFrom(type))
            throw new CxamlParseException("Unsupported CXAML control: " + name);
        try
        {
            return (IControl)Activator.CreateInstance(type)!;
        }
        catch (MissingMethodException error)
        {
            throw new CxamlParseException("Unable to create CXAML control: " + name, error);
        }
        catch (MemberAccessException error)
        {
            throw new CxamlParseException("Unable to create CXAML control: " + name, error);
        }
        catch (InvalidCastException error)
        {
            throw new CxamlParseException("Unable to create CXAML control: " + name, error);
        }
        catch (System.Reflection.TargetInvocationException error)
        {
            throw new CxamlParseException("Unable to create CXAML control: " + name, error);
        }
    }

    private static void ApplyAttribute(IControl control, string name, string value, CxamlLoadContext? context, IDictionary<string, IControl>? namedControls)
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
            case "Tag":
                control.Tag = value;
                break;
            case "Accelerator":
                if (value.Length != 1)
                    throw new CxamlParseException("Accelerator must contain exactly one character.");
                control.Accelerator = value[0];
                break;
            case "Shadow":
                control.Shadow = ParseBool(value, name);
                break;
            case "Text":
                control.Text = value;
                break;
            case "Width":
                control.size = new Size(ParseInt(value, name), control.size.Height);
                break;
            case "Height":
                control.size = new Size(control.size.Width, ParseInt(value, name));
                break;
            case "X":
                control.Position = new Point(ParseInt(value, name), control.Position.Y);
                break;
            case "Y":
                control.Position = new Point(control.Position.X, ParseInt(value, name));
                break;
            case "Visible":
                control.Visible = ParseBool(value, name);
                break;
            case "Enabled":
                control.Enabled = ParseBool(value, name);
                break;
            case "BackColor":
                control.BackColor = ParseColor(value, name);
                break;
            case "ForeColor":
                control.ForeColor = ParseColor(value, name);
                break;
            case "BorderStyle":
                SetBorderStyle(control, value);
                break;
            case "BorderColor":
                SetBorderColor(control, value);
                break;
            case "HLBackColor" when control is Button button:
                button.HLBackColor = ParseColor(value, name);
                break;
            case "IsChecked" when control is CheckBox checkBox:
                checkBox.IsChecked = ParseBool(value, name);
                break;
            case "Grid.Row":
                Grid.SetRow(control, ParseInt(value, name));
                break;
            case "Grid.Column":
                Grid.SetColumn(control, ParseInt(value, name));
                break;
            case "Grid.RowSpan":
                Grid.SetRowSpan(control, ParseInt(value, name));
                break;
            case "Grid.ColumnSpan":
                Grid.SetColumnSpan(control, ParseInt(value, name));
                break;
            case "RowDefinitions" when control is Grid grid:
                ParseCompactRows(grid, value);
                break;
            case "ColumnDefinitions" when control is Grid grid:
                ParseCompactColumns(grid, value);
                break;
            default:
                throw new CxamlParseException("Unsupported CXAML attribute '" + name + "' on " + control.GetType().Name);
        }
    }

    private static int ParseInt(string value, string name) =>
        int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : throw new CxamlParseException("Invalid integer value for " + name + ": " + value);

    private static bool ParseBool(string value, string name) =>
        bool.TryParse(value, out var result)
            ? result
            : throw new CxamlParseException("Invalid boolean value for " + name + ": " + value);

    private static ConsoleColor ParseColor(string value, string name) =>
        Enum.TryParse<ConsoleColor>(value, ignoreCase: true, out var result)
            ? result
            : throw new CxamlParseException("Invalid console color value for " + name + ": " + value);

    private static bool IsSupportedAttribute(string name, Type? controlType) =>
        name is "Name" or "Text" or "Width" or "Height" or "X" or "Y" or "Visible" or "Enabled" or "BackColor" or "ForeColor" or "Tag" or "Accelerator" or "Shadow" or "Command" or "ItemsSource"
        || name is "Grid.Row" or "Grid.Column" or "Grid.RowSpan" or "Grid.ColumnSpan"
        || name is "RowDefinitions" or "ColumnDefinitions" && controlType is not null && typeof(Grid).IsAssignableFrom(controlType)
        || name is ("BorderStyle" or "BorderColor") && controlType is not null &&
            (typeof(IHasBorder).IsAssignableFrom(controlType) || typeof(Terminal).IsAssignableFrom(controlType))
        || name == "HLBackColor" && controlType == typeof(Button)
        || name == "IsChecked" && controlType == typeof(CheckBox);

    private static bool IsGridDefinitionElement(string name) =>
        name is "Grid.RowDefinitions" or "Grid.ColumnDefinitions";

    private static void ReadGridDefinitions(XmlReader reader, Grid grid)
    {
        var isRows = reader.Name == "Grid.RowDefinitions";
        if (reader.IsEmptyElement)
            return;

        var depth = reader.Depth;
        reader.Read();
        while (!(reader.NodeType == XmlNodeType.EndElement && reader.Depth == depth))
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                var definitionName = reader.LocalName;
                if (isRows && definitionName == "RowDefinition")
                    grid.RowDefinitions.Add(new RowDefinition { Height = ReadGridLength(reader, "Height") });
                else if (!isRows && definitionName == "ColumnDefinition")
                    grid.ColumnDefinitions.Add(new ColumnDefinition { Width = ReadGridLength(reader, "Width") });
                else
                    throw new CxamlParseException("Unsupported Grid definition element: " + reader.Name);
            }
            reader.Read();
        }
    }

    private static GridLength ReadGridLength(XmlReader reader, string attributeName)
    {
        var value = reader.GetAttribute(attributeName);
        if (string.IsNullOrWhiteSpace(value))
            return GridLength.Star;
        return ParseGridLength(value);
    }

    private static void ParseCompactRows(Grid grid, string value)
    {
        grid.RowDefinitions.Clear();
        foreach (var item in SplitDefinitionList(value))
            grid.RowDefinitions.Add(new RowDefinition { Height = ParseGridLength(item) });
    }

    private static void ParseCompactColumns(Grid grid, string value)
    {
        grid.ColumnDefinitions.Clear();
        foreach (var item in SplitDefinitionList(value))
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = ParseGridLength(item) });
    }

    private static IEnumerable<string> SplitDefinitionList(string value) =>
        value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim())
            .Where(item => item.Length != 0);

    private static GridLength ParseGridLength(string value)
    {
        if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase))
            return GridLength.Auto;
        if (value.EndsWith("*", StringComparison.Ordinal))
        {
            var starValue = value[..^1];
            var weight = starValue.Length == 0
                ? 1
                : double.TryParse(starValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed)
                    ? parsed
                    : throw new CxamlParseException("Invalid Grid star length: " + value);
            return new GridLength(weight, GridUnitType.Star);
        }
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var pixels))
            return new GridLength(pixels, GridUnitType.Pixel);
        throw new CxamlParseException("Invalid Grid length: " + value);
    }

    private static void SetBorderStyle(IControl control, string value)
    {
        if (!Enum.TryParse<BorderStyle>(value, true, out var style))
            throw new CxamlParseException("Invalid border style value: " + value);
        if (control is Panel panel)
            panel.BorderStyle = style;
        else if (control is Terminal terminal)
            terminal.BorderStyle = style;
        else if (control is IHasBorder border)
            border.BorderDefinition.Style = style;
        else
            throw new CxamlParseException("BorderStyle is not supported on " + control.GetType().Name);
    }

    private static void SetBorderColor(IControl control, string value)
    {
        var color = ParseColor(value, "BorderColor");
        if (control is Panel panel)
            panel.BorderColor = color;
        else if (control is Terminal terminal)
            terminal.BorderColor = color;
        else if (control is IHasBorder border)
            border.BorderDefinition.BorderColor = color;
        else
            throw new CxamlParseException("BorderColor is not supported on " + control.GetType().Name);
    }

    private static bool TryGetBindingPath(string value, out string path)
    {
        const string prefix = "{Binding ";
        path = string.Empty;
        if (!value.StartsWith(prefix, StringComparison.Ordinal) || !value.EndsWith("}", StringComparison.Ordinal))
            return false;
        path = value.Substring(prefix.Length, value.Length - prefix.Length - 1).Trim();
        return path.Length != 0;
    }

    private static void ApplyBinding(IControl control, string attribute, string path, CxamlLoadContext? context)
    {
        if (context?.DataContext is not INotifyPropertyChanged notifyingContext)
        {
            if (context?.AllowUnresolvedBindings == true)
            {
                ApplyDesignBindingPlaceholder(control, attribute, path);
                return;
            }
            throw new CxamlParseException("CXAML binding '" + path + "' requires an INotifyPropertyChanged data context.");
        }

        var property = context.DataContext.GetType().GetProperty(path, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (property is null)
        {
            if (context.AllowUnresolvedBindings)
            {
                ApplyDesignBindingPlaceholder(control, attribute, path);
                return;
            }
            throw new CxamlParseException("CXAML binding target was not found: " + path);
        }

        if (attribute == "Command" && control is CommandControl commandControl)
        {
            if (!typeof(ICommand).IsAssignableFrom(property.PropertyType))
                throw new CxamlParseException("CXAML command binding is not an ICommand: " + path);
            commandControl.Command = property.GetValue(context.DataContext) as ICommand;
            return;
        }

        if (attribute == "Text")
        {
            control.Text = property.GetValue(context.DataContext)?.ToString() ?? string.Empty;
            control.Binding = (notifyingContext, property.Name);
            return;
        }

        if (attribute == "ItemsSource" && control is ListBox listBox)
        {
            listBox.ItemsSource = property.GetValue(context.DataContext) as System.Collections.IList
                ?? throw new CxamlParseException("CXAML ItemsSource binding is not an IList: " + path);
            return;
        }

        throw new CxamlParseException("CXAML binding is not supported for " + attribute + " on " + control.GetType().Name);
    }

    private static void ApplyDesignBindingPlaceholder(IControl control, string attribute, string path)
    {
        if (attribute == "Text")
            control.Text = "[" + path + "]";
    }

    private static bool IsIdentifier(string value) =>
        (char.IsLetter(value[0]) || value[0] == '_')
        && value.All(character => char.IsLetterOrDigit(character) || character == '_');
}
