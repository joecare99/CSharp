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
    bool AllowsSafeChildRemoval = false);

public static class AxamlControlCatalog
{
    private static readonly IReadOnlyList<AxamlControlCapability> s_capabilities =
    [
        Create("Border", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Button", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "Content", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsCancel", "IsDefault", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("Command", "ICommand", IsCommand: true)], supportsMvvmGeneration: true),
        Create("CheckBox", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "Content", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsChecked", "IsThreeState", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ComboBox", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "PlaceholderText", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("SelectedIndex", "int", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ContentControl", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("DockPanel", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "HorizontalSpacing", "IsVisible", "LastChildFill", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "VerticalSpacing", "Width"], maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("Expander", ["ClipToBounds", "FlowDirection", "Header", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsExpanded", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Grid", ["ClipToBounds", "ColumnSpacing", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "RowSpacing", "ShowGridLines", "UseLayoutRounding", "VerticalAlignment", "Width"], maximumChildren: int.MaxValue),
        Create("GroupBox", ["ClipToBounds", "FlowDirection", "Header", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Label", ["ClipToBounds", "Content", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MaxLines", "MinHeight", "MinWidth", "Opacity", "TextAlignment", "TextTrimming", "TextWrapping", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"]),
        Create("ListBox", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("NumericUpDown", ["AllowSpin", "Background", "BorderBrush", "BorderThickness", "ButtonSpinnerLocation", "ClipToBounds", "ClipValueToMinMax", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "FormatString", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "Increment", "IsVisible", "Margin", "Maximum", "MaxHeight", "MaxWidth", "Minimum", "MinHeight", "MinWidth", "Opacity", "Padding", "ParsingNumberStyle", "ShowButtonSpinner", "TextAlignment", "UseLayoutRounding", "Value", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("Value", "decimal?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ProgressBar", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsIndeterminate", "IsVisible", "Margin", "Maximum", "MaxHeight", "MaxWidth", "Minimum", "MinHeight", "MinWidth", "Opacity", "Orientation", "Padding", "ProgressTextFormat", "ShowProgressText", "UseLayoutRounding", "Value", "VerticalAlignment", "Width"]),
        Create("RadioButton", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "Content", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "GroupName", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsChecked", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ScrollViewer", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "HorizontalScrollBarVisibility", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "VerticalScrollBarVisibility", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Separator", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "UseLayoutRounding", "VerticalAlignment", "Width"]),
        Create("Slider", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsDirectionReversed", "IsSnapToTickEnabled", "IsVisible", "Margin", "Maximum", "MaxHeight", "MaxWidth", "Minimum", "MinHeight", "MinWidth", "Opacity", "Orientation", "Padding", "TickFrequency", "TickPlacement", "UseLayoutRounding", "Value", "VerticalAlignment", "Width"],
            [new("Value", "double", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("StackPanel", ["ClipToBounds", "FlowDirection", "Height", "HorizontalAlignment", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Orientation", "Spacing", "UseLayoutRounding", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("TextBlock", ["ClipToBounds", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsVisible", "LineHeight", "Margin", "MaxHeight", "MaxLines", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Text", "TextAlignment", "TextTrimming", "TextWrapping", "UseLayoutRounding", "VerticalAlignment", "Width"],
            [new("Text", "string", IsCommand: false, DefaultBindingMode: AxamlBindingMode.OneWay)], supportsMvvmGeneration: true),
        Create("TextBox", ["AcceptsReturn", "AcceptsTab", "Background", "BorderBrush", "BorderThickness", "ClipToBounds", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsReadOnly", "IsVisible", "LineHeight", "Margin", "MaxHeight", "MaxLength", "MaxLines", "MaxWidth", "MinHeight", "MinLines", "MinWidth", "Opacity", "Padding", "PlaceholderForeground", "PlaceholderText", "Text", "TextAlignment", "TextWrapping", "UseFloatingPlaceholder", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new(
                "Text",
                "string",
                IsCommand: false,
                DefaultBindingMode: AxamlBindingMode.TwoWay,
                SupportsUpdateSourceTrigger: true)], supportsMvvmGeneration: true),
        Create("ToggleSwitch", ["Background", "BorderBrush", "BorderThickness", "ClipToBounds", "Content", "CornerRadius", "FlowDirection", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "HorizontalContentAlignment", "IsChecked", "IsThreeState", "IsVisible", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "OffContent", "OnContent", "Opacity", "Padding", "UseLayoutRounding", "VerticalAlignment", "VerticalContentAlignment", "Width"],
            [new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
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
        bool allowsSafeChildRemoval = false)
    {
        return new AxamlControlCapability(
            typeName,
            Array.AsReadOnly(editableProperties ?? []),
            Array.AsReadOnly(bindableProperties ?? []),
            supportsMvvmGeneration,
            maximumChildren,
            allowsAppendingChildren,
            allowsSafeChildRemoval);
    }
}
