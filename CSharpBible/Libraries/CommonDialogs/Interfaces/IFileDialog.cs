namespace CommonDialogs.Interfaces;

public interface IFileDialog
{
    string FileName { get; set; }
    string Filter { get; set; }
    int FilterIndex { get; set; }
    string InitialDirectory { get; set; }
    bool RestoreDirectory { get; set; }

    bool? ShowDialog();
    bool? ShowDialog(object owner);
}
