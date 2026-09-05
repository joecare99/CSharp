using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Builds a deterministic detail projection from a notice and read ports.</summary>
public sealed class NoticeDetailService : INoticeDetailService
{
    private readonly INoticeRepository _repository;

    public NoticeDetailService(INoticeRepository repository) => _repository = repository;

    public async Task<NoticeDetailResult> LoadAsync(
        NoticeDetailRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Notice);
        cancellationToken.ThrowIfCancellationRequested();

        var candidates = request.Notice.Id > 0
            ? await _repository.GetLinkCandidatesAsync(request.Notice.Id, cancellationToken).ConfigureAwait(false)
            : Array.Empty<DeathNotice>();
        var media = new NoticeMediaState(
            !string.IsNullOrWhiteSpace(request.Notice.PdfFile),
            !string.IsNullOrWhiteSpace(request.Notice.PngFile),
            !string.IsNullOrWhiteSpace(request.Notice.ProfileImage));
        return new NoticeDetailResult(
            NoticeProjection.FromDomain(request.Notice),
            candidates.Select(NoticeProjection.FromDomain).ToArray(),
            request.Notice.Place,
            media);
    }
}
