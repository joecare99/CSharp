using System;
using System.Collections.Generic;
using System.Linq;

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

    public AxamlDesignerSession(AxamlPreviewService? previewService = null)
    {
        _previewService = previewService ?? new AxamlPreviewService();
    }

    public string Markup { get; private set; } = string.Empty;

    public AxamlPreviewNode? Root { get; private set; }

    public AxamlPreviewNode? SelectedNode { get; private set; }

    public string? SelectedProperty { get; private set; }

    public string SelectedValue { get; private set; } = string.Empty;

    public bool CanUndo => _undoHistory.Count > 0;

    public bool CanRedo => _redoHistory.Count > 0;

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
        AxamlPreviewNode root = _previewService.Parse(markup);
        Markup = markup;
        Root = root;
        SelectedNode = selectedType is null ? null : FindNode(root, selectedType, selectedOffset);
        ClearHistory();
        RefreshSelectedValue();
    }

    public void Reset()
    {
        Markup = string.Empty;
        Root = null;
        SelectedNode = null;
        SelectedProperty = null;
        SelectedValue = string.Empty;
        ClearHistory();
    }

    public void SelectNode(AxamlPreviewNode? node)
    {
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

        SelectedProperty = propertyName;
        RefreshSelectedValue();
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
        PushHistory(_undoHistory, Capture());
        _redoHistory.Clear();
        Markup = updatedMarkup;
        Root = root;
        SelectedNode = FindNode(root, selectedNode.TypeName, selectedNode.SourceStartOffset);
        SelectedProperty = selectedProperty;
        RefreshSelectedValue();
        return true;
    }

    public bool Undo()
    {
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

    private sealed record Snapshot(
        string Markup,
        string? SelectedType,
        int SelectedOffset,
        string? SelectedProperty);
}
