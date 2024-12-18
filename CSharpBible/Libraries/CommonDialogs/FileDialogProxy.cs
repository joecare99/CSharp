using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

public class FileDialogProxy<T> : IFileDialog where T : FileDialog
{
    private T _fileDialog;
    public FileDialogProxy(T fileDialog)
    {
        _fileDialog = fileDialog;
    }

    public T This => _fileDialog;

    public string FileName
    {
        get => _fileDialog.FileName;
        set => _fileDialog.FileName = value;
    }

    public bool? ShowDialog()
    {
        return _fileDialog.ShowDialog();
    }

    public bool? ShowDialog(object owner)
    {
        return _fileDialog.ShowDialog(owner as System.Windows.Window);
    }

}