using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConsoleLib;
using ConsoleLib.Cxaml.Designer.Preview;
using ConsoleLib.Interfaces;

namespace ConsoleLib.Cxaml.Designer.ViewModels;

public sealed partial class DesignerViewModel : ObservableObject
{
    private readonly AvaloniaPreviewRenderer _previewRenderer = new();
    private readonly ConsolePreviewRenderer _consolePreviewRenderer = new();
    private IControl? _previewControl;
    private DesignerPreviewState _previewState = DesignerPreviewState.Unavailable();
    private string _preview = "Preview unavailable";
    private string _diagnostics = "No diagnostics";
    private string _virtualAxaml = "No virtual AXAML generated.";
    private IReadOnlyList<string> _inspectorProperties = Array.Empty<string>();
    private IReadOnlyList<InspectorPropertyViewModel> _categorizedInspectorProperties = Array.Empty<InspectorPropertyViewModel>();
    private string? _selectedPreviewId;
    private string _consolePreview = string.Empty;
    private bool _updatingSourceCaret;
    private readonly ObservableCollection<GridDefinitionViewModel> _gridRows = new();
    private readonly ObservableCollection<GridDefinitionViewModel> _gridColumns = new();

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

    [ObservableProperty]
    private int _sourceCaretOffset;

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
    public IReadOnlyList<InspectorPropertyViewModel> CategorizedInspectorProperties => _categorizedInspectorProperties;
    public ObservableCollection<GridDefinitionViewModel> GridRows => _gridRows;
    public ObservableCollection<GridDefinitionViewModel> GridColumns => _gridColumns;
    public bool IsGridSelected => _previewState.SelectedMapping?.ConsoleControl is ConsoleLib.CommonControls.Grid;
    public IReadOnlyList<string> PreviewModes { get; } = new[] { "Visual", "Console" };
    public int SelectedPreviewTabIndex
    {
        get => IsConsolePreview ? 1 : 0;
        set => SelectedPreviewMode = value == 1 ? "Console" : "Visual";
    }

    [ObservableProperty]
    private string _selectedPreviewMode = "Visual";

    [ObservableProperty]
    private string _selectedConsoleFrameSize = "Designer Size";

    public IReadOnlyList<string> ConsoleFrameSizes { get; } = new[] { "Designer Size", "80x25", "80x50", "132x60" };

    public bool IsConsolePreview => string.Equals(SelectedPreviewMode, "Console", StringComparison.Ordinal);
    public string ConsolePreview => _consolePreview;
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
    partial void OnSourceCaretOffsetChanged(int value)
    {
        if (!_updatingSourceCaret)
            SelectSourceElementAtOffset(value);
    }
    partial void OnSelectedPreviewModeChanged(string value)
    {
        OnPropertyChanged(nameof(IsConsolePreview));
        OnPropertyChanged(nameof(ConsolePreview));
        OnPropertyChanged(nameof(SelectedPreviewTabIndex));
    }
    partial void OnSelectedConsoleFrameSizeChanged(string value)
    {
        if (_previewControl is not null)
        {
            _consolePreview = _consolePreviewRenderer.Render(_previewControl, ParseConsoleFrameSize(value));
            OnPropertyChanged(nameof(ConsolePreview));
        }
    }

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
        var activated = _previewState.ActivateSelection(controlId);
        if (activated)
        {
            var mapping = _previewState.SelectedMapping;
            if (mapping is not null)
                SetSourceCaret(GetElementOffset(mapping.SourcePath));
        }
        return activated;
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
                _consolePreview = _consolePreviewRenderer.Render(_previewControl, ParseConsoleFrameSize(SelectedConsoleFrameSize));
                foreach (var mapping in _previewState.Mappings)
                {
                    mapping.SourceName = loadResult.NamedControls
                        .FirstOrDefault(pair => ReferenceEquals(pair.Value, mapping.ConsoleControl)).Key;
                }
                _previewState.SelectionChanged += PreviewState_SelectionChanged;
                _preview = _previewControl.GetType().Name + " (" + _previewControl.Children.Count + " children)";
                InspectorStatus = "Preview rendered: " + _previewState.Mappings.Count + " mapped control(s).";
                _categorizedInspectorProperties = GetInspectorProperties(_previewControl);
                RefreshGridDefinitions(_previewControl);
                _inspectorProperties = _categorizedInspectorProperties.Select(property => property.Name).ToArray();
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
        OnPropertyChanged(nameof(CategorizedInspectorProperties));
        OnPropertyChanged(nameof(ConsolePreview));
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
        _categorizedInspectorProperties = Array.Empty<InspectorPropertyViewModel>();
        _gridRows.Clear();
        _gridColumns.Clear();
        _selectedPreviewId = null;
        _consolePreview = string.Empty;
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

    private static System.Drawing.Size? ParseConsoleFrameSize(string value) =>
        value switch
        {
            "80x25" => new System.Drawing.Size(80, 25),
            "80x50" => new System.Drawing.Size(80, 50),
            "132x60" => new System.Drawing.Size(132, 60),
            _ => null
        };

