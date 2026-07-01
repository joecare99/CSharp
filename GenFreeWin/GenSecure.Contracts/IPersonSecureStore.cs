using System.Collections.Generic;

namespace GenSecure.Contracts;

/// <summary>
/// Provides encrypted JSON storage for person records with per-person data keys.
/// </summary>
public interface IPersonSecureStore
{
    /// <summary>
    /// Stores or updates a person record.
    /// </summary>
    /// <typeparam name="T">The record type to serialize.</typeparam>
    /// <param name="sPersonId">The stable person identifier.</param>
    /// <param name="value">The record instance to persist.</param>
    /// <param name="eStoreMode">
    /// Storage strategy. Use <see cref="StoreMode.Encrypted"/> (default) for living persons
    /// and <see cref="StoreMode.Plaintext"/> for deceased persons.
    /// </param>
    void Save<T>(string sPersonId, T value, StoreMode eStoreMode = StoreMode.Encrypted);

    /// <summary>
    /// Loads and decrypts a person record.
    /// </summary>
    /// <typeparam name="T">The record type to deserialize.</typeparam>
    /// <param name="sPersonId">The stable person identifier.</param>
    /// <returns>The decrypted record instance.</returns>
    T Load<T>(string sPersonId);

    /// <summary>
    /// Determines whether a person payload exists on disk.
    /// </summary>
    /// <param name="sPersonId">The stable person identifier.</param>
    /// <returns><see langword="true"/> when a payload exists; otherwise <see langword="false"/>.</returns>
    bool Exists(string sPersonId);

    /// <summary>
    /// Deletes a person record using the requested strategy.
    /// </summary>
    /// <param name="sPersonId">The stable person identifier.</param>
    /// <param name="eMode">The deletion mode to apply.</param>
    void Delete(string sPersonId, DeleteMode eMode);

    /// <summary>
    /// Grants access to a principal identifier for an existing person record.
    /// </summary>
    /// <param name="sPersonId">The stable person identifier.</param>
    /// <param name="sPrincipalId">The principal identifier to add.</param>
    void GrantAccess(string sPersonId, string sPrincipalId);

    /// <summary>
    /// Gets the hashes of principal identifiers that are permitted to decrypt a person record.
    /// The raw principal identifiers are never persisted; only their hashes are stored.
    /// </summary>
    /// <param name="sPersonId">The stable person identifier.</param>
    /// <returns>Lowercase hex hashes of the configured principal identifiers.</returns>
    IReadOnlyCollection<string> GetAllowedPrincipalHashes(string sPersonId);
}
