using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Persistence boundary for notices, places, link candidates, and atomic edits.</summary>
public interface INoticeRepository
{
    Task<IReadOnlyList<DeathNotice>> FindAsync(NoticeFilter filter, CancellationToken cancellationToken = default);
    Task SaveAsync(DeathNotice notice, CancellationToken cancellationToken = default);
    Task<bool> UpsertImportedAsync(DeathNotice notice, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetPlaceNamesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeathNotice>> GetLinkCandidatesAsync(long noticeId, CancellationToken cancellationToken = default);
}
