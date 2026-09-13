using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.UI.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Navigation;

/// <summary>
/// Opens source documents through the workbench's composed single editor.
/// </summary>
public sealed class WorkbenchEditorDocumentHost : IEditorDocumentHost
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a document host for the composed workbench editor.
    /// </summary>
    /// <param name="serviceProvider">The workbench service provider.</param>
    public WorkbenchEditorDocumentHost(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public async Task<IEditorDocument?> OpenOrActivateAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("A file path is required.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            return null;
        }

        var normalizedPath = Path.GetFullPath(filePath);
        var editorComponent = _serviceProvider.GetRequiredService<IAvaloniaEditorComponent>();
        if (!string.Equals(
                editorComponent.EditorViewModel.CurrentFilePath,
                normalizedPath,
                StringComparison.OrdinalIgnoreCase))
        {
            await editorComponent.EditorViewModel
                .OpenFileAsync(normalizedPath, cancellationToken)
                .ConfigureAwait(false);
        }

        return new WorkbenchEditorDocument(normalizedPath);
    }
}
