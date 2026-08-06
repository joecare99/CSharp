using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Acquisition;

/// <summary>Acquires HTML bytes from a local file or an HTTP(S) endpoint.</summary>
public interface IHtmlAcquisitionService
{
    /// <summary>Reads one source and optionally archives it atomically.</summary>
    Task<HtmlAcquisitionResult> AcquireAsync(
        HtmlAcquisitionRequest request,
        CancellationToken cancellationToken = default);
}
