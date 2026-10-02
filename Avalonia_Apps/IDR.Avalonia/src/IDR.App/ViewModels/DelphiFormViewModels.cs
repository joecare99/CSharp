using IDR.Core.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace IDR.App.ViewModels;

public sealed class DelphiFormViewModel
{
    public DelphiFormViewModel(DelphiForm form)
    {
        ResourceName = form.ResourceName;
        RootComponent = new DelphiFormComponentViewModel(form.Root, 0, 0, 0);
        Components = Flatten(RootComponent).ToArray();
        PreviewComponents = Components
            .Where(component => !ReferenceEquals(component, RootComponent) && component.IsVisible)
            .ToArray();
        DisplayName = $"{ResourceName} ({form.Root.Name}: {form.Root.ClassName})";
        Width = Math.Clamp(ReadDimension(RootComponent, "Width", 640), 320, 1600);
        Height = Math.Clamp(ReadDimension(RootComponent, "Height", 480), 240, 1200);
    }

    public string ResourceName { get; }

    public string DisplayName { get; }

    public DelphiFormComponentViewModel RootComponent { get; }

    public IReadOnlyList<DelphiFormComponentViewModel> Components { get; }

    public IReadOnlyList<DelphiFormComponentViewModel> PreviewComponents { get; }

    public double Width { get; }

    public double Height { get; }

    private static IEnumerable<DelphiFormComponentViewModel> Flatten(
        DelphiFormComponentViewModel root)
    {
        yield return root;
        foreach (DelphiFormComponentViewModel child in root.Children)
        {
            foreach (DelphiFormComponentViewModel component in Flatten(child))
            {
                yield return component;
            }
        }
    }

    private static int ReadDimension(DelphiFormComponentViewModel component, string name, int fallback)
    {
        DelphiFormPropertyViewModel? property = component.Properties
            .FirstOrDefault(candidate => string.Equals(candidate.Name, name, StringComparison.OrdinalIgnoreCase));
        return property is not null
            && int.TryParse(property.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)
                ? value
                : fallback;
    }
}

public sealed class DelphiFormComponentViewModel
{
    private static readonly HashSet<string> SupportedVisualClasses = new(StringComparer.OrdinalIgnoreCase)
    {
        "TBitBtn", "TButton", "TCheckBox", "TComboBox", "TEdit", "TGroupBox",
        "TLabel", "TLabeledEdit", "TListBox", "TMemo", "TPanel", "TRadioButton",
        "TStaticText"
    };

    public DelphiFormComponentViewModel(
        DelphiFormComponent component,
        int depth,
        int parentLeft,
        int parentTop)
    {
        Name = component.Name;
        ClassName = component.ClassName;
        Depth = depth;
        Properties = component.Properties
            .Select(property => new DelphiFormPropertyViewModel(property.Name, property.Value))
            .ToArray();

        int left = ReadInt32("Left");
        int top = ReadInt32("Top");
        Left = checked(parentLeft + left);
        Top = checked(parentTop + top);
        Width = Math.Clamp(ReadInt32("Width", 80), 16, 1200);
        Height = Math.Clamp(ReadInt32("Height", 28), 12, 800);
        IsVisible = !string.Equals(ReadProperty("Visible"), bool.FalseString, StringComparison.OrdinalIgnoreCase);
        string caption = ReadProperty("Caption") ?? ReadProperty("Text") ?? Name;
        IsPlaceholder = !IsKnownVisualClass(ClassName);
        PreviewText = IsPlaceholder
            ? $"? {ClassName}\n{caption}"
            : caption;
        TreeLabel = $"{new string(' ', Math.Min(depth * 2, 40))}{Name}: {ClassName}";
        Children = component.Children
            .Select(child => new DelphiFormComponentViewModel(child, depth + 1, Left, Top))
            .ToArray();
    }

    public string Name { get; }

    public string ClassName { get; }

    public int Depth { get; }

    public string TreeLabel { get; }

    public IReadOnlyList<DelphiFormPropertyViewModel> Properties { get; }

    public IReadOnlyList<DelphiFormComponentViewModel> Children { get; }

    public int Left { get; }

    public int Top { get; }

    public double Width { get; }

    public double Height { get; }

    public bool IsVisible { get; }

    public bool IsPlaceholder { get; }

    public string PreviewText { get; }

    private int ReadInt32(string name, int fallback = 0)
    {
        string? value = ReadProperty(name);
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int result)
            ? result
            : fallback;
    }

    private string? ReadProperty(string name) =>
        Properties.FirstOrDefault(property =>
            string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))?.Value;

    private static bool IsKnownVisualClass(string className) =>
        SupportedVisualClasses.Contains(className);
}

public sealed record DelphiFormPropertyViewModel(string Name, string Value);
