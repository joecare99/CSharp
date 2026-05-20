using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using Db.Core.Abstractions.Sql;
using Db.Core.Abstractions.Sql.Interfaaces;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.Core;

/// <summary>
/// Encapsulates the MySQL access required for RNZ obituary persistence.
/// </summary>
public sealed class TrauerDataRepository : ITrauerDataRepository
{
    private const string TableAnzeigen = "Anzeigen";
    private const string TableTrauerfall = "Trauerfall";
    private const string TableLegacyAnzeigen = "RNZ-Traueranzeigen`.`Anzeigen";

    private readonly IDbConnection _dbConn;
    private readonly IDbStatementRenderer _xStatementRenderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrauerDataRepository"/> class.
    /// </summary>

    /// <summary>
    /// Initializes a new instance of the <see cref="TrauerDataRepository"/> class with an injected connection factory.
    /// </summary>
    public TrauerDataRepository(IDbConnectionFactory xConnectionFactory, DatabaseSettings xSettings)
    {
        ArgumentNullException.ThrowIfNull(xConnectionFactory);
        ArgumentNullException.ThrowIfNull(xSettings);

        var dbSettings = xConnectionFactory.CreateSettingsStub();
        dbSettings["Server"] = xSettings.DBhost;
        dbSettings["UserID"] = xSettings.DBuser;
        dbSettings["Password"] = xSettings.DBpass;
        dbSettings["Database"] = xSettings.DB;

        _dbConn = xConnectionFactory.CreateConnection(dbSettings);
        _xStatementRenderer = xConnectionFactory.CreateStatementRenderer(_dbConn);
        _dbConn.Open();
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerAnzId(int iId) 
        => Query(_xStatementRenderer.CreateQuery(TableAnzeigen, ["*"], [new DbFilterClause("idAnzeige", DbFilterOperator.Equal, "@id")]),
            xCommand => AddScalarParameter(xCommand, "@id", iId));

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerAnz(int iAnnouncement) 
        => Query(_xStatementRenderer.CreateQuery(TableAnzeigen, ["*"], [new DbFilterClause("Announcement", DbFilterOperator.Equal, "@announcement")]), 
            xCommand => AddScalarParameter(xCommand, "@announcement", iAnnouncement));

    /// <inheritdoc />
    public List<Dictionary<string, object?>> LegacyTrauerAnz(string sAuftrag) 
        => Query(_xStatementRenderer.CreateQuery(TableLegacyAnzeigen, ["*"], [new DbFilterClause("Auftrag", DbFilterOperator.Equal, "@auftrag")]), 
            xCommand => AddScalarParameter(xCommand, "@auftrag", sAuftrag));

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerAnzIsNull(string sField, int iLimit = 1) 
        => Query(_xStatementRenderer.CreateQuery(TableAnzeigen, ["*"], [new DbFilterClause(sField, DbFilterOperator.IsNull)], iLimit), xCommand => { });

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerFallIsNull(string sField, int iLimit = 1) 
        => Query(_xStatementRenderer.CreateQuery(TableTrauerfall, ["*"], [new DbFilterClause(sField, DbFilterOperator.IsNull)], iLimit), xCommand => { });

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerFallEquals(string sField, string sValue, int iLimit = 1) 
        => Query(_xStatementRenderer.CreateQuery(TableTrauerfall, ["*"], [new DbFilterClause(sField, DbFilterOperator.Equal, "@value")], iLimit), 
            xCommand => AddScalarParameter(xCommand, "@value", sValue));

    /// <inheritdoc />
    public bool UpdateTrauerFall(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues) 
        => UpdateRows(TableTrauerfall, arrNewValues, arrOldValues);

    /// <inheritdoc />
    public bool UpdateTrauerAnz(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues) 
        => UpdateRows(TableAnzeigen, arrNewValues, arrOldValues);

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerFallById(int iId) 
        => Query(_xStatementRenderer.CreateQuery(TableTrauerfall, ["*"], [new DbFilterClause("idTrauerfall", DbFilterOperator.Equal, "@id")]), xCommand => AddScalarParameter(xCommand, "@id", iId));

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerFallByUrl(string sUrl) 
        => Query(_xStatementRenderer.CreateQuery(TableTrauerfall, ["idTrauerfall", "url"], [new DbFilterClause("url", DbFilterOperator.Equal, "@url")]), xCommand => AddScalarParameter(xCommand, "@url", sUrl));
    /// <inheritdoc />
    public Dictionary<string, long> BuildTrauerFallIndex()
    {
        var dIndex = new Dictionary<string, long>(StringComparer.Ordinal);
        using var xCommand = _xStatementRenderer.CreateQuery(TableTrauerfall, ["idTrauerfall", "url"], []);
        using var xReader = xCommand.ExecuteReader();
        while (xReader.Read())
        {
            dIndex[xReader.GetString(1)] = xReader.GetInt64(0);
        }

        return dIndex;
    }

    /// <inheritdoc />
    public long AppendTrauerFall(IReadOnlyDictionary<string, object?> dValues)
    {
        using var xCommand = _xStatementRenderer.CreateInsert(TableTrauerfall, [
            new KeyValuePair<string, string>("URL", "@url"),
            new KeyValuePair<string, string>("Created", "@created"),
            new KeyValuePair<string, string>("Preread_Birth", "@birth"),
            new KeyValuePair<string, string>("Preread_Death", "@death"),
            new KeyValuePair<string, string>("Fullname", "@fullName"),
            new KeyValuePair<string, string>("Firstname", "@firstName"),
            new KeyValuePair<string, string>("Lastname", "@lastName"),
            new KeyValuePair<string, string>("Birthname", "@birthName"),
            new KeyValuePair<string, string>("Place", "@place"),
            new KeyValuePair<string, string>("Created_by", "@createdBy")]);
        AddParameter(xCommand, "@url", dValues, "URL");
        AddParameter(xCommand, "@created", dValues, "Created");
        AddParameter(xCommand, "@birth", dValues, "Preread_Birth");
        AddParameter(xCommand, "@death", dValues, "Preread_Death");
        AddParameter(xCommand, "@fullName", dValues, "Fullname");
        AddParameter(xCommand, "@firstName", dValues, "Firstname");
        AddParameter(xCommand, "@lastName", dValues, "Lastname");
        AddParameter(xCommand, "@birthName", dValues, "Birthname");
        AddParameter(xCommand, "@place", dValues, "Place");
        AddParameter(xCommand, "@createdBy", dValues, "Created_by");
        xCommand.ExecuteNonQuery();
        return GetLastInsertedId(xCommand);
    }

    /// <inheritdoc />
    public long AppendTrauerAnz(IReadOnlyDictionary<string, object?> dValues)
    {
        using var xCommand = _xStatementRenderer.CreateInsert(TableAnzeigen, [
            new KeyValuePair<string, string>("idTrauerfall", "@idtf"),
            new KeyValuePair<string, string>("url", "@url"),
            new KeyValuePair<string, string>("Announcement", "@announcement"),
            new KeyValuePair<string, string>("release", "@release"),
            new KeyValuePair<string, string>("localpath", "@localpath"),
            new KeyValuePair<string, string>("pngFile", "@pngFile"),
            new KeyValuePair<string, string>("pdfFile", "@pdfFile"),
            new KeyValuePair<string, string>("Additional", "@additional"),
            new KeyValuePair<string, string>("Firstname", "@firstName"),
            new KeyValuePair<string, string>("Lastname", "@lastName"),
            new KeyValuePair<string, string>("Birthname", "@birthName"),
            new KeyValuePair<string, string>("Birth", "@birth"),
            new KeyValuePair<string, string>("Death", "@death"),
            new KeyValuePair<string, string>("Place", "@place"),
            new KeyValuePair<string, string>("Info", "@info"),
            new KeyValuePair<string, string>("ProfileImg", "@profileImg"),
            new KeyValuePair<string, string>("Rubrik", "@rubrik")]);
        AddParameter(xCommand, "@idtf", dValues, "idTrauerfall");
        AddParameter(xCommand, "@url", dValues, "url");
        AddParameter(xCommand, "@announcement", dValues, "Announcement");
        AddParameter(xCommand, "@release", dValues, "release");
        AddParameter(xCommand, "@localpath", dValues, "localpath");
        AddParameter(xCommand, "@pngFile", dValues, "pngFile");
        AddParameter(xCommand, "@pdfFile", dValues, "pdfFile");
        AddParameter(xCommand, "@additional", dValues, "Additional");
        AddParameter(xCommand, "@firstName", dValues, "Firstname");
        AddParameter(xCommand, "@lastName", dValues, "Lastname");
        AddParameter(xCommand, "@birthName", dValues, "Birthname");
        AddParameter(xCommand, "@birth", dValues, "Birth");
        AddParameter(xCommand, "@death", dValues, "Death");
        AddParameter(xCommand, "@place", dValues, "Place");
        AddParameter(xCommand, "@info", dValues, "Info");
        AddParameter(xCommand, "@profileImg", dValues, "ProfileImg");
        AddParameter(xCommand, "@rubrik", dValues, "Rubrik");
        xCommand.ExecuteNonQuery();
        return GetLastInsertedId(xCommand);
    }

    /// <inheritdoc />
    public long AppendLegacyTAnz(IReadOnlyDictionary<string, object?> dValues)
    {
        using var xCommand = _xStatementRenderer.CreateInsert(TableLegacyAnzeigen, [
            new KeyValuePair<string, string>("Auftrag", "@auftrag"),
            new KeyValuePair<string, string>("url", "@url"),
            new KeyValuePair<string, string>("Announcement", "@announcement"),
            new KeyValuePair<string, string>("release", "@release"),
            new KeyValuePair<string, string>("localpath", "@localpath"),
            new KeyValuePair<string, string>("pngFile", "@pngFile"),
            new KeyValuePair<string, string>("pdfFile", "@pdfFile"),
            new KeyValuePair<string, string>("Additional", "@additional")]);
        AddParameter(xCommand, "@auftrag", dValues, "Auftrag");
        AddParameter(xCommand, "@url", dValues, "url");
        AddParameter(xCommand, "@announcement", dValues, "Announcement");
        AddParameter(xCommand, "@release", dValues, "release");
        AddParameter(xCommand, "@localpath", dValues, "localpath");
        AddParameter(xCommand, "@pngFile", dValues, "pngFile");
        AddParameter(xCommand, "@pdfFile", dValues, "pdfFile");
        AddParameter(xCommand, "@additional", dValues, "Additional");
        xCommand.ExecuteNonQuery();
        return GetLastInsertedId(xCommand);
    }

    /// <inheritdoc />
    public void Dispose() => _dbConn.Dispose();

    private static void AddParameter(IDbCommand xCommand, string sParameterName, IReadOnlyDictionary<string, object?> dValues, string sKey)
    {
        var xParameter = xCommand.CreateParameter();
        xParameter.ParameterName = sParameterName;
        xParameter.Value = dValues.TryGetValue(sKey, out var xValue) ? xValue ?? DBNull.Value : DBNull.Value;
        _ = xCommand.Parameters.Add(xParameter);
    }

    private bool UpdateRows(string sTable, List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues)
    {
        var xChanged = false;
        if (arrNewValues.Count == 0)
        {
            return false;
        }

        var sKeyField = arrNewValues[0].Keys.FirstOrDefault(k => k.StartsWith("id", StringComparison.OrdinalIgnoreCase));
        if (string.IsNullOrEmpty(sKeyField))
        {
            return false;
        }

        var dOldById = arrOldValues.ToDictionary(x => Convert.ToString(x[sKeyField], CultureInfo.InvariantCulture) ?? string.Empty, StringComparer.Ordinal);
        foreach (var dNewRow in arrNewValues)
        {
            var sKey = Convert.ToString(dNewRow[sKeyField], CultureInfo.InvariantCulture) ?? string.Empty;
            if (!dOldById.TryGetValue(sKey, out var dOldRow))
            {
                continue;
            }

            foreach (var (sColumn, xValue) in dNewRow)
            {
                dOldRow.TryGetValue(sColumn, out var xOldValue);
                if (Equals(xOldValue, xValue))
                {
                    continue;
                }

                using var xCommand = _xStatementRenderer.CreateUpdate(sTable, [new KeyValuePair<string, string>(sColumn, "@value")], [new DbFilterClause(sKeyField, DbFilterOperator.Equal, "@key")]);
                AddScalarParameter(xCommand, "@value", xValue ?? DBNull.Value);
                AddScalarParameter(xCommand, "@key", dNewRow[sKeyField]);
                xCommand.ExecuteNonQuery();
                xChanged = true;
            }
        }

        return xChanged;
    }

    private List<Dictionary<string, object?>> Query(IDbCommand xCommand, Action<IDbCommand> xBind)
    {
        var arrData = new List<Dictionary<string, object?>>();
        xBind(xCommand);
        using var xReader = xCommand.ExecuteReader();
        while (xReader.Read())
        {
            var dRow = new Dictionary<string, object?>(StringComparer.Ordinal);
            for (var iIndex = 0; iIndex < xReader.FieldCount; iIndex++)
            {
                if (xReader.IsDBNull(iIndex))
                {
                    dRow[xReader.GetName(iIndex)] = null;
                    continue;
                }

                var xValue = xReader.GetValue(iIndex);
                dRow[xReader.GetName(iIndex)] = xValue switch
                {
                    DateTime dtValue => dtValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                    _ => xValue
                };
            }

            arrData.Add(dRow);
        }

        return arrData;
    }


    private IDbCommand CreateUpdate(string sTable, IEnumerable<KeyValuePair<string, string>> arrFields, IEnumerable<DbFilterClause> arrFilters) 
        => _xStatementRenderer.CreateUpdate(sTable, arrFields, arrFilters);

    private static void AddScalarParameter(IDbCommand xCommand, string sParameterName, object? xValue)
    {
        var xParameter = xCommand.CreateParameter();
        xParameter.ParameterName = sParameterName;
        xParameter.Value = xValue ?? DBNull.Value;
        _ = xCommand.Parameters.Add(xParameter);
    }

    private static long GetLastInsertedId(IDbCommand xCommand)
    {
        var xProperty = xCommand.GetType().GetProperty("LastInsertedId");
        if (xProperty?.GetValue(xCommand) is long iValue)
        {
            return iValue;
        }

        if (xProperty?.GetValue(xCommand) is int iValue32)
        {
            return iValue32;
        }

        return 0L;
    }
}
