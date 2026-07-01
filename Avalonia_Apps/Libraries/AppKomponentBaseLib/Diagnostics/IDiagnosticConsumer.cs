using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AppKomponentBaseLib.Diagnostics;

/// <summary>
/// Consumes provider-neutral diagnostics for output, visualization, or further processing.
/// </summary>
public interface IDiagnosticConsumer
{
    /// <summary>
    /// Consumes a sequence of diagnostics.
    /// </summary>
    /// <param name="diagnostics">The diagnostics to consume.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that completes when consumption is done.</returns>
    ValueTask ConsumeAsync(IEnumerable<Diagnostic> diagnostics, CancellationToken cancellationToken = default);
}
