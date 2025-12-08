namespace VTileEdit.Views
{
    public interface IVisual
    {
        bool ShowFileDialog(IFileDialogData fileDialog);
        void HandleUserInput();
    }
}