    private void PreviewState_SelectionChanged(object? sender, PreviewSelectionChangedEventArgs e)
    {
        _selectedPreviewId = e.Mapping.Id;
        _categorizedInspectorProperties = GetInspectorProperties(e.Mapping.ConsoleControl);
        RefreshGridDefinitions(e.Mapping.ConsoleControl);
        _inspectorProperties = _categorizedInspectorProperties.Select(property => property.Name).ToArray();
        SetSourceCaret(GetElementOffset(e.Mapping.SourcePath));
        UpdateSelectedPropertyValue();
        OnPropertyChanged(nameof(InspectorProperties));
        OnPropertyChanged(nameof(CategorizedInspectorProperties));
        OnPropertyChanged(nameof(GridRows));
        OnPropertyChanged(nameof(GridColumns));
        OnPropertyChanged(nameof(IsGridSelected));
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
            element = element.Elements().Where(IsControlElement).ElementAt(index);
        return element;
    }

    private static bool IsControlElement(XElement element) =>
        element.Name.LocalName is not ("RowDefinitions" or "ColumnDefinitions" or "Grid.RowDefinitions" or "Grid.ColumnDefinitions")
        && element.Name.LocalName is not ("RowDefinition" or "ColumnDefinition");

    private void RefreshGridDefinitions(IControl? control)
    {
        _gridRows.Clear();
        _gridColumns.Clear();
        if (control is not ConsoleLib.CommonControls.Grid grid)
            return;
        for (var i = 0; i < grid.RowDefinitions.Count; i++)
            _gridRows.Add(new GridDefinitionViewModel(i, true, grid.RowDefinitions[i].Height));
        for (var i = 0; i < grid.ColumnDefinitions.Count; i++)
            _gridColumns.Add(new GridDefinitionViewModel(i, false, grid.ColumnDefinitions[i].Width));
        if (_gridRows.Count == 0)
            _gridRows.Add(new GridDefinitionViewModel(0, true, ConsoleLib.CommonControls.GridLength.Star));
        if (_gridColumns.Count == 0)
            _gridColumns.Add(new GridDefinitionViewModel(0, false, ConsoleLib.CommonControls.GridLength.Star));
    }

    [RelayCommand]
    private void AddGridRow() => AddGridDefinition(true);

    [RelayCommand]
    private void AddGridColumn() => AddGridDefinition(false);

    [RelayCommand]
    private void RemoveGridRow(GridDefinitionViewModel? definition) => RemoveGridDefinition(definition, true);

    [RelayCommand]
    private void RemoveGridColumn(GridDefinitionViewModel? definition) => RemoveGridDefinition(definition, false);

    [RelayCommand]
    private void ApplyGridDefinitions()
    {
        var grid = _previewState.SelectedMapping?.ConsoleControl as ConsoleLib.CommonControls.Grid;
        var mapping = _previewState.SelectedMapping;
        if (grid is null || mapping is null)
        {
            InspectorStatus = "Select a Grid to edit its definitions.";
            return;
        }
        grid.RowDefinitions.Clear();
        foreach (var row in _gridRows)
            grid.RowDefinitions.Add(new ConsoleLib.CommonControls.RowDefinition { Height = row.ToGridLength() });
        grid.ColumnDefinitions.Clear();
        foreach (var column in _gridColumns)
            grid.ColumnDefinitions.Add(new ConsoleLib.CommonControls.ColumnDefinition { Width = column.ToGridLength() });
        UpdateGridDefinitionElements(mapping.SourcePath);
        InspectorStatus = "Grid definitions applied.";
        RefreshPreview();
    }

    private void AddGridDefinition(bool row)
    {
        if (!IsGridSelected)
        {
            InspectorStatus = "Select a Grid to edit its definitions.";
            return;
        }
        var definitions = row ? _gridRows : _gridColumns;
        definitions.Add(new GridDefinitionViewModel(definitions.Count, row, ConsoleLib.CommonControls.GridLength.Star));
    }

    private void RemoveGridDefinition(GridDefinitionViewModel? definition, bool row)
    {
        if (definition is null)
            return;
        var definitions = row ? _gridRows : _gridColumns;
        if (definitions.Count <= 1)
        {
            InspectorStatus = "A Grid must keep at least one definition.";
            return;
        }
        definitions.Remove(definition);
        for (var i = 0; i < definitions.Count; i++)
            definitions[i].Reindex(i);
    }

    private void UpdateGridDefinitionElements(IReadOnlyList<int> path)
    {
        var document = XDocument.Parse(Markup, LoadOptions.PreserveWhitespace);
        var element = FindElement(document.Root!, path);
        element.Elements().Where(child => child.Name.LocalName.EndsWith("RowDefinitions", StringComparison.Ordinal)
            || child.Name.LocalName.EndsWith("ColumnDefinitions", StringComparison.Ordinal)).Remove();
        element.Add(CreateDefinitionElement("Grid.RowDefinitions", "RowDefinition", "Height", _gridRows));
        element.Add(CreateDefinitionElement("Grid.ColumnDefinitions", "ColumnDefinition", "Width", _gridColumns));
        Markup = document.ToString(SaveOptions.DisableFormatting);
    }

