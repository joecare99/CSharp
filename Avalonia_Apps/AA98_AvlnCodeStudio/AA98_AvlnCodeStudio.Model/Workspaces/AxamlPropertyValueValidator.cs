using System;
using System.Globalization;
using System.Linq;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

public static class AxamlPropertyValueValidator
{
    private static readonly string[] s_orientations = ["Horizontal", "Vertical"];
    private static readonly string[] s_horizontalAlignments = ["Left", "Center", "Right", "Stretch"];
    private static readonly string[] s_verticalAlignments = ["Top", "Center", "Bottom", "Stretch"];
    private static readonly string[] s_fontWeights = ["Thin", "Light", "Normal", "Medium", "SemiBold", "Bold", "ExtraBold", "Black"];
    private static readonly string[] s_stretches = ["None", "Fill", "Uniform", "UniformToFill"];

    public static AxamlPropertyDescriptor Describe(AxamlPreviewNode node, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        string value = node.Properties.TryGetValue(propertyName, out string? attributeValue)
            ? attributeValue
            : propertyName switch
            {
                "Text" or "Content" or "Header" => node.Text ?? string.Empty,
                "Orientation" => "Vertical",
                "HorizontalAlignment" or "VerticalAlignment" => "Stretch",
                "FontWeight" => "Normal",
                "Stretch" => "Uniform",
                _ => string.Empty,
            };
        (AxamlPropertyEditorKind editor, string[] options) = GetEditor(node.TypeName, propertyName);
        bool readOnly = !new AxamlPreviewService().CanEditProperty(node, propertyName);
        string? reason = readOnly
            ? GetReadOnlyReason(node, propertyName)
            : null;
        string? validationError = readOnly ? null : Validate(node.TypeName, propertyName, value);

        return new AxamlPropertyDescriptor(
            propertyName,
            value,
            editor,
            Array.AsReadOnly(options),
            readOnly,
            reason,
            validationError);
    }

    public static string? Validate(string controlType, string propertyName, string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(controlType);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentNullException.ThrowIfNull(value);

        if (value.TrimStart().StartsWith('{'))
        {
            return "Bindings and resource expressions are read-only in this property editor.";
        }

        if (value.Length == 0)
        {
            return null;
        }

        if (propertyName is "Text" or "Content" or "Header" or "PlaceholderText")
        {
            return null;
        }

        if (propertyName == "IsExpanded" || propertyName == "IsChecked")
        {
            return bool.TryParse(value, out _)
                ? null
                : "Choose True or False.";
        }

        if (propertyName is "Orientation" or "HorizontalAlignment" or "VerticalAlignment" or "FontWeight" or "Stretch")
        {
            string[] options = GetEditor(controlType, propertyName).Options;
            return options.Contains(value, StringComparer.OrdinalIgnoreCase)
                ? null
                : $"Choose one of: {string.Join(", ", options)}.";
        }

        if (propertyName is "Rows" or "Columns" or "FirstColumn")
        {
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int integer) && integer >= 0
                ? null
                : "Enter a non-negative whole number.";
        }

        if (propertyName is "Width" or "Height" or "FontSize" or "Spacing")
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double number)
                && double.IsFinite(number)
                && number >= 0
                    ? null
                    : "Enter a finite, non-negative number using a period as the decimal separator.";
        }

        if (propertyName is "Margin" or "Padding" or "BorderThickness")
        {
            return IsThickness(value)
                ? null
                : "Enter one, two, or four finite numbers separated by commas.";
        }

        if (propertyName is "Background")
        {
            return IsSimpleBrush(value)
                ? null
                : "Use a color name or a hexadecimal color such as #AABBCC.";
        }

        return null;
    }

    private static (AxamlPropertyEditorKind Editor, string[] Options) GetEditor(string controlType, string propertyName)
    {
        if (propertyName is "IsExpanded" or "IsChecked")
        {
            return (AxamlPropertyEditorKind.Boolean, []);
        }

        if (propertyName is "Orientation" or "HorizontalAlignment" or "VerticalAlignment" or "FontWeight" or "Stretch")
        {
            string[] options = propertyName switch
            {
                "Orientation" => s_orientations,
                "HorizontalAlignment" => s_horizontalAlignments,
                "VerticalAlignment" => s_verticalAlignments,
                "FontWeight" => s_fontWeights,
                "Stretch" => s_stretches,
                _ => [],
            };
            return (AxamlPropertyEditorKind.Enum, options);
        }

        if (propertyName is "Width" or "Height" or "FontSize" or "Spacing" or "Rows" or "Columns" or "FirstColumn")
        {
            return (AxamlPropertyEditorKind.Number, []);
        }

        return (AxamlPropertyEditorKind.Text, []);
    }

    private static string? GetReadOnlyReason(AxamlPreviewNode node, string propertyName)
    {
        if (node.Properties.TryGetValue(propertyName, out string? value)
            && value.TrimStart().StartsWith('{'))
        {
            return "Binding or resource expression; editing it is not supported yet.";
        }

        if (node.PropertyElementNames.Contains($"{node.TypeName}.{propertyName}", StringComparer.Ordinal))
        {
            return "Defined with a property element; this form is read-only.";
        }

        if (node.SourceStartOffset < 0)
        {
            return "The source location is not safe to edit in this preview.";
        }

        return "This property is not supported by the current safe editor.";
    }

    private static bool IsThickness(string value)
    {
        string[] parts = value.Split(',', StringSplitOptions.TrimEntries);
        return parts.Length is 1 or 2 or 4
            && parts.All(part => double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out double number)
                && double.IsFinite(number));
    }

    private static bool IsSimpleBrush(string value)
    {
        string trimmed = value.Trim();
        if (trimmed.Length is 4 or 5 or 7 or 9
            && trimmed[0] == '#')
        {
            return trimmed.AsSpan(1).ToString().All(Uri.IsHexDigit);
        }

        return trimmed.All(static character => char.IsLetter(character) || character is ' ' or '-');
    }
}
