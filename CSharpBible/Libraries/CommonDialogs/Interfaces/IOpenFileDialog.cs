namespace CommonDialogs.Interfaces;

public interface IOpenFileDialog: IFileDialog
{
    string Filter { get; set; }
    string InitialDirectory { get; set; }
    string Title { get; set; }
    bool Multiselect { get; set; }
    string[] FileNames { get; }
    string SafeFileName { get; }
    string[] SafeFileNames { get; }
    string DefaultExt { get; set; }
    string FileNameExtension { get; }
}
