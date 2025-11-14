namespace CommonDialogs.Interfaces;

public interface IOpenFileDialog: IFileDialog
{
    bool Multiselect { get; set; }
    string[] FileNames { get; }
    string SafeFileName { get; }
    string[] SafeFileNames { get; }
    string FileNameExtension { get; }
}
