using System;
using System.Threading;
using System.Threading.Tasks;
using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>
/// Validates and delegates one notice update. The composition root does not
/// register this service until transaction and conflict behavior is approved.
/// </summary>
public sealed class NoticeUpdateService : INoticeUpdateService
{
    private readonly INoticeRepository _repository;

    public NoticeUpdateService(INoticeRepository repository) => _repository = repository;

    public async Task<NoticeUpdateResult> UpdateAsync(
        NoticeUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Notice);
        cancellationToken.ThrowIfCancellationRequested();

        var validation = Validate(request.Notice);
        if (validation is not null)
            return validation;

        try
        {
            await _repository.SaveAsync(request.Notice, cancellationToken).ConfigureAwait(false);
            return NoticeUpdateResult.Success();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return NoticeUpdateResult.Failure(exception);
        }
    }

    private static NoticeUpdateResult? Validate(DeathNotice notice)
    {
        if (notice.Id <= 0)
            return NoticeUpdateResult.Invalid("notice.id_required", "A persisted notice id is required.");
        if (string.IsNullOrWhiteSpace(notice.OrderNumber))
            return NoticeUpdateResult.Invalid("notice.order_required", "An order number is required.");
        if (!Enum.IsDefined(notice.Category))
            return NoticeUpdateResult.Invalid("notice.category_invalid", "The notice category is not supported.");
        return null;
    }
}
