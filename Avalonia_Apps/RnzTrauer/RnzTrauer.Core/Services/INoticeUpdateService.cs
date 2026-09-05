using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Core.Services;

/// <summary>Application boundary for a future validated notice update.</summary>
public interface INoticeUpdateService
{
    Task<NoticeUpdateResult> UpdateAsync(
        NoticeUpdateRequest request,
        CancellationToken cancellationToken = default);
}
