using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

public partial class AxamlDesignerViewModel : ObservableObject
{
    private readonly AxamlDesignerSession _session;
    private bool _synchronizing;

    public Func<AxamlBindingRowViewModel, Task<bool>>? ApplyMissingBindingAsync { get; set; }

    public AxamlDesignerViewModel(AxamlPreviewService? previewService = null)
    {
        _session = new AxamlDesignerSession(previewService);
    }

    private void SynchronizeBindingRows()
    {
        if (_session.SelectedNode is not { } node
            || !AxamlControlCatalog.TryGet(node.TypeName, out AxamlControlCapability capability))
        {
            BindingRows.Clear();
            return;
        }

        SynchronizeBindingRows(node, capability);
    }

    private void SynchronizeBindingRows(AxamlPreviewNode node, AxamlControlCapability capability)
    {
        Dictionary<string, AxamlBindingRowViewModel> existing = BindingRows
            .ToDictionary(static row => row.PropertyName, StringComparer.Ordinal);
        List<AxamlBindingRowViewModel> rows = [];
        foreach (AxamlBindablePropertyCapability property in capability.BindableProperties)
        {
            string? expression = node.Properties.TryGetValue(property.Name, out string? value)
                ? value
                : null;
            string memberName = AxamlBindingRowViewModel.ReadBindingPath(expression) ?? string.Empty;
            bool isNewRow = !existing.TryGetValue(property.Name, out AxamlBindingRowViewModel? current)
                || current.ControlType != node.TypeName;
            AxamlBindingRowViewModel row = isNewRow
                ? new AxamlBindingRowViewModel(
                    node.TypeName,
                    property.Name,
                    expression,
                    property.TypeName,
                    property.IsCommand)
                : current!;
            bool bindingChanged = !string.Equals(row.MemberName, memberName, StringComparison.Ordinal);
            row.MemberName = memberName;
            row.Diagnostic = _bindingContextDiagnostic;
            AxamlMvvmMemberRequirement[] requirements = _bindingMembers
                .Select(member => new AxamlMvvmMemberRequirement(member.Name, property.TypeName, property.IsCommand))
                .ToArray();
            string[] compatibleMembers = new AxamlMvvmMemberPlanner()
                .Plan(requirements, _bindingMembers, createMissingMembers: false)
                .Where(static entry => entry.Action == AxamlMvvmMemberPlanAction.Reuse)
                .Select(static entry => entry.Requirement.Name)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            row.SetAvailableMembers(compatibleMembers);
            if (isNewRow || bindingChanged)
            {
                if (compatibleMembers.Contains(memberName, StringComparer.Ordinal))
                {
                    row.SelectedMember = memberName;
                    row.NewMemberName = string.Empty;
                    row.SelectExistingMemberSource();
                }
                else
                {
                    row.SelectedMember = null;
                    row.NewMemberName = memberName;
                    row.MemberSource = "Create new member";
                }
            }
            rows.Add(row);
        }

        BindingRows.Clear();
        foreach (AxamlBindingRowViewModel row in rows)
        {
            BindingRows.Add(row);
        }

        OnPropertyChanged(nameof(HasBindingRows));
    }

