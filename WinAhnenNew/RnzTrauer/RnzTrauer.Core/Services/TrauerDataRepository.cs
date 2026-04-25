using System.Globalization;
using MySqlConnector;

namespace RnzTrauer.Core;

/// <summary>
/// Encapsulates the MySQL access required for RNZ obituary persistence.
/// </summary>
public sealed class TrauerDataRepository : ITrauerDataRepository
{
    private const int DefaultMySqlPort = 3306;

    private const string TableAnzeigen = "Anzeigen";
    private const string TableTrauerfall = "Trauerfall";
    private const string LegacyAnnouncementTable = "`RNZ-Traueranzeigen`.`Anzeigen`";

    private const string SqlSelectAnnouncementById = "SELECT * FROM Anzeigen WHERE idAnzeige=@id";
    private const string SqlSelectAnnouncementByAnnouncement = "SELECT * FROM Anzeigen WHERE Announcement=@announcement";
    private const string SqlSelectLegacyAnnouncementByAuftrag = "SELECT * FROM `RNZ-Traueranzeigen`.`Anzeigen` WHERE Auftrag=@auftrag";
    private const string SqlSelectTrauerfallById = "SELECT * FROM Trauerfall WHERE idTrauerfall=@id";
    private const string SqlSelectTrauerfallByUrl = "SELECT idTrauerfall, url FROM Trauerfall WHERE url=@url";
    private const string SqlSelectTrauerfallIndex = "SELECT idTrauerfall,url FROM Trauerfall";
    private const string SqlInsertTrauerfall = "INSERT INTO `Trauerfall` (`URL`, `Created`, `Preread_Birth`, `Preread_Death`, `Fullname`, `Firstname`, `Lastname`, `Birthname`, `Place`, `Created_by`) VALUES (@url, @created, @birth, @death, @fullName, @firstName, @lastName, @birthName, @place, @createdBy);";
    private const string SqlInsertAnnouncement = "INSERT INTO `Anzeigen` (`idTrauerfall`, `url`, `Announcement`, `release`,`localpath`, `pngFile`, `pdfFile`, `Additional`, `Firstname`,`Lastname`, `Birthname`, `Birth`, `Death`, `Place`, `Info`, `ProfileImg`, `Rubrik`) VALUES (@idtf, @url, @announcement, @release, @localpath, @pngFile, @pdfFile, @additional, @firstName, @lastName, @birthName, @birth, @death, @place, @info, @profileImg, @rubrik);";
    private const string SqlInsertLegacyAnnouncement = "INSERT INTO `RNZ-Traueranzeigen`.`Anzeigen` (`Auftrag`, `url`, `Announcement`, `release`,`localpath`, `pngFile`, `pdfFile`, `Additional`) VALUES (@auftrag, @url, @announcement, @release, @localpath, @pngFile, @pdfFile, @additional);";

