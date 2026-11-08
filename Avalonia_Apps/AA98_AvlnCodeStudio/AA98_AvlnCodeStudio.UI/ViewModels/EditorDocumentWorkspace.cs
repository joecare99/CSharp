using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.Editor.Services;
using AA98_AvlnCodeStudio.Model.Documents;
using AA98_AvlnCodeStudio.Model.Workspaces;
using AA98_AvlnCodeStudio.UI.Components;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

/// <summary>
/// Owns the editor tabs hosted by the workbench.
/// </summary>
public sealed partial class EditorDocumentWorkspace : ObservableObject, IWorkspaceOpenDocumentChangeProvider, IDisposable
{
    private readonly IAvaloniaEditorComponentFactory? _componentFactory;
    private readonly DocumentEditorRegistry<EditorTabViewModel>? _editorRegistry;
    private readonly object _gate = new();
    private readonly Dictionary<string, EditorTabViewModel> _savingPaths = new(WorkspacePathIdentity.Comparer);
    private long _activationVersion;
    private bool _disposed;

    public EditorDocumentWorkspace(
        IAvaloniaEditorComponent initialComponent,
        IAvaloniaEditorComponentFactory? componentFactory = null,
        IEnumerable<IDocumentEditorFactory<EditorTabViewModel>>? editorFactories = null)
    {
        ArgumentNullException.ThrowIfNull(initialComponent);
        _componentFactory = componentFactory;
        if (componentFactory is not null)
        {
            IDocumentEditorFactory<EditorTabViewModel>[] builtInFactories =
            [
                new WorkbenchDocumentEditorFactory(componentFactory, "axaml", 100, ".axaml"),
                new WorkbenchDocumentEditorFactory(componentFactory, "text", 0,
                    ".cs", ".xaml", ".xml", ".json", ".txt", ".md", ".csproj", ".sln", ".slnx",
                    ".slnf", ".props", ".targets", ".config", ".yml", ".yaml", ".toml", ".ini",
                    ".gitignore", ".editorconfig", ".resx", ".sql", ".sh", ".ps1", ".bat", ".cmd",
                    ".html", ".css", ".js", ".ts", ".fs", ".fsproj", ".vb", ".vbproj", ".razor",
                    ".cshtml", ".http", ".log", ".csv", ".nuspec"),
            ];
            _editorRegistry = new DocumentEditorRegistry<EditorTabViewModel>(
                builtInFactories.Concat(editorFactories ?? []));
        }

        EditorTabViewModel initialTab = new(initialComponent);
        ConfigureSaveCoordination(initialTab);
        Tabs.Add(initialTab);
        _activeTab = Tabs[0];
    }

    public ObservableCollection<EditorTabViewModel> Tabs { get; } = [];

    public event Action<EditorTabViewModel>? TabClosed;

    /// <summary>Signals a document navigation intent before opening or creating a tab.</summary>
    public event Action<CancellationToken>? NavigationRequested;

    [ObservableProperty]
    private EditorTabViewModel _activeTab;

    partial void OnActiveTabChanged(EditorTabViewModel value) => Interlocked.Increment(ref _activationVersion);

    public void CancelPendingPreviews() => Interlocked.Increment(ref _activationVersion);

