using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
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

    private bool _closeConfirmed;
    private bool _isClosingPromptActive;

    public MainWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Closing += OnWindowClosing;
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is AxamlDesignerHostViewModel viewModel)
        {
            viewModel.SetScreenshotCapture(DesignerView.CapturePreviewPngAsync);
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

    private async void OnOpenPathRequested(object? sender, RoutedEventArgs e)
    {
        if (DataContext is AxamlDesignerHostViewModel viewModel)
        {
            await OpenDocumentAsync(viewModel, viewModel.FilePath);
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
        if (viewModel.IsDirty)
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
        if (_closeConfirmed || _isClosingPromptActive
            || DataContext is not AxamlDesignerHostViewModel viewModel
            || !viewModel.IsDirty)
        {
            return;
        }

        e.Cancel = true;
        _isClosingPromptActive = true;
        AxamlSaveDecision decision = await AskForUnsavedChangesAsync(viewModel, "closing the designer");
        bool closeAllowed = decision switch
        {
            AxamlSaveDecision.Discard => true,
            AxamlSaveDecision.Save => await SaveDocumentAsync(viewModel),
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
            actionTitle);
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
