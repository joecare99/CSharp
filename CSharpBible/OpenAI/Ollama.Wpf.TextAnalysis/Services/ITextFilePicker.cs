using System.Threading;
using System.Threading.Tasks;

namespace Ollama.Wpf.TextAnalysis.Services;

/// <summary>
/// Selects and reads a text file for the user interface.
/// </summary>
public interface ITextFilePicker
{
    /// <summary>
    /// Lets the user select a text file and returns its contents.
    /// </summary>
    /// <param name="cancellationToken">The operation cancellation token.</param>
    /// <returns>The selected file, or <see langword="null"/> if the user cancels.</returns>
    Task<TextFileSelection?> PickAndReadAsync(CancellationToken cancellationToken = default);
}
