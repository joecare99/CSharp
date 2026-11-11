using System;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

public sealed record AxamlBindablePropertyCapability(
    string Name,
    string TypeName,
    bool IsCommand,
    AxamlBindingMode DefaultBindingMode = AxamlBindingMode.Default,
    bool SupportsUpdateSourceTrigger = false);

public sealed record AxamlControlCapability(
    string TypeName,
    IReadOnlyList<string> EditableProperties,
    IReadOnlyList<AxamlBindablePropertyCapability> BindableProperties,
    bool SupportsMvvmGeneration,
    int MaximumChildren,
    bool AllowsAppendingChildren,
    IReadOnlyList<string> AllowedChildTypes,
    bool AllowsSafeChildRemoval = false);

public static class AxamlControlCatalog
{
    private static readonly IReadOnlyList<AxamlControlCapability> s_capabilities =
    [
        Create("AutoCompleteBox", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "FilterMode", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsDropDownOpen", "IsTextCompletionEnabled", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "MinimumPrefixLength", "Opacity", "Padding", "PlaceholderForeground", "PlaceholderText", "Text", "UseLayoutRounding", "VerticalAlignment", "Width"],
            [new("Text", "string?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("Border", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Button", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CommandParameter", "Content", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsCancel", "IsDefault", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("Command", "ICommand", IsCommand: true)], supportsMvvmGeneration: true),
        Create("Canvas", ["Background", "ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowsSafeChildRemoval: true),
        Create("Calendar", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "DisplayDate", "DisplayDateEnd", "DisplayDateStart", "DisplayMode", "FirstDayOfWeek", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsTodayHighlighted", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "SelectionMode", "SelectedDate", "UseLayoutRounding", "VerticalAlignment", "Width"],
            [new("SelectedDate", "DateTime?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("CalendarDatePicker", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsDropDownOpen", "IsTodayHighlighted", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "PlaceholderText", "PlaceholderForeground", "SelectedDate", "UseFloatingPlaceholder", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("SelectedDate", "DateTime?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)],
            supportsMvvmGeneration: true),
        Create("CheckBox", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CommandParameter", "Content", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsChecked", "IsThreeState", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("Command", "ICommand", IsCommand: true), new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ColorPicker", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "Color", "ColorModel", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsAccentColorsVisible", "IsAlphaEnabled", "IsAlphaVisible", "IsColorComponentsVisible", "IsColorModelVisible", "IsColorPaletteVisible", "IsColorPreviewVisible", "IsColorSpectrumSliderVisible", "IsColorSpectrumVisible", "IsComponentSliderVisible", "IsComponentTextInputVisible", "IsHexInputVisible", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "Width"],
            [new("Color", "global::Avalonia.Media.Color", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ComboBox", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsDropDownOpen", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "PlaceholderForeground", "PlaceholderText", "SelectedIndex", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("SelectedIndex", "int", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)],
            supportsMvvmGeneration: true,
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["ComboBoxItem", "Label", "TextBlock"],
            allowsSafeChildRemoval: true),
        Create("ComboBoxItem", ["ClipToBounds", "Content", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("ContextMenu", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["MenuItem", "Separator"]),
        Create("ContentControl", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("DatePicker", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "DayFormat", "DayVisible", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MaxYear", "MinHeight", "MinWidth", "MinYear", "MonthFormat", "MonthVisible", "Opacity", "Padding", "SelectedDate", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width", "YearFormat", "YearVisible"],
            [new("SelectedDate", "DateTimeOffset?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("DockPanel", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "HorizontalSpacing", "IsVisible", "LastChildFill", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "VerticalSpacing", "Width"], maximumChildren: int.MaxValue, allowsAppendingChildren: true, allowsSafeChildRemoval: true),
        Create("Ellipse", ["ClipToBounds", "Fill", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Stroke", "StrokeThickness", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("Expander", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Header", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsExpanded", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Grid", ["ClipToBounds", "ColumnSpacing", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "RowSpacing", "ShowGridLines", "UseLayoutRounding", "VerticalAlignment", "Width"], maximumChildren: int.MaxValue),
        Create("GroupBox", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Header", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Image", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Source", "Stretch", "StretchDirection", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("ItemsControl", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["Label", "TextBlock"],
            allowsSafeChildRemoval: true),
        Create("Label", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "Content", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "LineHeight", "Margin", "MaxHeight", "MaxLines", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "TextAlignment", "TextTrimming", "TextWrapping", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"]),
        Create("Line", ["ClipToBounds", "EndPoint", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "StartPoint", "Stroke", "StrokeThickness", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("ListBox", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "SelectedIndex", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["Label", "ListBoxItem", "TextBlock"],
            allowsSafeChildRemoval: true),
        Create("ListBoxItem", ["ClipToBounds", "Content", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("Menu", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["MenuItem", "Separator"]),
        Create("MenuItem", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CommandParameter", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "GroupName", "Header", "Height", "HorizontalAlignment", "HotKey", "Icon", "InputGesture", "IsChecked", "IsSubMenuOpen", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "StaysOpenOnClick", "ToggleType", "UseLayoutRounding", "VerticalAlignment", "Width"],
            [new("Command", "ICommand", IsCommand: true)],
            supportsMvvmGeneration: true,
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["MenuItem", "Separator"]),
        Create("NumericUpDown", ["AllowSpin", "Background", "BorderBrush", "BorderThickness", "ButtonSpinnerLocation", "ClipToBounds", "ClipValueToMinMax", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "FormatString", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "Increment", "IsVisible", "Margin", "Maximum", "MaxHeight", "MaxWidth", "Minimum", "MinHeight", "MinWidth", "Opacity", "Padding", "ParsingNumberStyle", "PlaceholderForeground", "ShowButtonSpinner", "TextAlignment", "UseLayoutRounding", "Value", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("Value", "decimal?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ProgressBar", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsIndeterminate", "IsVisible", "Margin", "Maximum", "MaxHeight", "MaxWidth", "Minimum", "MinHeight", "MinWidth", "Opacity", "Orientation", "Padding", "ProgressTextFormat", "ShowProgressText", "UseLayoutRounding", "Value", "VerticalAlignment", "Width"]),
        Create("Polygon", ["ClipToBounds", "Fill", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Points", "Stroke", "StrokeThickness", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("Polyline", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Points", "Stroke", "StrokeThickness", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("Rectangle", ["ClipToBounds", "Fill", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Stroke", "StrokeThickness", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("RadioButton", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CommandParameter", "Content", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "GroupName", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsChecked", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("Command", "ICommand", IsCommand: true), new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ScrollViewer", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "HorizontalScrollBarVisibility", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "VerticalScrollBarVisibility", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Separator", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("Slider", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsDirectionReversed", "IsSnapToTickEnabled", "IsVisible", "Margin", "Maximum", "MaxHeight", "MaxWidth", "Minimum", "MinHeight", "MinWidth", "Opacity", "Orientation", "Padding", "TickFrequency", "TickPlacement", "UseLayoutRounding", "Value", "VerticalAlignment", "Width"],
            [new("Value", "double", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("StackPanel", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Orientation", "Spacing", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("TextBlock", ["ClipToBounds", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsVisible", "LineHeight", "Margin", "MaxHeight", "MaxLines", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Text", "TextAlignment", "TextTrimming", "TextWrapping", "UseLayoutRounding", "VerticalAlignment", "Width"],
            [new("Text", "string", IsCommand: false, DefaultBindingMode: AxamlBindingMode.OneWay)], supportsMvvmGeneration: true),
        Create("TextBox", ["AcceptsReturn", "AcceptsTab", "Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsReadOnly", "IsVisible", "LineHeight", "Margin", "MaxHeight", "MaxLength", "MaxLines", "MaxWidth", "MinHeight", "MinLines", "MinWidth", "Opacity", "Padding", "PlaceholderForeground", "PlaceholderText", "Text", "TextAlignment", "TextWrapping", "UseFloatingPlaceholder", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new(
                "Text",
                "string",
                IsCommand: false,
                DefaultBindingMode: AxamlBindingMode.TwoWay,
                SupportsUpdateSourceTrigger: true)], supportsMvvmGeneration: true),
        Create("TimePicker", ["Background", "BorderBrush", "BorderThickness", "ClockIdentifier", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "MinuteIncrement", "Opacity", "Padding", "SecondIncrement", "SelectedTime", "UseLayoutRounding", "UseSeconds", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("SelectedTime", "TimeSpan?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("TreeView", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["TreeViewItem"],
            allowsSafeChildRemoval: true),
        Create("TreeViewItem", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Header", "Height", "HorizontalAlignment", "IsExpanded", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["TreeViewItem"],
            allowsSafeChildRemoval: true),
        Create("ToolTip", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: 1,
            allowsAppendingChildren: true,
            allowedChildTypes: ["Border", "StackPanel", "TextBlock"],
            allowsSafeChildRemoval: true),
        Create("TabControl", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "SelectedIndex", "UseLayoutRounding", "VerticalAlignment", "Width"],
            [new("SelectedIndex", "int", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)],
            supportsMvvmGeneration: true,
            maximumChildren: int.MaxValue,
            allowsAppendingChildren: true,
            allowedChildTypes: ["TabItem"]),
        Create("TabItem", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Header", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            maximumChildren: 1,
            allowsAppendingChildren: true,
            allowsSafeChildRemoval: true),
        Create("ToggleSwitch", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CommandParameter", "Content", "CornerRadius", "FlowDirection", "FontFamily", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsChecked", "IsThreeState", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "OffContent", "OnContent", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("Command", "ICommand", IsCommand: true), new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("UniformGrid", ["ClipToBounds", "ColumnSpacing", "Columns", "FirstColumn", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "RowSpacing", "Rows", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("UserControl", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"], maximumChildren: int.MaxValue),
        Create("Viewbox", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Stretch", "StretchDirection", "UseLayoutRounding", "VerticalAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Window", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"], maximumChildren: int.MaxValue),
        Create("WrapPanel", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "ItemHeight", "ItemSpacing", "ItemWidth", "ItemsAlignment", "LineSpacing", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Orientation", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue, allowsAppendingChildren: true),
    ];

    private static readonly IReadOnlyDictionary<string, AxamlControlCapability> s_capabilitiesByName =
        CreateLookup();

    public static IReadOnlyList<AxamlControlCapability> All => s_capabilities;

    public static bool TryGet(string typeName, out AxamlControlCapability capability)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);
        return s_capabilitiesByName.TryGetValue(typeName, out capability!);
    }

    private static IReadOnlyDictionary<string, AxamlControlCapability> CreateLookup()
    {
        Dictionary<string, AxamlControlCapability> capabilities = new(StringComparer.Ordinal);
        foreach (AxamlControlCapability capability in s_capabilities)
        {
            capabilities.Add(capability.TypeName, capability);
        }

        return capabilities;
    }

    private static AxamlControlCapability Create(
        string typeName,
        string[]? editableProperties = null,
        AxamlBindablePropertyCapability[]? bindableProperties = null,
        bool supportsMvvmGeneration = false,
        int maximumChildren = 0,
        bool allowsAppendingChildren = false,
        string[]? allowedChildTypes = null,
        bool allowsSafeChildRemoval = false)
    {
        string[] properties = editableProperties is null
            ? ["IsEnabled"]
            : [.. editableProperties, "IsEnabled"];
        return new AxamlControlCapability(
            typeName,
            Array.AsReadOnly(properties),
            Array.AsReadOnly(bindableProperties ?? []),
            supportsMvvmGeneration,
            maximumChildren,
            allowsAppendingChildren,
            Array.AsReadOnly(allowedChildTypes ?? []),
            allowsSafeChildRemoval);
    }
}
