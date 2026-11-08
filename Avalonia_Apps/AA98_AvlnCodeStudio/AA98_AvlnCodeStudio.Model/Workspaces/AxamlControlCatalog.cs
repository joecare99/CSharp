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
        Create("Border", ["Background", "BorderBrush", "BorderThickness", "CornerRadius", "Height", "HorizontalAlignment", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "VerticalAlignment", "Width"],
            maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Button", ["Background", "BorderBrush", "BorderThickness", "Content", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsCancel", "IsDefault", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "VerticalAlignment", "Width"],
            [new("Command", "ICommand", IsCommand: true)], supportsMvvmGeneration: true),
        Create("CheckBox", ["Background", "BorderBrush", "BorderThickness", "Content", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsChecked", "IsThreeState", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "VerticalAlignment", "Width"],
            [new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ComboBox", ["Background", "BorderBrush", "BorderThickness", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "HorizontalAlignment", "Margin", "Opacity", "Padding", "PlaceholderText", "VerticalAlignment"],
            [new("SelectedIndex", "int", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ContentControl", ["HorizontalAlignment", "Margin", "Opacity", "VerticalAlignment"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("DockPanel", ["HorizontalAlignment", "Margin", "Opacity", "VerticalAlignment"], maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("Expander", ["Header", "HorizontalAlignment", "IsExpanded", "Margin", "Opacity", "VerticalAlignment"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Grid", ["Height", "HorizontalAlignment", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "VerticalAlignment", "Width"], maximumChildren: int.MaxValue),
        Create("GroupBox", ["Header", "Height", "HorizontalAlignment", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "VerticalAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Label", ["Content", "Height", "HorizontalAlignment", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "VerticalAlignment", "Width"]),
        Create("ListBox", ["HorizontalAlignment", "Margin", "Opacity", "VerticalAlignment"]),
        Create("NumericUpDown", ["AllowSpin", "Background", "BorderBrush", "BorderThickness", "ButtonSpinnerLocation", "ClipValueToMinMax", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "FormatString", "HorizontalAlignment", "Increment", "Margin", "Maximum", "Minimum", "Opacity", "Padding", "ParsingNumberStyle", "ShowButtonSpinner", "TextAlignment", "Value", "VerticalAlignment"],
            [new("Value", "decimal?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ProgressBar", ["Background", "BorderBrush", "BorderThickness", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "HorizontalAlignment", "IsIndeterminate", "Margin", "Maximum", "Minimum", "Opacity", "Orientation", "Padding", "ProgressTextFormat", "ShowProgressText", "Value", "VerticalAlignment"]),
        Create("RadioButton", ["Background", "BorderBrush", "BorderThickness", "Content", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "GroupName", "Height", "HorizontalAlignment", "IsChecked", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "VerticalAlignment", "Width"],
            [new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("ScrollViewer", ["HorizontalAlignment", "Margin", "Opacity", "VerticalAlignment"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Separator", ["HorizontalAlignment", "Margin", "Opacity", "VerticalAlignment"]),
        Create("Slider", ["Background", "BorderBrush", "BorderThickness", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "HorizontalAlignment", "IsDirectionReversed", "IsSnapToTickEnabled", "Margin", "Maximum", "Minimum", "Opacity", "Orientation", "Padding", "TickFrequency", "TickPlacement", "Value", "VerticalAlignment"],
            [new("Value", "double", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("StackPanel", ["Height", "HorizontalAlignment", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Orientation", "Spacing", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("TextBlock", ["FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "LineHeight", "Margin", "MaxHeight", "MaxLines", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Text", "TextAlignment", "TextTrimming", "TextWrapping", "VerticalAlignment", "Width"],
            [new("Text", "string", IsCommand: false, DefaultBindingMode: AxamlBindingMode.OneWay)], supportsMvvmGeneration: true),
        Create("TextBox", ["AcceptsReturn", "AcceptsTab", "Background", "BorderBrush", "BorderThickness", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "Height", "HorizontalAlignment", "IsReadOnly", "Margin", "MaxHeight", "MaxLength", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Padding", "PlaceholderText", "Text", "TextAlignment", "TextWrapping", "VerticalAlignment", "Width"],
            [new(
                "Text",
                "string",
                IsCommand: false,
                DefaultBindingMode: AxamlBindingMode.TwoWay,
                SupportsUpdateSourceTrigger: true)], supportsMvvmGeneration: true),
        Create("ToggleSwitch", ["Background", "BorderBrush", "BorderThickness", "Content", "CornerRadius", "FontSize", "FontStyle", "FontWeight", "Foreground", "HorizontalAlignment", "IsChecked", "IsThreeState", "Margin", "OffContent", "OnContent", "Opacity", "Padding", "VerticalAlignment"],
            [new("IsChecked", "bool?", IsCommand: false, DefaultBindingMode: AxamlBindingMode.TwoWay)], supportsMvvmGeneration: true),
        Create("UniformGrid", ["Columns", "FirstColumn", "Height", "HorizontalAlignment", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Rows", "VerticalAlignment", "Width"],
            maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("UserControl", ["HorizontalAlignment", "Margin", "Opacity", "VerticalAlignment"], maximumChildren: int.MaxValue),
        Create("Viewbox", ["Height", "HorizontalAlignment", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Stretch", "VerticalAlignment", "Width"], maximumChildren: 1, allowsSafeChildRemoval: true),
        Create("Window", ["HorizontalAlignment", "Margin", "Opacity", "VerticalAlignment"], maximumChildren: int.MaxValue),
        Create("WrapPanel", ["Height", "HorizontalAlignment", "Margin", "MaxHeight", "MaxWidth", "MinHeight", "MinWidth", "Opacity", "Orientation", "VerticalAlignment", "Width"],
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
