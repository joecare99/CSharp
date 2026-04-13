namespace RnzTrauer.Core;

/// <summary>
/// Describes a progress update emitted by <see cref="WebHandler"/>.
/// </summary>
public sealed record WebHandlerProgress(string Text, bool WriteLine = false);
