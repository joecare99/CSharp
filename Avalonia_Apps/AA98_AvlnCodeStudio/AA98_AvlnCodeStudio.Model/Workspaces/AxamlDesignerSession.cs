using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace AA98_AvlnCodeStudio.Model.Workspaces;

/// <summary>
/// UI-neutral editing session for the safe AXAML designer subset.
/// </summary>
public sealed class AxamlDesignerSession
{
    private const int MaximumHistoryEntries = 100;
    private readonly AxamlPreviewService _previewService;
    private readonly Stack<Snapshot> _undoHistory = [];
    private readonly Stack<Snapshot> _redoHistory = [];
    private Snapshot? _propertyEditSnapshot;
    private string? _propertyEditName;
    private bool _propertyEditHasChanges;

    public AxamlDesignerSession(AxamlPreviewService? previewService = null)
    {
        _previewService = previewService ?? new AxamlPreviewService();
    }

    public string Markup { get; private set; } = string.Empty;

    public AxamlPreviewNode? Root { get; private set; }

    public string? PreviewDiagnostic { get; private set; }

    public AxamlPreviewNode? SelectedNode { get; private set; }

    public string? SelectedProperty { get; private set; }

    public string SelectedValue { get; private set; } = string.Empty;

    public bool CanUndo => _undoHistory.Count > 0;

    public bool CanRedo => _redoHistory.Count > 0;

    public bool CanRemoveSelectedControl => TryGetRemovablePosition(out _, out _);

    public bool CanMoveSelectedControlEarlier =>
        TryGetReorderPosition(out _, out int index) && index > 0;

    public bool CanMoveSelectedControlLater =>
        TryGetReorderPosition(out AxamlPreviewNode? parent, out int index)
        && index < parent!.Children.Count - 1;

    public bool IsSelectedGridChild =>
        TryGetSelectedParent(out AxamlPreviewNode? parent, out _) && parent!.TypeName == "Grid";

    public bool CanEditSelectedGridLayout =>
        TryGetSelectedParent(out AxamlPreviewNode? parent, out _)
        && parent is { IsSupported: true, TypeName: "Grid", HasSafeGridLayout: true }
        && SelectedNode is { IsSupported: true };

    public bool CanMoveSelectedGridControl(int rowChange, int columnChange) =>
        CanEditSelectedGridLayout
        && TryGetSelectedParent(out AxamlPreviewNode? parent, out _)
        && _previewService.CanMoveGridChild(parent!, SelectedNode!, rowChange, columnChange);

    public IReadOnlyList<string> EditableProperties => SelectedNode is null
        ? []
        : _previewService.GetEditableProperties(SelectedNode)
            .Where(property => _previewService.CanEditProperty(SelectedNode, property))
            .ToArray();

    public void Load(string markup)
    {
        ArgumentNullException.ThrowIfNull(markup);
        if (string.Equals(Markup, markup, StringComparison.Ordinal))
        {
            return;
        }

        string? selectedType = SelectedNode?.TypeName;
        int selectedOffset = SelectedNode?.SourceStartOffset ?? -1;
        AxamlPreviewParseResult result = _previewService.ParseForPreview(markup);
        AxamlPreviewNode root = result.Root
            ?? throw new XmlException(result.Diagnostic ?? "The AXAML document could not be previewed.");
        Markup = markup;
        Root = root;
        PreviewDiagnostic = result.Diagnostic;
        SelectedNode = selectedType is null ? null : FindNode(root, selectedType, selectedOffset);
        ClearHistory();
        EndPropertyEdit();
        RefreshSelectedValue();
    }

    public void Reset()
    {
        Markup = string.Empty;
        Root = null;
        PreviewDiagnostic = null;
        SelectedNode = null;
        SelectedProperty = null;
        SelectedValue = string.Empty;
        ClearHistory();
        EndPropertyEdit();
    }

