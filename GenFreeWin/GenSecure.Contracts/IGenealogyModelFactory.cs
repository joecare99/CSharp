using System;
using GenInterfaces.Data;
using GenInterfaces.Interfaces;
using GenInterfaces.Interfaces.Genealogic;

namespace GenSecure.Contracts;

/// <summary>
/// Creates `GenInterfaces`-compatible model instances during genealogy store loading.
/// </summary>
public interface IGenealogyModelFactory
{
    /// <summary>
    /// Creates a genealogy root object.
    /// </summary>
    /// <param name="gUid">The persisted unique identifier.</param>
    /// <returns>A new genealogy instance.</returns>
    IGenealogy CreateGenealogy(Guid gUid);

    /// <summary>
    /// Creates a person entity.
    /// </summary>
    /// <param name="gUid">The persisted unique identifier.</param>
    /// <param name="iId">The persisted local identifier.</param>
    /// <param name="dtLastChange">The persisted last change timestamp.</param>
    /// <returns>A new person entity instance.</returns>
    IGenPerson CreatePerson(Guid gUid, int iId, DateTime? dtLastChange);

    /// <summary>
    /// Creates a family entity.
    /// </summary>
    /// <param name="gUid">The persisted unique identifier.</param>
    /// <param name="iId">The persisted local identifier.</param>
    /// <param name="dtLastChange">The persisted last change timestamp.</param>
    /// <returns>A new family entity instance.</returns>
    IGenFamily CreateFamily(Guid gUid, int iId, DateTime? dtLastChange);

    /// <summary>
    /// Creates a place.
    /// </summary>
    /// <param name="gUid">The persisted unique identifier.</param>
    /// <param name="iId">The persisted local identifier.</param>
    /// <param name="dtLastChange">The persisted last change timestamp.</param>
    /// <returns>A new place instance.</returns>
    IGenPlace CreatePlace(Guid gUid, int iId, DateTime? dtLastChange);

    /// <summary>
    /// Creates a fact for an entity owner.
    /// </summary>
    /// <param name="genOwner">The entity that owns the fact.</param>
    /// <param name="gUid">The persisted unique identifier.</param>
    /// <param name="iId">The persisted local identifier.</param>
    /// <param name="dtLastChange">The persisted last change timestamp.</param>
    /// <param name="eFactType">The fact type.</param>
    /// <returns>A new fact instance.</returns>
    IGenFact CreateFact(IGenEntity genOwner, Guid gUid, int iId, DateTime? dtLastChange, EFactType eFactType);

    /// <summary>
    /// Creates a connection to another entity.
    /// </summary>
    /// <param name="genConnectedEntity">The connected entity.</param>
    /// <param name="gUid">The persisted unique identifier.</param>
    /// <param name="eConnectionType">The connection type.</param>
    /// <returns>A new connection instance.</returns>
    IGenConnects CreateConnection(IGenEntity? genConnectedEntity, Guid gUid, EGenConnectionType eConnectionType);

    /// <summary>
    /// Creates a genealogical date.
    /// </summary>
    /// <param name="gUid">The persisted unique identifier.</param>
    /// <param name="iId">The persisted local identifier.</param>
    /// <param name="dtLastChange">The persisted last change timestamp.</param>
    /// <returns>A new date instance.</returns>
    IGenDate CreateDate(Guid gUid, int iId, DateTime? dtLastChange);

    /// <summary>
    /// Creates a transaction entry.
    /// </summary>
    /// <param name="gUid">The persisted unique identifier.</param>
    /// <param name="iId">The persisted local identifier.</param>
    /// <param name="dtLastChange">The persisted last change timestamp.</param>
    /// <param name="genClass">The referenced class object.</param>
    /// <param name="genEntry">The referenced entry object.</param>
    /// <param name="objData">The current payload.</param>
    /// <param name="objOldData">The previous payload.</param>
    /// <param name="dtTimestamp">The persisted transaction timestamp.</param>
    /// <param name="genPrev">The previous transaction in the chain.</param>
    /// <returns>A new transaction instance.</returns>
    IGenTransaction CreateTransaction(
        Guid gUid,
        int iId,
        DateTime? dtLastChange,
        IGenBase genClass,
        IGenBase genEntry,
        object? objData,
        object? objOldData,
        DateTime dtTimestamp,
        IGenTransaction? genPrev);
}
