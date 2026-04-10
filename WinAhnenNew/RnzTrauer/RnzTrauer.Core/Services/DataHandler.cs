using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using MySqlConnector;

namespace RnzTrauer.Core;

/// <summary>
/// Provides database access and extraction helpers for the RNZ obituary data.
/// </summary>
public sealed class DataHandler : IDisposable
{
    private readonly MySqlConnection _dbConn;
    private readonly IFile _xFile;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataHandler"/> class.
    /// </summary>
    public DataHandler(DatabaseSettings xSettings, IFile xFile)
    {
        _xFile = xFile ?? throw new ArgumentNullException(nameof(xFile));

        var sConnectionString = new MySqlConnectionStringBuilder
        {
            Server = xSettings.DBhost,
            Port = 3306,
            UserID = xSettings.DBuser,
            Password = xSettings.DBpass,
            Database = xSettings.DB,
            AllowUserVariables = true
        }.ConnectionString;

        _dbConn = new MySqlConnection(sConnectionString);
        _dbConn.Open();
    }

    /// <summary>
    /// Gets the in-memory obituary case index.
    /// </summary>
    public Dictionary<string, long> TfIdx { get; private set; } = new(StringComparer.Ordinal);

    /// <summary>
    /// Extracts obituary dictionaries from the stored page JSON structure.
    /// </summary>
    public List<Dictionary<string, object?>> ExtractTrauerData(JsonNode? xData, string sLocalPathRoot)
    {
        var arrResult = new List<Dictionary<string, object?>>();
        if (xData is not JsonObject xRoot || xRoot["sections"] is not JsonArray arrSections)
        {
            return arrResult;
        }

        foreach (var xSectionNode in arrSections.OfType<JsonObject>())
        {
            var arrTexts = xSectionNode["text"] as JsonArray;
            if (arrTexts is null || arrTexts.Count == 0 || !(arrTexts[0]?.ToString() ?? string.Empty).StartsWith("ANZ", StringComparison.Ordinal))
            {
                continue;
            }

            JsonObject xTrauerfallData = new();
            if (xSectionNode["links"] is JsonArray arrLinks && arrLinks.Count > 0 && arrLinks[0] is JsonObject xLink0)
            {
                var sLinkHref = xLink0["href"]?.ToString() ?? string.Empty;
                if (xRoot[$"{sLinkHref}/anzeigen"] is JsonObject xInline)
                {
                    xTrauerfallData = xInline;
                    Console.Write('.');
                }
                else
                {
                    var sLocalPath = PortedHelpers.GetLocalPath(sLinkHref, sLocalPathRoot);
                    var sFullName = Path.HasExtension(sLocalPath) ? sLocalPath : sLocalPath + ".json";
                    if (_xFile.Exists(sFullName))
                    {
                        xTrauerfallData = JsonNode.Parse(_xFile.ReadAllText(sFullName)) as JsonObject ?? new JsonObject();
                        Console.Write(',');
                    }
                    else
                    {
                        Console.Write('-');
                    }
                }
            }

            if (xTrauerfallData["sections"] is not JsonArray arrTrauerfallSections)
            {
                continue;
            }

            var sProfileImagePath = string.Empty;
            foreach (var xAnnouncementNode in arrTrauerfallSections.OfType<JsonObject>())
            {
                var sCssClass = xAnnouncementNode["class"]?.ToString() ?? string.Empty;
                if (sCssClass.StartsWith("col-12", StringComparison.Ordinal))
                {
                    try
                    {
                        if (xAnnouncementNode["imgs"] is JsonArray arrImages && arrImages.Count > 0 && arrImages[0] is JsonObject xImage0)
                        {
                            var sSource = xImage0["src"]?.ToString() ?? string.Empty;
                            if (sSource.Contains("MEDIA", StringComparison.Ordinal))
                            {
                                sProfileImagePath = PortedHelpers.GetLocalPath(sSource.LCropStr("?"), sLocalPathRoot);
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                if (!sCssClass.StartsWith("container", StringComparison.Ordinal))
                {
                    continue;
                }

                var dTrauerfall = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    ["profImg"] = sProfileImagePath,
                    ["filter"] = xAnnouncementNode["filter"]?.ToString() ?? string.Empty
                };

                var sId = xAnnouncementNode["id"]?.ToString() ?? string.Empty;
                var arrSplit = sId.Split('_', 2);
                dTrauerfall["id"] = arrSplit.Length > 1 ? arrSplit[1] : string.Empty;

                if (xAnnouncementNode["text"] is JsonArray arrAnnouncementText && arrAnnouncementText.Count > 1)
                {
                    var sPublish = arrAnnouncementText[1]?.ToString() ?? string.Empty;
                    if (sPublish.StartsWith("vom", StringComparison.Ordinal))
                    {
                        dTrauerfall["publish"] = sPublish.Length > 4 ? sPublish[4..] : string.Empty;
                    }
                }

                if (xTrauerfallData["name"] is not null)
                {
                    dTrauerfall["name"] = xTrauerfallData["name"]!.ToString();
                }

                if (arrTrauerfallSections.Count > 0 && arrTrauerfallSections[0] is JsonObject xFirstSection && xFirstSection["text"] is JsonArray arrFirstText && arrFirstText.Count > 1)
                {
                    var sBirthName = arrFirstText[1]?.ToString() ?? string.Empty;
                    if (sBirthName.StartsWith("geb.", StringComparison.Ordinal))
                    {
                        dTrauerfall["Birthname"] = sBirthName.Length > 5 ? sBirthName[5..] : string.Empty;
                    }
                }

                foreach (var sKey in new[] { "url", "Birth", "Death", "Place", "created_by", "created_on", "visits" })
                {
                    if (xTrauerfallData[sKey] is not null)
                    {
                        dTrauerfall[sKey] = xTrauerfallData[sKey]!.ToString();
                    }
                }

                if (xAnnouncementNode["links"] is JsonArray arrAnnouncementLinks && arrAnnouncementLinks.Count >= 3)
                {
                    var sImage = arrAnnouncementLinks[1]?["href"]?.ToString() ?? string.Empty;
                    dTrauerfall["img"] = PortedHelpers.GetLocalPath(sImage, sLocalPathRoot);
                    var sPdf = arrAnnouncementLinks[2]?["href"]?.ToString() ?? string.Empty;
                    dTrauerfall["pdf"] = PortedHelpers.GetLocalPath(sPdf, sLocalPathRoot);
                    if (xTrauerfallData[sPdf] is JsonObject xPdfObject && xPdfObject["pdfText"] is not null)
                    {
                        dTrauerfall["pdfText"] = xPdfObject["pdfText"]!.ToString();
                    }
                    else
                    {
                        var sPdfFile = dTrauerfall["pdf"]?.ToString() ?? string.Empty;
                        if (_xFile.Exists(sPdfFile))
                        {
                            dTrauerfall["pdfText"] = PortedHelpers.PdfText(_xFile.ReadAllBytes(sPdfFile));
                        }
                    }
                }

                arrResult.Add(dTrauerfall);
            }
        }

        return arrResult;
    }

    /// <summary>
    /// Reads obituary rows by internal id.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerAnzId(int iId)
    {
        return Query("SELECT * FROM Anzeigen WHERE idAnzeige=@id", xCommand => xCommand.Parameters.AddWithValue("@id", iId));
    }

    /// <summary>
    /// Reads obituary rows by announcement id.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerAnz(int iAnnouncement)
    {
        return Query("SELECT * FROM Anzeigen WHERE Announcement=@announcement", xCommand => xCommand.Parameters.AddWithValue("@announcement", iAnnouncement));
    }

    /// <summary>
    /// Reads legacy obituary rows by legacy order id.
    /// </summary>
    public List<Dictionary<string, object?>> LegacyTrauerAnz(string sAuftrag)
    {
        return Query("SELECT * FROM `RNZ-Traueranzeigen`.`Anzeigen` WHERE Auftrag=@auftrag", xCommand => xCommand.Parameters.AddWithValue("@auftrag", sAuftrag));
    }

    /// <summary>
    /// Reads obituary rows where the specified field is null.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerAnzIsNull(string sField, int iLimit = 1)
    {
        return Query($"SELECT * FROM `Anzeigen` WHERE `{sField}` is null limit @limit", xCommand => xCommand.Parameters.AddWithValue("@limit", iLimit));
    }

    /// <summary>
    /// Reads obituary case rows where the specified field is null.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerFallIsNull(string sField, int iLimit = 1)
    {
        return Query($"SELECT * FROM `Trauerfall` WHERE `{sField}` is null limit @limit", xCommand => xCommand.Parameters.AddWithValue("@limit", iLimit));
    }

    /// <summary>
    /// Reads obituary case rows where the specified field matches the provided value.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerFallEquals(string sField, string sValue, int iLimit = 1)
    {
        return Query($"SELECT * FROM `Trauerfall` WHERE `{sField}`=@value limit @limit", xCommand =>
        {
            xCommand.Parameters.AddWithValue("@value", sValue);
            xCommand.Parameters.AddWithValue("@limit", iLimit);
        });
    }

    /// <summary>
    /// Updates obituary case rows when values have changed.
    /// </summary>
    public void UpdateTrauerFall(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues)
    {
        UpdateRows("Trauerfall", arrNewValues, arrOldValues);
    }

    /// <summary>
    /// Updates obituary announcement rows when values have changed.
    /// </summary>
    public bool UpdateTrauerAnz(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues)
    {
        return UpdateRows("Anzeigen", arrNewValues, arrOldValues);
    }

    /// <summary>
    /// Reads obituary case rows by internal id.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerFallById(int iId)
    {
        return Query("SELECT * FROM Trauerfall WHERE idTrauerfall=@id", xCommand => xCommand.Parameters.AddWithValue("@id", iId));
    }

    /// <summary>
    /// Reads obituary case rows by URL.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerFallByUrl(string sUrl)
    {
        return Query("SELECT * FROM Trauerfall WHERE url=@url", xCommand => xCommand.Parameters.AddWithValue("@url", sUrl));
    }

    /// <summary>
    /// Builds an in-memory index of obituary case URLs.
    /// </summary>
    public void BuildTrauerFallIndex()
    {
        TfIdx = new Dictionary<string, long>(StringComparer.Ordinal);
        using var xCommand = new MySqlCommand("SELECT idTrauerfall,url FROM Trauerfall", _dbConn);
        using var xReader = xCommand.ExecuteReader();
        while (xReader.Read())
        {
            TfIdx[xReader.GetString(1)] = xReader.GetInt64(0);
        }
    }

    /// <summary>
    /// Inserts a new obituary case row.
    /// </summary>
    public long AppendTrauerFall(Dictionary<string, object?> dTrauerfall)
    {
        var (sLastName, sFirstName) = dTrauerfall.Cond("name").SplitName();
        using var xCommand = new MySqlCommand(
            "INSERT INTO `Trauerfall` (`URL`, `Created`, `Preread_Birth`, `Preread_Death`, `Fullname`, `Firstname`, `Lastname`, `Birthname`, `Place`, `Created_by`) VALUES (@url, @created, @birth, @death, @fullName, @firstName, @lastName, @birthName, @place, @createdBy);",
            _dbConn);
        xCommand.Parameters.AddWithValue("@url", dTrauerfall.Cond("url"));
        xCommand.Parameters.AddWithValue("@created", ToDbValue(PortedHelpers.Str2Date(dTrauerfall.Cond("created_on"))));
        xCommand.Parameters.AddWithValue("@birth", ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(dTrauerfall.Cond("Birth")))));
        xCommand.Parameters.AddWithValue("@death", ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(dTrauerfall.Cond("Death")))));
        xCommand.Parameters.AddWithValue("@fullName", $"{sLastName}, {sFirstName}");
        xCommand.Parameters.AddWithValue("@firstName", sFirstName);
        xCommand.Parameters.AddWithValue("@lastName", sLastName);
        xCommand.Parameters.AddWithValue("@birthName", dTrauerfall.Cond("Birthname"));
        xCommand.Parameters.AddWithValue("@place", dTrauerfall.Cond("Place"));
        xCommand.Parameters.AddWithValue("@createdBy", dTrauerfall.Cond("created_by"));
        xCommand.ExecuteNonQuery();
        return xCommand.LastInsertedId;
    }

    /// <summary>
    /// Inserts a new obituary announcement row.
    /// </summary>
    public long AppendTrauerAnz(long iTrauerfallId, Dictionary<string, object?> dTrauerfall, string sLocalPath)
    {
        var sPath = Directory.GetParent(dTrauerfall.Cond("img"))?.FullName ?? string.Empty;
        var sNormalizedLocalPath = sPath.Replace(sLocalPath, string.Empty, StringComparison.Ordinal);
        var sProfileBase = Directory.GetParent(Directory.GetParent(sPath)?.FullName ?? string.Empty)?.FullName ?? string.Empty;
        var sProfileImage = dTrauerfall.Cond("profImg").Replace(sProfileBase, "..\\..", StringComparison.Ordinal);
        var iRubrik = GetRubrik(dTrauerfall);
        var (sLastName, sFirstName) = dTrauerfall.Cond("name").SplitName();

        using var xCommand = new MySqlCommand(
            "INSERT INTO `Anzeigen` (`idTrauerfall`, `url`, `Announcement`, `release`,`localpath`, `pngFile`, `pdfFile`, `Additional`, `Firstname`,`Lastname`, `Birthname`, `Birth`, `Death`, `Place`, `Info`, `ProfileImg`, `Rubrik`) VALUES (@idtf, @url, @announcement, @release, @localpath, @pngFile, @pdfFile, @additional, @firstName, @lastName, @birthName, @birth, @death, @place, @info, @profileImg, @rubrik);",
            _dbConn);
        xCommand.Parameters.AddWithValue("@idtf", iTrauerfallId);
        xCommand.Parameters.AddWithValue("@url", dTrauerfall.Cond("url"));
        xCommand.Parameters.AddWithValue("@announcement", int.TryParse(dTrauerfall.Cond("id"), out var iId) ? iId : 0);
        xCommand.Parameters.AddWithValue("@release", ToDbValue(PortedHelpers.Str2Date(dTrauerfall.Cond("publish"))));
        xCommand.Parameters.AddWithValue("@localpath", sNormalizedLocalPath);
        xCommand.Parameters.AddWithValue("@pngFile", Path.GetFileName(dTrauerfall.Cond("img")));
        xCommand.Parameters.AddWithValue("@pdfFile", Path.GetFileName(dTrauerfall.Cond("pdf")));
        xCommand.Parameters.AddWithValue("@additional", JsonSerializer.Serialize(dTrauerfall, PortedHelpers.JsonOptions));
        xCommand.Parameters.AddWithValue("@firstName", sFirstName);
        xCommand.Parameters.AddWithValue("@lastName", sLastName);
        xCommand.Parameters.AddWithValue("@birthName", dTrauerfall.Cond("Birthname"));
        xCommand.Parameters.AddWithValue("@birth", ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(dTrauerfall.Cond("Birth")))));
        xCommand.Parameters.AddWithValue("@death", ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(dTrauerfall.Cond("Death")))));
        xCommand.Parameters.AddWithValue("@place", dTrauerfall.Cond("Place"));
        xCommand.Parameters.AddWithValue("@info", dTrauerfall.Cond("pdfText"));
        xCommand.Parameters.AddWithValue("@profileImg", sProfileImage);
        xCommand.Parameters.AddWithValue("@rubrik", iRubrik);
        xCommand.ExecuteNonQuery();
        return xCommand.LastInsertedId;
    }

    /// <summary>
    /// Applies announcement values to the current row dictionary.
    /// </summary>
    public void SetTrauerAnz(Dictionary<string, object?> dCurrent, Dictionary<string, object?> dTrauerfall, string sLocalPath)
    {
        var sPath = Directory.GetParent(PortedHelpers.Cond(dTrauerfall, "img"))?.FullName ?? string.Empty;
        var sNormalizedLocalPath = sPath.Replace(sLocalPath, string.Empty, StringComparison.Ordinal);
        var sProfileBase = Directory.GetParent(Directory.GetParent(sPath)?.FullName ?? string.Empty)?.FullName ?? string.Empty;
        var sProfileImage = PortedHelpers.Cond(dTrauerfall, "profImg").Replace(sProfileBase, "..\\..", StringComparison.Ordinal);
        var iRubrik = dCurrent.TryGetValue("Rubrik", out var xCurrentRubrik) && int.TryParse(Convert.ToString(xCurrentRubrik, CultureInfo.InvariantCulture), out var iParsed) ? iParsed : 8050;
        var sFilter = PortedHelpers.Cond(dTrauerfall, "filter");
        if (sFilter == "danksagungen")
        {
            iRubrik = 8060;
        }
        else if (sFilter == "nachrufe")
        {
            iRubrik = 8070;
        }
        else if (sFilter == "todesanzeigen")
        {
            iRubrik = 8050;
        }
        else
        {
            try
            {
                var xJson = JsonNode.Parse(Convert.ToString(dCurrent.GetValueOrDefault("Additional"), CultureInfo.InvariantCulture) ?? string.Empty) as JsonObject;
                if (xJson?["filter"] is not null)
                {
                    dTrauerfall["filter"] = xJson["filter"]!.ToString();
                }
            }
            catch
            {
            }
        }

        var (sLastName, sFirstName) = PortedHelpers.Cond(dTrauerfall, "name").SplitName();
        foreach (var kvPair in new Dictionary<string, object?>
        {
            ["url"] = PortedHelpers.Cond(dTrauerfall, "url"),
            ["Announcement"] = int.TryParse(PortedHelpers.Cond(dTrauerfall, "id"), out var iId) ? iId : 0,
            ["release"] = ToDbValue(PortedHelpers.Str2Date(PortedHelpers.Cond(dTrauerfall, "publish"))),
            ["localpath"] = sNormalizedLocalPath,
            ["pngFile"] = Path.GetFileName(PortedHelpers.Cond(dTrauerfall, "img")),
            ["pdfFile"] = Path.GetFileName(PortedHelpers.Cond(dTrauerfall, "pdf")),
            ["Additional"] = JsonSerializer.Serialize(dTrauerfall, PortedHelpers.JsonOptions),
            ["Firstname"] = sFirstName,
            ["Lastname"] = sLastName,
            ["Birthname"] = PortedHelpers.Cond(dTrauerfall, "Birthname"),
            ["Birth"] = ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(PortedHelpers.Cond(dTrauerfall, "Birth")))),
            ["Death"] = ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(PortedHelpers.Cond(dTrauerfall, "Death")))),
            ["Place"] = PortedHelpers.Cond(dTrauerfall, "Place"),
            ["Info"] = PortedHelpers.Cond(dTrauerfall, "pdfText"),
            ["ProfileImg"] = sProfileImage,
            ["Rubrik"] = iRubrik
        })
        {
            dCurrent[kvPair.Key] = kvPair.Value;
        }
    }

    /// <summary>
    /// Inserts a legacy obituary announcement row.
    /// </summary>
    public long AppendLegacyTAnz(string sAuftrag, Dictionary<string, object?> dTrauerfall, string sLocalPath)
    {
        var sNormalizedLocalPath = Directory.GetParent(PortedHelpers.Cond(dTrauerfall, "pdf"))?.FullName?.Replace(sLocalPath, string.Empty, StringComparison.Ordinal) ?? string.Empty;
        using var xCommand = new MySqlCommand(
            "INSERT INTO `RNZ-Traueranzeigen`.`Anzeigen` (`Auftrag`, `url`, `Announcement`, `release`,`localpath`, `pngFile`, `pdfFile`, `Additional`) VALUES (@auftrag, @url, @announcement, @release, @localpath, @pngFile, @pdfFile, @additional);",
            _dbConn);
        xCommand.Parameters.AddWithValue("@auftrag", sAuftrag);
        xCommand.Parameters.AddWithValue("@url", PortedHelpers.Cond(dTrauerfall, "url"));
        xCommand.Parameters.AddWithValue("@announcement", int.TryParse(PortedHelpers.Cond(dTrauerfall, "id"), out var iId) ? iId : 0);
        xCommand.Parameters.AddWithValue("@release", ToDbValue(PortedHelpers.Str2Date(PortedHelpers.Cond(dTrauerfall, "publish"))));
        xCommand.Parameters.AddWithValue("@localpath", sNormalizedLocalPath);
        xCommand.Parameters.AddWithValue("@pngFile", Path.GetFileName(PortedHelpers.Cond(dTrauerfall, "img")));
        xCommand.Parameters.AddWithValue("@pdfFile", Path.GetFileName(PortedHelpers.Cond(dTrauerfall, "pdf")));
        xCommand.Parameters.AddWithValue("@additional", JsonSerializer.Serialize(dTrauerfall, PortedHelpers.JsonOptions));
        xCommand.ExecuteNonQuery();
        return xCommand.LastInsertedId;
    }

    /// <summary>
    /// Imports extracted obituary data into the database.
    /// </summary>
    public void TrauerDataToDb(IEnumerable<Dictionary<string, object?>> arrData, string sLocalPath)
    {
        foreach (var dAnnouncement in arrData)
        {
            var arrCurrentCases = TrauerFallByUrl(PortedHelpers.Cond(dAnnouncement, "url"));
            var iTrauerfallId = arrCurrentCases.Count == 0
                ? AppendTrauerFall(dAnnouncement)
                : Convert.ToInt64(arrCurrentCases[0].Values.First(), CultureInfo.InvariantCulture);

            var arrCurrentAnnouncements = TrauerAnz(int.TryParse(PortedHelpers.Cond(dAnnouncement, "id"), out var iId) ? iId : 0);
            if (arrCurrentAnnouncements.Count == 0)
            {
                Console.Write('+');
                AppendTrauerAnz(iTrauerfallId, dAnnouncement, sLocalPath);
            }
            else
            {
                Console.Write('-');
                var arrCopy = arrCurrentAnnouncements.Select(d => new Dictionary<string, object?>(d, StringComparer.Ordinal)).ToList();
                SetTrauerAnz(arrCopy[0], dAnnouncement, sLocalPath);
                if (UpdateTrauerAnz(arrCopy, arrCurrentAnnouncements))
                {
                    Console.Write("\bx");
                }
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _dbConn.Dispose();
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
                dRow[xReader.GetName(iIndex)] = xReader.IsDBNull(iIndex)
                    ? null
                    : xReader.GetValue(iIndex) switch
                    {
                        DateTime dtValue => dtValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        _ => xReader.GetValue(iIndex)
                    };
            }

            arrData.Add(dRow);
        }

        return arrData;
    }

    private static object ToDbValue(DateOnly? dtValue)
    {
        return dtValue.HasValue
            ? dtValue.Value.ToDateTime(TimeOnly.MinValue)
            : DBNull.Value;
    }

    private static string TrimLeadingTwo(string sValue)
    {
        return sValue.Length > 2 ? sValue[2..] : string.Empty;
    }

    private static int GetRubrik(IReadOnlyDictionary<string, object?> dTrauerfall)
    {
        return PortedHelpers.Cond(dTrauerfall, "filter") switch
        {
            "danksagungen" => 8060,
            "nachrufe" => 8070,
            _ => 8050
        };
    }
}
