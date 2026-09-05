using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Input for the future notice-update command.</summary>
public sealed record NoticeUpdateRequest(DeathNotice Notice);
