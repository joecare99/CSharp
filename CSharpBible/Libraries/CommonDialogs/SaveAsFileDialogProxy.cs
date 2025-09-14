using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

public class SaveFileDialogProxy : FileDialogProxy<SaveFileDialog>, IFileDialog
{
    public SaveFileDialogProxy()
        : base(new SaveFileDialog())
    {
    }

    public string Filter
    {
        get => This.Filter;
        set => This.Filter = value;
    }

    public int FilterIndex
    {
        get => This.FilterIndex;
        set => This.FilterIndex = value;
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

    public string DefaultExt
    {
        get => This.DefaultExt;
        set => This.DefaultExt = value;
    }

    public bool OverwritePrompt
    {
        get => This.OverwritePrompt;
        set => This.OverwritePrompt = value;
    }

    public bool AddExtension
    {
        get => This.AddExtension;
        set => This.AddExtension = value;
    }
    public bool RestoreDirectory
    {
        get => This.RestoreDirectory;
        set => This.RestoreDirectory = value;
    }
    public string FileNameExtension => This.DefaultExt;
}