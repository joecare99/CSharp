using Code.Navigation;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Editor.Navigation;

/// <summary>
/// Bridges the neutral code-location contract to an editor host and its caret service.
/// </summary>
public sealed class EditorCodeNavigator : ICodeNavigator
{
    private readonly IEditorDocumentHost _documentHost;
    private readonly IEditorCaretService _caretService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorCodeNavigator"/> class.
    /// </summary>
    /// <param name="documentHost">The document and activation host.</param>
    /// <param name="caretService">The editor focus and caret service.</param>
    public EditorCodeNavigator(
        IEditorDocumentHost documentHost,
        IEditorCaretService caretService)
    {
        _documentHost = documentHost ?? throw new ArgumentNullException(nameof(documentHost));
        _caretService = caretService ?? throw new ArgumentNullException(nameof(caretService));
    }

    /// <inheritdoc/>
    public async Task NavigateAsync(
        CodeLocation location,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(location);
        cancellationToken.ThrowIfCancellationRequested();

        var document = await _documentHost
            .OpenOrActivateAsync(location.Path, cancellationToken)
            .ConfigureAwait(false);

        if (document is null)
        {
            throw new FileNotFoundException(
                "The editor host could not open or activate the source document.",
                location.Path);
        }

        await _caretService
            .FocusAndMoveCaretAsync(document, location, cancellationToken)
            .ConfigureAwait(false);
    }
}