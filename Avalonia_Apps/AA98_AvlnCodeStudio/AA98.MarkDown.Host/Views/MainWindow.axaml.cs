using AA98.MarkDown.Host.Models;
using AA98.MarkDown.Host.ViewModels;
using AA98.MarkDown.UI.Controls;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AA98.MarkDown.Host.Views;

/// <summary>
/// Represents the main window of the markdown mini host.
/// </summary>
public partial class MainWindow : Window
{
    private static readonly IReadOnlyList<FilePickerFileType> MarkdownFileTypes =
    [
        new("Markdown")
        {
            Patterns = ["*.md"],
            MimeTypes = ["text/markdown", "text/plain"],
        },
    ];

    private bool _closeConfirmed;
    private bool _isClosingPromptActive;

    internal Func<FilePickerSaveOptions, Task<string?>>? SavePathPicker { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        Closing += OnWindowClosing;
    }

    private void OnMarkdownLinkInvoked(object? sender, MarkdownLinkInvokedEventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.OpenLinkedDocument(e.LinkTarget);
        }
    }

    private async void OnOpenRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null)
        {
            return;
        }

        IReadOnlyList<IStorageFile> files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Markdown-Datei öffnen",
            AllowMultiple = false,
            FileTypeFilter = MarkdownFileTypes,
        });

        if (files.Count == 0)
        {
            return;
        }

        string? path = TryGetLocalPath(files[0]);
        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "Ausgewählte Datei kann lokal nicht aufgelöst werden.";
            return;
        }

        viewModel.OpenDocument(path);
    }

    private async void OnSaveAsRequested(object? sender, RoutedEventArgs e)
    {
        await SaveAsWithPickerAsync();
    }

    internal async Task SaveAsWithPickerAsync()
    {
        if (DataContext is not MainWindowViewModel viewModel || viewModel.SelectedDocument is not MarkdownDocumentTab document)
        {
            return;
        }

        await PickAndSaveDocumentAsAsync(viewModel, document, false);
    }

    private async Task<string?> PickSavePathAsync(FilePickerSaveOptions options)
    {
        if (SavePathPicker is not null)
        {
            return await SavePathPicker(options);
        }

        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null)
        {
            return null;
        }

        IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(options);
        return file is null ? null : TryGetLocalPath(file) ?? string.Empty;
    }

    private async void OnCloseTabRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel || viewModel.SelectedDocument is null)
        {
            return;
        }

        MarkdownDocumentTab selected = viewModel.SelectedDocument;
        if (!selected.IsDirty)
        {
            viewModel.RemoveDocument(selected);
            return;
        }

        SaveDecision decision = await AskForUnsavedChangesAsync(selected.Title, "Tab schließen");
        switch (decision)
        {
            case SaveDecision.Cancel:
                return;
            case SaveDecision.Save:
                if (!await SaveDocumentWithPickerAsync(viewModel, selected))
                {
                    return;
                }

                break;
            case SaveDecision.Discard:
                break;
        }

        viewModel.RemoveDocument(selected);
    }

    private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_closeConfirmed)
        {
            return;
        }

        if (_isClosingPromptActive)
        {
            e.Cancel = true;
            return;
        }

        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        List<MarkdownDocumentTab> dirtyDocuments = viewModel.DirtyDocuments.ToList();
        if (dirtyDocuments.Count == 0)
        {
            return;
        }

        e.Cancel = true;
        _isClosingPromptActive = true;

        bool closeAllowed = await ConfirmUnsavedDocumentsAsync(viewModel, dirtyDocuments);

        _isClosingPromptActive = false;
        if (!closeAllowed)
        {
            return;
        }

        _closeConfirmed = true;
        Close();
    }

    private async Task<bool> ConfirmUnsavedDocumentsAsync(MainWindowViewModel viewModel, IReadOnlyList<MarkdownDocumentTab> dirtyDocuments)
    {
        foreach (MarkdownDocumentTab document in dirtyDocuments)
        {
            SaveDecision decision = await AskForUnsavedChangesAsync(document.Title, "Anwendung beenden");
            switch (decision)
            {
                case SaveDecision.Cancel:
                    return false;
                case SaveDecision.Save:
                    if (!await SaveDocumentWithPickerAsync(viewModel, document))
                    {
                        return false;
                    }

                    break;
                case SaveDecision.Discard:
                    break;
            }
        }

        return true;
    }

    private async Task<bool> SaveDocumentWithPickerAsync(MainWindowViewModel viewModel, MarkdownDocumentTab document)
    {
        if (!ReferenceEquals(DataContext, viewModel) || !viewModel.Documents.Contains(document) || !IsVisible)
        {
            return false;
        }

        viewModel.SelectedDocument = document;

        if (!string.IsNullOrWhiteSpace(document.FilePath))
        {
            viewModel.SaveCommand.Execute(null);
            return !document.IsDirty;
        }

        return await PickAndSaveDocumentAsAsync(viewModel, document, true);
    }

    private async Task<bool> PickAndSaveDocumentAsAsync(MainWindowViewModel viewModel, MarkdownDocumentTab document, bool reportCancel)
    {
        if (!ReferenceEquals(DataContext, viewModel) || !viewModel.Documents.Contains(document) || !IsVisible)
        {
            return false;
        }

        bool invalidated = false;
        void DocumentChanged(object? sender, PropertyChangedEventArgs e) => invalidated = true;
        void DocumentsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset
                || (e.OldItems is not null && e.OldItems.Contains(document)))
            {
                invalidated = true;
            }
        }

        void ContextChanged(object? sender, EventArgs e) => invalidated = true;
        document.PropertyChanged += DocumentChanged;
        viewModel.Documents.CollectionChanged += DocumentsChanged;
        DataContextChanged += ContextChanged;
        Closed += ContextChanged;
        string? path;
        try
        {
            path = await PickSavePathAsync(new FilePickerSaveOptions
            {
                Title = "Markdown-Datei speichern unter",
                SuggestedFileName = GetSuggestedFileName(document),
                FileTypeChoices = MarkdownFileTypes,
                DefaultExtension = "md",
            });
        }
        finally
        {
            document.PropertyChanged -= DocumentChanged;
            viewModel.Documents.CollectionChanged -= DocumentsChanged;
            DataContextChanged -= ContextChanged;
            Closed -= ContextChanged;
        }

        if (invalidated || !IsVisible || !ReferenceEquals(DataContext, viewModel) || !viewModel.Documents.Contains(document))
        {
            return false;
        }

        if (path is null)
        {
            if (reportCancel)
            {
                viewModel.StatusText = "Speichern wurde abgebrochen.";
            }

            return false;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "Zieldatei kann lokal nicht aufgelöst werden.";
            return false;
        }

        return viewModel.SaveDocumentAs(document, path);
    }

    private async Task<SaveDecision> AskForUnsavedChangesAsync(string documentTitle, string actionTitle)
    {
        UnsavedChangesDialog dialog = new(documentTitle, actionTitle);
        return await dialog.ShowDialog(this);
    }

    private static string GetSuggestedFileName(MarkdownDocumentTab document)
    {
        string candidate = document.Title;
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return "Document.md";
        }

        return string.Equals(Path.GetExtension(candidate), ".md", StringComparison.OrdinalIgnoreCase)
            ? candidate
            : $"{candidate}.md";
    }

    private static string? TryGetLocalPath(IStorageItem item)
    {
        if (item.Path.IsAbsoluteUri && item.Path.IsFile)
        {
            return item.Path.LocalPath;
        }

        return null;
    }
}
