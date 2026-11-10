using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

public static class AxamlPropertyValueValidator
{
    private static readonly string[] s_orientations = ["Horizontal", "Vertical"];
    private static readonly string[] s_tickPlacements = ["None", "TopLeft", "BottomRight", "Outside"];
    private static readonly string[] s_spinnerLocations = ["Left", "Right"];
    private static readonly string[] s_horizontalAlignments = ["Left", "Center", "Right", "Stretch"];
    private static readonly string[] s_verticalAlignments = ["Top", "Center", "Bottom", "Stretch"];
    private static readonly string[] s_flowDirections = ["LeftToRight", "RightToLeft"];
    private static readonly string[] s_fontStyles = ["Normal", "Italic", "Oblique"];
    private static readonly string[] s_fontWeights = ["Thin", "Light", "Normal", "Medium", "SemiBold", "Bold", "ExtraBold", "Black"];
    private static readonly string[] s_stretches = ["None", "Fill", "Uniform", "UniformToFill"];
    private static readonly string[] s_stretchDirections = ["Both", "DownOnly", "UpOnly"];
    private static readonly string[] s_textAlignments = ["Left", "Center", "Right", "Start", "End", "DetectFromContent", "Justify"];
    private static readonly string[] s_textTrimmings = ["None", "CharacterEllipsis", "WordEllipsis"];
    private static readonly string[] s_textWrappings = ["NoWrap", "Wrap", "WrapWithOverflow"];
    private static readonly string[] s_numberStyles = ["Any", "Currency", "Float", "Integer", "None", "Number"];
    private static readonly string[] s_wrapPanelItemsAlignments = ["Start", "Center", "End"];
    private static readonly string[] s_scrollBarVisibilities = ["Auto", "Disabled", "Hidden", "Visible"];
    private static readonly Regex s_progressTextFormatPattern = new(
        @"\A(?:[^{}]|\{\{|\}\}|\{[01](?::(?:[A-Za-z][0-9]{0,2}|[0#]+(?:\.[0#]+)?))?\}){0,64}\z",
        RegexOptions.CultureInvariant);

    public static AxamlPropertyDescriptor Describe(AxamlPreviewNode node, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);

        string value = node.Properties.TryGetValue(propertyName, out string? attributeValue)
            ? attributeValue
            : propertyName switch
            {
                "Text" or "Content" or "Header" => node.Text ?? string.Empty,
                "PlaceholderText" when node.TypeName == "ComboBox" => "ComboBox",
                "GroupName" when node.TypeName == "RadioButton" => string.Empty,
                "Margin" => "0",
                "ClipToBounds" => "False",
                "IsVisible" => "True",
                "UseFloatingPlaceholder" when node.TypeName == "TextBox" => "False",
                "Orientation" when node.TypeName is "ProgressBar" or "Slider" => "Horizontal",
                "Orientation" => "Vertical",
                "Spacing" when node.TypeName == "StackPanel" => "0",
                "ItemSpacing" or "LineSpacing" when node.TypeName == "WrapPanel" => "0",
                "ItemsAlignment" when node.TypeName == "WrapPanel" => "Start",
                "HorizontalSpacing" or "VerticalSpacing" when node.TypeName == "DockPanel" => "0",
                "LastChildFill" when node.TypeName == "DockPanel" => "True",
                "RowSpacing" or "ColumnSpacing" when node.TypeName is "Grid" or "UniformGrid" => "0",
                "ShowGridLines" when node.TypeName == "Grid" => "False",
                "HorizontalScrollBarVisibility" or "VerticalScrollBarVisibility" when node.TypeName == "ScrollViewer" => "Auto",
                "TickFrequency" when node.TypeName == "Slider" => "0",
                "TickPlacement" when node.TypeName == "Slider" => "None",
                "HorizontalAlignment" or "VerticalAlignment" => "Stretch",
                "TextAlignment" when node.TypeName is "TextBlock" or "TextBox" => "Left",
                "TextAlignment" when node.TypeName == "NumericUpDown" => "Start",
                "TextTrimming" when node.TypeName == "TextBlock" => "None",
                "TextWrapping" when node.TypeName == "TextBlock" => "Wrap",
                "TextWrapping" when node.TypeName == "TextBox" => "NoWrap",
                "CornerRadius" when node.TypeName == "Border" => "0",
                "MaxLines" when node.TypeName == "TextBlock" => "0",
                "Opacity" => "1",
                "IsCancel" or "IsDefault" when node.TypeName == "Button" => "False",
                "AllowSpin" or "ShowButtonSpinner" when node.TypeName == "NumericUpDown" => "True",
                "ClipValueToMinMax" when node.TypeName == "NumericUpDown" => "False",
                "ButtonSpinnerLocation" when node.TypeName == "NumericUpDown" => "Right",
                "ParsingNumberStyle" when node.TypeName == "NumericUpDown" => "Any",
                "IsReadOnly" when node.TypeName == "TextBox" => "False",
                "AcceptsReturn" when node.TypeName == "TextBox" => "False",
                "AcceptsTab" when node.TypeName == "TextBox" => "False",
                "MaxLength" when node.TypeName == "TextBox" => "0",
                "IsChecked" when node.TypeName is "CheckBox" or "RadioButton" or "ToggleSwitch" => "False",
                "IsThreeState" when node.TypeName is "CheckBox" or "ToggleSwitch" => "False",
                "FontWeight" => "Normal",
                "FontStyle" when node.TypeName is "TextBlock" or "TextBox" => "Normal",
                "Stretch" => "Uniform",
                "StretchDirection" when node.TypeName == "Viewbox" => "Both",
                "IsIndeterminate" when node.TypeName == "ProgressBar" => "False",
                "ShowProgressText" when node.TypeName == "ProgressBar" => "False",
                "IsDirectionReversed" when node.TypeName == "Slider" => "False",
                "IsSnapToTickEnabled" when node.TypeName == "Slider" => "False",
                "ProgressTextFormat" when node.TypeName == "ProgressBar" => "{1:0}%",
                "Increment" when node.TypeName == "NumericUpDown" => "1",
                "Minimum" when node.TypeName is "ProgressBar" or "Slider" => "0",
                "Maximum" when node.TypeName is "ProgressBar" or "Slider" => "100",
                "Value" when node.TypeName == "Slider" => "50",
                "Value" when node.TypeName == "ProgressBar" => "50",
                _ => string.Empty,
            };
        if (IsFormatProperty(node.TypeName, propertyName) && value.StartsWith("{}", StringComparison.Ordinal))
        {
            value = value[2..];
        }

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

