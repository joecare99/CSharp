using System.Collections.Generic;
namespace RnzTrauer.Core.Services;

/// <summary>Read-only detail projection used by the Avalonia detail pane.</summary>
public sealed record NoticeDetailResult(
    NoticeProjection Notice,
    IReadOnlyList<NoticeProjection> LinkCandidates,
    string? PlaceName,
    NoticeMediaState Media);