    public event EventHandler<AxamlDesignerMarkupChangedEventArgs>? MarkupChanged;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private AxamlPreviewNode? _previewRoot;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDockPanelSelected))]
    [NotifyPropertyChangedFor(nameof(SelectedElementName))]
    [NotifyPropertyChangedFor(nameof(SelectedCapabilityText))]
    private AxamlPreviewNode? _selectedNode;

    [ObservableProperty]
    private AxamlDockPanelPlacement _selectedDockPanelPlacement = AxamlDockPanelPlacement.Client;

    private IReadOnlyList<AxamlMvvmSemanticMember> _bindingMembers = [];

    private string? _bindingContextDiagnostic;

    [ObservableProperty]
    private string? _selectedProperty;

    [ObservableProperty]
    private string _selectedValue = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasPreview))]
    private string _errorText = string.Empty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    public ObservableCollection<string> EditableProperties { get; } = [];

    public ObservableCollection<AxamlPropertyRowViewModel> PropertyRows { get; } = [];

    public ObservableCollection<AxamlBindingRowViewModel> BindingRows { get; } = [];

    public bool HasBindingRows => BindingRows.Count > 0;

    public IReadOnlyList<AxamlDockPanelPlacement> DockPanelPlacements { get; } =
    [
        AxamlDockPanelPlacement.Client,
        AxamlDockPanelPlacement.Left,
        AxamlDockPanelPlacement.Top,
        AxamlDockPanelPlacement.Right,
        AxamlDockPanelPlacement.Bottom,
    ];

    public bool IsDockPanelSelected =>
        string.Equals(SelectedNode?.TypeName, "DockPanel", StringComparison.Ordinal);

    private bool IsDockPanelClientFillEnabled =>
        SelectedNode is not null
        && (!SelectedNode.Properties.TryGetValue("LastChildFill", out string? value)
            || !string.Equals(value, "False", StringComparison.OrdinalIgnoreCase));

    public bool HasPreview => PreviewRoot is not null || !string.IsNullOrWhiteSpace(ErrorText);

    public string SelectedElementName => SelectedNode?.TypeName ?? "Select a control in the preview.";

    public string SelectedCapabilityText
    {
        get
        {
            if (SelectedNode is null)
            {
                return "Select a control to inspect its designer capabilities.";
            }

            if (!SelectedNode.IsSupported
                || !AxamlControlCatalog.TryGet(SelectedNode.TypeName, out AxamlControlCapability capability))
            {
                return "Safe placeholder preview only; property editing, binding analysis, and MVVM generation are unavailable.";
            }

            string editableProperties = capability.EditableProperties.Count == 0
                ? "none"
                : string.Join(", ", capability.EditableProperties);
            string bindings = capability.BindableProperties.Count == 0
                ? "none"
                : string.Join(", ", capability.BindableProperties.Select(static property => property.Name));
            string generation = capability.SupportsMvvmGeneration
                ? "supported for named controls"
                : "not supported";
            string childLimit = capability.MaximumChildren == int.MaxValue
                ? "multiple"
                : $"up to {capability.MaximumChildren}";
            string children = capability.MaximumChildren == 0
                ? "none"
                : capability.TypeName == "Grid"
                    ? SelectedNode.CanAddGridChild
                        ? $"{childLimit}; next unoccupied cell"
                        : $"{childLimit}; correct invalid or unsupported markup before adding controls"
                : capability.TypeName == "DockPanel"
                    ? SelectedNode.CanAppendChildren
                        ? IsDockPanelClientFillEnabled
                            ? $"{childLimit}; choose placement; docked children stay before Client"
                            : $"{childLimit}; choose an explicit dock side; LastChildFill is disabled"
                        : $"{childLimit}; correct invalid or unsupported markup before adding controls"
                : capability.AllowsAppendingChildren && SelectedNode.CanAppendChildren
                    ? $"{childLimit}; appending supported"
                : capability.AllowsAppendingChildren
                    ? $"{childLimit}; correct invalid or unsupported markup before adding controls"
                    : $"{childLimit}; empty containers only";
            return $"Editable: {editableProperties}. Binding analysis: {bindings}. MVVM generation: {generation}. Children: {children}.";
        }
    }

    public string PreviewTitle => "AXAML Preview (safe subset)";

    public bool HasInvalidPropertyDraft => PropertyRows.Any(static row => row.IsDraft && row.HasValidationError);

    public bool CanUndo => _session.CanUndo;

    public bool CanRedo => _session.CanRedo;

    public bool ResizeElement(AxamlPreviewNode node, double width, double height)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (!double.IsFinite(width) || !double.IsFinite(height))
        {
            StatusText = "AXAML resize failed: dimensions must be finite numbers.";
            return false;
        }

        try
        {
            _session.SelectNode(node);
            Dictionary<string, string> values = new(StringComparer.Ordinal)
            {
                ["Width"] = FormatLayoutSize(width),
                ["Height"] = FormatLayoutSize(height),
            };
            if (!_session.ApplyValues(values, "Width"))
            {
                return false;
            }

            SynchronizeState();
            StatusText = "Designer size applied to AXAML.";
            RaiseMarkupChanged();
            return true;
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            StatusText = $"AXAML resize failed: {exception.Message}";
            return false;
        }
    }

    public void LoadMarkup(string? markup)
    {
        if (string.IsNullOrWhiteSpace(markup))
        {
            _session.Reset();
            ErrorText = string.Empty;
            SynchronizeState();
            return;
        }

        try
        {
            _session.Load(markup);
            ErrorText = _session.PreviewDiagnostic ?? string.Empty;
            SynchronizeState();
        }
        catch (Exception exception) when (exception is XmlException or InvalidDataException)
        {
            _session.Reset();
            SynchronizeState();
            ErrorText = exception.Message;
        }
    }

    [RelayCommand(CanExecute = nameof(CanApplySelectedValue))]
    private void ApplySelectedValue()
    {
        if (SelectedProperty is null || SelectedNode is null)
        {
            return;
        }

        try
        {
            _session.SelectProperty(SelectedProperty);
            if (_session.ApplySelectedValue(SelectedValue))
            {
                SynchronizeState();
                StatusText = "Designer property applied to AXAML.";
                RaiseMarkupChanged();
            }
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            StatusText = $"AXAML designer edit failed: {exception.Message}";
        }
    }

    public void ApplyPropertyRow(AxamlPropertyRowViewModel row, bool finalize)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (row.IsReadOnly || row.HasValidationError || SelectedNode is null || !PropertyRows.Contains(row))
        {
            return;
        }

        try
        {
            row.IsEditing = true;
            _session.BeginPropertyEdit(row.Name);
            if (_session.ApplySelectedValue(row.Value))
            {
                SynchronizeState();
                StatusText = $"'{row.Name}' applied to AXAML.";
                RaiseMarkupChanged();
            }

            row.IsDraft = false;
            if (finalize)
            {
                _session.EndPropertyEdit();
                row.IsEditing = false;
            }
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            _session.EndPropertyEdit();
            StatusText = $"AXAML property edit failed: {exception.Message}";
        }
    }

    public void BeginPropertyDraft(AxamlPropertyRowViewModel row, bool hasChanged)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (row.IsReadOnly || !PropertyRows.Contains(row))
        {
            return;
        }

        row.IsEditing = true;
        if (hasChanged)
        {
            row.IsDraft = true;
        }

        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    public void SetBindingContext(
        IEnumerable<AxamlMvvmSemanticMember> members,
        string? diagnostic = null)
    {
        ArgumentNullException.ThrowIfNull(members);
        _bindingMembers = members.ToArray();
        _bindingContextDiagnostic = diagnostic;
        SynchronizeBindingRows();
    }

    public void ApplyBindingRow(AxamlBindingRowViewModel row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (!TryResolveBindingRow(row, out string memberName))
        {
            return;
        }

        try
        {
            if (_session.ApplyBinding(row.PropertyName, memberName))
            {
                SynchronizeState();
                StatusText = $"Binding {row.PropertyName} to '{memberName}' applied.";
                RaiseMarkupChanged();
            }
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            row.Diagnostic = exception.Message;
            StatusText = $"AXAML binding edit failed: {exception.Message}";
        }
    }

    public async Task ApplyBindingRowAsync(AxamlBindingRowViewModel row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (!TryResolveBindingRow(row, out string memberName))
        {
            return;
        }

        bool memberExists = _bindingMembers.Any(member =>
            string.Equals(member.Name, memberName, StringComparison.Ordinal));
        if (!memberExists && ApplyMissingBindingAsync is { } applyMissingBindingAsync)
        {
            try
            {
                if (!await applyMissingBindingAsync(row))
                {
                    return;
                }

            }
            catch (Exception exception) when (exception is InvalidDataException
                or InvalidOperationException
                or ArgumentException
                or System.IO.IOException
                or UnauthorizedAccessException
                or AggregateException
                or System.Xml.XmlException)
            {
                row.Diagnostic = exception.Message;
                StatusText = $"AXAML binding change failed: {exception.Message}";
            }

            return;
        }

        ApplyBindingRow(row);
    }

    public string CreateBindingMarkupForRow(AxamlBindingRowViewModel row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (!TryResolveBindingRow(row, out string memberName))
        {
            throw new InvalidOperationException(row.Diagnostic ?? "The binding row is no longer available.");
        }

        return _session.CreateBindingMarkup(row.PropertyName, memberName);
    }

    private bool TryResolveBindingRow(AxamlBindingRowViewModel row, out string memberName)
    {
        memberName = string.Empty;
        if (!BindingRows.Contains(row) || SelectedNode is null)
        {
            return false;
        }

        string? requestedMemberName = row.GetRequestedMember();
        if (requestedMemberName is null)
        {
            return false;
        }
        memberName = requestedMemberName;

        if (AxamlControlCatalog.TryGet(row.ControlType, out AxamlControlCapability capability)
            && capability.BindableProperties.FirstOrDefault(property => property.Name == row.PropertyName) is { } bindableProperty
            && _bindingMembers.FirstOrDefault(member => member.Name == requestedMemberName) is { } existingMember)
        {
            AxamlMvvmMemberPlanEntry compatibility = new AxamlMvvmMemberPlanner()
                .Plan(
                    [new AxamlMvvmMemberRequirement(memberName, bindableProperty.TypeName, bindableProperty.IsCommand)],
                    [existingMember],
                    createMissingMembers: false)[0];
            if (compatibility.Action != AxamlMvvmMemberPlanAction.Reuse)
            {
                row.Diagnostic = compatibility.Diagnostic;
                memberName = string.Empty;
                return false;
            }
        }

        return true;
    }

    public void EndPropertyEdit()
    {
        _session.EndPropertyEdit();
    }

    public void DiscardPropertyDraft(AxamlPropertyRowViewModel row)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (SelectedNode is null || !PropertyRows.Contains(row))
        {
            return;
        }

        _session.EndPropertyEdit();
        row.IsEditing = false;
        row.IsDraft = false;
        AxamlPropertyDescriptor descriptor = AxamlPropertyValueValidator.Describe(SelectedNode, row.Name);
        row.Synchronize(descriptor);
        StatusText = $"Unapplied value for '{row.Name}' discarded.";
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void AddTextBlock()
    {
        AddChildControl("TextBlock");
    }

    [RelayCommand]
    private void AddLabel()
    {
        AddChildControl("Label");
    }

    [RelayCommand]
    private void AddTextBox()
    {
        AddChildControl("TextBox");
    }

    [RelayCommand]
    private void AddButton()
    {
        AddChildControl("Button");
    }

    [RelayCommand]
    private void AddCheckBox()
    {
        AddChildControl("CheckBox");
    }

    [RelayCommand]
    private void AddComboBox()
    {
        AddChildControl("ComboBox");
    }

    [RelayCommand]
    private void AddNumericUpDown()
    {
        AddChildControl("NumericUpDown");
    }

    [RelayCommand]
    private void AddRadioButton()
    {
        AddChildControl("RadioButton");
    }

    [RelayCommand]
    private void AddSlider()
    {
        AddChildControl("Slider");
    }

    [RelayCommand]
    private void AddToggleSwitch()
    {
        AddChildControl("ToggleSwitch");
    }

    [RelayCommand]
    private void AddBorder()
    {
        AddChildControl("Border");
    }

    [RelayCommand]
    private void AddContentControl()
    {
        AddChildControl("ContentControl");
    }

    [RelayCommand]
    private void AddDockPanel()
    {
        AddChildControl("DockPanel");
    }

    [RelayCommand]
    private void AddExpander()
    {
        AddChildControl("Expander");
    }

    [RelayCommand]
    private void AddGrid()
    {
        AddChildControl("Grid");
    }

    [RelayCommand]
    private void AddGroupBox()
    {
        AddChildControl("GroupBox");
    }

    [RelayCommand]
    private void AddProgressBar()
    {
        AddChildControl("ProgressBar");
    }

    [RelayCommand]
    private void AddScrollViewer()
    {
        AddChildControl("ScrollViewer");
    }

    [RelayCommand]
    private void AddSeparator()
    {
        AddChildControl("Separator");
    }

    [RelayCommand]
    private void AddStackPanel()
    {
        AddChildControl("StackPanel");
    }

    [RelayCommand]
    private void AddWrapPanel()
    {
        AddChildControl("WrapPanel");
    }

    [RelayCommand]
    private void AddViewbox()
    {
        AddChildControl("Viewbox");
    }

    [RelayCommand]
    private void AddUniformGrid()
    {
        AddChildControl("UniformGrid");
    }

    private void AddChildControl(string controlType)
    {
        try
        {
            AxamlDockPanelPlacement? dockPanelPlacement = IsDockPanelSelected
                ? SelectedDockPanelPlacement
                : null;
            if (!_session.AddChildControl(controlType, dockPanelPlacement))
            {
                return;
            }

            SynchronizeState();
            StatusText = $"{controlType} inserted into the selected container.";
            RaiseMarkupChanged();
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            StatusText = $"Could not insert {controlType}: {exception.Message}";
        }
    }

    private bool CanApplySelectedValue()
    {
        return SelectedNode is not null
            && SelectedProperty is not null
            && _session.EditableProperties.Contains(SelectedProperty, StringComparer.Ordinal);
    }

    partial void OnSelectedNodeChanged(AxamlPreviewNode? value)
    {
        if (_synchronizing)
        {
            return;
        }

        if (HasInvalidPropertyDraft && !ReferenceEquals(value, _session.SelectedNode))
        {
            _synchronizing = true;
            SelectedNode = _session.SelectedNode;
            _synchronizing = false;
            StatusText = "Resolve the invalid property value or discard it before changing the selected element.";
            return;
        }

        try
        {
            _session.SelectNode(value);
            SynchronizeState();
        }
        catch (InvalidOperationException exception)
        {
            StatusText = $"AXAML selection failed: {exception.Message}";
        }

        ApplySelectedValueCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedPropertyChanged(string? value)
    {
        if (!_synchronizing && !string.IsNullOrWhiteSpace(value))
        {
            try
            {
                _session.SelectProperty(value);
                SelectedValue = _session.SelectedValue;
            }
            catch (InvalidOperationException exception)
            {
                StatusText = $"AXAML property selection failed: {exception.Message}";
                SelectedProperty = null;
            }
        }

        ApplySelectedValueCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(CanUndoDesigner))]
    private void Undo()
    {
        if (_session.Undo())
        {
            SynchronizeState();
            RaiseMarkupChanged();
        }
    }

    private bool CanUndoDesigner() => _session.CanUndo && !HasInvalidPropertyDraft;

    [RelayCommand(CanExecute = nameof(CanRedoDesigner))]
    private void Redo()
    {
        if (_session.Redo())
        {
            SynchronizeState();
            RaiseMarkupChanged();
        }
    }

    private bool CanRedoDesigner() => _session.CanRedo && !HasInvalidPropertyDraft;

    private void SynchronizeState()
    {
        _synchronizing = true;
        try
        {
            PreviewRoot = _session.Root;
            SelectedNode = _session.SelectedNode;
            EditableProperties.Clear();
            foreach (string property in _session.EditableProperties)
            {
                EditableProperties.Add(property);
            }

            SelectedProperty = _session.SelectedProperty;
            SelectedValue = _session.SelectedValue;
            SynchronizePropertyRows();
        }
        finally
        {
            _synchronizing = false;
        }

        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        ApplySelectedValueCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
    }

    private void SynchronizePropertyRows()
    {
        AxamlPreviewNode? node = _session.SelectedNode;
        if (node is null)
        {
            PropertyRows.Clear();
            BindingRows.Clear();
            OnPropertyChanged(nameof(HasBindingRows));
            return;
        }

        HashSet<string> propertyNames = new(node.Properties.Keys, StringComparer.Ordinal);
        if (AxamlControlCatalog.TryGet(node.TypeName, out AxamlControlCapability capability))
        {
            propertyNames.UnionWith(capability.EditableProperties);
            SynchronizeBindingRows(node, capability);
        }
        else
        {
            BindingRows.Clear();
        }

        foreach (string elementName in node.PropertyElementNames)
        {
            int separator = elementName.LastIndexOf('.');
            if (separator >= 0 && separator < elementName.Length - 1)
            {
                propertyNames.Add(elementName[(separator + 1)..]);
            }
        }

        List<AxamlPropertyDescriptor> descriptors = propertyNames
            .OrderBy(static name => name, StringComparer.Ordinal)
            .Select(name => AxamlPropertyValueValidator.Describe(node, name))
            .ToList();
        Dictionary<string, AxamlPropertyRowViewModel> existing = PropertyRows
            .ToDictionary(static row => row.Name, StringComparer.Ordinal);

        for (int index = PropertyRows.Count - 1; index >= 0; index--)
        {
            if (!descriptors.Any(descriptor => descriptor.Name == PropertyRows[index].Name))
            {
                PropertyRows.RemoveAt(index);
            }
        }

        for (int index = 0; index < descriptors.Count; index++)
        {
            AxamlPropertyDescriptor descriptor = descriptors[index];
            if (existing.TryGetValue(descriptor.Name, out AxamlPropertyRowViewModel? row)
                && row.ControlType == node.TypeName)
            {
                row.Synchronize(descriptor);
                int currentIndex = PropertyRows.IndexOf(row);
                if (currentIndex != index)
                {
                    PropertyRows.Move(currentIndex, index);
                }
            }
            else
            {
                PropertyRows.Insert(index, new AxamlPropertyRowViewModel(node.TypeName, descriptor));
            }
        }
    }

    private void RaiseMarkupChanged()
    {
        MarkupChanged?.Invoke(this, new AxamlDesignerMarkupChangedEventArgs(_session.Markup));
    }

    private static string FormatLayoutSize(double value)
    {
        return Math.Clamp(value, 16, 4096).ToString("0.#", CultureInfo.InvariantCulture);
    }
}

public sealed class AxamlDesignerMarkupChangedEventArgs(string markup) : EventArgs
{
    public string Markup { get; } = markup;
}
