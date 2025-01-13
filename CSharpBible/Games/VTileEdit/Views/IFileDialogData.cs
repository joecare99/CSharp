namespace VTileEdit.Views
{
    public interface IFileDialogData
    {
        string Title { get; set; }
        string Filter { get; set; }
        string FileName { get; set; }
        string InitialDirectory { get; set; }
        bool Multiselect { get; set; }
        bool CheckFileExists { get; set; }
        bool CheckPathExists { get; set; }       
    }
}