using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib;
using ConsoleLib.Cxaml.Designer.Preview;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Cxaml.Designer.ViewModels;

public sealed partial class DesignerViewModel : ObservableObject
{
    private readonly AvaloniaPreviewRenderer _previewRenderer = new();
    private IControl? _previewControl;
    private DesignerPreviewState _previewState = DesignerPreviewState.Unavailable();
    private string _preview = "Preview unavailable";
    private string _diagnostics = "No diagnostics";
    private string _virtualAxaml = "No virtual AXAML generated.";
    private IReadOnlyList<string> _inspectorProperties = Array.Empty<string>();
    private string? _selectedPreviewId;

    public IReadOnlyList<string> Toolbox { get; } = new[]
    {
        "Panel", "StackPanel", "Grid", "Button", "Label", "TextBox", "CheckBox", "ComboBox", "TreeView", "TileView"
    };

    [ObservableProperty]
    private string _markup = "<StackPanel Width=\"40\"><Button Text=\"Preview\" /></StackPanel>";

    [ObservableProperty]
    private string? _selectedTool;

    [ObservableProperty]
    private string? _selectedPropertyName;

    [ObservableProperty]
    private string _selectedPropertyValue = string.Empty;

    [ObservableProperty]
    private string _inspectorStatus = string.Empty;

    [ObservableProperty]
    private string _filePath = string.Empty;

    public string Preview => _preview;
    public string Diagnostics => _diagnostics;
    public string VirtualAxaml => _virtualAxaml;
    public IControl? PreviewControl => _previewControl;
    public Avalonia.Controls.Control? RenderedPreview => _previewState.Root;
    public DesignerPreviewState PreviewState => _previewState;
    public IReadOnlyList<PreviewControlMapping> PreviewMappings => _previewState.Mappings;
    public string? SelectedPreviewControlId => _selectedPreviewId;
    public string? SelectedSourceElementPath => _selectedPreviewId;
    public IReadOnlyList<string> InspectorProperties => _inspectorProperties;
    public string SelectedElement
    {
        get
        {
            var selected = _previewState.SelectedMapping;
            if (selected is not null)
                return $"Selected: {selected.ElementName} ({selected.Id})";
            return SelectedTool is null ? "Select a control from the toolbox." : "Selected: " + SelectedTool;
        }
    }

    public DesignerViewModel() => RefreshPreview();

    partial void OnMarkupChanged(string value) => RefreshPreview();

    partial void OnSelectedPropertyNameChanged(string? value) => UpdateSelectedPropertyValue();

    [RelayCommand]
    private void InsertSelectedTool()
    {
        if (string.IsNullOrWhiteSpace(SelectedTool))
            return;

        Markup = "<" + SelectedTool + " />";
    }

