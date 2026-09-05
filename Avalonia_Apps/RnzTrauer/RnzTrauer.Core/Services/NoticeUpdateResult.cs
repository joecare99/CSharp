using System;

namespace RnzTrauer.Core.Services;

public enum NoticeUpdateStatus
{
    Succeeded,
    InvalidRequest,
    Failed,
}

/// <summary>Typed outcome for a notice update without enabling a UI write path.</summary>
public sealed record NoticeUpdateResult(
    NoticeUpdateStatus Status,
    string? ErrorCode = null,
    string? ErrorMessage = null)
{
    public static NoticeUpdateResult Success() => new(NoticeUpdateStatus.Succeeded);

    public static NoticeUpdateResult Invalid(string code, string message) =>
        new(NoticeUpdateStatus.InvalidRequest, code, message);

    public static NoticeUpdateResult Failure(Exception exception) =>
        new(NoticeUpdateStatus.Failed, "notice.update_failed", exception.Message);
}
