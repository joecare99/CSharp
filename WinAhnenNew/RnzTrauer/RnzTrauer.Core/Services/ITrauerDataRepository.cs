namespace RnzTrauer.Core;

/// <summary>
/// Defines the database operations required for loading and persisting RNZ obituary data.
/// </summary>
public interface ITrauerDataRepository : IDisposable
{
    /// <summary>
    /// Reads obituary rows by internal id.
    /// </summary>
    List<Dictionary<string, object?>> TrauerAnzId(int iId);

    /// <summary>
    /// Reads obituary rows by announcement id.
    /// </summary>
    List<Dictionary<string, object?>> TrauerAnz(int iAnnouncement);

    /// <summary>
    /// Reads legacy obituary rows by legacy order id.
    /// </summary>
    List<Dictionary<string, object?>> LegacyTrauerAnz(string sAuftrag);

    /// <summary>
    /// Reads obituary rows where the specified field is null.
    /// </summary>
    List<Dictionary<string, object?>> TrauerAnzIsNull(string sField, int iLimit = 1);

    /// <summary>
    /// Reads obituary case rows where the specified field is null.
    /// </summary>
    List<Dictionary<string, object?>> TrauerFallIsNull(string sField, int iLimit = 1);

    /// <summary>
    /// Reads obituary case rows where the specified field matches the provided value.
    /// </summary>
    List<Dictionary<string, object?>> TrauerFallEquals(string sField, string sValue, int iLimit = 1);

    /// <summary>
    /// Updates obituary case rows when values have changed.
    /// </summary>
    void UpdateTrauerFall(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues);

    /// <summary>
    /// Updates obituary announcement rows when values have changed.
    /// </summary>
    bool UpdateTrauerAnz(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues);

    /// <summary>
    /// Reads obituary case rows by internal id.
    /// </summary>
    List<Dictionary<string, object?>> TrauerFallById(int iId);

    /// <summary>
    /// Reads obituary case rows by URL.
    /// </summary>
    List<Dictionary<string, object?>> TrauerFallByUrl(string sUrl);

    /// <summary>
    /// Builds an in-memory index of obituary case URLs.
    /// </summary>
    Dictionary<string, long> BuildTrauerFallIndex();

    /// <summary>
    /// Inserts a new obituary case row.
    /// </summary>
    long AppendTrauerFall(IReadOnlyDictionary<string, object?> dValues);

    /// <summary>
    /// Inserts a new obituary announcement row.
    /// </summary>
    long AppendTrauerAnz(IReadOnlyDictionary<string, object?> dValues);

    /// <summary>
    /// Inserts a legacy obituary announcement row.
    /// </summary>
    long AppendLegacyTAnz(IReadOnlyDictionary<string, object?> dValues);
}