    [RelayCommand]
    private void LoadFile()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            InspectorStatus = "Enter a CXAML file path.";
            return;
        }

        try
        {
            Markup = File.ReadAllText(FilePath);
            InspectorStatus = "CXAML file loaded.";
        }
        catch (IOException error)
        {
            InspectorStatus = "Unable to load CXAML file: " + error.Message;
        }
        catch (UnauthorizedAccessException error)
        {
            InspectorStatus = "Unable to load CXAML file: " + error.Message;
        }
    }

    [RelayCommand]
    private void SaveFile()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            InspectorStatus = "Enter a CXAML file path.";
            return;
        }

        try
        {
            File.WriteAllText(FilePath, Markup);
            InspectorStatus = "CXAML file saved.";
        }
        catch (IOException error)
        {
            InspectorStatus = "Unable to save CXAML file: " + error.Message;
        }
        catch (UnauthorizedAccessException error)
        {
            InspectorStatus = "Unable to save CXAML file: " + error.Message;
        }
    }

    [RelayCommand]
    private void ApplySelectedProperty()
    {
        var selected = _previewState.SelectedMapping;
        var control = selected?.ConsoleControl ?? _previewControl;
        if (control is null || string.IsNullOrWhiteSpace(SelectedPropertyName))
        {
            InspectorStatus = "No preview property selected.";
            return;
        }

        try
        {
            var propertyName = SelectedPropertyName;
            if (string.Equals(propertyName, "Width", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(propertyName, "Height", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(SelectedPropertyValue, out var dimension))
                {
                    InspectorStatus = "Invalid property value.";
                    return;
                }

                var current = control.size;
                control.size = string.Equals(propertyName, "Width", StringComparison.OrdinalIgnoreCase)
                    ? new System.Drawing.Size(dimension, current.Height)
                    : new System.Drawing.Size(current.Width, dimension);
            }
            else
            {
                var property = control.GetType().GetProperty(
                    propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (property is null || !property.CanWrite)
                {
                    InspectorStatus = "Property is not editable.";
                    return;
                }

                var converter = TypeDescriptor.GetConverter(property.PropertyType);
                if (!converter.CanConvertFrom(typeof(string)))
                {
                    InspectorStatus = "Property value cannot be converted.";
                    return;
                }

                property.SetValue(control, converter.ConvertFromInvariantString(SelectedPropertyValue));
            }

            UpdateSourceAttribute(selected, propertyName, SelectedPropertyValue);
            InspectorStatus = "Property applied.";
        }
        catch (Exception error) when (error is FormatException or InvalidCastException or ArgumentException or TargetInvocationException)
        {
            InspectorStatus = "Invalid property value: " + error.Message;
        }
    }

    public bool ActivatePreviewSelection(string controlId)
    {
        return _previewState.ActivateSelection(controlId);
    }

    public bool SelectPreviewControl(string controlId) => ActivatePreviewSelection(controlId);
    public bool SelectSourceElement(string controlId) => ActivatePreviewSelection(controlId);

    public void RefreshPreview()
    {
        var previousSelection = _selectedPreviewId;
        InspectorStatus = "Validating CXAML...";
        var loader = new CxamlLoader();
        var diagnostics = loader.Validate(new StringReader(Markup));
        _diagnostics = diagnostics.Count == 0
            ? "No diagnostics"
            : string.Join(Environment.NewLine, diagnostics.Select(diagnostic => diagnostic.Severity + ": " + diagnostic.Message));

        if (diagnostics.Any(diagnostic => diagnostic.Severity == CxamlDiagnosticSeverity.Error))
        {
            SetUnavailable("Preview validation failed.", clearVirtualAxaml: true);
        }
        else
        {
            try
            {
                var loadResult = loader.Load(
                    new StringReader(Markup),
                    new CxamlLoadContext(new object(), allowUnresolvedBindings: true));
                _previewControl = loadResult.Root;
                _virtualAxaml = CreateVirtualAxaml(_previewControl).ToString(SaveOptions.None);
                InspectorStatus = "Rendering preview...";
                _previewState = _previewRenderer.Render(_previewControl);
                foreach (var mapping in _previewState.Mappings)
                {
                    mapping.SourceName = loadResult.NamedControls
                        .FirstOrDefault(pair => ReferenceEquals(pair.Value, mapping.ConsoleControl)).Key;
                }
                _previewState.SelectionChanged += PreviewState_SelectionChanged;
                _preview = _previewControl.GetType().Name + " (" + _previewControl.Children.Count + " children)";
                InspectorStatus = "Preview rendered: " + _previewState.Mappings.Count + " mapped control(s).";
                _inspectorProperties = GetInspectorProperties(_previewControl);
                if (previousSelection is not null)
                    _previewState.ActivateSelection(previousSelection);
            }
            catch (CxamlParseException error)
            {
                SetUnavailable("Preview error (" + error.GetType().Name + "): " + error.Message, clearVirtualAxaml: false);
            }
            catch (XmlException error)
            {
                SetUnavailable("Preview error (" + error.GetType().Name + "): " + error.Message, clearVirtualAxaml: false);
            }
            catch (InvalidOperationException error)
            {
                SetUnavailable("Preview error (" + error.GetType().Name + "): " + error.Message, clearVirtualAxaml: false);
            }
        }

        OnPropertyChanged(nameof(Preview));
        OnPropertyChanged(nameof(Diagnostics));
        OnPropertyChanged(nameof(VirtualAxaml));
        OnPropertyChanged(nameof(PreviewControl));
        OnPropertyChanged(nameof(RenderedPreview));
        OnPropertyChanged(nameof(PreviewState));
        OnPropertyChanged(nameof(PreviewMappings));
        OnPropertyChanged(nameof(InspectorProperties));
        OnPropertyChanged(nameof(SelectedElement));
        OnPropertyChanged(nameof(SelectedPreviewControlId));
        OnPropertyChanged(nameof(SelectedSourceElementPath));
        UpdateSelectedPropertyValue();
    }

    private void SetUnavailable(string message, bool clearVirtualAxaml)
    {
        _previewState.SelectionChanged -= PreviewState_SelectionChanged;
        _previewControl = null;
        _previewState = DesignerPreviewState.Unavailable(message);
        _preview = "Preview unavailable";
        InspectorStatus = message;
        if (clearVirtualAxaml)
            _virtualAxaml = "No virtual AXAML generated.";
        _inspectorProperties = Array.Empty<string>();
        _selectedPreviewId = null;
    }

    private static XElement CreateVirtualAxaml(IControl control)
    {
        var element = new XElement(control.GetType().Name,
            new XAttribute("Width", control.size.Width),
            new XAttribute("Height", control.size.Height),
            new XAttribute("X", control.Position.X),
            new XAttribute("Y", control.Position.Y),
            new XAttribute("Visible", control.Visible),
            new XAttribute("Enabled", control.Enabled),
            new XAttribute("BackColor", control.GetActualBackColor()),
            new XAttribute("ForeColor", control.GetActualForeColor()));

        if (!string.IsNullOrEmpty(control.Text))
            element.SetAttributeValue("Text", control.Text);

        foreach (var child in control.Children)
            element.Add(CreateVirtualAxaml(child));

        return element;
    }

    private void PreviewState_SelectionChanged(object? sender, PreviewSelectionChangedEventArgs e)
    {
        _selectedPreviewId = e.Mapping.Id;
        _inspectorProperties = GetInspectorProperties(e.Mapping.ConsoleControl);
        UpdateSelectedPropertyValue();
        OnPropertyChanged(nameof(InspectorProperties));
        OnPropertyChanged(nameof(SelectedElement));
        OnPropertyChanged(nameof(SelectedPreviewControlId));
        OnPropertyChanged(nameof(SelectedSourceElementPath));
    }

    private void UpdateSourceAttribute(PreviewControlMapping? selected, string propertyName, string value)
    {
        var sourcePath = selected?.SourcePath ?? Array.Empty<int>();
        var document = XDocument.Parse(Markup, LoadOptions.PreserveWhitespace);
        var element = FindElement(document.Root!, sourcePath);
        var attributeName = propertyName.Equals("Width", StringComparison.OrdinalIgnoreCase)
            ? "Width"
            : propertyName.Equals("Height", StringComparison.OrdinalIgnoreCase)
                ? "Height"
                : propertyName;
        element.SetAttributeValue(attributeName, value);
        Markup = document.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement FindElement(XElement root, IReadOnlyList<int> path)
    {
        var element = root;
        foreach (var index in path)
            element = element.Elements().ElementAt(index);
        return element;
    }

    private static IReadOnlyList<string> GetInspectorProperties(IControl control)
    {
        return control.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanWrite &&
                (property.PropertyType == typeof(string) || property.PropertyType.IsValueType ||
                 property.PropertyType.IsEnum))
            .Select(property => property.Name)
            .Concat(new[] { "Width", "Height" })
            .Distinct(StringComparer.Ordinal)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
    }

    private void UpdateSelectedPropertyValue()
    {
        var selected = _previewState.SelectedMapping?.ConsoleControl ?? _previewControl;
        if (selected is null || string.IsNullOrWhiteSpace(SelectedPropertyName))
            return;

        if (SelectedPropertyName.Equals("Width", StringComparison.OrdinalIgnoreCase))
            SelectedPropertyValue = selected.size.Width.ToString();
        else if (SelectedPropertyName.Equals("Height", StringComparison.OrdinalIgnoreCase))
            SelectedPropertyValue = selected.size.Height.ToString();
        else
        {
            var property = selected.GetType().GetProperty(
                SelectedPropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (property?.CanRead == true)
                SelectedPropertyValue = property.GetValue(selected)?.ToString() ?? string.Empty;
        }
    }
}
