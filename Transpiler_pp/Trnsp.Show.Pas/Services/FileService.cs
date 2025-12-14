using System.IO;
using Microsoft.Win32;

namespace Trnsp.Show.Pas.Services
{
    public class FileService : IFileService
    {
        public string? OpenFileDialog(string title, string filter)
        {
            var dialog = new OpenFileDialog
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
}
