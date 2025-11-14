using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

public class OpenFileDialogProxy : FileDialogProxy<OpenFileDialog>, IOpenFileDialog
{
    public OpenFileDialogProxy()
        : base(new OpenFileDialog())
    {
    }

    public bool Multiselect
    {
        get => This.Multiselect;
        set => This.Multiselect = value;
    }

    public string[] FileNames => This.FileNames;

    public string SafeFileName => This.SafeFileName;

    public string[] SafeFileNames => This.SafeFileNames;

    public string FileNameExtension => This.DefaultExt;
}