namespace RnzTrauer.Core.Services;

/// <summary>One incremental HTML callback with its raw source representation.</summary>
public sealed record HtmlCallbackEvent(
    HtmlCallbackKind Kind,
    string Value,
    string Raw,
    string? TagName = null,
    string? TagPath = null);
