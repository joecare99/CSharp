using System;
using GenInterfaces.Interfaces.Genealogic;

namespace GenSecure.Contracts;

/// <summary>
/// Persists complete genealogies in a file-system-friendly structure with per-person security.
/// </summary>
public interface IGenealogySecureStore
{
    /// <summary>
    /// Saves a genealogy package using a deterministic, Git-friendly file layout.
    /// </summary>
    /// <param name="sGenealogyId">The stable genealogy identifier.</param>
    /// <param name="genealogy">The genealogy graph to persist.</param>
    /// <param name="getPersonStoreMode">
    /// Optional resolver for person storage mode.
    /// If omitted, living persons are encrypted and deceased persons are stored as plaintext.
    /// </param>
    void Save(string sGenealogyId, IGenealogy genealogy, Func<IGenPerson, StoreMode>? getPersonStoreMode = null);

    /// <summary>
    /// Loads a persisted genealogy package.
    /// </summary>
    /// <param name="sGenealogyId">The stable genealogy identifier.</param>
    /// <param name="factory">The model factory used to rehydrate `GenInterfaces` objects.</param>
    /// <returns>The loaded genealogy graph.</returns>
    IGenealogy Load(string sGenealogyId, IGenealogyModelFactory factory);

    /// <summary>
    /// Determines whether a genealogy manifest exists.
    /// </summary>
    /// <param name="sGenealogyId">The stable genealogy identifier.</param>
    /// <returns><see langword="true"/> when the genealogy package exists; otherwise <see langword="false"/>.</returns>
    bool Exists(string sGenealogyId);

    /// <summary>
    /// Deletes a genealogy package.
    /// </summary>
    /// <param name="sGenealogyId">The stable genealogy identifier.</param>
    /// <param name="eMode">The deletion mode to apply.</param>
    void Delete(string sGenealogyId, DeleteMode eMode);
}