    public void SelectNode(AxamlPreviewNode? node)
    {
        EndPropertyEdit();
        if (node is null)
        {
            SelectedNode = null;
            SelectedProperty = null;
            SelectedValue = string.Empty;
            return;
        }

        if (Root is null || FindNode(Root, node.TypeName, node.SourceStartOffset) is not { } currentNode)
        {
            throw new InvalidOperationException("The selected AXAML control is no longer part of the current preview.");
        }

        SelectedNode = currentNode;
        SelectedProperty = EditableProperties.FirstOrDefault();
        RefreshSelectedValue();
    }

    public void SelectProperty(string propertyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        if (!_previewService.CanEditProperty(SelectedNode
                ?? throw new InvalidOperationException("Select an AXAML control before editing a property."),
                propertyName))
        {
            throw new InvalidOperationException($"The '{propertyName}' property cannot be edited on the selected AXAML control.");
        }

        if (!string.Equals(SelectedProperty, propertyName, StringComparison.Ordinal))
        {
            EndPropertyEdit();
        }

        SelectedProperty = propertyName;
        RefreshSelectedValue();
    }

    public void BeginPropertyEdit(string propertyName)
    {
        SelectProperty(propertyName);
        if (string.Equals(_propertyEditName, propertyName, StringComparison.Ordinal))
        {
            return;
        }

        EndPropertyEdit();
        _propertyEditName = propertyName;
        _propertyEditSnapshot = Capture();
    }

    public void EndPropertyEdit()
    {
        _propertyEditName = null;
        _propertyEditSnapshot = null;
        _propertyEditHasChanges = false;
    }

