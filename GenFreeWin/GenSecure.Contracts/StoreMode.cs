namespace GenSecure.Contracts;

/// <summary>
/// Determines the storage strategy applied when saving a person record.
/// </summary>
public enum StoreMode
{
    /// <summary>
    /// The record is encrypted with AES-256-GCM using a per-person data-encryption key (DEK).
    /// Use for living persons whose data is subject to DSGVO protection.
    /// </summary>
    Encrypted = 0,

    /// <summary>
    /// The record is stored as plain, human-readable JSON without encryption.
    /// Suitable for persons who are deceased or can be reasonably assumed to be
    /// no longer living — e.g. based on birth date, death date, or event dates.
    /// No key file is created; access control is not applicable.
    /// </summary>
    Plaintext = 1,
}
