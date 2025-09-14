namespace CommonDialogs.Interfaces;

public interface IFileDialog
{
    string FileName { get; set; }
    string Filter { get; set; }
    int FilterIndex { get; set; }
    string InitialDirectory { get; set; }
    bool RestoreDirectory { get; set; }
    bool AddExtension { get; set; }
    bool CheckFileExists { get; set; }
    string Title { get; set; }
    string DefaultExt { get; set; }

    bool? ShowDialog();
    bool? ShowDialog(object owner);
}
