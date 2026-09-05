using System;
using System.Collections.Generic;

namespace RnzTrauer.Core.Services;

public enum NoticeSearchStatus
{
    Succeeded,
    InvalidRequest,
    Failed,
}

/// <summary>Typed outcome of a read-only notice search.</summary>
public sealed record NoticeSearchResult(
    NoticeSearchStatus Status,
    IReadOnlyList<NoticeProjection> Notices,
    string? ErrorCode = null,
    string? ErrorMessage = null)
{
    public bool IsSuccess => Status == NoticeSearchStatus.Succeeded;

    public static NoticeSearchResult Success(IReadOnlyList<NoticeProjection> notices) =>
        new(NoticeSearchStatus.Succeeded, notices);

    public static NoticeSearchResult Invalid(string code, string message) =>
        new(NoticeSearchStatus.InvalidRequest, Array.Empty<NoticeProjection>(), code, message);

    public static NoticeSearchResult Failure(Exception exception) =>
        new(NoticeSearchStatus.Failed, Array.Empty<NoticeProjection>(), "search.failed", exception.Message);
}
