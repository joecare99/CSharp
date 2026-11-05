using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.Model.Workspaces;
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
public sealed partial class EditorDocumentWorkspace : ObservableObject, IWorkspaceOpenDocumentChangeProvider
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

        string normalizedPath = WorkspacePathIdentity.Normalize(filePath);
        if (!File.Exists(normalizedPath))
        {
            return null;
        }

        EditorTabViewModel? existingTab = FindByPath(normalizedPath);
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

        string normalizedPath = WorkspacePathIdentity.Normalize(filePath);
        EditorTabViewModel? match = null;
        foreach (EditorTabViewModel tab in Tabs)
        {
            if (tab.FilePath is not { } tabPath
                || !WorkspacePathIdentity.AreEqual(tabPath, normalizedPath))
            {
                continue;
            }

            if (match is not null)
            {
                throw new InvalidOperationException(
                    $"More than one editor tab represents the file '{normalizedPath}'.");
            }

            match = tab;
        }

        return match;
    }

    public bool IsDocumentOpen(string fullPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);
        string normalizedPath = WorkspacePathIdentity.Normalize(fullPath);
        return Tabs.Any(tab => tab.FilePath is { } filePath
            && WorkspacePathIdentity.AreEqual(filePath, normalizedPath));
    }

    public bool CanApplyChange(string fullPath, string expectedContent, string expectedSavedContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);
        ArgumentNullException.ThrowIfNull(expectedContent);
        ArgumentNullException.ThrowIfNull(expectedSavedContent);
        EditorTabViewModel? tab;
        try
        {
            tab = FindByPath(fullPath);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return tab is not null && tab.Editor.CanApplyExternalChange(fullPath, expectedContent, expectedSavedContent);
    }

    public void ApplyChange(
        string fullPath,
        string expectedContent,
        string expectedSavedContent,
        string proposedContent,
        string proposedSavedContent)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);
        ArgumentNullException.ThrowIfNull(expectedContent);
        ArgumentNullException.ThrowIfNull(expectedSavedContent);
        ArgumentNullException.ThrowIfNull(proposedContent);
        ArgumentNullException.ThrowIfNull(proposedSavedContent);
        EditorTabViewModel? tab = FindByPath(fullPath);
        if (tab is null)
        {
            throw new InvalidOperationException($"The document '{fullPath}' is no longer open.");
        }

        tab.Editor.ApplyExternalChange(
            fullPath,
            expectedContent,
            expectedSavedContent,
            proposedContent,
            proposedSavedContent);
    }
}
