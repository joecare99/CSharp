using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

/// <summary>
/// Represents a proxy for the <see cref="SaveFileDialog"/> class.
/// <para>
/// This class encapsulates the functionality of the standard WPF <see cref="SaveFileDialog"/>
/// and adapts it to the <see cref="IFileDialog"/> interface, facilitating dependency injection
/// and testing within an MVVM architecture.
/// </para>
/// </summary>
/// <seealso cref="FileDialogProxy{T}" />
/// <seealso cref="IFileDialog" />
public class SaveFileDialogProxy : FileDialogProxy<object>, IFileDialog
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SaveFileDialogProxy"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor instantiates the underlying <see cref="SaveFileDialog"/> which is used
    /// to perform the actual dialog operations.
    /// </remarks>
    public SaveFileDialogProxy()
        : base(new SaveFileDialog())
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Save As dialog displays a warning if the user specifies a file name that already exists.
    /// </summary>
    /// <value>
    /// <c>true</c> if the dialog should prompt the user before overwriting an existing file; otherwise, <c>false</c>.
    /// The default value is <c>true</c>.
    /// </value>
    public bool OverwritePrompt
    {
        get => ((SaveFileDialog)This).OverwritePrompt;
        set => ((SaveFileDialog)This).OverwritePrompt = value;
    }

    /// <summary>
    /// Gets the default file name extension.
    /// </summary>
    /// <value>
    /// The default file name extension string. The returned string does not include the period (.).
    /// </value>
    public string FileNameExtension => ((SaveFileDialog)This).DefaultExt;
}