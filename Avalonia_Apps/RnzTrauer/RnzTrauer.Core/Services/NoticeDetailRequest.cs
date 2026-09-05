using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Input for loading the read-only detail projection of a notice.</summary>
public sealed record NoticeDetailRequest(DeathNotice Notice);
