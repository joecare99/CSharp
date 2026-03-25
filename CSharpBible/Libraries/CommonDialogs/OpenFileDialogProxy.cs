using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

/// <summary>
/// Represents a proxy class for the <see cref="OpenFileDialog"/>.
/// <para>
/// This class wraps the standard WPF <see cref="OpenFileDialog"/> to implement the <see cref="IOpenFileDialog"/> interface,
/// allowing for dependency injection and easier unit testing of view models that require file opening functionality.
/// </para>
/// </summary>
/// <seealso cref="FileDialogProxy{T}" />
/// <seealso cref="IOpenFileDialog" />
public class OpenFileDialogProxy : FileDialogProxy<OpenFileDialog>, IOpenFileDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenFileDialogProxy"/> class.
    /// <para>
    /// This constructor creates a new underlying <see cref="OpenFileDialog"/> instance.
    /// </para>
    /// </summary>
    public OpenFileDialogProxy()
        : base(new OpenFileDialog())
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether the dialog box allows multiple files to be selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if the dialog box allows multiple files to be selected together or concurrently; otherwise, <c>false</c>.
    ///   The default value is <c>false</c>.
    /// </value>
    public bool Multiselect
    {
        get => This.Multiselect;
        set => This.Multiselect = value;
    }

    /// <summary>
    /// Gets an array that contains one file name for each selected file.
    /// </summary>
    /// <value>
    /// An array of <see cref="string" /> containing the full paths of the selected files.
    /// If only one file is selected, the array contains a single element.
    /// </value>
    public string[] FileNames => This.FileNames;

    /// <summary>
    /// Gets the file name and extension for the selected file. The file name does not include the path.
    /// </summary>
    /// <value>
    /// A <see cref="string" /> containing the name and extension of the selected file.
    /// The file name does not include the path. If no file is selected, an empty string is returned.
    /// </value>
    public string SafeFileName => This.SafeFileName;

    /// <summary>
    /// Gets an array that contains one safe file name for each selected file.
    /// </summary>
    /// <value>
    /// An array of <see cref="string" /> containing the names and extensions of the selected files.
    /// The file names do not include the path.
    /// </value>
    public string[] SafeFileNames => This.SafeFileNames;

    /// <summary>
    /// Gets the default file name extension.
    /// </summary>
    /// <value>
    /// The default file name extension. The returned string includes the leading period (.).
    /// </value>
    public string FileNameExtension => This.DefaultExt;
}