namespace RepoMigrator.Core;

/// <summary>
/// Defines the kind of indirection used to resolve a referenced binary payload.
/// </summary>
public enum BinaryPayloadReferenceKind
{
    /// <summary>
    /// Represents a payload stored under a runtime-defined artifact path relative to a known root.
    /// </summary>
    RelativeArtifactPath,

    /// <summary>
    /// Represents a provider-owned payload handle whose storage semantics remain provider-specific.
    /// </summary>
    ProviderOwnedHandle
}