        if (value.TrimStart().StartsWith('{')
            && !IsFormatProperty(controlType, propertyName))
        {
            return "Bindings and resource expressions are read-only in this property editor.";
        }

        if (value.Length == 0 && controlType is "ProgressBar" or "Slider" && propertyName == "Value")
        {
            return "Enter a finite number using a period as the decimal separator.";
        }

        if (value.Length == 0 && controlType == "ProgressBar" && propertyName == "IsIndeterminate")
        {
            return "Choose True or False.";
        }

        if (value.Length == 0
            && propertyName == "IsChecked"
            && controlType is "CheckBox" or "RadioButton" or "ToggleSwitch")
        {
            return "Choose True or False.";
        }

        if (value.Length == 0
            && controlType is "CheckBox" or "ToggleSwitch"
            && propertyName == "IsThreeState")
        {
            return "Choose True or False.";
        }

        if (value.Length == 0 && controlType == "TextBox" && propertyName == "AcceptsReturn")
        {
            return "Choose True or False.";
        }

        if (value.Length == 0 && controlType == "TextBox" && propertyName == "AcceptsTab")
        {
            return "Choose True or False.";
        }

        if (value.Length == 0 && controlType == "Button" && (propertyName is "IsCancel" or "IsDefault"))
        {
            return "Choose True or False.";
        }

        if (value.Length == 0 && controlType == "TextBox" && propertyName == "MaxLength")
        {
            return "Enter a non-negative whole number.";
        }

        if (value.Length == 0)
        {
            return null;
        }

        if (propertyName is "Text" or "Content" or "Header" or "PlaceholderText" or "OffContent" or "OnContent" or "GroupName")
        {
            return null;
        }

        if (propertyName is "IsExpanded" or "IsChecked" or "IsIndeterminate" or "IsDirectionReversed" or "IsReadOnly"
            or "IsSnapToTickEnabled" or "IsThreeState" or "ShowProgressText"
            or "ShowGridLines" or "ClipToBounds" or "IsVisible"
            or "AcceptsReturn" or "AcceptsTab" or "UseFloatingPlaceholder" or "UseLayoutRounding" or "IsCancel" or "IsDefault"
            or "AllowSpin" or "ClipValueToMinMax" or "ShowButtonSpinner" or "LastChildFill")
        {
            return bool.TryParse(value, out _)
                ? null
                : "Choose True or False.";
        }

        if (controlType == "NumericUpDown" && propertyName == "Value")
        {
            return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
                ? null
                : "Enter a decimal number using a period as the decimal separator.";
        }

        if (controlType == "NumericUpDown" && propertyName is "Minimum" or "Maximum")
        {
            return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
                ? null
                : "Enter a decimal number using a period as the decimal separator.";
        }

