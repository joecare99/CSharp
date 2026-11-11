using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using AA98_AvlnCodeStudio.AxamlDesigner.UI.Views;
using AA98_AxamlDesigner.Host.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AA98_AxamlDesigner.Host.Views;

public partial class MainWindow : Window
{
    private static readonly IReadOnlyList<FilePickerFileType> s_axamlFileTypes =
    [
        new("AXAML")
        {
            Patterns = ["*.axaml"],
            MimeTypes = ["application/xml", "text/xml"],
        },
    ];

    private static readonly IReadOnlyList<FilePickerFileType> s_workspaceEntryFileTypes =
    [
        new("C# workspace")
        {
            Patterns = ["*.sln", "*.slnx", "*.csproj"],
        },
    ];

    private bool _closeConfirmed;
    private bool _isClosingPromptActive;
    private bool _synchronizingSelection;
    private AxamlDesignerHostViewModel? _selectionHost;

    public MainWindow()
    {
        InitializeComponent();
        SourceEditor.CaretPositionChanged += OnCaretPositionChanged;
        DataContextChanged += OnDataContextChanged;
        Closing += OnWindowClosing;
        Closed += OnWindowClosed;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_selectionHost is not null)
        {
            _selectionHost.Designer.SelectionActivated -= OnDesignerSelectionActivated;
        }

        _selectionHost = DataContext as AxamlDesignerHostViewModel;
        if (_selectionHost is not null)
        {
            _selectionHost.Designer.SelectionActivated += OnDesignerSelectionActivated;
        }

