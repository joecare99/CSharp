using RnzTrauer.Core.Domain;

namespace RnzTrauer.Core.Services;

/// <summary>Input for the read-only notice search use case.</summary>
public sealed record NoticeSearchRequest(NoticeFilter Filter);
