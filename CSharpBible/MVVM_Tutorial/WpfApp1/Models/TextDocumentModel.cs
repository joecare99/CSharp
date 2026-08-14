using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp.Models;

/// <summary>
/// Represents a text document that can be loaded from or saved to a file.
/// </summary>
public partial class TextDocumentModel : ObservableObject
{
    /// <summary>
    /// Gets or sets the file path associated with the document.
    /// </summary>
    [ObservableProperty]
    private string? _filePath;

    /// <summary>
    /// Gets or sets the document text.
    /// </summary>
    [ObservableProperty]
    private string _text = string.Empty;
}
