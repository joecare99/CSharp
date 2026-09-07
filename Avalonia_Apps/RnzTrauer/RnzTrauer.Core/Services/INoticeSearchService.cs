using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Core.Services;

/// <summary>
/// Application boundary for read-only notice search.
/// </summary>
public interface INoticeSearchService
{
    Task<NoticeSearchResult> SearchAsync(
        NoticeSearchRequest request,
        CancellationToken cancellationToken = default);
}
