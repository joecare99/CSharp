namespace RepoMigrator.Core;

/// <summary>
/// Represents a normalized binary change payload.
/// </summary>
public sealed class MigrationBinaryChange
{
    /// <summary>
    /// Gets the binary payload mode used by the change.
    /// </summary>
    public MigrationBinaryPayloadMode PayloadMode { get; init; } = MigrationBinaryPayloadMode.Inline;

    /// <summary>
    /// Gets the inline payload when the payload mode is inline.
    /// </summary>
    public byte[]? InlinePayload { get; init; }

    /// <summary>
    /// Gets the referenced payload location when the payload mode is file-reference-based.
    /// </summary>
    public BinaryPayloadReference? PayloadReference { get; init; }

    /// <summary>
    /// Gets the logical payload length in bytes.
    /// </summary>
    public long PayloadLength { get; init; }

    /// <summary>
    /// Gets the optional payload hash preserved for diagnostics or validation.
    /// </summary>
    public string? PayloadHash { get; init; }

    /// <summary>
    /// Gets the optional source-format hint preserved for diagnostics.
    /// </summary>
    public string? SourceFormatHint { get; init; }
}
