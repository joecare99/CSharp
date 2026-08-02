namespace RnzTrauer.Core.Services;

/// <summary>One output instruction emitted by the legacy-compatible schema filter.</summary>
public sealed record SchemaFilterEmission(byte Mode, string Text);
