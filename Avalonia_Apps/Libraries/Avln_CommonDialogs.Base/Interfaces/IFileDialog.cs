namespace Avln_CommonDialogs.Base.Interfaces;

public interface IFileDialog
{
    string? Title { get; set; }
    string? InitialDirectory { get; set; }
    bool RestoreDirectory { get; set; }
    bool AddExtension { get; set; }
    bool CheckFileExists { get; set; }
    string? DefaultExtension { get; set; }

    IReadOnlyList<FileTypeFilter> Filters { get; }
}
