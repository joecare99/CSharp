using System.IO;
using Microsoft.Win32;

namespace ScriptedSvgWpf.Services;

public interface IDocumentFileService
{
    string? ChooseOpenPath();
    string? ChooseSavePath(string suggestedName, string filter);
    string ReadText(string path);
    void WriteText(string path, string text);
}

public sealed class WpfDocumentFileService : IDocumentFileService
{
    public string? ChooseOpenPath()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Scripted SVG (*.ssvg)|*.ssvg|Text files (*.txt)|*.txt|All files (*.*)|*.*",
            CheckFileExists = true,
            Multiselect = false
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string? ChooseSavePath(string suggestedName, string filter)
    {
        var dialog = new SaveFileDialog
        {
            FileName = suggestedName,
            Filter = filter,
            AddExtension = true,
            OverwritePrompt = true
        };
        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }

    public string ReadText(string path) => File.ReadAllText(path);

    public void WriteText(string path, string text) => File.WriteAllText(path, text);
}
