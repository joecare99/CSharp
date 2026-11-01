using AA98_AvlnCodeStudio.AxamlDesigner.UI.ViewModels;
using AA98_AvlnCodeStudio.Model.Workspaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AxamlDesigner.Host.ViewModels;

public partial class AxamlDesignerHostViewModel : ObservableObject
{
    private string _markup = string.Empty;
    private string _savedMarkup = string.Empty;
    private Func<CancellationToken, Task<byte[]>>? _screenshotCapture;

    public AxamlDesignerHostViewModel()
    {
        Designer.MarkupChanged += OnMarkupChanged;
    }

    public AxamlDesignerViewModel Designer { get; } = new();

    [ObservableProperty]
    private string _filePath = string.Empty;

    [ObservableProperty]
    private string _statusText = "Open an AXAML file or enter a path.";

    public bool IsDirty => !string.Equals(_markup, _savedMarkup, StringComparison.Ordinal);

    public string WindowTitle => $"AA98 AXAML Designer{(IsDirty ? " *" : string.Empty)}";

    [RelayCommand]
    private void Open()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            StatusText = "Enter an AXAML file path to open.";
            return;
        }

        OpenDocument(FilePath);
    }

    public bool OpenDocument(string path, bool discardUnsavedChanges = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (IsDirty && !discardUnsavedChanges)
        {
            StatusText = "Save or discard the current changes before opening another file.";
            return false;
        }

        try
        {
            string fullPath = Path.GetFullPath(path);
            string markup = File.ReadAllText(fullPath);
            FilePath = fullPath;
            _markup = markup;
            _savedMarkup = markup;
            Designer.LoadMarkup(markup);
            NotifyDocumentStateChanged();
            StatusText = $"Opened {Path.GetFileName(fullPath)}.";
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            StatusText = $"Open failed: {exception.Message}";
            return false;
        }
    }

    [RelayCommand]
    private void Save()
    {
        SaveDocument();
    }

    public bool SaveDocument()
    {
        if (string.IsNullOrWhiteSpace(FilePath))
        {
            StatusText = "Enter a destination path before saving.";
            return false;
        }

        try
        {
            string fullPath = Path.GetFullPath(FilePath);
            File.WriteAllText(fullPath, _markup);
            FilePath = fullPath;
            _savedMarkup = _markup;
            NotifyDocumentStateChanged();
            StatusText = $"Saved {Path.GetFileName(fullPath)}.";
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            StatusText = $"Save failed: {exception.Message}";
            return false;
        }
    }

    public bool SaveDocumentAs(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            string fullPath = Path.GetFullPath(path);
            File.WriteAllText(fullPath, _markup);
            FilePath = fullPath;
            _savedMarkup = _markup;
            NotifyDocumentStateChanged();
            StatusText = $"Saved {Path.GetFileName(fullPath)}.";
            return true;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or ArgumentException)
        {
            StatusText = $"Save failed: {exception.Message}";
            return false;
        }
    }

    public void SetScreenshotCapture(Func<CancellationToken, Task<byte[]>> screenshotCapture)
    {
        ArgumentNullException.ThrowIfNull(screenshotCapture);
        _screenshotCapture = screenshotCapture;
    }

    public Task<byte[]> CaptureScreenshotPngAsync(CancellationToken cancellationToken = default)
    {
        return (_screenshotCapture
            ?? throw new InvalidOperationException("The designer preview view is not available for screenshot capture."))(cancellationToken);
    }

    public bool SelectElement(string nodePath)
    {
        ArgumentNullException.ThrowIfNull(nodePath);
        AxamlPreviewNode? node = Designer.PreviewRoot;
        if (node is null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(nodePath))
        {
            foreach (string segment in nodePath.Split('/'))
            {
                if (!int.TryParse(segment, out int index) || index < 0 || index >= node.Children.Count)
                {
                    return false;
                }

                node = node.Children[index];
            }
        }

        Designer.SelectedNode = node;
        return true;
    }

    public bool SetElementProperty(string nodePath, string propertyName, string value)
    {
        if (!SelectElement(nodePath))
        {
            return false;
        }

        Designer.SelectedProperty = propertyName;
        Designer.SelectedValue = value;
        if (!Designer.ApplySelectedValueCommand.CanExecute(null))
        {
            throw new InvalidOperationException($"The property '{propertyName}' cannot be edited on the selected element.");
        }

        Designer.ApplySelectedValueCommand.Execute(null);
        return true;
    }

    public string GetDesignerStateJson()
    {
        return System.Text.Json.JsonSerializer.Serialize(new
        {
            FilePath,
            Designer.ErrorText,
            SelectedElementName = Designer.SelectedElementName,
            SelectedProperty = Designer.SelectedProperty,
            SelectedValue = Designer.SelectedValue,
            Elements = Flatten(Designer.PreviewRoot, string.Empty),
        });
    }

    private static IReadOnlyList<object> Flatten(AxamlPreviewNode? node, string path)
    {
        if (node is null)
        {
            return [];
        }

        List<object> result = [new
        {
            Path = path,
            node.TypeName,
            node.Text,
            EditableProperties = node.IsSupported
                ? new AxamlPreviewService().GetEditableProperties(node)
                : [],
        }];
        for (int i = 0; i < node.Children.Count; i++)
        {
            result.AddRange(Flatten(node.Children[i], string.IsNullOrEmpty(path) ? i.ToString() : $"{path}/{i}"));
        }

        return result;
    }

    private void OnMarkupChanged(object? sender, AxamlDesignerMarkupChangedEventArgs e)
    {
        _markup = e.Markup;
        NotifyDocumentStateChanged();
        StatusText = "Designer changes are unsaved.";
    }

    private void NotifyDocumentStateChanged()
    {
        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(WindowTitle));
    }
}
