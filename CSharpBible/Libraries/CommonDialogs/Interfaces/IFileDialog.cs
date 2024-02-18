namespace CommonDialogs.Interfaces
{
    public interface IFileDialog
    {
        string FileName { get; set; }

        bool? ShowDialog();
        bool? ShowDialog(object owner);
    }
}