    private static XElement CreateDefinitionElement(string name, string itemName, string attribute, IEnumerable<GridDefinitionViewModel> definitions)
    {
        var container = new XElement(name);
        foreach (var definition in definitions)
        {
            var length = definition.ToGridLength();
            var value = length.GridUnitType == ConsoleLib.CommonControls.GridUnitType.Auto
                ? "Auto"
                : length.GridUnitType == ConsoleLib.CommonControls.GridUnitType.Star
                    ? (length.Value == 1 ? "*" : length.Value.ToString(CultureInfo.InvariantCulture) + "*")
                    : length.Value.ToString(CultureInfo.InvariantCulture);
            container.Add(new XElement(itemName, new XAttribute(attribute, value)));
        }
        return container;
    }

    private void SelectSourceElementAtOffset(int offset)
    {
        if (_previewState.Mappings.Count == 0)
            return;

        try
        {
            var document = XDocument.Parse(Markup, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
            var selected = _previewState.Mappings
                .Select(mapping => (mapping, start: GetElementOffset(document, mapping.SourcePath), length: GetElementLength(document, mapping.SourcePath)))
                .Where(item => item.start >= 0 && offset >= item.start && offset <= item.start + item.length)
                .OrderByDescending(item => item.start)
                .FirstOrDefault();
            if (selected.mapping is not null && selected.mapping.Id != _selectedPreviewId)
                _previewState.ActivateSelection(selected.mapping.Id);
        }
        catch (XmlException)
        {
            // Validation diagnostics are already exposed by RefreshPreview.
        }
    }

    private void SetSourceCaret(int offset)
    {
        _updatingSourceCaret = true;
        SourceCaretOffset = Math.Clamp(offset, 0, Markup.Length);
        _updatingSourceCaret = false;
    }

    private int GetElementOffset(IReadOnlyList<int> path)
    {
        try
        {
            var document = XDocument.Parse(Markup, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace);
            return GetElementOffset(document, path);
        }
        catch (XmlException)
        {
            return 0;
        }
    }

    private int GetElementOffset(XDocument document, IReadOnlyList<int> path)
    {
        var element = FindElement(document.Root!, path);
        if (element is not IXmlLineInfo lineInfo || !lineInfo.HasLineInfo())
            return 0;
        return GetLineStartOffset(Markup, lineInfo.LineNumber) + lineInfo.LinePosition - 1;
    }

    private int GetElementLength(XDocument document, IReadOnlyList<int> path)
    {
        var element = FindElement(document.Root!, path);
        return element.ToString(SaveOptions.DisableFormatting).Length;
    }

    private static int GetLineStartOffset(string text, int line)
    {
        var currentLine = 1;
        for (var index = 0; index < text.Length; index++)
        {
            if (currentLine == line)
                return index;
            if (text[index] == '\n')
                currentLine++;
        }
        return text.Length;
    }

    private IReadOnlyList<InspectorPropertyViewModel> GetInspectorProperties(IControl control)
    {
        var names = control.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.CanRead &&
                (property.PropertyType == typeof(string) || property.PropertyType.IsValueType ||
                 property.PropertyType.IsEnum))
            .Select(property => (property.Name, property.PropertyType))
            .Concat(new[] { ("Width", typeof(int)), ("Height", typeof(int)) })
            .GroupBy(item => item.Item1, StringComparer.Ordinal)
            .Select(group => group.First())
            .Select(item => new InspectorPropertyViewModel(
                control,
                item.Item1,
                GetPropertyCategory(item.Item1),
                item.Item2,
                GetPropertyValue(control, item.Item1),
                !CanEditProperty(control, item.Item1),
                ApplyInspectorValue))
            .OrderBy(item => item.Category, StringComparer.Ordinal)
            .ThenBy(item => item.Name, StringComparer.Ordinal)
            .ToArray();
        return names;
    }

    private void ApplyInspectorValue(string propertyName, string value)
    {
        SelectedPropertyName = propertyName;
        SelectedPropertyValue = value;
        ApplySelectedProperty();
    }

    private static bool CanEditProperty(IControl control, string name) =>
        name.Equals("Width", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("Height", StringComparison.OrdinalIgnoreCase) ||
        control.GetType().GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase)?.CanWrite == true;

    private static string GetPropertyValue(IControl control, string name)
    {
        if (name.Equals("Width", StringComparison.OrdinalIgnoreCase))
            return control.size.Width.ToString();
        if (name.Equals("Height", StringComparison.OrdinalIgnoreCase))
            return control.size.Height.ToString();
        return Convert.ToString(control.GetType().GetProperty(name)?.GetValue(control), System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static string GetPropertyCategory(string name) =>
        name switch
        {
            "Width" or "Height" or "X" or "Y" => "Layout",
            "BackColor" or "ForeColor" or "BorderColor" or "BorderStyle" => "Appearance",
            "Text" or "Items" or "ItemsSource" => "Content",
            "Enabled" or "Visible" or "Shadow" => "Behavior",
            _ => "Advanced"
        };

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
