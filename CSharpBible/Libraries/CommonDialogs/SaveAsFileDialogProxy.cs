using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

public class SaveFileDialogProxy : FileDialogProxy<object>, IFileDialog
{
    public SaveFileDialogProxy()
        : base(new SaveFileDialog())
    {
    }

    public bool OverwritePrompt
    {
        get => ((SaveFileDialog)This).OverwritePrompt;
        set => ((SaveFileDialog)This).OverwritePrompt = value;
    }

     public string FileNameExtension => ((SaveFileDialog)This).DefaultExt;
}