    private readonly MySqlConnection _dbConn;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrauerDataRepository"/> class.
    /// </summary>
    public TrauerDataRepository(DatabaseSettings xSettings)
    {
        ArgumentNullException.ThrowIfNull(xSettings);

        var sConnectionString = new MySqlConnectionStringBuilder
        {
            Server = xSettings.DBhost,
            Port = DefaultMySqlPort,
            UserID = xSettings.DBuser,
            Password = xSettings.DBpass,
            Database = xSettings.DB,
            AllowUserVariables = true,
            ConvertZeroDateTime = true
        }.ConnectionString;

        _dbConn = new MySqlConnection(sConnectionString);
        _dbConn.Open();
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerAnzId(int iId)
    {
        return Query(SqlSelectAnnouncementById, xCommand => xCommand.Parameters.AddWithValue("@id", iId));
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerAnz(int iAnnouncement)
    {
        return Query(SqlSelectAnnouncementByAnnouncement, xCommand => xCommand.Parameters.AddWithValue("@announcement", iAnnouncement));
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> LegacyTrauerAnz(string sAuftrag)
    {
        return Query(SqlSelectLegacyAnnouncementByAuftrag, xCommand => xCommand.Parameters.AddWithValue("@auftrag", sAuftrag));
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerAnzIsNull(string sField, int iLimit = 1)
    {
        return Query($"SELECT * FROM `{TableAnzeigen}` WHERE `{sField}` is null limit @limit", xCommand => xCommand.Parameters.AddWithValue("@limit", iLimit));
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerFallIsNull(string sField, int iLimit = 1)
    {
        return Query($"SELECT * FROM `{TableTrauerfall}` WHERE `{sField}` is null limit @limit", xCommand => xCommand.Parameters.AddWithValue("@limit", iLimit));
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerFallEquals(string sField, string sValue, int iLimit = 1)
    {
        return Query($"SELECT * FROM `{TableTrauerfall}` WHERE `{sField}`=@value limit @limit", xCommand =>
        {
            xCommand.Parameters.AddWithValue("@value", sValue);
            xCommand.Parameters.AddWithValue("@limit", iLimit);
        });
    }

    /// <inheritdoc />
    public void UpdateTrauerFall(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues)
    {
        _ = UpdateRows(TableTrauerfall, arrNewValues, arrOldValues);
    }

    /// <inheritdoc />
    public bool UpdateTrauerAnz(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues)
    {
        return UpdateRows(TableAnzeigen, arrNewValues, arrOldValues);
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerFallById(int iId)
    {
        return Query(SqlSelectTrauerfallById, xCommand => xCommand.Parameters.AddWithValue("@id", iId));
    }

    /// <inheritdoc />
    public List<Dictionary<string, object?>> TrauerFallByUrl(string sUrl)
    {
        return Query(SqlSelectTrauerfallByUrl, xCommand => xCommand.Parameters.AddWithValue("@url", sUrl));
    }

    /// <inheritdoc />
    public Dictionary<string, long> BuildTrauerFallIndex()
    {
        var dIndex = new Dictionary<string, long>(StringComparer.Ordinal);
        using var xCommand = new MySqlCommand(SqlSelectTrauerfallIndex, _dbConn);
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
        using var xCommand = new MySqlCommand(SqlInsertTrauerfall, _dbConn);
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
        return xCommand.LastInsertedId;
    }

    /// <inheritdoc />
    public long AppendTrauerAnz(IReadOnlyDictionary<string, object?> dValues)
    {
        using var xCommand = new MySqlCommand(SqlInsertAnnouncement, _dbConn);
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
        return xCommand.LastInsertedId;
    }

    /// <inheritdoc />
    public long AppendLegacyTAnz(IReadOnlyDictionary<string, object?> dValues)
    {
        using var xCommand = new MySqlCommand(SqlInsertLegacyAnnouncement, _dbConn);
        AddParameter(xCommand, "@auftrag", dValues, "Auftrag");
        AddParameter(xCommand, "@url", dValues, "url");
        AddParameter(xCommand, "@announcement", dValues, "Announcement");
        AddParameter(xCommand, "@release", dValues, "release");
        AddParameter(xCommand, "@localpath", dValues, "localpath");
        AddParameter(xCommand, "@pngFile", dValues, "pngFile");
        AddParameter(xCommand, "@pdfFile", dValues, "pdfFile");
        AddParameter(xCommand, "@additional", dValues, "Additional");
        xCommand.ExecuteNonQuery();
        return xCommand.LastInsertedId;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _dbConn.Dispose();
    }

    private static void AddParameter(MySqlCommand xCommand, string sParameterName, IReadOnlyDictionary<string, object?> dValues, string sKey)
    {
        _ = xCommand.Parameters.AddWithValue(
            sParameterName,
            dValues.TryGetValue(sKey, out var xValue) ? xValue ?? DBNull.Value : DBNull.Value);
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

                using var xCommand = new MySqlCommand($"UPDATE `{sTable}` SET `{sColumn}`=@value WHERE `{sKeyField}`=@key", _dbConn);
                xCommand.Parameters.AddWithValue("@value", xValue ?? DBNull.Value);
                xCommand.Parameters.AddWithValue("@key", dNewRow[sKeyField]);
                xCommand.ExecuteNonQuery();
                xChanged = true;
            }
        }

        return xChanged;
    }

    private List<Dictionary<string, object?>> Query(string sSql, Action<MySqlCommand> xBind)
    {
        var arrData = new List<Dictionary<string, object?>>();
        using var xCommand = new MySqlCommand(sSql, _dbConn);
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
}
