using System;

namespace SharpHack.Persist.Models;

/// <summary>
/// Stores recovery and integrity information for durable save handling.
/// </summary>
public sealed class SaveRecoveryMetadataDto
{
    /// <summary>
    /// Gets or sets the stable run identifier for correlating recovery files.
    /// </summary>
    public Guid RunId { get; set; }

    /// <summary>
    /// Gets or sets the monotonically increasing save sequence number.
    /// </summary>
    public long SequenceNumber { get; set; }

    /// <summary>
    /// Gets or sets the optional checksum of the logical JSON payload.
    /// </summary>
    public string Checksum { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether a recovery copy exists.
    /// </summary>
    public bool HasRecoveryCopy { get; set; }

    /// <summary>
    /// Gets or sets the optional UTC timestamp of the recovery copy.
    /// </summary>
    public DateTimeOffset? RecoveryUpdatedUtc { get; set; }
}