        if (controlType == "NumericUpDown" && propertyName == "Increment")
        {
            return decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal increment)
                && increment > 0
                    ? null
                    : "Enter a positive decimal increment using a period as the decimal separator.";
        }

        if (controlType == "NumericUpDown" && propertyName == "FormatString")
        {
            return IsSafeNumericFormatString(value)
                ? null
                : "Enter a valid numeric format string no longer than 64 characters.";
        }

        if (controlType == "ProgressBar" && propertyName == "ProgressTextFormat")
        {
            return IsSafeProgressTextFormat(value)
                ? null
                : "Use a supported progress format with only {0} or {1} placeholders and no alignment.";
        }

        if (controlType is "ProgressBar" or "Slider"
            && propertyName is "Minimum" or "Maximum" or "Value")
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double number)
                && double.IsFinite(number)
                    ? null
                    : "Enter a finite number using a period as the decimal separator.";
        }

        if (propertyName is "Orientation" or "TickPlacement" or "ButtonSpinnerLocation" or "ParsingNumberStyle" or "ItemsAlignment" or "HorizontalAlignment" or "VerticalAlignment" or "HorizontalContentAlignment" or "VerticalContentAlignment" or "FlowDirection" or "FontStyle" or "FontWeight" or "Stretch" or "StretchDirection"
            or "HorizontalScrollBarVisibility" or "VerticalScrollBarVisibility"
            or "TextAlignment" or "TextTrimming" or "TextWrapping")
        {
            string[] options = GetEditor(controlType, propertyName).Options;
            return options.Contains(value, StringComparer.OrdinalIgnoreCase)
                ? null
                : $"Choose one of: {string.Join(", ", options)}.";
        }

        if (propertyName is "Rows" or "Columns" or "FirstColumn" or "MaxLength" or "MaxLines" or "MinLines")
        {
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int integer) && integer >= 0
                ? null
                : "Enter a non-negative whole number.";
        }

        if (propertyName is "Width" or "Height" or "MinWidth" or "MaxWidth" or "MinHeight" or "MaxHeight"
            or "FontSize" or "Spacing" or "ItemSpacing" or "LineSpacing" or "ItemWidth" or "ItemHeight"
            or "HorizontalSpacing" or "VerticalSpacing" or "RowSpacing" or "ColumnSpacing")
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double number)
                && double.IsFinite(number)
                && number >= 0
                    ? null
                    : "Enter a finite, non-negative number using a period as the decimal separator.";
        }

        if (propertyName == "LineHeight")
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double lineHeight)
                && double.IsFinite(lineHeight)
                && lineHeight > 0
                    ? null
                    : "Enter a finite, positive line height using a period as the decimal separator.";
        }

        if (propertyName == "Opacity")
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double opacity)
                && double.IsFinite(opacity)
                && opacity is >= 0 and <= 1
                    ? null
                    : "Enter a finite opacity between 0 and 1.";
        }

        if (controlType == "Slider" && propertyName == "TickFrequency")
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double tickFrequency)
                && double.IsFinite(tickFrequency)
                && tickFrequency >= 0
                    ? null
                    : "Enter a finite, non-negative tick frequency using a period as the decimal separator.";
        }

        if (propertyName == "CornerRadius")
        {
            return IsCornerRadius(value)
                ? null
                : "Enter one or four finite, non-negative corner radii separated by commas.";
        }

        if (propertyName is "Margin" or "Padding" or "BorderThickness")
        {
            return IsThickness(value, allowNegative: propertyName == "Margin")
                ? null
                : propertyName == "Margin"
                    ? "Enter one, two, or four finite numbers separated by commas."
                    : "Enter one, two, or four finite, non-negative numbers separated by commas.";
        }

        if (propertyName is "Background" or "BorderBrush" or "Foreground" or "PlaceholderForeground")
        {
            return IsSimpleBrush(value)
                ? null
                : "Use a color name or a hexadecimal color such as #AABBCC.";
        }

        return null;
    }

    private static (AxamlPropertyEditorKind Editor, string[] Options) GetEditor(string controlType, string propertyName)
    {
        if (propertyName is "IsExpanded" or "IsChecked" or "IsIndeterminate" or "IsDirectionReversed" or "IsReadOnly"
            or "IsSnapToTickEnabled" or "IsThreeState" or "ShowProgressText"
            or "ShowGridLines" or "ClipToBounds" or "IsVisible"
            or "AcceptsReturn" or "AcceptsTab" or "UseFloatingPlaceholder" or "UseLayoutRounding" or "IsCancel" or "IsDefault"
            or "AllowSpin" or "ClipValueToMinMax" or "ShowButtonSpinner" or "LastChildFill")
        {
            return (AxamlPropertyEditorKind.Boolean, []);
        }

        if (propertyName is "Orientation" or "TickPlacement" or "ButtonSpinnerLocation" or "ParsingNumberStyle" or "ItemsAlignment" or "HorizontalAlignment" or "VerticalAlignment" or "HorizontalContentAlignment" or "VerticalContentAlignment" or "FlowDirection" or "FontStyle" or "FontWeight" or "Stretch" or "StretchDirection"
            or "HorizontalScrollBarVisibility" or "VerticalScrollBarVisibility"
            or "TextAlignment" or "TextTrimming" or "TextWrapping")
        {
            string[] options = propertyName switch
            {
                "Orientation" => s_orientations,
                "TickPlacement" => s_tickPlacements,
                "ButtonSpinnerLocation" => s_spinnerLocations,
                "ParsingNumberStyle" => s_numberStyles,
                "ItemsAlignment" => s_wrapPanelItemsAlignments,
                "HorizontalAlignment" => s_horizontalAlignments,
                "VerticalAlignment" => s_verticalAlignments,
                "HorizontalContentAlignment" => s_horizontalAlignments,
                "VerticalContentAlignment" => s_verticalAlignments,
                "FlowDirection" => s_flowDirections,
                "FontStyle" => s_fontStyles,
                "FontWeight" => s_fontWeights,
                "Stretch" => s_stretches,
                "StretchDirection" => s_stretchDirections,
                "TextAlignment" => s_textAlignments,
                "TextTrimming" => s_textTrimmings,
                "TextWrapping" => s_textWrappings,
                "HorizontalScrollBarVisibility" or "VerticalScrollBarVisibility" => s_scrollBarVisibilities,
                _ => [],
            };
            return (AxamlPropertyEditorKind.Enum, options);
        }

        if (propertyName is "Width" or "Height" or "MinWidth" or "MaxWidth" or "MinHeight" or "MaxHeight"
            or "FontSize" or "LineHeight" or "Opacity" or "Spacing" or "ItemSpacing" or "LineSpacing" or "ItemWidth" or "ItemHeight"
            or "HorizontalSpacing" or "VerticalSpacing" or "RowSpacing" or "ColumnSpacing"
            or "Rows" or "Columns" or "FirstColumn" or "MaxLength" or "MaxLines" or "MinLines"
            || controlType == "Slider" && propertyName == "TickFrequency"
            || controlType is "ProgressBar" or "Slider" && propertyName is "Minimum" or "Maximum" or "Value"
            || controlType == "NumericUpDown" && propertyName is "Increment" or "Minimum" or "Maximum" or "Value")
        {
            return (AxamlPropertyEditorKind.Number, []);
        }

        return (AxamlPropertyEditorKind.Text, []);
    }

    private static string? GetReadOnlyReason(AxamlPreviewNode node, string propertyName)
    {
        if (node.Properties.TryGetValue(propertyName, out string? value)
            && value.TrimStart().StartsWith('{')
            && !(IsFormatProperty(node.TypeName, propertyName)
                && value.TrimStart().StartsWith("{}", StringComparison.Ordinal)))
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

    private static bool IsThickness(string value, bool allowNegative)
    {
        string[] parts = value.Split(',', StringSplitOptions.TrimEntries);
        return parts.Length is 1 or 2 or 4
            && parts.All(part => double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out double number)
                && double.IsFinite(number)
                && (allowNegative || number >= 0));
    }

    private static bool IsCornerRadius(string value)
    {
        string[] parts = value.Split(',', StringSplitOptions.TrimEntries);
        return parts.Length is 1 or 4
            && parts.All(part => double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out double number)
                && double.IsFinite(number)
                && number >= 0);
    }

    private static bool IsSimpleBrush(string value)
    {
        string trimmed = value.Trim();
        if (trimmed.Length is 4 or 5 or 7 or 9
            && trimmed[0] == '#')
        {
            return trimmed.AsSpan(1).ToString().All(Uri.IsHexDigit);
        }

        System.Drawing.Color namedColor = System.Drawing.Color.FromName(trimmed);
        return namedColor.IsKnownColor && !namedColor.IsSystemColor;
    }

    internal static bool IsFormatProperty(string controlType, string propertyName)
    {
        return controlType == "NumericUpDown" && propertyName == "FormatString"
            || controlType == "ProgressBar" && propertyName == "ProgressTextFormat";
    }

    private static bool IsSafeNumericFormatString(string value)
    {
        if (value.Length > 64)
        {
            return false;
        }

        try
        {
            _ = 1234.5m.ToString(value, CultureInfo.InvariantCulture);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static bool IsSafeProgressTextFormat(string value)
    {
        if (value.Length > 64 || !s_progressTextFormatPattern.IsMatch(value))
        {
            return false;
        }

        try
        {
            _ = string.Format(CultureInfo.InvariantCulture, value, 42.5d, 0.425d);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