    public void Activate(EditorTabViewModel tab)
    {
        ArgumentNullException.ThrowIfNull(tab);
        ObjectDisposedException.ThrowIf(_disposed, this);
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

        NavigationRequested?.Invoke(CancellationToken.None);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            EditorTabViewModel tab = new(_componentFactory.Create());
            ConfigureSaveCoordination(tab);
            Tabs.Add(tab);
            Activate(tab);
            return tab;
        }
    }

    public async Task<IEditorDocument?> OpenOrActivateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        return await OpenAsync(filePath, preview: false, cancellationToken);
    }

    public async Task<IEditorDocument?> OpenPreviewAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        return await OpenAsync(filePath, preview: true, cancellationToken);
    }

    private async Task<IEditorDocument?> OpenAsync(
        string filePath,
        bool preview,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        cancellationToken.ThrowIfCancellationRequested();
        NavigationRequested?.Invoke(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();

        string normalizedPath = WorkspacePathIdentity.Normalize(filePath);
        long requestVersion = Interlocked.Increment(ref _activationVersion);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            EditorTabViewModel? existingTab = FindByPath(normalizedPath);
            if (existingTab is not null)
            {
                if (!preview)
                {
                    existingTab.Pin();
                }

                Activate(existingTab);
                return existingTab.IsDiagnostic ? null : new Navigation.WorkbenchEditorDocument(normalizedPath);
            }
            if (_savingPaths.ContainsKey(normalizedPath))
            {
                throw new IOException("The requested file is currently being saved. Try opening it again when the save completes.");
            }
        }

        if (_componentFactory is null || _editorRegistry is null)
        {
            throw new InvalidOperationException("No editor component factory is available to open a document.");
        }

        IDocumentEditorFactory<EditorTabViewModel>? factory = _editorRegistry.FindFactory(normalizedPath);
        EditorTabViewModel newTab = factory?.Create(normalizedPath) ?? new EditorTabViewModel(_componentFactory.Create());
        ConfigureSaveCoordination(newTab);
        newTab.IsPreview = preview;
        if (newTab.IsProtected)
        {
            newTab.Pin();
        }
        try
        {
            if (!File.Exists(normalizedPath))
            {
                newTab.ShowDiagnostic(normalizedPath, "The file is missing or is not accessible.");
            }
            else if (factory is null)
            {
                newTab.ShowDiagnostic(normalizedPath, "No editor is registered for this file type. Its contents were not opened as text.");
            }
            else
            {
                try
                {
                    await newTab.OpenDocumentAsync(normalizedPath, cancellationToken);
                }
                catch (Exception exception) when (exception is IOException or InvalidDataException or UnauthorizedAccessException)
                {
                    newTab.ShowDiagnostic(normalizedPath, $"The file could not be opened safely: {exception.Message}");
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (newTab.IsProtected)
            {
                newTab.Pin();
            }
            lock (_gate)
            {
                ObjectDisposedException.ThrowIf(_disposed, this);
                bool isCurrentRequest = requestVersion == Interlocked.Read(ref _activationVersion);
                EditorTabViewModel? existingTab = FindByPath(normalizedPath);
                if (existingTab is not null)
                {
                    newTab.Dispose();
                    if (!preview)
                    {
                        existingTab.Pin();
                    }

                    if (isCurrentRequest)
                    {
                        Activate(existingTab);
                    }

                    return isCurrentRequest && !existingTab.IsDiagnostic
                        ? new Navigation.WorkbenchEditorDocument(normalizedPath)
                        : null;
                }

                if (_savingPaths.ContainsKey(normalizedPath))
                {
                    throw new IOException("The requested file is currently being saved. Try opening it again when the save completes.");
                }

                if (preview && !isCurrentRequest)
                {
                    newTab.Dispose();
                    return null;
                }

                foreach (EditorTabViewModel savingTab in Tabs.Where(tab =>
                             tab.IsPreview && IsSavingCore(tab)))
                {
                    savingTab.Pin();
                }

                EditorTabViewModel? oldPreview = preview
                    ? Tabs.FirstOrDefault(tab => tab.IsPreview && !tab.IsProtected)
                    : null;
                Tabs.Add(newTab);
                if (isCurrentRequest)
                {
                    Activate(newTab);
                }

                if (oldPreview is not null)
                {
                    Tabs.Remove(oldPreview);
                    TabClosed?.Invoke(oldPreview);
                    oldPreview.Dispose();
                }

                return isCurrentRequest && !newTab.IsDiagnostic
                    ? new Navigation.WorkbenchEditorDocument(normalizedPath)
                    : null;
            }
        }
        catch
        {
            newTab.Dispose();
            throw;
        }
    }

    public bool TryClose(EditorTabViewModel tab, bool discardProtected = false)
    {
        ArgumentNullException.ThrowIfNull(tab);
        lock (_gate)
        {
            if (!Tabs.Contains(tab)
                || IsSavingCore(tab)
                || (tab.IsProtected && !discardProtected))
            {
                return false;
            }

            Interlocked.Increment(ref _activationVersion);
            if (Tabs.Count == 1)
            {
                if (_componentFactory is null)
                {
                    return false;
                }

                EditorTabViewModel replacement = new(_componentFactory.Create());
                ConfigureSaveCoordination(replacement);
                Tabs.Add(replacement);
            }

            bool wasActive = ReferenceEquals(ActiveTab, tab);
            if (wasActive)
            {
                Activate(Tabs.Last(candidate => !ReferenceEquals(candidate, tab)));
            }

            Tabs.Remove(tab);
            TabClosed?.Invoke(tab);
            tab.Dispose();
            return true;
        }
    }

    public EditorTabViewModel? FindByPath(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        string normalizedPath = WorkspacePathIdentity.Normalize(filePath);
        lock (_gate)
        {
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
    }

    public bool IsDocumentOpen(string fullPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);
        return FindByPath(fullPath) is not null;
    }

    public bool HasInProgressSaves
    {
        get
        {
            lock (_gate)
            {
                return _savingPaths.Count != 0 || Tabs.Any(IsSavingCore);
            }
        }
    }

    public bool IsSaving(EditorTabViewModel tab)
    {
        ArgumentNullException.ThrowIfNull(tab);
        lock (_gate)
        {
            return IsSavingCore(tab);
        }
    }

    private bool IsSavingCore(EditorTabViewModel tab) =>
        _savingPaths.Values.Contains(tab) || tab.SaveCommand.IsRunning || tab.SaveAsCommand.IsRunning;

    public void Dispose()
    {
        lock (_gate)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            Interlocked.Increment(ref _activationVersion);
            foreach (EditorTabViewModel tab in Tabs.ToArray())
            {
                tab.SaveCommand.Cancel();
                tab.SaveAsCommand.Cancel();
                Tabs.Remove(tab);
                TabClosed?.Invoke(tab);
                tab.Dispose();
            }
        }
    }

    private void ConfigureSaveCoordination(EditorTabViewModel tab)
    {
        Func<string, IDisposable> acquireSaveTarget = path => ReserveSaveTarget(tab, path);
        tab.AcquireSaveTarget = acquireSaveTarget;
        if (tab.Component?.Workflow is ICoordinatedEditorSave workflow)
        {
            workflow.AcquireSaveTarget = acquireSaveTarget;
        }
    }

    private IDisposable ReserveSaveTarget(EditorTabViewModel tab, string filePath)
    {
        string normalizedPath = WorkspacePathIdentity.Normalize(filePath);
        lock (_gate)
        {
            if (!Tabs.Contains(tab))
            {
                throw new InvalidOperationException("The document is no longer open.");
            }

            EditorTabViewModel? existing = FindByPath(normalizedPath);
            if (existing is not null && !ReferenceEquals(existing, tab))
            {
                throw new InvalidOperationException("The save destination is already open in another editor tab.");
            }

            if (tab.Designer is not null && tab.HasUnappliedDrafts
                && !string.Equals(Path.GetExtension(normalizedPath), ".axaml", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Apply or discard the AXAML property drafts before saving as a different file type.");
            }

            if (_savingPaths.ContainsKey(normalizedPath) || _savingPaths.Values.Contains(tab))
            {
                throw new InvalidOperationException("This document or save destination is already being saved.");
            }

            _savingPaths.Add(normalizedPath, tab);
            return new SavePathReservation(this, normalizedPath);
        }
    }

    private sealed class SavePathReservation(EditorDocumentWorkspace owner, string path) : IDisposable
    {
        private EditorDocumentWorkspace? _owner = owner;

        public void Dispose()
        {
            EditorDocumentWorkspace? current = Interlocked.Exchange(ref _owner, null);
            if (current is not null)
            {
                lock (current._gate)
                {
                    current._savingPaths.Remove(path);
                }
            }
        }
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

        return tab is not null
            && tab.HasTextEditor
            && tab.Editor.CanApplyExternalChange(fullPath, expectedContent, expectedSavedContent);
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

        if (!tab.HasTextEditor)
        {
            throw new InvalidOperationException("Text changes cannot be applied to this document editor.");
        }

        tab.Editor.ApplyExternalChange(
            fullPath,
            expectedContent,
            expectedSavedContent,
            proposedContent,
            proposedSavedContent);
    }
}
