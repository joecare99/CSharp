using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Ollama.Wpf.TextAnalysis.Services;

/// <summary>
/// Provides the WPF file-dialog implementation for selecting text input.
/// </summary>
public sealed class OpenFileDialogTextFilePicker : ITextFilePicker
{
    internal static Func<OpenFileDialog, bool?> DialogPresenter { get; set; } =
        (Func<OpenFileDialog, bool?>)typeof(OpenFileDialog)
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Single(static method => method.Name == nameof(OpenFileDialog.ShowDialog) && method.GetParameters().Length == 0)
            .CreateDelegate(typeof(Func<OpenFileDialog, bool?>));

    /// <inheritdoc />
    public async Task<TextFileSelection?> PickAndReadAsync(CancellationToken cancellationToken = default)
    {
        OpenFileDialog dialog = new()
        {
            Filter = "Text files|*.txt;*.md;*.log|All files|*.*",
            CheckFileExists = true,
            Multiselect = false,
        };

        if (DialogPresenter(dialog) != true)
        {
            return null;
        }

        string content = await File.ReadAllTextAsync(dialog.FileName, cancellationToken);
        return new TextFileSelection(dialog.FileName, content);
    }

}
