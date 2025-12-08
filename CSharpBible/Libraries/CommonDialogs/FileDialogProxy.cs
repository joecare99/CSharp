using Microsoft.Win32;
using CommonDialogs.Interfaces;

namespace CommonDialogs;

public class FileDialogProxy<T> : IFileDialog where T : class
{
    private FileDialog _fileDialog;
    public FileDialogProxy(T fileDialog)
    {
        _fileDialog = fileDialog as FileDialog;
    }

    public T This => _fileDialog as T;

    public string FileName
    {
        get => _fileDialog.FileName;
        set => _fileDialog.FileName = value;
    }
    public string Filter { get => _fileDialog.Filter; set => _fileDialog.Filter = value; }
    public int FilterIndex { get => _fileDialog.FilterIndex; set => _fileDialog.FilterIndex = value; }
    public string InitialDirectory { get => _fileDialog.InitialDirectory; set => _fileDialog.InitialDirectory = value; }
    public bool RestoreDirectory { get => _fileDialog.RestoreDirectory; set => _fileDialog.RestoreDirectory = value; }
    public bool AddExtension { get => _fileDialog.AddExtension; set => _fileDialog.AddExtension = value; }
    public bool CheckFileExists { get => _fileDialog.CheckFileExists; set => _fileDialog.CheckFileExists = value; }
    public string DefaultExt { get => _fileDialog.DefaultExt; set => _fileDialog.DefaultExt = value; }
    public string Title { get => _fileDialog.Title; set => _fileDialog.Title = value; }

    public bool? ShowDialog()
    {
        return _fileDialog.ShowDialog();
    }

    public bool? ShowDialog(object owner)
    {
        return _fileDialog.ShowDialog(owner as System.Windows.Window);
    }

}