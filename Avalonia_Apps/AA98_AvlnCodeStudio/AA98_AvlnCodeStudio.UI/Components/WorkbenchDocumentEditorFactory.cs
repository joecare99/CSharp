using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;

namespace AA98_AvlnCodeStudio.UI.Components;

internal sealed class WorkbenchDocumentEditorFactory : IDocumentEditorFactory<EditorTabViewModel>
{
    private readonly IAvaloniaEditorComponentFactory _componentFactory;
    private readonly HashSet<string> _extensions;

    public WorkbenchDocumentEditorFactory(
        IAvaloniaEditorComponentFactory componentFactory,
        string id,
        int priority,
        params string[] extensions)
    {
        _componentFactory = componentFactory;
        Id = id;
        Priority = priority;
        _extensions = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);
    }

    public string Id { get; }

    public int Priority { get; }

    public bool CanOpen(string filePath) => _extensions.Contains(Path.GetExtension(filePath));

    public EditorTabViewModel Create(string filePath) => new(_componentFactory.Create(), Id);
}
