using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Property.Editor;
using Property.Editor.Avalonia;

namespace AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;

public partial class AxamlDesignerViewModel : ObservableObject
{
    private readonly AxamlDesignerSession _session;
    private bool _synchronizing;
    private string _currentMarkup = string.Empty;

    public Func<AxamlBindingRowViewModel, Task<bool>>? ApplyMissingBindingAsync { get; set; }

    public AxamlDesignerViewModel(AxamlPreviewService? previewService = null)
    {
        _session = new AxamlDesignerSession(previewService);
        PropertyProvider = new AxamlDesignerPropertyProvider(this);
        PropertyEditor.EditStarted += OnPropertyEditorEditStarted;
        PropertyEditor.EditCompleted += OnPropertyEditorEditCompleted;
    }

    public AxamlDesignerPropertyProvider PropertyProvider { get; }

    public PropertyEditorViewModel PropertyEditor { get; } = new();

    public event Action<AxamlPropertyRowViewModel>? PropertyDraftChanged;

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
            string? parsedMemberName = AxamlBindingRowViewModel.ReadBindingPath(
                expression,
                out AxamlBindingMode parsedBindingMode,
                out AxamlBindingUpdateSourceTrigger parsedUpdateSourceTrigger);
            string memberName = parsedMemberName ?? string.Empty;
            bool isNewRow = !existing.TryGetValue(property.Name, out AxamlBindingRowViewModel? current)
                || current.ControlType != node.TypeName;
            AxamlBindingRowViewModel row = isNewRow
                ? new AxamlBindingRowViewModel(
                    node.TypeName,
                    property.Name,
                    expression,
                    property.TypeName,
                    property.IsCommand,
                    property.DefaultBindingMode,
                    property.SupportsUpdateSourceTrigger)
                : current!;
            bool bindingChanged = !string.Equals(row.MemberName, memberName, StringComparison.Ordinal);
            AxamlBindingMode desiredBindingMode = parsedMemberName is null
                ? property.DefaultBindingMode
                : parsedBindingMode;
            bool bindingModeChanged = row.BindingMode != desiredBindingMode;
            AxamlBindingUpdateSourceTrigger desiredUpdateSourceTrigger = parsedMemberName is null
                ? AxamlBindingUpdateSourceTrigger.Default
                : parsedUpdateSourceTrigger;
            bool updateSourceTriggerChanged = row.UpdateSourceTrigger != desiredUpdateSourceTrigger;
            row.MemberName = memberName;
            if (isNewRow || bindingChanged || bindingModeChanged)
            {
                row.BindingMode = desiredBindingMode;
            }
            if (isNewRow || bindingChanged || updateSourceTriggerChanged)
            {
                row.UpdateSourceTrigger = desiredUpdateSourceTrigger;
            }
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
    [NotifyPropertyChangedFor(nameof(StructureRoots))]
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

    public ObservableCollection<AxamlPropertyDraft> UnappliedDrafts { get; } = [];

    public bool HasUnappliedPropertyDrafts =>
        UnappliedDrafts.Count > 0 || PropertyRows.Any(static row => row.IsDraft);

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

    public bool IsSelectedGridChild => _session.IsSelectedGridChild;

    private bool IsDockPanelClientFillEnabled =>
        SelectedNode is not null
        && (!SelectedNode.Properties.TryGetValue("LastChildFill", out string? value)
            || !string.Equals(value, "False", StringComparison.OrdinalIgnoreCase));

    public bool HasPreview => PreviewRoot is not null || !string.IsNullOrWhiteSpace(ErrorText);

    public IReadOnlyList<AxamlPreviewNode> StructureRoots => PreviewRoot is { } root ? [root] : [];

    public string StructureAvailabilityText => HasUnappliedPropertyDrafts
        ? "Apply or discard all property drafts before changing the structure."
        : IsSelectedGridChild
            ? _session.CanEditSelectedGridLayout
                ? "Move by row or column into unoccupied cells; spans are preserved. Remove is available for safe content."
                : "Grid editing requires an unambiguous static layout with supported children and non-overlapping cells."
            : "Remove from supported single-child containers; reorder simple panels; move Grid children by cell.";

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
                : capability.AllowsSafeChildRemoval
                    ? SelectedNode.CanRemoveSingleChild
                        ? $"{childLimit}; safe removal for direct content"
                        : SelectedNode.Children.Count == 0
                            ? $"{childLimit}; empty containers only"
                            : $"{childLimit}; correct unsupported content before editing"
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
            SelectedNode = node;
            if (!ReferenceEquals(_session.SelectedNode, node))
            {
                return false;
            }
            if (!EnsureNoCurrentDrafts())
            {
                return false;
            }
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
        markup ??= string.Empty;
        if (!string.Equals(_currentMarkup, markup, StringComparison.Ordinal))
        {
            StoreCurrentDrafts();
            ClearPropertyRows();
        }

