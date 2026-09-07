using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Core.Services;

/// <summary>Application boundary for loading notice detail without mutation.</summary>
public interface INoticeDetailService
{
    Task<NoticeDetailResult> LoadAsync(
        NoticeDetailRequest request,
        CancellationToken cancellationToken = default);
}
