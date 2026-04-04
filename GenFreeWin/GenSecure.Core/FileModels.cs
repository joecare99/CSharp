using System;
using System.Collections.Generic;
using System.Text.Json;

namespace GenSecure.Core;

internal sealed class EncryptedPersonRecord
{
    public required string PersonId { get; init; }

    public required string Algorithm { get; init; }

    public required string Nonce { get; init; }

    public required string Ciphertext { get; init; }

    public required string Tag { get; init; }

    public DateTimeOffset UpdatedUtc { get; init; }
}

internal sealed class PersonKeyRecord
{
    public required string PersonId { get; init; }

    public required string Algorithm { get; init; }

    public required string Nonce { get; init; }

    public required string WrappedPersonKey { get; init; }

    public required string Tag { get; init; }

    public required string OwnerWindowsSidHash { get; init; }

    public required List<string> AllowedWindowsSidHashes { get; init; }

    public DateTimeOffset CreatedUtc { get; init; }

    public DateTimeOffset UpdatedUtc { get; set; }
}

internal sealed class RecoveryKeyRecord
{
    public required string Algorithm { get; init; }

    public required string Salt { get; init; }

    public required int IterationCount { get; init; }

    public required string Nonce { get; init; }

    public required string Ciphertext { get; init; }

    public required string Tag { get; init; }

    public DateTimeOffset CreatedUtc { get; init; }
}

/// <summary>
/// Stores a person record as plain, human-readable JSON.
/// Used for deceased persons that no longer require DSGVO encryption.
/// No key file is created for this record type.
/// </summary>
internal sealed class PlaintextPersonRecord
{
    public required string PersonId { get; init; }

    /// <summary>Always "plaintext" — used by <see cref="PersonSecureStore"/> to detect the storage mode.</summary>
    public string Algorithm { get; init; } = "plaintext";

    /// <summary>The person payload embedded directly as a JSON object — human-readable without any tools.</summary>
    public required JsonElement Data { get; init; }

    public DateTimeOffset UpdatedUtc { get; init; }
}