        _currentMarkup = markup;
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
        if (SelectedProperty is null || SelectedNode is null || !EnsureNoCurrentDrafts())
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

    public bool ApplyPropertyRow(AxamlPropertyRowViewModel row, bool finalize)
    {
        ArgumentNullException.ThrowIfNull(row);
        if (row.IsReadOnly || row.HasValidationError || SelectedNode is null || !PropertyRows.Contains(row))
        {
            return false;
        }

        try
        {
            row.IsEditing = true;
            _session.BeginPropertyEdit(row.Name);
            bool applied = _session.ApplySelectedValue(row.Value);
            if (applied)
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

            return applied;
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            _session.EndPropertyEdit();
            StatusText = $"AXAML property edit failed: {exception.Message}";
            return false;
        }
    }

    public void ApplyPropertyDrafts()
    {
        AxamlPropertyRowViewModel[] drafts = PropertyRows
            .Where(static row => row.IsDraft)
            .ToArray();
        AxamlPropertyRowViewModel? invalidDraft = drafts.FirstOrDefault(
            static row => row.IsReadOnly || row.HasValidationError);
        if (invalidDraft is not null)
        {
            throw new InvalidOperationException(
                invalidDraft.ValidationError
                ?? invalidDraft.ReadOnlyReason
                ?? $"Property '{invalidDraft.Name}' cannot be applied.");
        }

        foreach (AxamlPropertyRowViewModel row in drafts)
        {
            if (!ApplyPropertyRow(row, finalize: true))
            {
                throw new InvalidOperationException(
                    $"The AXAML property '{row.Name}' could not be applied. {StatusText}");
            }
        }
    }

    public void DiscardPropertyDrafts()
    {
        foreach (AxamlPropertyRowViewModel row in PropertyRows.Where(static row => row.IsDraft).ToArray())
        {
            DiscardPropertyDraft(row);
        }
    }

    public void DiscardAllPropertyDrafts()
    {
        DiscardPropertyDrafts();
        UnappliedDrafts.Clear();
        NotifyDraftStateChanged();
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
        NotifyDraftStateChanged();
    }

    public bool RestoreDraft(AxamlPropertyDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        if (!UnappliedDrafts.Contains(draft) || !draft.CanRestore)
        {
            return false;
        }

        AxamlPropertyRowViewModel? row = PropertyRows.FirstOrDefault(item =>
            item.Name == draft.PropertyName && !item.IsReadOnly && !item.IsDraft);
        if (row is null)
        {
            return false;
        }

        _synchronizing = true;
        try
        {
            row.IsEditing = true;
            row.IsDraft = true;
            row.Value = draft.Value;
        }
        finally
        {
            _synchronizing = false;
        }
        UnappliedDrafts.Remove(draft);
        StatusText = $"Draft for '{draft.PropertyName}' restored; review it before applying.";
        NotifyDraftStateChanged();
        RefreshDraftAvailability();
        return true;
    }

    public void DiscardStoredDraft(AxamlPropertyDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);
        if (UnappliedDrafts.Remove(draft))
        {
            NotifyDraftStateChanged();
        }
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
            if (_session.ApplyBinding(
                row.PropertyName,
                memberName,
                row.BindingMode,
                row.UpdateSourceTrigger))
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

