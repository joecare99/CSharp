using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using ConsoleLib.Interfaces;
using AControl = Avalonia.Controls.Control;

namespace ConsoleLib.Cxaml.Designer.Preview;

public sealed class PreviewSelectionChangedEventArgs : EventArgs
{
    public PreviewSelectionChangedEventArgs(PreviewControlMapping mapping) => Mapping = mapping;

    public PreviewControlMapping Mapping { get; }
}

public sealed class DesignerPreviewState
{
    private readonly IReadOnlyDictionary<string, PreviewControlMapping> _byId;

    internal DesignerPreviewState(
        IControl consoleRoot,
        AControl root,
        IReadOnlyList<PreviewControlMapping> mappings)
    {
        ConsoleRoot = consoleRoot;
        Root = root;
        Mappings = mappings;
        _byId = mappings.ToDictionary(mapping => mapping.Id, StringComparer.Ordinal);
        Status = "Preview rendered";
    }

    private DesignerPreviewState(string status)
    {
        Status = status;
        Mappings = Array.Empty<PreviewControlMapping>();
        _byId = new Dictionary<string, PreviewControlMapping>(StringComparer.Ordinal);
    }

    public static DesignerPreviewState Unavailable(string status = "Preview unavailable") => new(status);

    public IControl? ConsoleRoot { get; }
    public AControl? Root { get; }
    public AControl? RenderedControl => Root;
    public IReadOnlyList<PreviewControlMapping> Mappings { get; }
    public bool IsAvailable => Root is not null;
    public string Status { get; }
    public PreviewControlMapping? SelectedMapping { get; private set; }

    public event EventHandler<PreviewSelectionChangedEventArgs>? SelectionChanged;

    public bool ActivateSelection(string controlId)
    {
        if (!_byId.TryGetValue(controlId, out var mapping))
            return false;

        SelectedMapping = mapping;
        SelectionChanged?.Invoke(this, new PreviewSelectionChangedEventArgs(mapping));
        return true;
    }

    public bool Select(string controlId) => ActivateSelection(controlId);
}
