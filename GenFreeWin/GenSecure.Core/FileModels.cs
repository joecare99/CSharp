using System;
using System.Collections.Generic;

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

    public required string OwnerWindowsSid { get; init; }

    public required List<string> AllowedWindowsSids { get; init; }

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
