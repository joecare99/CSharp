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
    private static readonly string[] s_dockPanelPlacements = ["Left", "Top", "Right", "Bottom"];
    private static readonly string[] s_horizontalAlignments = ["Left", "Center", "Right", "Stretch"];
    private static readonly string[] s_verticalAlignments = ["Top", "Center", "Bottom", "Stretch"];
    private static readonly string[] s_flowDirections = ["LeftToRight", "RightToLeft"];
    private static readonly string[] s_clockIdentifiers = ["12HourClock", "24HourClock"];
    private static readonly string[] s_dayOfWeeks = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    private static readonly string[] s_calendarDisplayModes = ["Month", "Year", "Decade"];
    private static readonly string[] s_calendarSelectionModes = ["None", "SingleDate", "SingleRange", "MultipleRange"];
    private static readonly string[] s_menuItemToggleTypes = ["None", "CheckBox", "Radio"];
    private static readonly string[] s_colorModels = ["Hsva", "Rgba"];
    private static readonly string[] s_autoCompleteFilterModes =
    [
        "None",
        "StartsWith",
        "StartsWithCaseSensitive",
        "StartsWithOrdinal",
        "StartsWithOrdinalCaseSensitive",
        "Contains",
        "ContainsCaseSensitive",
        "ContainsOrdinal",
        "ContainsOrdinalCaseSensitive",
        "Equals",
        "EqualsCaseSensitive",
        "EqualsOrdinal",
        "EqualsOrdinalCaseSensitive",
    ];
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
    private static readonly string[] s_keyGestureKeys =
    [
        "Back", "Tab", "LineFeed", "Clear", "Return", "Enter", "Pause", "CapsLock", "Escape", "Space",
        "PageUp", "PageDown", "End", "Home", "Left", "Up", "Right", "Down", "Select", "Print",
        "Execute", "Snapshot", "PrintScreen", "Insert", "Delete", "Help", "LWin", "RWin", "Apps", "Sleep",
        "Multiply", "Add", "Separator", "Subtract", "Decimal", "Divide", "NumLock", "Scroll",
        "BrowserBack", "BrowserForward", "BrowserRefresh", "BrowserStop", "BrowserSearch", "BrowserFavorites",
        "BrowserHome", "VolumeMute", "VolumeDown", "VolumeUp", "MediaNextTrack", "MediaPreviousTrack",
        "MediaStop", "MediaPlayPause", "LaunchMail", "SelectMedia", "LaunchApplication1", "LaunchApplication2",
        "OemSemicolon", "OemPlus", "OemComma", "OemMinus", "OemPeriod", "OemQuestion", "OemTilde",
        "AbntC1", "AbntC2", "OemOpenBrackets", "OemPipe", "OemCloseBrackets", "OemQuotes", "Oem8",
        "OemBackslash", "Oem102", "ImeProcessed", "System", "OemAttn", "OemFinish", "OemCopy", "OemAuto",
        "OemEnlw", "OemBackTab", "Attn", "CrSel", "ExSel", "EraseEof", "Play", "Zoom", "NoName",
        "Pa1", "OemClear", "DeadCharProcessed", "FnLeftArrow", "FnRightArrow", "FnUpArrow", "FnDownArrow",
        "MediaHome", "MediaChannelList", "MediaChannelRaise", "MediaChannelLower", "MediaRecord", "MediaRed",
        "MediaGreen", "MediaYellow", "MediaBlue", "MediaMenu", "MediaMore", "MediaOption", "MediaInfo",
        "MediaSearch", "MediaSubtitle", "MediaTvGuide", "MediaPreviousChannel",
    ];
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
                "PlaceholderText" when node.TypeName == "CalendarDatePicker" => "Choose a date",
                "GroupName" when node.TypeName == "RadioButton" => string.Empty,
                "Margin" => "0",
                "ClipToBounds" => "False",
                "IsEnabled" => "True",
                "IsVisible" => "True",
                "IsDropDownOpen" when node.TypeName is "AutoCompleteBox" or "ComboBox" or "CalendarDatePicker" => "False",
                "IsChecked" when node.TypeName == "MenuItem" => "False",
                "IsSubMenuOpen" when node.TypeName == "MenuItem" => "False",
                "StaysOpenOnClick" when node.TypeName == "MenuItem" => "False",
                "GroupName" when node.TypeName == "MenuItem" => string.Empty,
                "ToggleType" when node.TypeName == "MenuItem" => "None",
                "UseFloatingPlaceholder" when node.TypeName is "TextBox" or "CalendarDatePicker" => "False",
                "Orientation" when node.TypeName is "ProgressBar" or "Slider" => "Horizontal",
                "Orientation" => "Vertical",
                "Spacing" when node.TypeName == "StackPanel" => "0",
                "ItemSpacing" or "LineSpacing" when node.TypeName == "WrapPanel" => "0",
                "ItemsAlignment" when node.TypeName == "WrapPanel" => "Start",
                "HorizontalSpacing" or "VerticalSpacing" when node.TypeName == "DockPanel" => "0",
                "LastChildFill" when node.TypeName == "DockPanel" => "True",
                "DockPanel.Dock" when node.ParentTypeName == "DockPanel" => "Left",
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
                "StartPoint" when node.TypeName == "Line" => "0,0",
                "EndPoint" when node.TypeName == "Line" => "80,40",
                "Points" when node.TypeName == "Polygon" => "40,0 80,60 0,60",
                "Points" when node.TypeName == "Polyline" => "0,0 80,40",
                "IsExpanded" when node.TypeName == "TreeViewItem" => "False",
                "SelectedIndex" when node.TypeName is "ComboBox" or "ListBox" or "TabControl" => "-1",
                "DayVisible" or "MonthVisible" or "YearVisible" when node.TypeName == "DatePicker" => "True",
                "UseSeconds" when node.TypeName == "TimePicker" => "False",
                "ClockIdentifier" when node.TypeName == "TimePicker" => "24HourClock",
                "MinuteIncrement" or "SecondIncrement" when node.TypeName == "TimePicker" => "1",
                "SelectionMode" when node.TypeName == "Calendar" => "SingleDate",
                "DisplayMode" when node.TypeName == "Calendar" => "Month",
                "FirstDayOfWeek" when node.TypeName == "Calendar" => "Monday",
                "IsTodayHighlighted" when node.TypeName is "Calendar" or "CalendarDatePicker" => "True",
                "FilterMode" when node.TypeName == "AutoCompleteBox" => "StartsWith",
                "MinimumPrefixLength" when node.TypeName == "AutoCompleteBox" => "1",
                "IsTextCompletionEnabled" when node.TypeName == "AutoCompleteBox" => "False",
                "Color" when node.TypeName == "ColorPicker" => "White",
                "ColorModel" when node.TypeName == "ColorPicker" => "Rgba",
                "IsAccentColorsVisible" or "IsAlphaEnabled" or "IsAlphaVisible"
                    or "IsColorComponentsVisible" or "IsColorModelVisible" or "IsColorPaletteVisible"
                    or "IsColorPreviewVisible" or "IsColorSpectrumSliderVisible" or "IsColorSpectrumVisible"
                    or "IsComponentSliderVisible" or "IsComponentTextInputVisible" or "IsHexInputVisible"
                    when node.TypeName == "ColorPicker" => "True",
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
            && controlType is "CheckBox" or "RadioButton" or "ToggleSwitch" or "MenuItem")
        {
            return "Choose True or False.";
        }

        if (value.Length == 0 && controlType == "MenuItem" && propertyName == "IsSubMenuOpen")
        {
            return "Choose True or False.";
        }

        if (value.Length == 0 && controlType == "MenuItem" && propertyName == "StaysOpenOnClick")
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

        if (value.Length == 0 && controlType == "Line" && propertyName is "StartPoint" or "EndPoint")
        {
            return "Enter two finite point coordinates separated by a comma.";
        }

        if (value.Length == 0 && (controlType is "Polyline" or "Polygon") && propertyName == "Points")
        {
            return "Enter a list of finite point coordinates.";
        }

        if (controlType is "ComboBox" or "ListBox" or "TabControl" && propertyName == "SelectedIndex")
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int selectedIndex)
                && selectedIndex >= -1
                    ? null
                    : "Enter -1 for no selection or a non-negative whole number.";
        }

        if (value.Length == 0
            && controlType == "DatePicker"
            && (propertyName is "DayVisible" or "MonthVisible" or "YearVisible"))
        {
            return "Choose True or False.";
        }

        if (value.Length == 0)
        {
            return null;
        }

        if (propertyName == "CommandParameter" && value.TrimStart().StartsWith('{'))
        {
            return "Use a plain literal string; bindings and markup extensions are not evaluated.";
        }

        if (propertyName is "Canvas.Left" or "Canvas.Top")
        {
            return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double offset)
                && double.IsFinite(offset)
                && offset is >= -100_000 and <= 100_000
                    ? null
                    : "Enter a finite canvas coordinate between -100000 and 100000.";
        }

        if (controlType == "Image" && propertyName == "Source")
        {
            return value.Length <= 256 && !value.Any(char.IsControl)
                ? null
                : "Enter an image source reference of at most 256 characters without control characters.";
        }

        if (propertyName is "Text" or "Content" or "Header" or "Icon" or "PlaceholderText" or "OffContent" or "OnContent" or "GroupName" or "CommandParameter")
        {
            return null;
        }

        if (controlType == "MenuItem" && propertyName is "HotKey" or "InputGesture")
        {
            return IsValidInputGesture(value)
                ? null
                : "Enter one Avalonia key gesture such as Ctrl+S.";
        }

        if ((controlType is "DatePicker" or "CalendarDatePicker") && propertyName == "SelectedDate")
        {
            return DateTimeOffset.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _)
                    ? null
                    : "Enter a valid date in yyyy-MM-dd format.";
        }

        if (controlType == "DatePicker" && propertyName is "DayFormat" or "MonthFormat" or "YearFormat")
        {
            return IsSafeDateFormatString(value)
                ? null
                : "Enter a valid date format string no longer than 32 characters.";
        }

        if (controlType == "DatePicker" && propertyName is "MinYear" or "MaxYear")
        {
            return DateTimeOffset.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _)
                    ? null
                    : "Enter a valid date in yyyy-MM-dd format.";
        }

        if (controlType == "TimePicker" && propertyName == "SelectedTime")
        {
            return TimeSpan.TryParseExact(
                value,
                ["hh\\:mm", "hh\\:mm\\:ss"],
                CultureInfo.InvariantCulture,
                TimeSpanStyles.None,
                out _)
                    ? null
                    : "Enter a valid time in HH:mm or HH:mm:ss format.";
        }

        if (controlType == "Calendar"
            && propertyName is "SelectedDate" or "DisplayDate" or "DisplayDateStart" or "DisplayDateEnd")
        {
            return DateTime.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _)
                    ? null
                    : "Enter a valid date in yyyy-MM-dd format.";
        }

        if (controlType == "TimePicker" && propertyName is "MinuteIncrement" or "SecondIncrement")
        {
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int increment)
                && increment is >= 1 and <= 59
                    ? null
                    : "Enter a whole-number increment between 1 and 59.";
        }

        if (controlType == "ColorPicker" && propertyName == "Color")
        {
            return IsSimpleBrush(value)
                ? null
                : "Use a named color or a valid hexadecimal color such as #AABBCC.";
        }

        if (propertyName is "IsExpanded" or "IsChecked" or "IsIndeterminate" or "IsDirectionReversed" or "IsReadOnly"
            or "IsSnapToTickEnabled" or "IsThreeState" or "ShowProgressText"
            or "ShowGridLines" or "ClipToBounds" or "IsEnabled" or "IsVisible" or "IsTodayHighlighted" or "IsDropDownOpen" or "IsSubMenuOpen" or "StaysOpenOnClick"
            or "AcceptsReturn" or "AcceptsTab" or "UseFloatingPlaceholder" or "UseLayoutRounding" or "IsCancel" or "IsDefault" or "UseSeconds"
            or "DayVisible" or "MonthVisible" or "YearVisible" or "IsTextCompletionEnabled"
            or "IsAccentColorsVisible" or "IsAlphaEnabled" or "IsAlphaVisible"
            or "IsColorComponentsVisible" or "IsColorModelVisible" or "IsColorPaletteVisible"
            or "IsColorPreviewVisible" or "IsColorSpectrumSliderVisible" or "IsColorSpectrumVisible"
            or "IsComponentSliderVisible" or "IsComponentTextInputVisible" or "IsHexInputVisible"
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

        if (propertyName is "Orientation" or "TickPlacement" or "ButtonSpinnerLocation" or "ParsingNumberStyle" or "ItemsAlignment" or "HorizontalAlignment" or "VerticalAlignment" or "HorizontalContentAlignment" or "VerticalContentAlignment" or "FlowDirection" or "FontStyle" or "FontWeight" or "Stretch" or "StretchDirection" or "ClockIdentifier" or "ColorModel" or "FilterMode" or "ToggleType"
            or "HorizontalScrollBarVisibility" or "VerticalScrollBarVisibility"
            or "TextAlignment" or "TextTrimming" or "TextWrapping" or "DockPanel.Dock"
            or "FirstDayOfWeek" or "SelectionMode" or "DisplayMode")
        {
            string[] options = GetEditor(controlType, propertyName).Options;
            return options.Contains(value, StringComparer.OrdinalIgnoreCase)
                ? null
                : $"Choose one of: {string.Join(", ", options)}.";
        }

        if (controlType == "AutoCompleteBox" && propertyName == "MinimumPrefixLength")
        {
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int prefixLength)
                && prefixLength >= 0
                    ? null
                    : "Enter a non-negative whole-number prefix length.";
        }

        if (propertyName is "Rows" or "Columns" or "FirstColumn" or "MaxLength" or "MaxLines" or "MinLines")
        {
            return int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out int integer) && integer >= 0
                ? null
                : "Enter a non-negative whole number.";
        }

        if (propertyName is "Width" or "Height" or "MinWidth" or "MaxWidth" or "MinHeight" or "MaxHeight" or "StrokeThickness"
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

        if (propertyName is "Background" or "BorderBrush" or "Fill" or "Foreground" or "PlaceholderForeground" or "Stroke")
        {
            return IsSimpleBrush(value)
                ? null
                : "Use a color name or a hexadecimal color such as #AABBCC.";
        }

        if (propertyName == "FontFamily")
        {
            return IsSafeFontFamily(value)
                ? null
                : "Enter a plain font family name using letters, numbers, spaces, dots, underscores or hyphens.";
        }

        if (controlType == "Line" && propertyName is "StartPoint" or "EndPoint")
        {
            return IsSafePoint(value)
                ? null
                : "Enter two finite point coordinates separated by a comma; each coordinate must be between -10000 and 10000.";
        }

        if ((controlType is "Polyline" or "Polygon") && propertyName == "Points")
        {
            return IsSafePointList(controlType, value)
                ? null
                : controlType == "Polygon"
                    ? "Enter 3 to 128 finite coordinate pairs separated by spaces; each coordinate must be between -10000 and 10000."
                    : "Enter 2 to 128 finite coordinate pairs separated by spaces; each coordinate must be between -10000 and 10000.";
        }

        return null;
    }

    private static (AxamlPropertyEditorKind Editor, string[] Options) GetEditor(string controlType, string propertyName)
    {
        if (propertyName is "IsExpanded" or "IsChecked" or "IsIndeterminate" or "IsDirectionReversed" or "IsReadOnly"
            or "IsSnapToTickEnabled" or "IsThreeState" or "ShowProgressText"
            or "ShowGridLines" or "ClipToBounds" or "IsEnabled" or "IsVisible" or "IsTodayHighlighted" or "IsDropDownOpen" or "IsSubMenuOpen" or "StaysOpenOnClick"
            or "AcceptsReturn" or "AcceptsTab" or "UseFloatingPlaceholder" or "UseLayoutRounding" or "IsCancel" or "IsDefault"
            or "DayVisible" or "MonthVisible" or "YearVisible" or "IsTextCompletionEnabled"
            or "IsAccentColorsVisible" or "IsAlphaEnabled" or "IsAlphaVisible"
            or "IsColorComponentsVisible" or "IsColorModelVisible" or "IsColorPaletteVisible"
            or "IsColorPreviewVisible" or "IsColorSpectrumSliderVisible" or "IsColorSpectrumVisible"
            or "IsComponentSliderVisible" or "IsComponentTextInputVisible" or "IsHexInputVisible"
            or "AllowSpin" or "ClipValueToMinMax" or "ShowButtonSpinner" or "LastChildFill")
        {
            return (AxamlPropertyEditorKind.Boolean, []);
        }

        if (propertyName is         "Orientation" or "TickPlacement" or "ButtonSpinnerLocation" or "ParsingNumberStyle" or "ItemsAlignment" or "HorizontalAlignment" or "VerticalAlignment" or "HorizontalContentAlignment" or "VerticalContentAlignment" or "FlowDirection" or "FontStyle" or "FontWeight" or "Stretch" or "StretchDirection" or "ClockIdentifier" or "ColorModel" or "FilterMode" or "ToggleType" or "DockPanel.Dock"
            or "HorizontalScrollBarVisibility" or "VerticalScrollBarVisibility"
            or "TextAlignment" or "TextTrimming" or "TextWrapping" or "FirstDayOfWeek" or "SelectionMode" or "DisplayMode")
        {
            string[] options = propertyName switch
            {
                "Orientation" => s_orientations,
                "TickPlacement" => s_tickPlacements,
                "ButtonSpinnerLocation" => s_spinnerLocations,
                "DockPanel.Dock" => s_dockPanelPlacements,
                "ParsingNumberStyle" => s_numberStyles,
                "ItemsAlignment" => s_wrapPanelItemsAlignments,
                "HorizontalAlignment" => s_horizontalAlignments,
                "VerticalAlignment" => s_verticalAlignments,
                "HorizontalContentAlignment" => s_horizontalAlignments,
                "VerticalContentAlignment" => s_verticalAlignments,
                "FlowDirection" => s_flowDirections,
                "ClockIdentifier" => s_clockIdentifiers,
                "ColorModel" => s_colorModels,
                "FilterMode" => s_autoCompleteFilterModes,
                "FirstDayOfWeek" => s_dayOfWeeks,
                "SelectionMode" => s_calendarSelectionModes,
                "DisplayMode" => s_calendarDisplayModes,
                "ToggleType" => s_menuItemToggleTypes,
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

        if (controlType is "ComboBox" or "ListBox" or "TabControl" && propertyName == "SelectedIndex")
        {
            return (AxamlPropertyEditorKind.Number, []);
        }

        if (propertyName is "Width" or "Height" or "MinWidth" or "MaxWidth" or "MinHeight" or "MaxHeight" or "MinimumPrefixLength"
            or "FontSize" or "LineHeight" or "Opacity" or "Spacing" or "StrokeThickness" or "ItemSpacing" or "LineSpacing" or "ItemWidth" or "ItemHeight"
            or "HorizontalSpacing" or "VerticalSpacing" or "RowSpacing" or "ColumnSpacing"
            or "Rows" or "Columns" or "FirstColumn" or "MaxLength" or "MaxLines" or "MinLines"
            || controlType == "Slider" && propertyName == "TickFrequency"
            || controlType is "ProgressBar" or "Slider" && propertyName is "Minimum" or "Maximum" or "Value"
            || controlType == "NumericUpDown" && propertyName is "Increment" or "Minimum" or "Maximum" or "Value"
            || controlType == "TimePicker" && propertyName is "MinuteIncrement" or "SecondIncrement")
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

    private static bool IsValidInputGesture(string value)
    {
        string normalized = value.Trim();
        if (normalized.Length is 0 or > 80)
        {
            return false;
        }

        bool isPlusKey = normalized.EndsWith("++", StringComparison.Ordinal);
        string[] parts = (isPlusKey ? normalized[..^1] : normalized)
            .Split('+', StringSplitOptions.TrimEntries);
        if (parts.Length == 0 || parts.Any(static part => part.Length == 0) != isPlusKey)
        {
            return false;
        }

        int modifierCount = parts.Length - 1;
        if (modifierCount < 0
            || parts.Take(modifierCount).Any(static modifier => !IsSupportedInputGestureModifier(modifier))
            || parts.Take(modifierCount).Distinct(StringComparer.OrdinalIgnoreCase).Count() != modifierCount)
        {
            return false;
        }

        return isPlusKey || IsSupportedInputGestureKey(parts[^1]);
    }

    private static bool IsSupportedInputGestureKey(string key)
    {
        if (key.Length == 1 && key[0] is >= 'A' and <= 'Z' or >= 'a' and <= 'z')
        {
            return true;
        }

        if (key.Length == 2 && (key[0] is 'D' or 'd') && char.IsAsciiDigit(key[1]))
        {
            return true;
        }

        if (key.StartsWith("NumPad", StringComparison.OrdinalIgnoreCase)
            && key.Length == 7
            && char.IsAsciiDigit(key[^1]))
        {
            return true;
        }

        if (key.StartsWith('F')
            || key.StartsWith('f'))
        {
            return int.TryParse(key.AsSpan(1), NumberStyles.None, CultureInfo.InvariantCulture, out int functionKey)
                && functionKey is >= 1 and <= 24
                && string.Equals(key, $"F{functionKey}", StringComparison.OrdinalIgnoreCase);
        }

        return s_keyGestureKeys.Contains(key, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsSupportedInputGestureModifier(string modifier)
    {
        return modifier.Equals("Ctrl", StringComparison.OrdinalIgnoreCase)
            || modifier.Equals("Shift", StringComparison.OrdinalIgnoreCase)
            || modifier.Equals("Alt", StringComparison.OrdinalIgnoreCase)
            || modifier.Equals("Meta", StringComparison.OrdinalIgnoreCase);
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

    private static bool IsSafeFontFamily(string value)
    {
        return value.Length is > 0 and <= 64
            && string.Equals(value, value.Trim(), StringComparison.Ordinal)
            && char.IsLetterOrDigit(value[0])
            && value.All(static character =>
                char.IsLetterOrDigit(character)
                || character is ' ' or '.' or '_' or '-');
    }

    private static bool IsSafePoint(string value)
    {
        string[] coordinates = value.Split(',', StringSplitOptions.TrimEntries);
        return value.Length <= 64
            && coordinates.Length == 2
            && coordinates.All(static coordinate =>
                double.TryParse(coordinate, NumberStyles.Float, CultureInfo.InvariantCulture, out double number)
                && double.IsFinite(number)
                && number is >= -10000 and <= 10000);
    }

    private static bool IsSafePointList(string controlType, string value)
    {
        string[] points = value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        int minimumCount = controlType == "Polygon" ? 3 : 2;
        return value.Length <= 2048
            && points.Length >= minimumCount
            && points.Length <= 128
            && points.All(IsSafePoint);
    }

    internal static bool IsFormatProperty(string controlType, string propertyName)
    {
        return controlType == "NumericUpDown" && propertyName == "FormatString"
            || controlType == "ProgressBar" && propertyName == "ProgressTextFormat"
            || controlType == "DatePicker" && propertyName is "DayFormat" or "MonthFormat" or "YearFormat";
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

    private static bool IsSafeDateFormatString(string value)
    {
        if (value.Length > 32)
        {
            return false;
        }

        try
        {
            _ = DateTimeOffset.UnixEpoch.ToString(value, CultureInfo.InvariantCulture);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
