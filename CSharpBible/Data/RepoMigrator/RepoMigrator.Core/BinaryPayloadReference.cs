namespace RepoMigrator.Core;

/// <summary>
/// Represents an indirect reference to a binary payload stored outside the in-memory change model.
/// </summary>
public sealed class BinaryPayloadReference
{
    /// <summary>
    /// Gets the reference kind used to resolve the payload.
    /// </summary>
    public BinaryPayloadReferenceKind Kind { get; init; } = BinaryPayloadReferenceKind.RelativeArtifactPath;

    /// <summary>
    /// Gets the provider-agnostic relative path or provider-owned reference value.
    /// </summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the reference is expected to remain portable across machines.
    /// </summary>
    public bool IsPortable { get; init; } = true;
}
