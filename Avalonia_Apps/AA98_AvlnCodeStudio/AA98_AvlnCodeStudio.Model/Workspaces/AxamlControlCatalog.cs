using System;
using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

public sealed record AxamlBindablePropertyCapability(
    string Name,
    string TypeName,
    bool IsCommand);

public sealed record AxamlControlCapability(
    string TypeName,
    IReadOnlyList<string> EditableProperties,
    IReadOnlyList<AxamlBindablePropertyCapability> BindableProperties,
    bool SupportsMvvmGeneration,
    int MaximumChildren,
    bool AllowsAppendingChildren);

public static class AxamlControlCatalog
{
    private static readonly IReadOnlyList<AxamlControlCapability> s_capabilities =
    [
        Create("Border", ["Background", "BorderThickness", "Height", "Margin", "Padding", "Width"],
            maximumChildren: 1),
        Create("Button", ["Content", "Height", "HorizontalAlignment", "Margin", "VerticalAlignment", "Width"],
            [new("Command", "ICommand", IsCommand: true)], supportsMvvmGeneration: true),
        Create("CheckBox", ["Content", "Height", "HorizontalAlignment", "Margin", "VerticalAlignment", "Width"],
            [new("IsChecked", "bool?", IsCommand: false)], supportsMvvmGeneration: true),
        Create("ComboBox", [], [new("SelectedIndex", "int", IsCommand: false)], supportsMvvmGeneration: true),
        Create("ContentControl", [], maximumChildren: 1),
        Create("DockPanel", [], maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("Expander", ["Header", "IsExpanded"], maximumChildren: 1),
        Create("Grid", ["Height", "Margin", "Width"], maximumChildren: int.MaxValue),
        Create("GroupBox", ["Header", "Height", "Margin", "Width"], maximumChildren: 1),
        Create("Label", ["Content", "Height", "HorizontalAlignment", "Margin", "VerticalAlignment", "Width"]),
        Create("ListBox", []),
        Create("NumericUpDown", [], [new("Value", "decimal?", IsCommand: false)], supportsMvvmGeneration: true),
        Create("ProgressBar", []),
        Create("RadioButton", ["Content", "Height", "HorizontalAlignment", "Margin", "VerticalAlignment", "Width"],
            [new("IsChecked", "bool?", IsCommand: false)], supportsMvvmGeneration: true),
        Create("ScrollViewer", [], maximumChildren: 1),
        Create("Separator", []),
        Create("Slider", [], [new("Value", "double", IsCommand: false)], supportsMvvmGeneration: true),
        Create("StackPanel", ["Height", "Margin", "Orientation", "Spacing", "Width"],
            maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("TextBlock", ["FontSize", "FontWeight", "Height", "HorizontalAlignment", "Margin", "Text", "VerticalAlignment", "Width"],
            [new("Text", "string", IsCommand: false)], supportsMvvmGeneration: true),
        Create("TextBox", ["Height", "Margin", "PlaceholderText", "Text", "Width"],
            [new("Text", "string", IsCommand: false)], supportsMvvmGeneration: true),
        Create("ToggleSwitch", [], [new("IsChecked", "bool?", IsCommand: false)], supportsMvvmGeneration: true),
        Create("UniformGrid", ["Columns", "FirstColumn", "Height", "Margin", "Rows", "Width"],
            maximumChildren: int.MaxValue, allowsAppendingChildren: true),
        Create("UserControl", [], maximumChildren: int.MaxValue),
        Create("Viewbox", ["Height", "Margin", "Stretch", "Width"], maximumChildren: 1),
        Create("Window", [], maximumChildren: int.MaxValue),
        Create("WrapPanel", ["Height", "Margin", "Orientation", "Width"],
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
        bool allowsAppendingChildren = false)
    {
        return new AxamlControlCapability(
            typeName,
            Array.AsReadOnly(editableProperties ?? []),
            Array.AsReadOnly(bindableProperties ?? []),
            supportsMvvmGeneration,
            maximumChildren,
            allowsAppendingChildren);
    }
}