        if (DataContext is AxamlDesignerHostViewModel viewModel)
        {
            viewModel.SetScreenshotCapture(DesignerView.CapturePreviewPngAsync);
            viewModel.ReviewWorkspaceFileChangesAsync = async (proposal, cancellationToken) =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                string filePath = viewModel.FilePath;
                string workspaceRoot = viewModel.WorkspaceRootPath;
                string markup = viewModel.Markup;
                var selection = viewModel.Designer.SelectedNode;
                WorkspaceFileChangeReviewDialog dialog = new(
                    proposal,
                    viewModel.WorkspaceFileChangeService,
                    () => ReferenceEquals(DataContext, viewModel)
                        && string.Equals(viewModel.FilePath, filePath, StringComparison.Ordinal)
                        && string.Equals(viewModel.WorkspaceRootPath, workspaceRoot, StringComparison.Ordinal)
                        && string.Equals(viewModel.Markup, markup, StringComparison.Ordinal)
                        && ReferenceEquals(viewModel.Designer.SelectedNode, selection));
                return await dialog.ShowDialog(this);
            };
        }
    }

    private void OnCaretPositionChanged(object? sender, EventArgs e)
    {
        if (_synchronizingSelection || _selectionHost is not { } host)
        {
            return;
        }

        _synchronizingSelection = true;
        try
        {
            host.Designer.SelectAtCaret(host.Markup, SourceEditor.CaretOffset);
        }
        finally
        {
            _synchronizingSelection = false;
        }
    }

    private void OnDesignerSelectionActivated(object? sender, EventArgs e)
    {
        if (_synchronizingSelection || _selectionHost?.Designer.SelectedNode is not { SourceStartOffset: >= 0 } node)
        {
            return;
        }

        _synchronizingSelection = true;
        try
        {
            SourceEditor.MoveCaretWithoutFocus(node.SourceStartOffset);
        }
        finally
        {
            _synchronizingSelection = false;
        }
    }

    private void OnWindowClosed(object? sender, EventArgs e)
    {
        SourceEditor.CaretPositionChanged -= OnCaretPositionChanged;
        DataContextChanged -= OnDataContextChanged;
        if (_selectionHost is not null)
        {
            _selectionHost.Designer.SelectionActivated -= OnDesignerSelectionActivated;
            _selectionHost = null;
        }
    }

    private async void OnBrowseRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not AxamlDesignerHostViewModel viewModel
            || TopLevel.GetTopLevel(this)?.StorageProvider is not { } storageProvider)
        {
            return;
        }

        IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open AXAML file",
            AllowMultiple = false,
            FileTypeFilter = s_axamlFileTypes,
        });
        if (files.Count == 0)
        {
            return;
        }

        string? path = TryGetLocalPath(files[0]);
        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "The selected file cannot be resolved to a local path.";
            return;
        }

        await OpenDocumentAsync(viewModel, path);
    }

    private async void OnWorkspaceBrowseRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not AxamlDesignerHostViewModel viewModel
            || TopLevel.GetTopLevel(this)?.StorageProvider is not { } storageProvider)
        {
            return;
        }

        IReadOnlyList<IStorageFolder> folders = await storageProvider.OpenFolderPickerAsync(
            new FolderPickerOpenOptions
            {
                Title = "Select project/workspace directory",
                AllowMultiple = false,
            });
        if (folders.Count == 0)
        {
            return;
        }

        string? path = TryGetLocalPath(folders[0]);
        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "The selected directory cannot be resolved to a local path.";
            return;
        }

        viewModel.SetWorkspaceContext(path);
    }

    private async void OnWorkspaceEntryBrowseRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not AxamlDesignerHostViewModel viewModel
            || TopLevel.GetTopLevel(this)?.StorageProvider is not { } storageProvider)
        {
            return;
        }

        IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Select C# solution or project",
                AllowMultiple = false,
                FileTypeFilter = s_workspaceEntryFileTypes,
            });
        if (files.Count == 0)
        {
            return;
        }

        string? path = TryGetLocalPath(files[0]);
        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "The selected workspace entry cannot be resolved to a local path.";
            return;
        }

        viewModel.SetWorkspaceEntry(path);
    }

    private async void OnOpenPathRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AxamlDesignerHostViewModel viewModel)
        {
            await OpenDocumentAsync(viewModel, viewModel.FilePathInput);
        }
    }

    private async Task OpenDocumentAsync(AxamlDesignerHostViewModel viewModel, string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "Select or enter an AXAML file path to open.";
            return;
        }

        bool discardChanges = false;
        if (viewModel.HasUnsavedChanges)
        {
            AxamlSaveDecision decision = await AskForUnsavedChangesAsync(viewModel, "opening another file");
            if (decision == AxamlSaveDecision.Cancel)
            {
                return;
            }

            if (decision == AxamlSaveDecision.Save && !await SaveDocumentAsync(viewModel))
            {
                return;
            }

            discardChanges = decision == AxamlSaveDecision.Discard;
        }

        viewModel.OpenDocument(path, discardChanges);
    }

    private async void OnSaveRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AxamlDesignerHostViewModel viewModel)
        {
            await SaveDocumentAsync(viewModel);
        }
    }

    private async Task<bool> SaveDocumentAsync(AxamlDesignerHostViewModel viewModel)
    {
        if (!string.IsNullOrWhiteSpace(viewModel.FilePath))
        {
            return viewModel.SaveDocument();
        }

        if (TopLevel.GetTopLevel(this)?.StorageProvider is not { } storageProvider)
        {
            viewModel.StatusText = "Save is unavailable because file storage is not available.";
            return false;
        }

        IStorageFile? file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save AXAML file",
            SuggestedFileName = GetSuggestedFileName(viewModel),
            DefaultExtension = "axaml",
            FileTypeChoices = s_axamlFileTypes,
        });
        if (file is null)
        {
            viewModel.StatusText = "Save was cancelled.";
            return false;
        }

        string? path = TryGetLocalPath(file);
        if (string.IsNullOrWhiteSpace(path))
        {
            viewModel.StatusText = "The selected destination cannot be resolved to a local path.";
            return false;
        }

        return viewModel.SaveDocumentAs(path);
    }

    private async void OnWindowClosing(object? sender, WindowClosingEventArgs e)
    {
        if (_closeConfirmed || DataContext is not AxamlDesignerHostViewModel viewModel)
        {
            return;
        }

        if (_isClosingPromptActive)
        {
            e.Cancel = true;
            return;
        }

        if (!viewModel.HasUnsavedChanges)
        {
            return;
        }

        e.Cancel = true;
        _isClosingPromptActive = true;
        AxamlSaveDecision decision = await AskForUnsavedChangesAsync(viewModel, "closing the designer");
        bool closeAllowed = decision switch
        {
            AxamlSaveDecision.Discard => true,
            AxamlSaveDecision.Save => await SaveDocumentAsync(viewModel) && !viewModel.HasUnsavedChanges,
            _ => false,
        };
        _isClosingPromptActive = false;
        if (closeAllowed)
        {
            _closeConfirmed = true;
            Close();
        }
    }

    private async Task<AxamlSaveDecision> AskForUnsavedChangesAsync(
        AxamlDesignerHostViewModel viewModel,
        string actionTitle)
    {
        AxamlUnsavedChangesDialog dialog = new(
            string.IsNullOrWhiteSpace(viewModel.FilePath)
                ? "Untitled AXAML document"
                : Path.GetFileName(viewModel.FilePath),
            actionTitle,
            viewModel.Designer.HasUnappliedPropertyDrafts);
        return await dialog.ShowAndGetDecisionAsync(this);
    }

    private static string GetSuggestedFileName(AxamlDesignerHostViewModel viewModel)
    {
        return string.IsNullOrWhiteSpace(viewModel.FilePath)
            ? "View.axaml"
            : Path.GetFileName(viewModel.FilePath);
    }

    private static string? TryGetLocalPath(IStorageItem item)
    {
        return item.Path.IsAbsoluteUri && item.Path.IsFile
            ? item.Path.LocalPath
            : null;
    }
}
