using System.IO;
using CommonDialogs;

namespace Trnsp.Show.Pas.Services;

public class FileService : IFileService
{
    public string? OpenFileDialog(string title, string filter)
    {
        var dialog = new OpenFileDialogProxy
        {
            Title = title,
            Filter = filter
        };

        if (dialog.ShowDialog() == true)
        {
            return dialog.FileName;
        }

        return null;
    }

    public string ReadAllText(string path)
    {
        return File.ReadAllText(path);
    }
}
