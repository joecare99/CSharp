using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

public class OpenFileDialogProxy : FileDialogProxy<OpenFileDialog>, IOpenFileDialog
{
    public OpenFileDialogProxy()
        : base(new OpenFileDialog())
    {
    }

    public string Filter
    {
        get => This.Filter;
        set => This.Filter = value;
    }

    public string InitialDirectory
    {
        get => This.InitialDirectory;
        set => This.InitialDirectory = value;
    }

    public string Title
    {
        get => This.Title;
        set => This.Title = value;
    }

    public bool Multiselect
    {
        get => This.Multiselect;
        set => This.Multiselect = value;
    }

    public string[] FileNames => This.FileNames;

    public string SafeFileName => This.SafeFileName;

    public string[] SafeFileNames => This.SafeFileNames;

    public string DefaultExt
    {
        get => This.DefaultExt;
        set => This.DefaultExt = value;
    }

    public string FileNameExtension => This.DefaultExt;
}