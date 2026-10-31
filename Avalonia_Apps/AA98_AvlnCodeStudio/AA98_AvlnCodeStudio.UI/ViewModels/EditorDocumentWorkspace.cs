using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.UI.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Owns the editor tabs hosted by the workbench.
/// </summary>
public sealed partial class EditorDocumentWorkspace : ObservableObject
{
    private readonly IAvaloniaEditorComponentFactory? _componentFactory;

    public EditorDocumentWorkspace(
        IAvaloniaEditorComponent initialComponent,
        IAvaloniaEditorComponentFactory? componentFactory = null)
    {
        ArgumentNullException.ThrowIfNull(initialComponent);
        _componentFactory = componentFactory;

        Tabs.Add(new EditorTabViewModel(initialComponent));
        _activeTab = Tabs[0];
    }

    public ObservableCollection<EditorTabViewModel> Tabs { get; } = [];

    [ObservableProperty]
    private EditorTabViewModel _activeTab;

    public void Activate(EditorTabViewModel tab)
    {
        ArgumentNullException.ThrowIfNull(tab);
        if (!Tabs.Contains(tab))
        {
            throw new ArgumentException("The tab does not belong to this editor workspace.", nameof(tab));
        }

        ActiveTab = tab;
    }

    public EditorTabViewModel CreateNewDocument()
    {
        if (_componentFactory is null)
        {
            throw new InvalidOperationException("No editor component factory is available to create a document tab.");
        }

        EditorTabViewModel tab = new(_componentFactory.Create());
        Tabs.Add(tab);
        Activate(tab);
        return tab;
    }

    public async Task<IEditorDocument?> OpenOrActivateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        cancellationToken.ThrowIfCancellationRequested();

        string normalizedPath = Path.GetFullPath(filePath);
        if (!File.Exists(normalizedPath))
        {
            return null;
        }

        EditorTabViewModel? existingTab = Tabs.FirstOrDefault(tab =>
            string.Equals(tab.FilePath, normalizedPath, StringComparison.OrdinalIgnoreCase));
        if (existingTab is not null)
        {
            Activate(existingTab);
            return new Navigation.WorkbenchEditorDocument(normalizedPath);
        }

        EditorTabViewModel newTab = CreateNewDocument();
        try
        {
            await newTab.Editor.OpenFileAsync(normalizedPath, cancellationToken);
            return new Navigation.WorkbenchEditorDocument(normalizedPath);
        }
        catch
        {
            Tabs.Remove(newTab);
            if (Tabs.Count > 0)
            {
                Activate(Tabs[^1]);
            }

            throw;
        }
    }

    public EditorTabViewModel? FindByPath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string normalizedPath = Path.GetFullPath(filePath);
        return Tabs.FirstOrDefault(tab =>
            string.Equals(tab.FilePath, normalizedPath, StringComparison.OrdinalIgnoreCase));
    }
}
