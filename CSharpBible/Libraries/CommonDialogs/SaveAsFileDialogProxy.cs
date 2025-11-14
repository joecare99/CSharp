using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

public class SaveFileDialogProxy : FileDialogProxy<SaveFileDialog>, IFileDialog
{
    public SaveFileDialogProxy()
        : base(new SaveFileDialog())
    {
    }

    public bool OverwritePrompt
    {
        get => This.OverwritePrompt;
        set => This.OverwritePrompt = value;
    }

     public string FileNameExtension => This.DefaultExt;
}