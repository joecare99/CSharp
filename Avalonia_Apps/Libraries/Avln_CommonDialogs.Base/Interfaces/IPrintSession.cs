namespace Avln_CommonDialogs.Base.Interfaces;

public interface IPrintSession : IAsyncDisposable
{
    PrintDocumentInfo DocumentInfo { get; }

    /// <summary>
    /// Begins a new page and returns a canvas-like target to draw onto.
    /// Caller owns the lifetime of the returned object only until <see cref="EndPageAsync"/>.
    /// </summary>
    ValueTask<IPrintPageCanvas> BeginPageAsync(CancellationToken cancellationToken = default);

    ValueTask EndPageAsync(CancellationToken cancellationToken = default);

    ValueTask CommitAsync(CancellationToken cancellationToken = default);
    ValueTask CancelAsync(CancellationToken cancellationToken = default);
}
