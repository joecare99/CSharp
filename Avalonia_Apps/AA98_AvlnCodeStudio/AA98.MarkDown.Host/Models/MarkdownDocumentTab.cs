using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;

namespace AA98.MarkDown.Host.Models;

/// <summary>
/// Represents one markdown document tab in the mini host.
/// </summary>
public partial class MarkdownDocumentTab : ObservableObject
{
    private string _savedText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownDocumentTab"/> class.
    /// </summary>
    /// <param name="title">The initial tab title.</param>
    /// <param name="text">The initial markdown text.</param>
    /// <param name="filePath">The source file path, if any.</param>
    public MarkdownDocumentTab(string title, string text, string? filePath)
    {
        _title = title;
        _text = text;
        _filePath = filePath;
        _savedText = text;
        _isDirty = false;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayTitle))]
    private string _title;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayTitle))]
    private bool _isDirty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayPath))]
    private string? _filePath;

    [ObservableProperty]
    private string _text;

    /// <summary>
    /// Gets the title with dirty indicator for tab header rendering.
    /// </summary>
    public string DisplayTitle => IsDirty ? $"{Title}*" : Title;

    /// <summary>
    /// Gets the full path for diagnostics display.
    /// </summary>
    public string DisplayPath => FilePath ?? string.Empty;

    /// <summary>
    /// Updates the path and title according to a persisted file location.
    /// </summary>
    /// <param name="path">The persisted file path.</param>
    public void SetFilePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        FilePath = path;
        Title = Path.GetFileName(path);
    }

    /// <summary>
    /// Marks the tab as synchronized with persisted storage.
    /// </summary>
    public void MarkSaved()
    {
        _savedText = Text;
        IsDirty = false;
    }

    partial void OnTextChanged(string value)
    {
        IsDirty = !string.Equals(value, _savedText, StringComparison.Ordinal);
    }
}
