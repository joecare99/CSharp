using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RnzTrauer.Core.Services;

/// <summary>
/// Coordinates the read-only notice search use case.
/// </summary>
public sealed class NoticeSearchService : INoticeSearchService
{
    private readonly INoticeRepository _repository;

    public NoticeSearchService(INoticeRepository repository)
    {
        _repository = repository;
    }

    public async Task<NoticeSearchResult> SearchAsync(
        NoticeSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var validation = Validate(request);
        if (validation is not null)
            return validation;

        try
        {
            var notices = await _repository
                .FindAsync(request.Filter, cancellationToken)
                .ConfigureAwait(false);
            return NoticeSearchResult.Success(
                notices.Select(NoticeProjection.FromDomain).ToArray());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return NoticeSearchResult.Failure(exception);
        }
    }

    private static NoticeSearchResult? Validate(NoticeSearchRequest request)
    {
        var filter = request.Filter ?? throw new ArgumentException("A notice filter is required.", nameof(request));
        if (ContainsControlCharacters(filter.OrderNumberPrefix)
            || ContainsControlCharacters(filter.KeywordContains))
            return NoticeSearchResult.Invalid(
                "search.invalid_text",
                "Search text must not contain control characters.");

        if (filter.OrderNumberPrefix?.Length > 200 || filter.KeywordContains?.Length > 200)
            return NoticeSearchResult.Invalid(
                "search.text_too_long",
                "Search text must not exceed 200 characters.");

        if (!Enum.IsDefined(filter.Kind))
            return NoticeSearchResult.Invalid(
                "search.invalid_queue",
                "The selected review queue is not supported.");

        return null;
    }

    private static bool ContainsControlCharacters(string? value)
    {
        if (value is null)
            return false;
        foreach (var character in value)
            if (char.IsControl(character))
                return true;
        return false;
    }
}
