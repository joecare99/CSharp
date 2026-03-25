using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

/// <summary>
/// Represents a generic proxy for file dialogs, wrapping a specific <see cref="FileDialog"/> implementation.
/// </summary>
/// <typeparam name="T">The type of the file dialog to wrap. Must be a reference type.</typeparam>
/// <remarks>
/// This class adapts the standard WPF <see cref="FileDialog"/> (and its subclasses like <see cref="OpenFileDialog"/> or <see cref="SaveFileDialog"/>)
/// to the <see cref="IFileDialog"/> interface. This abstraction facilitates unit testing and dependency injection
/// by allowing the dialog logic to be mocked or substituted.
/// </remarks>
public class FileDialogProxy<T> : IFileDialog where T : class
{
    /// <summary>
    /// The internal reference to the wrapped <see cref="FileDialog"/>.
    /// </summary>
    private FileDialog _fileDialog;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDialogProxy{T}"/> class.
    /// </summary>
    /// <param name="fileDialog">The file dialog instance to wrap.</param>
    /// <remarks>
    /// The provided <paramref name="fileDialog"/> is cast to <see cref="FileDialog"/>.
    /// Ensure that <typeparamref name="T"/> is compatible with <see cref="FileDialog"/>.
    /// </remarks>
    public FileDialogProxy(T fileDialog)
    {
        _fileDialog = (fileDialog as FileDialog)!;
    }

    /// <summary>
    /// Gets the underlying file dialog instance.
    /// </summary>
    /// <value>The wrapped file dialog instance cast to type <typeparamref name="T"/>.</value>
    public T This => (_fileDialog as T)!;

    /// <summary>
    /// Gets or sets the full path of the file selected in the file dialog.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> containing the full path of the selected file.
    /// </value>
    public string FileName
    {
        get => _fileDialog.FileName;
        set => _fileDialog.FileName = value;
    }

    /// <summary>
    /// Gets or sets the filter string that determines what types of files are displayed in the file dialog.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> containing the filter options.
    /// For example: "Text files (*.txt)|*.txt|All files (*.*)|*.*"
    /// </value>
    public string Filter { get => _fileDialog.Filter; set => _fileDialog.Filter = value; }

    /// <summary>
    /// Gets or sets the index of the filter currently selected in the file dialog.
    /// </summary>
    /// <value>
    /// The integer index of the selected filter. The first filter is index 1.
    /// </value>
    public int FilterIndex { get => _fileDialog.FilterIndex; set => _fileDialog.FilterIndex = value; }

    /// <summary>
    /// Gets or sets the initial directory displayed by the file dialog.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the path to the initial directory.
    /// </value>
    public string InitialDirectory { get => _fileDialog.InitialDirectory; set => _fileDialog.InitialDirectory = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog restores the directory to the previously selected directory before closing.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the dialog restores the current directory to its original value; otherwise, <see langword="false"/>.
    /// </value>
    public bool RestoreDirectory { get => _fileDialog.RestoreDirectory; set => _fileDialog.RestoreDirectory = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog automatically adds an extension to a file name if the user omits the extension.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the dialog adds an extension; otherwise, <see langword="false"/>.
    /// </value>
    public bool AddExtension { get => _fileDialog.AddExtension; set => _fileDialog.AddExtension = value; }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog checks if the specified file exists.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the dialog checks that the file exists; otherwise, <see langword="false"/>.
    /// </value>
    public bool CheckFileExists { get => _fileDialog.CheckFileExists; set => _fileDialog.CheckFileExists = value; }

    /// <summary>
    /// Gets or sets the default file name extension.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the default extension (without the leading dot).
    /// </value>
    public string DefaultExt { get => _fileDialog.DefaultExt; set => _fileDialog.DefaultExt = value; }

    /// <summary>
    /// Gets or sets the title of the file dialog.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> containing the text displayed in the title bar of the dialog.
    /// </value>
    public string Title { get => _fileDialog.Title; set => _fileDialog.Title = value; }

    /// <summary>
    /// Displays the file dialog.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the user clicks OK; <see langword="false"/> if the user clicks Cancel; otherwise <see langword="null"/>.
    /// </returns>
    public bool? ShowDialog()
    {
        return _fileDialog.ShowDialog();
    }

    /// <summary>
    /// Displays the file dialog with the specified owner.
    /// </summary>
    /// <param name="owner">The window that owns this dialog. It is expected to be a <see cref="System.Windows.Window"/>.</param>
    /// <returns>
    /// <see langword="true"/> if the user clicks OK; <see langword="false"/> if the user clicks Cancel; otherwise <see langword="null"/>.
    /// </returns>
    public bool? ShowDialog(object owner)
    {
        return _fileDialog.ShowDialog(owner as System.Windows.Window);
    }
}