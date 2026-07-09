using AA98.MarkDown.Host.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace AA98.MarkDown.Host.ViewModels;

/// <summary>
/// Provides the root view model for the markdown mini host.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    private int _untitledCounter = 1;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
    /// </summary>
    public MainWindowViewModel()
    {
        StatusText = "Bereit.";
        NewDocument();
    }

    /// <summary>
    /// Gets the open markdown document tabs.
    /// </summary>
    public ObservableCollection<MarkdownDocumentTab> Documents { get; } = [];

    [ObservableProperty]
    private MarkdownDocumentTab? _selectedDocument;

    [ObservableProperty]
    private string _documentPathInput = string.Empty;

    [ObservableProperty]
    private string _statusText = string.Empty;

    [ObservableProperty]
    private bool _isPreviewEnabled = true;

    /// <summary>
    /// Gets all currently modified documents.
    /// </summary>
    public IReadOnlyList<MarkdownDocumentTab> DirtyDocuments => Documents.Where(static document => document.IsDirty).ToList();

    /// <summary>
    /// Opens an explicit markdown file path in a tab.
    /// </summary>
    /// <param name="path">The file path to open.</param>
    public void OpenDocument(string path)
    {
        DocumentPathInput = path ?? string.Empty;
        OpenPath();
    }

    /// <summary>
    /// Saves the active document under an explicit target file path.
    /// </summary>
    /// <param name="path">The target markdown file path.</param>
    public void SaveDocumentAs(string path)
    {
        DocumentPathInput = path ?? string.Empty;
        SaveAsPath();
    }

    /// <summary>
    /// Opens a markdown link target relative to the active markdown file.
    /// </summary>
    /// <param name="linkTarget">The markdown link target.</param>
    public void OpenLinkedDocument(string linkTarget)
    {
        if (string.IsNullOrWhiteSpace(linkTarget))
        {
            return;
        }

        if (Uri.TryCreate(linkTarget, UriKind.Absolute, out Uri? absoluteUri) &&
            (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps))
        {
            StatusText = $"Web-Links werden nicht im Host geöffnet: {linkTarget}";
            return;
        }

        string? baseDirectory = SelectedDocument?.FilePath is not null
            ? Path.GetDirectoryName(SelectedDocument.FilePath)
            : null;

        string resolvedPath = ResolvePath(linkTarget, baseDirectory);
        OpenDocument(resolvedPath);
    }

    [RelayCommand]
    private void NewDocument()
    {
        string title = $"Untitled-{_untitledCounter++}.md";
        MarkdownDocumentTab tab = new(title, string.Empty, null);
        Documents.Add(tab);
        SelectedDocument = tab;
        StatusText = $"Neues Dokument erstellt: {title}";
    }

    /// <summary>
    /// Closes the selected document tab without save checks.
    /// </summary>
    public void CloseSelectedDocument()
    {
        if (SelectedDocument is null)
        {
            return;
        }

        RemoveDocument(SelectedDocument);
    }

    /// <summary>
    /// Removes a document tab from the host and updates the active selection.
    /// </summary>
    /// <param name="document">The document tab to remove.</param>
    public void RemoveDocument(MarkdownDocumentTab document)
    {
        ArgumentNullException.ThrowIfNull(document);

        int currentIndex = Documents.IndexOf(document);
        if (currentIndex < 0)
        {
            return;
        }

        Documents.RemoveAt(currentIndex);

        if (Documents.Count == 0)
        {
            NewDocument();
            return;
        }

        int nextIndex = Math.Min(currentIndex, Documents.Count - 1);
        SelectedDocument = Documents[nextIndex];
        StatusText = $"Dokument geschlossen: {document.Title}";
    }

    [RelayCommand]
    private void OpenPath()
    {
        if (string.IsNullOrWhiteSpace(DocumentPathInput))
        {
            StatusText = "Bitte einen Markdown-Pfad angeben.";
            return;
        }

        string path = Path.GetFullPath(DocumentPathInput.Trim());
        if (!File.Exists(path))
        {
            StatusText = $"Datei nicht gefunden: {path}";
            return;
        }

        if (!string.Equals(Path.GetExtension(path), ".md", StringComparison.OrdinalIgnoreCase))
        {
            StatusText = $"Nur .md-Dateien werden unterstützt: {path}";
            return;
        }

        MarkdownDocumentTab? existing = Documents.FirstOrDefault(tab =>
            tab.FilePath is not null &&
            string.Equals(Path.GetFullPath(tab.FilePath), path, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            SelectedDocument = existing;
            StatusText = $"Bereits geöffnet: {existing.Title}";
            return;
        }

        string text = File.ReadAllText(path);
        MarkdownDocumentTab document = new(Path.GetFileName(path), text, path);
        Documents.Add(document);
        SelectedDocument = document;
        DocumentPathInput = path;
        StatusText = $"Geöffnet: {document.Title}";
    }

    [RelayCommand]
    private void Save()
    {
        if (SelectedDocument is null)
        {
            StatusText = "Kein Dokument ausgewählt.";
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedDocument.FilePath))
        {
            SaveAsPath();
            return;
        }

        File.WriteAllText(SelectedDocument.FilePath, SelectedDocument.Text);
        SelectedDocument.MarkSaved();
        DocumentPathInput = SelectedDocument.FilePath;
        StatusText = $"Gespeichert: {SelectedDocument.Title}";
    }

    [RelayCommand]
    private void SaveAsPath()
    {
        if (SelectedDocument is null)
        {
            StatusText = "Kein Dokument ausgewählt.";
            return;
        }

        if (string.IsNullOrWhiteSpace(DocumentPathInput))
        {
            StatusText = "Bitte einen Zielpfad für Speichern unter angeben.";
            return;
        }

        string path = Path.GetFullPath(DocumentPathInput.Trim());
        if (!string.Equals(Path.GetExtension(path), ".md", StringComparison.OrdinalIgnoreCase))
        {
            path = Path.ChangeExtension(path, ".md");
        }

        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(path, SelectedDocument.Text);
        SelectedDocument.SetFilePath(path);
        SelectedDocument.MarkSaved();
        DocumentPathInput = path;
        StatusText = $"Gespeichert: {SelectedDocument.Title}";
    }

    private static string ResolvePath(string path, string? baseDirectory)
    {
        if (Path.IsPathRooted(path))
        {
            return Path.GetFullPath(path);
        }

        if (string.IsNullOrWhiteSpace(baseDirectory))
        {
            return Path.GetFullPath(path);
        }

        return Path.GetFullPath(Path.Combine(baseDirectory, path));
    }
}
