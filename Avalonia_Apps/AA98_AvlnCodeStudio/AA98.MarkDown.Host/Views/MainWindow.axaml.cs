using AA98.MarkDown.Host.Models;
using AA98.MarkDown.Host.ViewModels;
using AA98.MarkDown.UI.Controls;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
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
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null)
        {
            return;
        }

        IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Markdown-Datei speichern unter",
            SuggestedFileName = GetSuggestedFileName(viewModel),
            FileTypeChoices = MarkdownFileTypes,
            DefaultExtension = "md",
        });

        if (file is null)
        {
            return;
        }

        string? path = TryGetLocalPath(file);
        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "Zieldatei kann lokal nicht aufgelöst werden.";
            return;
        }

        viewModel.SaveDocumentAs(path);
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
        if (_closeConfirmed || _isClosingPromptActive)
        {
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
        viewModel.SelectedDocument = document;

        if (!string.IsNullOrWhiteSpace(document.FilePath))
        {
            viewModel.SaveCommand.Execute(null);
            return !document.IsDirty;
        }

        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.StorageProvider is null)
        {
            return false;
        }

        IStorageFile? file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Markdown-Datei speichern unter",
            SuggestedFileName = GetSuggestedFileName(viewModel),
            FileTypeChoices = MarkdownFileTypes,
            DefaultExtension = "md",
        });

        if (file is null)
        {
            viewModel.StatusText = "Speichern wurde abgebrochen.";
            return false;
        }

        string? path = TryGetLocalPath(file);
        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "Zieldatei kann lokal nicht aufgelöst werden.";
            return false;
        }

        viewModel.SaveDocumentAs(path);
        return !document.IsDirty;
    }

    private async Task<SaveDecision> AskForUnsavedChangesAsync(string documentTitle, string actionTitle)
    {
        UnsavedChangesDialog dialog = new(documentTitle, actionTitle);
        return await dialog.ShowDialog(this);
    }

    private static string GetSuggestedFileName(MainWindowViewModel viewModel)
    {
        string candidate = viewModel.SelectedDocument?.Title;
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
