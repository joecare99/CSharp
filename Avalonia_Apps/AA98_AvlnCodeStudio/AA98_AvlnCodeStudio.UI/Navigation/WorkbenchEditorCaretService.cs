using AA98_AvlnCodeStudio.Editor.Navigation;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.Controls;
using Code.Navigation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.UI.Navigation;

/// <summary>
/// Focuses the Avalonia workbench editor and positions its caret.
/// </summary>
public sealed class WorkbenchEditorCaretService : IEditorCaretService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a caret service for the composed workbench editor.
    /// </summary>
    /// <param name="serviceProvider">The workbench service provider.</param>
    public WorkbenchEditorCaretService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public Task FocusAndMoveCaretAsync(
        IEditorDocument document,
        CodeLocation location,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(location);
        cancellationToken.ThrowIfCancellationRequested();

        var editorComponent = _serviceProvider.GetRequiredService<IAvaloniaEditorComponent>();
        if (editorComponent.View is not EditorTextArea editor)
        {
            throw new InvalidOperationException("The workbench editor does not provide a caret-capable text area.");
        }

        editor.FocusAndMoveCaret(location);
        return Task.CompletedTask;
    }
}