        return _session.CreateBindingMarkup(
            row.PropertyName,
            memberName,
            row.BindingMode,
            row.UpdateSourceTrigger);
    }

    private bool TryResolveBindingRow(AxamlBindingRowViewModel row, out string memberName)
    {
        memberName = string.Empty;
        if (!BindingRows.Contains(row) || SelectedNode is null)
        {
            return false;
        }

        if (!EnsureNoCurrentDrafts())
        {
            row.Diagnostic = StatusText;
            return false;
        }

        if (!row.CanEditBinding)
        {
            row.Diagnostic = row.BindingExpressionDiagnostic;
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
    private void AddAutoCompleteBox()
    {
        AddChildControl("AutoCompleteBox");
    }

    [RelayCommand]
    private void AddCalendar()
    {
        AddChildControl("Calendar");
    }

    [RelayCommand]
    private void AddCalendarDatePicker()
    {
        AddChildControl("CalendarDatePicker");
    }

    [RelayCommand]
    private void AddColorPicker()
    {
        AddChildControl("ColorPicker");
    }

    [RelayCommand]
    private void AddDatePicker()
    {
        AddChildControl("DatePicker");
    }

    [RelayCommand]
    private void AddItemsControl()
    {
        AddChildControl("ItemsControl");
    }

    [RelayCommand]
    private void AddLabel()
    {
        AddChildControl("Label");
    }

    [RelayCommand]
    private void AddLine()
    {
        AddChildControl("Line");
    }

    [RelayCommand(CanExecute = nameof(CanAddComboBoxItem))]
    private void AddComboBoxItem()
    {
        AddChildControl("ComboBoxItem");
    }

    private bool CanAddComboBoxItem() => CanAddPaletteItem("ComboBox", "ComboBoxItem");

    [RelayCommand]
    private void AddListBox()
    {
        AddChildControl("ListBox");
    }

    [RelayCommand(CanExecute = nameof(CanAddListBoxItem))]
    private void AddListBoxItem()
    {
        AddChildControl("ListBoxItem");
    }

    private bool CanAddListBoxItem() => CanAddPaletteItem("ListBox", "ListBoxItem");

    [RelayCommand]
    private void AddPolygon()
    {
        AddChildControl("Polygon");
    }

    [RelayCommand]
    private void AddPolyline()
    {
        AddChildControl("Polyline");
    }

    [RelayCommand]
    private void AddTextBox()
    {
        AddChildControl("TextBox");
    }

    [RelayCommand]
    private void AddTimePicker()
    {
        AddChildControl("TimePicker");
    }

    [RelayCommand]
    private void AddTreeView()
    {
        AddChildControl("TreeView");
    }

    [RelayCommand(CanExecute = nameof(CanAddTreeViewItem))]
    private void AddTreeViewItem()
    {
        AddChildControl("TreeViewItem");
    }

    private bool CanAddTreeViewItem() => CanAddPaletteItem("TreeView", "TreeViewItem")
        || CanAddPaletteItem("TreeViewItem", "TreeViewItem");

    [RelayCommand]
    private void AddButton()
    {
        AddChildControl("Button");
    }

    [RelayCommand]
    private void AddEllipse()
    {
        AddChildControl("Ellipse");
    }

    [RelayCommand]
    private void AddRectangle()
    {
        AddChildControl("Rectangle");
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
    private void AddTabControl()
    {
        AddChildControl("TabControl");
    }

    [RelayCommand]
    private void AddTabItem()
    {
        AddChildControl("TabItem");
    }

    [RelayCommand]
    private void AddMenu()
    {
        AddChildControl("Menu");
    }

    [RelayCommand]
    private void AddMenuItem()
    {
        AddChildControl("MenuItem");
    }

    [RelayCommand]
    private void AddContextMenu()
    {
        if (!EnsureNoStructureDrafts())
        {
            return;
        }

        try
        {
            if (!_session.AddContextMenu())
            {
                return;
            }

            SynchronizeState();
            StatusText = "ContextMenu added to the selected control.";
            RaiseMarkupChanged();
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            StatusText = $"Could not add ContextMenu: {exception.Message}";
        }
    }

    [RelayCommand]
    private void AddToolTip()
    {
        if (!EnsureNoStructureDrafts())
        {
            return;
        }

        try
        {
            if (!_session.AddToolTip())
            {
                return;
            }

            SynchronizeState();
            StatusText = "ToolTip added to the selected control.";
            RaiseMarkupChanged();
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException)
        {
            StatusText = $"Could not add ToolTip: {exception.Message}";
        }
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
        if (!EnsureNoStructureDrafts())
        {
            return;
        }

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

    private bool CanAddPaletteItem(string parentType, string itemType)
    {
        return _session.SelectedNode is { IsSupported: true, CanAppendChildren: true } selectedNode
            && string.Equals(selectedNode.TypeName, parentType, StringComparison.Ordinal)
            && AxamlControlCatalog.TryGet(parentType, out AxamlControlCapability capability)
            && capability.AllowsAppendingChildren
            && capability.AllowedChildTypes.Contains(itemType, StringComparer.Ordinal);
    }

    [RelayCommand(CanExecute = nameof(CanRemoveStructure))]
    private void RemoveStructure() =>
        ApplyStructureEdit(_session.RemoveSelectedControl, "Control removed; Undo restores it.");

    [RelayCommand(CanExecute = nameof(CanMoveStructureEarlier))]
    private void MoveStructureEarlier() =>
        ApplyStructureEdit(() => _session.MoveSelectedControl(-1), "Control moved earlier.");

    [RelayCommand(CanExecute = nameof(CanMoveStructureLater))]
    private void MoveStructureLater() =>
        ApplyStructureEdit(() => _session.MoveSelectedControl(1), "Control moved later.");

    [RelayCommand(CanExecute = nameof(CanMoveGridRowEarlier))]
    private void MoveGridRowEarlier() =>
        ApplyStructureEdit(() => _session.MoveSelectedGridControl(-1, 0), "Control moved to the previous Grid row.");

    [RelayCommand(CanExecute = nameof(CanMoveGridRowLater))]
    private void MoveGridRowLater() =>
        ApplyStructureEdit(() => _session.MoveSelectedGridControl(1, 0), "Control moved to the next Grid row.");

    [RelayCommand(CanExecute = nameof(CanMoveGridColumnEarlier))]
    private void MoveGridColumnEarlier() =>
        ApplyStructureEdit(() => _session.MoveSelectedGridControl(0, -1), "Control moved to the previous Grid column.");

    [RelayCommand(CanExecute = nameof(CanMoveGridColumnLater))]
    private void MoveGridColumnLater() =>
        ApplyStructureEdit(() => _session.MoveSelectedGridControl(0, 1), "Control moved to the next Grid column.");

    private bool CanRemoveStructure() => !HasUnappliedPropertyDrafts && _session.CanRemoveSelectedControl;

    private bool CanMoveStructureEarlier() =>
        !HasUnappliedPropertyDrafts && _session.CanMoveSelectedControlEarlier;

    private bool CanMoveStructureLater() =>
        !HasUnappliedPropertyDrafts && _session.CanMoveSelectedControlLater;

    private bool CanMoveGridRowEarlier() =>
        !HasUnappliedPropertyDrafts && _session.CanMoveSelectedGridControl(-1, 0);

    private bool CanMoveGridRowLater() =>
        !HasUnappliedPropertyDrafts && _session.CanMoveSelectedGridControl(1, 0);

    private bool CanMoveGridColumnEarlier() =>
        !HasUnappliedPropertyDrafts && _session.CanMoveSelectedGridControl(0, -1);

    private bool CanMoveGridColumnLater() =>
        !HasUnappliedPropertyDrafts && _session.CanMoveSelectedGridControl(0, 1);

    private void ApplyStructureEdit(Func<bool> edit, string successMessage)
    {
        if (!EnsureNoStructureDrafts())
        {
            return;
        }

        try
        {
            if (!edit())
            {
                return;
            }

            SynchronizeState();
            StatusText = successMessage;
            RaiseMarkupChanged();
        }
        catch (Exception exception) when (exception is InvalidDataException or InvalidOperationException or ArgumentException or XmlException)
        {
            StatusText = $"AXAML structure edit failed: {exception.Message}";
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

        if (!ReferenceEquals(value, _session.SelectedNode))
        {
            StoreCurrentDrafts();
            ClearPropertyRows();
        }

        try
        {
            _session.SelectNode(value);
            SynchronizeState();
        }
        catch (InvalidOperationException exception)
        {
            StatusText = $"AXAML selection failed: {exception.Message}";
            SynchronizeState();
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
        if (!EnsureNoStructureDrafts())
        {
            return;
        }

        if (_session.Undo())
        {
            SynchronizeState();
            RaiseMarkupChanged();
        }
    }

    private bool CanUndoDesigner() => _session.CanUndo && !HasUnappliedPropertyDrafts;

    [RelayCommand(CanExecute = nameof(CanRedoDesigner))]
    private void Redo()
    {
        if (!EnsureNoStructureDrafts())
        {
            return;
        }

        if (_session.Redo())
        {
            SynchronizeState();
            RaiseMarkupChanged();
        }
    }

    private bool CanRedoDesigner() => _session.CanRedo && !HasUnappliedPropertyDrafts;

    private void SynchronizeState()
    {
        if (_session.Root is not null)
        {
            _currentMarkup = _session.Markup;
        }

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
            RestoreUnchangedDrafts();
            RefreshDraftAvailability();
        }
        finally
        {
            _synchronizing = false;
        }

        OnPropertyChanged(nameof(CanUndo));
        OnPropertyChanged(nameof(CanRedo));
        OnPropertyChanged(nameof(IsSelectedGridChild));
        OnPropertyChanged(nameof(StructureAvailabilityText));
        ApplySelectedValueCommand.NotifyCanExecuteChanged();
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
        NotifyStructureCommandsChanged();
        NotifyPaletteCommandsChanged();
    }

    private void SynchronizePropertyRows()
    {
        AxamlPreviewNode? node = _session.SelectedNode;
        if (node is null)
        {
            ClearPropertyRows();
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
            if (PropertyRows[index].ControlType != node.TypeName
                || !descriptors.Any(descriptor => descriptor.Name == PropertyRows[index].Name))
            {
                PropertyRows[index].ValueChanged -= OnPropertyRowValueChanged;
                PropertyRows[index].PropertyChanged -= OnPropertyRowPropertyChanged;
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
                AxamlPropertyRowViewModel newRow = new(node.TypeName, descriptor);
                newRow.ValueChanged += OnPropertyRowValueChanged;
                newRow.PropertyChanged += OnPropertyRowPropertyChanged;
                PropertyRows.Insert(index, newRow);
            }
        }

        PropertyEditor.SetItems(PropertyRows);
    }

    private void StoreCurrentDrafts()
    {
        AxamlPreviewNode? node = _session.SelectedNode;
        string? key = GetTargetKey(node);
        if (node is null || key is null)
        {
            return;
        }

        foreach (AxamlPropertyRowViewModel row in PropertyRows.Where(static row => row.IsDraft))
        {
            string target = string.IsNullOrWhiteSpace(node.StableName)
                ? $"{node.TypeName} ({key})"
                : $"{node.TypeName} '{node.StableName}'";
            UnappliedDrafts.Add(new AxamlPropertyDraft(key, target, node.TypeName, row.Name, row.Value, _currentMarkup));
        }

        NotifyDraftStateChanged();
    }

    private void ClearPropertyRows()
    {
        PropertyEditor.SetItems([]);
        foreach (AxamlPropertyRowViewModel row in PropertyRows)
        {
            row.ValueChanged -= OnPropertyRowValueChanged;
            row.PropertyChanged -= OnPropertyRowPropertyChanged;
        }

        PropertyRows.Clear();
        BindingRows.Clear();
        OnPropertyChanged(nameof(HasBindingRows));
    }

    private void RestoreUnchangedDrafts()
    {
        AxamlPreviewNode? node = _session.SelectedNode;
        string? key = GetTargetKey(node);
        if (node is null || key is null)
        {
            return;
        }

        foreach (AxamlPropertyDraft draft in UnappliedDrafts.Where(item =>
                     item.TargetKey == key && item.ControlType == node.TypeName
                     && string.Equals(item.SourceMarkup, _currentMarkup, StringComparison.Ordinal)).ToArray())
        {
            AxamlPropertyRowViewModel? row = PropertyRows.FirstOrDefault(item =>
                item.Name == draft.PropertyName && !item.IsReadOnly && !item.IsDraft);
            if (row is null)
            {
                continue;
            }

            row.IsEditing = true;
            row.IsDraft = true;
            row.Value = draft.Value;
            UnappliedDrafts.Remove(draft);
        }

        NotifyDraftStateChanged();
    }

    private void RefreshDraftAvailability()
    {
        AxamlPreviewNode? node = _session.SelectedNode;
        string? key = GetTargetKey(node);
        foreach (AxamlPropertyDraft draft in UnappliedDrafts)
        {
            bool unchanged = string.Equals(draft.SourceMarkup, _currentMarkup, StringComparison.Ordinal);
            draft.CanRestore = !unchanged && key is not null && key.StartsWith("name:", StringComparison.Ordinal)
                && draft.TargetKey == key && draft.ControlType == node?.TypeName
                && PropertyRows.Any(row => row.Name == draft.PropertyName && !row.IsReadOnly && !row.IsDraft);
            draft.Status = unchanged
                ? "Select the original control to restore this draft."
                : draft.CanRestore
                    ? "The source changed. Restore explicitly only after reviewing the target."
                    : "The source or target changed. Copy the retained value or discard it explicitly.";
        }
    }

    private string? GetTargetKey(AxamlPreviewNode? node)
    {
        if (node is null || _session.Root is not { } root)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(node.StableName)
            && CountNamedNodes(root, node.StableName) == 1)
        {
            return $"name:{node.StableName}";
        }

        return FindNodePath(root, node, "root");
    }

    private static int CountNamedNodes(AxamlPreviewNode node, string name) =>
        (string.Equals(node.StableName, name, StringComparison.Ordinal) ? 1 : 0)
        + node.Children.Sum(child => CountNamedNodes(child, name));

    private static string? FindNodePath(AxamlPreviewNode current, AxamlPreviewNode target, string path)
    {
        if (ReferenceEquals(current, target))
        {
            return $"path:{path}";
        }

        for (int index = 0; index < current.Children.Count; index++)
        {
            string? found = FindNodePath(current.Children[index], target, $"{path}.{index}");
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    private void OnPropertyRowPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(AxamlPropertyRowViewModel.IsDraft))
        {
            NotifyDraftStateChanged();
        }
    }

    private void NotifyDraftStateChanged()
    {
        OnPropertyChanged(nameof(HasUnappliedPropertyDrafts));
        OnPropertyChanged(nameof(HasInvalidPropertyDraft));
        OnPropertyChanged(nameof(StructureAvailabilityText));
        UndoCommand.NotifyCanExecuteChanged();
        RedoCommand.NotifyCanExecuteChanged();
        NotifyStructureCommandsChanged();
    }

    private void NotifyStructureCommandsChanged()
    {
        RemoveStructureCommand.NotifyCanExecuteChanged();
        MoveStructureEarlierCommand.NotifyCanExecuteChanged();
        MoveStructureLaterCommand.NotifyCanExecuteChanged();
        MoveGridRowEarlierCommand.NotifyCanExecuteChanged();
        MoveGridRowLaterCommand.NotifyCanExecuteChanged();
        MoveGridColumnEarlierCommand.NotifyCanExecuteChanged();
        MoveGridColumnLaterCommand.NotifyCanExecuteChanged();
    }

    private void NotifyPaletteCommandsChanged()
    {
        AddComboBoxItemCommand.NotifyCanExecuteChanged();
        AddListBoxItemCommand.NotifyCanExecuteChanged();
        AddTreeViewItemCommand.NotifyCanExecuteChanged();
    }

    private bool EnsureNoStructureDrafts()
    {
        if (!HasUnappliedPropertyDrafts)
        {
            return true;
        }

        StatusText = StructureAvailabilityText;
        return false;
    }

    private bool EnsureNoCurrentDrafts()
    {
        if (!PropertyRows.Any(static row => row.IsDraft))
        {
            return true;
        }

        StatusText = "Apply or discard the current control's property drafts before this designer operation.";
        return false;
    }

    private void OnPropertyEditorEditStarted(object? sender, PropertyEditorEditEventArgs eventArgs)
    {
        if (eventArgs.PropertyItem is AxamlPropertyRowViewModel row)
        {
            BeginPropertyDraft(row, hasChanged: false);
        }
    }

    private void OnPropertyEditorEditCompleted(object? sender, PropertyEditorEditEventArgs eventArgs)
    {
        if (eventArgs.PropertyItem is not AxamlPropertyRowViewModel row || !row.IsEditing)
        {
            return;
        }

        if (row.HasValidationError)
        {
            EndPropertyEdit();
            row.IsEditing = false;
            return;
        }

        ApplyPropertyRow(row, finalize: true);
    }

    private void OnPropertyRowValueChanged(object? sender, PropertyValueChangedEventArgs eventArgs)
    {
        if (!_synchronizing
            && sender is AxamlPropertyRowViewModel { IsEditing: true, IsDraft: true } row)
        {
            PropertyDraftChanged?.Invoke(row);
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