    public bool ApplySelectedValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        string selectedProperty = SelectedProperty
            ?? throw new InvalidOperationException("Select an AXAML property before applying a value.");
        return ApplyValues(
            new Dictionary<string, string>(StringComparer.Ordinal) { [selectedProperty] = value },
            selectedProperty);
    }

    public bool ApplyBinding(
        string propertyName,
        string memberName,
        AxamlBindingMode bindingMode = AxamlBindingMode.Default,
        AxamlBindingUpdateSourceTrigger updateSourceTrigger = AxamlBindingUpdateSourceTrigger.Default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(memberName);
        EndPropertyEdit();
        AxamlPreviewNode selectedNode = SelectedNode
            ?? throw new InvalidOperationException("Select an AXAML control before creating a binding.");
        string updatedMarkup = CreateBindingMarkup(propertyName, memberName, bindingMode, updateSourceTrigger);
        if (string.Equals(updatedMarkup, Markup, StringComparison.Ordinal))
        {
            return false;
        }

        AxamlPreviewNode root = _previewService.Parse(updatedMarkup);
        PushHistory(_undoHistory, Capture());
        _redoHistory.Clear();
        Markup = updatedMarkup;
        Root = root;
        SelectedNode = FindNode(root, selectedNode.TypeName, selectedNode.SourceStartOffset);
        SelectedProperty = propertyName;
        RefreshSelectedValue();
        return true;
    }

    public string CreateBindingMarkup(
        string propertyName,
        string memberName,
        AxamlBindingMode bindingMode = AxamlBindingMode.Default,
        AxamlBindingUpdateSourceTrigger updateSourceTrigger = AxamlBindingUpdateSourceTrigger.Default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(memberName);
        AxamlPreviewNode selectedNode = SelectedNode
            ?? throw new InvalidOperationException("Select an AXAML control before creating a binding.");
        return _previewService.UpdateBinding(
            Markup,
            selectedNode,
            propertyName,
            memberName,
            bindingMode,
            updateSourceTrigger);
    }

    public bool ApplyValues(IReadOnlyDictionary<string, string> values, string selectedProperty)
    {
        ArgumentNullException.ThrowIfNull(values);
        ArgumentException.ThrowIfNullOrWhiteSpace(selectedProperty);
        AxamlPreviewNode selectedNode = SelectedNode
            ?? throw new InvalidOperationException("Select an AXAML control before applying property values.");
        if (values.Count == 0)
        {
            return false;
        }

        if (!values.ContainsKey(selectedProperty))
        {
            throw new ArgumentException("The selected property must be included in the values being applied.", nameof(selectedProperty));
        }

        string updatedMarkup = Markup;
        AxamlPreviewNode currentNode = selectedNode;
        foreach ((string propertyName, string value) in values)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
            ArgumentNullException.ThrowIfNull(value);
            string? validationError = AxamlPropertyValueValidator.Validate(currentNode.TypeName, propertyName, value);
            if (validationError is not null)
            {
                throw new InvalidOperationException($"The '{propertyName}' value is invalid: {validationError}");
            }

            string previousMarkup = updatedMarkup;
            updatedMarkup = _previewService.UpdateAttribute(updatedMarkup, currentNode, propertyName, value);
            if (!string.Equals(updatedMarkup, previousMarkup, StringComparison.Ordinal))
            {
                AxamlPreviewNode updatedRoot = _previewService.Parse(updatedMarkup);
                currentNode = FindNode(updatedRoot, selectedNode.TypeName, selectedNode.SourceStartOffset)
                    ?? throw new InvalidOperationException("The selected AXAML control could not be located after editing.");
            }
        }

        if (string.Equals(updatedMarkup, Markup, StringComparison.Ordinal))
        {
            return false;
        }

        AxamlPreviewNode root = _previewService.Parse(updatedMarkup);
        if (string.Equals(_propertyEditName, selectedProperty, StringComparison.Ordinal)
            && _propertyEditSnapshot is not null)
        {
            if (!_propertyEditHasChanges)
            {
                PushHistory(_undoHistory, _propertyEditSnapshot);
                _redoHistory.Clear();
                _propertyEditHasChanges = true;
            }
        }
        else
        {
            PushHistory(_undoHistory, Capture());
            _redoHistory.Clear();
        }
        Markup = updatedMarkup;
        Root = root;
        SelectedNode = FindNode(root, selectedNode.TypeName, selectedNode.SourceStartOffset);
        SelectedProperty = selectedProperty;
        RefreshSelectedValue();
        return true;
    }

    public bool AddChildControl(
        string childTypeName,
        AxamlDockPanelPlacement? dockPanelPlacement = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(childTypeName);
        AxamlPreviewNode selectedNode = SelectedNode
            ?? throw new InvalidOperationException("Select a container before inserting a control.");
        string updatedMarkup = _previewService.InsertChildControl(
            Markup,
            selectedNode,
            childTypeName,
            dockPanelPlacement);
        if (string.Equals(updatedMarkup, Markup, StringComparison.Ordinal))
        {
            return false;
        }

        AxamlPreviewNode root = _previewService.Parse(updatedMarkup);
        PushHistory(_undoHistory, Capture());
        _redoHistory.Clear();
        Markup = updatedMarkup;
        Root = root;
        SelectedNode = FindNode(root, selectedNode.TypeName, selectedNode.SourceStartOffset);
        SelectedProperty = EditableProperties.FirstOrDefault();
        RefreshSelectedValue();
        return true;
    }

    public bool RemoveSelectedControl()
    {
        if (!TryGetRemovablePosition(out AxamlPreviewNode? parent, out _))
        {
            throw new InvalidOperationException(
                "Select a direct child of a simple panel, safe Grid or supported single-child container.");
        }

        string updatedMarkup = _previewService.RemoveChildControl(Markup, parent!, SelectedNode!);
        AxamlPreviewNode root = _previewService.Parse(updatedMarkup);
        AxamlPreviewNode newSelection = FindNode(root, parent!.TypeName, parent.SourceStartOffset)
            ?? throw new InvalidDataException("The parent could not be located after removing the control.");
        EndPropertyEdit();
        PushHistory(_undoHistory, Capture());
        _redoHistory.Clear();
        Markup = updatedMarkup;
        Root = root;
        SelectedNode = newSelection;
        SelectedProperty = EditableProperties.FirstOrDefault();
        RefreshSelectedValue();
        return true;
    }

    public bool MoveSelectedControl(int direction)
    {
        if (direction is not (-1 or 1))
        {
            throw new ArgumentOutOfRangeException(nameof(direction), "Move one position earlier or later.");
        }

        if (!TryGetReorderPosition(out AxamlPreviewNode? parent, out int index)
            || index + direction < 0 || index + direction >= parent!.Children.Count)
        {
            throw new InvalidOperationException(
                "Select a direct panel child with an adjacent control in that direction.");
        }

        AxamlPreviewNode selected = SelectedNode!;
        AxamlPreviewNode sibling = parent.Children[index + direction];
        (string updatedMarkup, int selectedOffset) = _previewService.MoveChildControl(
            Markup, parent, selected, sibling, moveEarlier: direction < 0);
        AxamlPreviewNode root = _previewService.Parse(updatedMarkup);
        AxamlPreviewNode newSelection = FindNode(root, selected.TypeName, selectedOffset)
            ?? throw new InvalidDataException("The selected control could not be located after moving it.");
        EndPropertyEdit();
        PushHistory(_undoHistory, Capture());
        _redoHistory.Clear();
        Markup = updatedMarkup;
        Root = root;
        SelectedNode = newSelection;
        SelectedProperty = EditableProperties.FirstOrDefault();
        RefreshSelectedValue();
        return true;
    }

    public bool MoveSelectedGridControl(int rowChange, int columnChange)
    {
        if (!((rowChange is -1 or 1 && columnChange == 0)
            || (columnChange is -1 or 1 && rowChange == 0)))
        {
            throw new ArgumentOutOfRangeException(nameof(rowChange), "Move one row or column at a time.");
        }

        if (!CanMoveSelectedGridControl(rowChange, columnChange)
            || !TryGetSelectedParent(out AxamlPreviewNode? parent, out _))
        {
            throw new InvalidOperationException(
                "Select a Grid child with an unoccupied adjacent position in a simple static layout.");
        }

        AxamlPreviewNode selected = SelectedNode!;
        string updatedMarkup = _previewService.MoveGridChild(Markup, parent!, selected, rowChange, columnChange);
        AxamlPreviewNode root = _previewService.Parse(updatedMarkup);
        AxamlPreviewNode newSelection = FindNode(root, selected.TypeName, selected.SourceStartOffset)
            ?? throw new InvalidDataException("The selected control could not be located after moving it.");
        EndPropertyEdit();
        PushHistory(_undoHistory, Capture());
        _redoHistory.Clear();
        Markup = updatedMarkup;
        Root = root;
        SelectedNode = newSelection;
        SelectedProperty = EditableProperties.FirstOrDefault();
        RefreshSelectedValue();
        return true;
    }

    public bool Undo()
    {
        EndPropertyEdit();
        if (_undoHistory.Count == 0)
        {
            return false;
        }

        PushHistory(_redoHistory, Capture());
        Restore(_undoHistory.Pop());
        return true;
    }

    public bool Redo()
    {
        EndPropertyEdit();
        if (_redoHistory.Count == 0)
        {
            return false;
        }

        PushHistory(_undoHistory, Capture());
        Restore(_redoHistory.Pop());
        return true;
    }

    private void RefreshSelectedValue()
    {
        if (SelectedNode is null || SelectedProperty is null)
        {
            SelectedValue = string.Empty;
            return;
        }

        SelectedValue = SelectedNode.Properties.TryGetValue(SelectedProperty, out string? value)
            ? value
            : SelectedProperty == "Text" || SelectedProperty == "Content"
                ? SelectedNode.Text ?? string.Empty
                : string.Empty;
    }

    private Snapshot Capture()
    {
        return new Snapshot(
            Markup,
            SelectedNode?.TypeName,
            SelectedNode?.SourceStartOffset ?? -1,
            SelectedProperty);
    }

    private void Restore(Snapshot snapshot)
    {
        AxamlPreviewNode root = _previewService.Parse(snapshot.Markup);
        Markup = snapshot.Markup;
        Root = root;
        SelectedNode = snapshot.SelectedType is null
            ? null
            : FindNode(root, snapshot.SelectedType, snapshot.SelectedOffset);
        SelectedProperty = snapshot.SelectedProperty;
        RefreshSelectedValue();
    }

    private void ClearHistory()
    {
        _undoHistory.Clear();
        _redoHistory.Clear();
    }

    private static void PushHistory(Stack<Snapshot> history, Snapshot snapshot)
    {
        if (history.Count == MaximumHistoryEntries)
        {
            Snapshot[] retained = history.Take(MaximumHistoryEntries - 1).Reverse().ToArray();
            history.Clear();
            foreach (Snapshot item in retained)
            {
                history.Push(item);
            }
        }

        history.Push(snapshot);
    }

    private static AxamlPreviewNode? FindNode(AxamlPreviewNode root, string typeName, int sourceStartOffset)
    {
        if (root.TypeName == typeName && root.SourceStartOffset == sourceStartOffset)
        {
            return root;
        }

        foreach (AxamlPreviewNode child in root.Children)
        {
            AxamlPreviewNode? match = FindNode(child, typeName, sourceStartOffset);
            if (match is not null)
            {
                return match;
            }
        }

        return null;
    }

    private bool TryGetStructurePosition(out AxamlPreviewNode? parent, out int index)
    {
        return TryGetSelectedParent(out parent, out index)
            && SelectedNode is { IsSupported: true }
            && (parent is { IsSupported: true, CanAppendChildren: true }
                    && parent.TypeName is "StackPanel" or "WrapPanel" or "UniformGrid"
                || parent is { IsSupported: true, TypeName: "Grid", HasSafeGridLayout: true });
    }

    private bool TryGetRemovablePosition(out AxamlPreviewNode? parent, out int index)
    {
        if (TryGetStructurePosition(out parent, out index))
        {
            return true;
        }

        return TryGetSelectedParent(out parent, out index)
            && SelectedNode is { IsSupported: true }
            && parent is { IsSupported: true, HasInlineText: false, CanRemoveSingleChild: true, Children.Count: 1 }
            && index == 0
            && parent.PropertyElementNames.Count == 0
            && AxamlControlCatalog.TryGet(parent.TypeName, out AxamlControlCapability capability)
            && capability.AllowsSafeChildRemoval;
    }

    private bool TryGetReorderPosition(out AxamlPreviewNode? parent, out int index) =>
        TryGetStructurePosition(out parent, out index) && parent!.TypeName != "Grid";

    private bool TryGetSelectedParent(out AxamlPreviewNode? parent, out int index)
    {
        parent = null;
        index = -1;
        return Root is not null && SelectedNode is { } selected
            && FindParent(Root, selected, out parent, out index);
    }

    private static bool FindParent(
        AxamlPreviewNode node,
        AxamlPreviewNode selected,
        out AxamlPreviewNode? parent,
        out int index)
    {
        for (int childIndex = 0; childIndex < node.Children.Count; childIndex++)
        {
            if (ReferenceEquals(node.Children[childIndex], selected))
            {
                parent = node;
                index = childIndex;
                return true;
            }

            if (FindParent(node.Children[childIndex], selected, out parent, out index))
            {
                return true;
            }
        }

        parent = null;
        index = -1;
        return false;
    }

    private sealed record Snapshot(
        string Markup,
        string? SelectedType,
        int SelectedOffset,
        string? SelectedProperty);
}
