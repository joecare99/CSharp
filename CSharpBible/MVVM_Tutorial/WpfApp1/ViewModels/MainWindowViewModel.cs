using System.IO;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WpfApp.Models;
using WpfApp.Services;

namespace WpfApp.ViewModels;

/// <summary>
/// Main view model for the WPF sample window.
/// </summary>
public partial class MainWindowViewModel : ObservableObject
{
    private readonly IFileDialogService _fileDialogService;
    private readonly ITextDocumentService _textDocumentService;

    /// <summary>
    /// Gets or sets the current text document.
    /// </summary>
    [ObservableProperty]
    private TextDocumentModel _document = new();

    /// <summary>
    /// Gets or sets the current status text.
    /// </summary>
    [ObservableProperty]
    private string _statusText = "Bereit.";

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel(ITextDocumentService textDocumentService, IFileDialogService fileDialogService)
    {
        _textDocumentService = textDocumentService;
        _fileDialogService = fileDialogService;
    }

    /// <summary>
    /// Creates a new empty document.
    /// </summary>
    [RelayCommand]
    private void NewDocument()
    {
        Document = new TextDocumentModel();
        StatusText = "Neues Dokument erstellt.";
    }

    /// <summary>
    /// Opens an existing text document.
    /// </summary>
    [RelayCommand]
    private async Task OpenDocumentAsync()
    {
        var sFilePath = _fileDialogService.PickOpenFilePath();
        if (string.IsNullOrWhiteSpace(sFilePath))
        {
            StatusText = "Öffnen abgebrochen.";
            return;
        }

        Document = await _textDocumentService.LoadAsync(sFilePath).ConfigureAwait(true);
        StatusText = $"Geladen: {Path.GetFileName(sFilePath)}";
    }

    /// <summary>
    /// Saves the current document.
    /// </summary>
    [RelayCommand]
    private async Task SaveDocumentAsync()
    {
        if (string.IsNullOrWhiteSpace(Document.FilePath))
        {
            await SaveDocumentAsAsync().ConfigureAwait(true);
            return;
        }

        await _textDocumentService.SaveAsync(Document).ConfigureAwait(true);
        StatusText = $"Gespeichert: {Path.GetFileName(Document.FilePath)}";
    }

    /// <summary>
    /// Saves the current document to a new file path.
    /// </summary>
    [RelayCommand]
    private async Task SaveDocumentAsAsync()
    {
        var sFilePath = _fileDialogService.PickSaveFilePath(Document.FilePath);
        if (string.IsNullOrWhiteSpace(sFilePath))
        {
            StatusText = "Speichern abgebrochen.";
            return;
        }

        Document.FilePath = sFilePath;
        await _textDocumentService.SaveAsync(Document).ConfigureAwait(true);
        StatusText = $"Gespeichert: {Path.GetFileName(sFilePath)}";
    }

    /// <summary>
    /// Saves the current document to a new file path.
    /// </summary>
    [RelayCommand]
    private async Task SendDocumentAsync()
    {
        StatusText = $"Gesendet: ";
    }
    /// <summary>
    /// Exits the application.
    /// </summary>
    [RelayCommand]
    private void ExitApplication()
    {
        Application.Current.Shutdown();
    }
}
