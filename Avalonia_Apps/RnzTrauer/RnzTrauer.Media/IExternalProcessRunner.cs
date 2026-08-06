using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Media;

/// <summary>Runs external tools without shell command composition.</summary>
public interface IExternalProcessRunner
{
    /// <summary>Runs the requested process and captures standard streams.</summary>
    Task<ExternalProcessResult> RunAsync(
        ExternalProcessRequest request,
        CancellationToken cancellationToken = default);
}
