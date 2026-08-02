using System;

namespace RnzTrauer.Core.Domain;

/// <summary>Safe, named replacement for the Pascal <c>SetFilter(FieldName, Filter)</c> string protocol.</summary>
public sealed record NoticeFilter(
    string? OrderNumberPrefix = null,
    string? KeywordContains = null,
    NoticeFilterKind Kind = NoticeFilterKind.All,
    DateTime? ChangedSince = null);

/// <summary>Review queues preserved from the RNZ filter frame.</summary>
public enum NoticeFilterKind
{
    All,
    MissingText,
    DeathNoticeWithoutPlace,
    ImplausibleDates,
    MissingSex,
    MissingLink,
    DuplicateCandidates,
    MaleWithMaidenName,
    RecentMissingProfileImage,
}
