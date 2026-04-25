using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace RnzTrauer.Core;

/// <summary>
/// Extracts RNZ obituary data from cached JSON payloads and coordinates persistence through a repository abstraction.
/// </summary>
public sealed class DataHandler : IDisposable
{
    private const string JsonSections = "sections";
    private const string JsonText = "text";
    private const string JsonLinks = "links";
    private const string JsonHref = "href";
    private const string JsonClass = "class";
    private const string JsonImages = "imgs";
    private const string JsonSource = "src";
    private const string JsonFilter = "filter";
    private const string JsonId = "id";
    private const string JsonName = "name";
    private const string JsonPdfText = "pdfText";

    private const string KeyAdditional = "Additional";
    private const string KeyAnnouncement = "Announcement";
    private const string KeyBirth = "Birth";
    private const string KeyBirthName = "Birthname";
    private const string KeyCreated = "Created";
    private const string KeyCreatedBy = "Created_by";
    private const string KeyCreatedBySource = "created_by";
    private const string KeyCreatedOnSource = "created_on";
    private const string KeyDeath = "Death";
    private const string KeyFilter = "filter";
    private const string KeyFirstName = "Firstname";
    private const string KeyFullName = "Fullname";
    private const string KeyIdTrauerfall = "idTrauerfall";
    private const string KeyId = "id";
    private const string KeyImage = "img";
    private const string KeyInfo = "Info";
    private const string KeyLastName = "Lastname";
    private const string KeyLocalPath = "localpath";
    private const string KeyPdf = "pdf";
    private const string KeyPdfFile = "pdfFile";
    private const string KeyPdfText = "pdfText";
    private const string KeyPlace = "Place";
    private const string KeyPngFile = "pngFile";
    private const string KeyPrereadBirth = "Preread_Birth";
    private const string KeyPrereadDeath = "Preread_Death";
    private const string KeyProfileImage = "ProfileImg";
    private const string KeyProfileImageSource = "profImg";
    private const string KeyPublish = "publish";
    private const string KeyRelease = "release";
    private const string KeyRubrik = "Rubrik";
    private const string KeyUrl = "url";
    private const string KeyUrlUpper = "URL";
    private const string KeyVisits = "visits";

    private const string AnnouncementPrefix = "ANZ";
    private const string BirthNamePrefix = "geb.";
    private const string ContainerCssPrefix = "container";
    private const string ImageColumnCssPrefix = "col-12";
    private const string InlineAnnouncementSuffix = "/anzeigen";
    private const string JsonExtension = ".json";
    private const string MediaMarker = "MEDIA";
    private const string ProfileImageRelativeRoot = "..\\..";
    private const string PublishPrefix = "vom";
    private const string QuerySeparator = "?";
    private const string StatusInlineData = ".";
    private const string StatusCachedData = ",";
    private const string StatusMissingData = "-";
    private const string StatusInserted = "+";
    private const string StatusExisting = "-";
    private const string StatusUpdated = "\bx";
    private const string MessageSkipIncompleteAnnouncement = "Skip incomplete announcement (missing created/publish): ";

    private const string FilterDanksagungen = "danksagungen";
    private const string FilterNachrufe = "nachrufe";
    private const string FilterTodesanzeigen = "todesanzeigen";

    private const int RubrikTodesanzeigen = 8050;
    private const int RubrikDanksagungen = 8060;
    private const int RubrikNachrufe = 8070;

    private static readonly string[] TrauerfallSourceKeys =
    [
        KeyUrl,
        KeyBirth,
        KeyDeath,
        KeyPlace,
        KeyCreatedBySource,
        KeyCreatedOnSource,
        KeyVisits
    ];

    private readonly IFile _xFile;
    private readonly ITrauerDataRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataHandler"/> class.
    /// </summary>
    public DataHandler(DatabaseSettings xSettings, IFile xFile)
        : this(new TrauerDataRepository(xSettings), xFile)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataHandler"/> class with an injected repository.
    /// </summary>
    public DataHandler(ITrauerDataRepository xRepository, IFile xFile)
    {
        _repository = xRepository ?? throw new ArgumentNullException(nameof(xRepository));
        _xFile = xFile ?? throw new ArgumentNullException(nameof(xFile));
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
        if (xData is not JsonObject xRoot || xRoot[JsonSections] is not JsonArray arrSections)
        {
            return arrResult;
        }

        foreach (var xSectionNode in arrSections.OfType<JsonObject>())
        {
            if (!IsAnnouncementSection(xSectionNode))
            {
                continue;
            }

            var xTrauerfallData = LoadTrauerfallData(xRoot, xSectionNode, sLocalPathRoot);
            if (xTrauerfallData[JsonSections] is not JsonArray arrTrauerfallSections)
            {
                continue;
            }

            var sProfileImagePath = string.Empty;
            foreach (var xAnnouncementNode in arrTrauerfallSections.OfType<JsonObject>())
            {
                TryUpdateProfileImagePath(xAnnouncementNode, sLocalPathRoot, ref sProfileImagePath);
                if (!StartsWithValue(xAnnouncementNode, JsonClass, ContainerCssPrefix))
                {
                    continue;
                }

                arrResult.Add(CreateTrauerfallEntry(xAnnouncementNode, xTrauerfallData, arrTrauerfallSections, sProfileImagePath, sLocalPathRoot));
            }
        }

        return arrResult;
    }

    /// <summary>
    /// Reads obituary rows by internal id.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerAnzId(int iId)
    {
        return _repository.TrauerAnzId(iId);
    }

    /// <summary>
    /// Reads obituary rows by announcement id.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerAnz(int iAnnouncement)
    {
        return _repository.TrauerAnz(iAnnouncement);
    }

    /// <summary>
    /// Reads legacy obituary rows by legacy order id.
    /// </summary>
    public List<Dictionary<string, object?>> LegacyTrauerAnz(string sAuftrag)
    {
        return _repository.LegacyTrauerAnz(sAuftrag);
    }

    /// <summary>
    /// Reads obituary rows where the specified field is null.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerAnzIsNull(string sField, int iLimit = 1)
    {
        return _repository.TrauerAnzIsNull(sField, iLimit);
    }

    /// <summary>
    /// Reads obituary case rows where the specified field is null.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerFallIsNull(string sField, int iLimit = 1)
    {
        return _repository.TrauerFallIsNull(sField, iLimit);
    }

    /// <summary>
    /// Reads obituary case rows where the specified field matches the provided value.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerFallEquals(string sField, string sValue, int iLimit = 1)
    {
        return _repository.TrauerFallEquals(sField, sValue, iLimit);
    }

    /// <summary>
    /// Updates obituary case rows when values have changed.
    /// </summary>
    public void UpdateTrauerFall(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues)
    {
        _repository.UpdateTrauerFall(arrNewValues, arrOldValues);
    }

    /// <summary>
    /// Updates obituary announcement rows when values have changed.
    /// </summary>
    public bool UpdateTrauerAnz(List<Dictionary<string, object?>> arrNewValues, List<Dictionary<string, object?>> arrOldValues)
    {
        return _repository.UpdateTrauerAnz(arrNewValues, arrOldValues);
    }

    /// <summary>
    /// Reads obituary case rows by internal id.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerFallById(int iId)
    {
        return _repository.TrauerFallById(iId);
    }

    /// <summary>
    /// Reads obituary case rows by URL.
    /// </summary>
    public List<Dictionary<string, object?>> TrauerFallByUrl(string sUrl)
    {
        return _repository.TrauerFallByUrl(sUrl);
    }

    /// <summary>
    /// Builds an in-memory index of obituary case URLs.
    /// </summary>
    public void BuildTrauerFallIndex()
    {
        TfIdx = _repository.BuildTrauerFallIndex();
    }

    /// <summary>
    /// Inserts a new obituary case row.
    /// </summary>
    public long AppendTrauerFall(Dictionary<string, object?> dTrauerfall)
    {
        var (sLastName, sFirstName) = dTrauerfall.Cond("name").SplitName();
        var dValues = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            [KeyUrlUpper] = dTrauerfall.Cond(KeyUrl),
            [KeyCreated] = ToDbValue(PortedHelpers.Str2Date(dTrauerfall.Cond(KeyCreatedOnSource))),
            [KeyPrereadBirth] = ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(dTrauerfall.Cond(KeyBirth)))),
            [KeyPrereadDeath] = ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(dTrauerfall.Cond(KeyDeath)))),
            [KeyFullName] = $"{sLastName}, {sFirstName}",
            [KeyFirstName] = sFirstName,
            [KeyLastName] = sLastName,
            [KeyBirthName] = dTrauerfall.Cond(KeyBirthName),
            [KeyPlace] = dTrauerfall.Cond(KeyPlace),
            [KeyCreatedBy] = dTrauerfall.Cond(KeyCreatedBySource)
        };

        return _repository.AppendTrauerFall(dValues);
    }

    /// <summary>
    /// Inserts a new obituary announcement row.
    /// </summary>
    public long AppendTrauerAnz(long iTrauerfallId, Dictionary<string, object?> dTrauerfall, string sLocalPath)
    {
        return _repository.AppendTrauerAnz(CreateAnnouncementRecord(iTrauerfallId, dTrauerfall, sLocalPath, GetRubrik(dTrauerfall)));
    }

    /// <summary>
    /// Applies announcement values to the current row dictionary.
    /// </summary>
    public void SetTrauerAnz(Dictionary<string, object?> dCurrent, Dictionary<string, object?> dTrauerfall, string sLocalPath)
    {
        var dValues = CreateAnnouncementRecord(0, dTrauerfall, sLocalPath, ResolveRubrik(dCurrent, dTrauerfall));
        foreach (var kvPair in dValues.Where(kvPair => !kvPair.Key.Equals(KeyIdTrauerfall, StringComparison.Ordinal)))
        {
            dCurrent[kvPair.Key] = kvPair.Value;
        }
    }

    /// <summary>
    /// Inserts a legacy obituary announcement row.
    /// </summary>
    public long AppendLegacyTAnz(string sAuftrag, Dictionary<string, object?> dTrauerfall, string sLocalPath)
    {
        return _repository.AppendLegacyTAnz(CreateLegacyAnnouncementRecord(sAuftrag, dTrauerfall, sLocalPath));
    }

    /// <summary>
    /// Imports extracted obituary data into the database.
    /// </summary>
    public void TrauerDataToDb(IEnumerable<Dictionary<string, object?>> arrData, string sLocalPath)
    {
        foreach (var dAnnouncement in arrData)
        {
            var dtCreated = PortedHelpers.Str2Date(dAnnouncement.Cond("created_on"));
            var dtPublished = PortedHelpers.Str2Date(dAnnouncement.Cond("publish"));
            if (!dtCreated.HasValue || !dtPublished.HasValue)
            {
                Console.WriteLine($"{MessageSkipIncompleteAnnouncement}{dAnnouncement.Cond(KeyUrl)}");
                continue;
            }

            var arrCurrentCases = TrauerFallByUrl(dAnnouncement.Cond(KeyUrl));
            var iTrauerfallId = arrCurrentCases.Count == 0
                ? AppendTrauerFall(dAnnouncement)
                : Convert.ToInt64(arrCurrentCases[0]["idTrauerfall"], CultureInfo.InvariantCulture);

            var arrCurrentAnnouncements = TrauerAnz(int.TryParse(dAnnouncement.Cond(KeyId), out var iId) ? iId : 0);
            if (arrCurrentAnnouncements.Count == 0)
            {
                Console.Write(StatusInserted);
                AppendTrauerAnz(iTrauerfallId, dAnnouncement, sLocalPath);
            }
            else
            {
                Console.Write(StatusExisting);
                var arrCopy = arrCurrentAnnouncements.Select(d => new Dictionary<string, object?>(d, StringComparer.Ordinal)).ToList();
                SetTrauerAnz(arrCopy[0], dAnnouncement, sLocalPath);
                if (UpdateTrauerAnz(arrCopy, arrCurrentAnnouncements))
                {
                    Console.Write(StatusUpdated);
                }
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _repository.Dispose();
    }

    private Dictionary<string, object?> CreateTrauerfallEntry(JsonObject xAnnouncementNode, JsonObject xTrauerfallData, JsonArray arrTrauerfallSections, string sProfileImagePath, string sLocalPathRoot)
    {
        var dTrauerfall = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            [KeyProfileImageSource] = sProfileImagePath,
            [KeyFilter] = xAnnouncementNode[JsonFilter]?.ToString() ?? string.Empty
        };

        var sId = xAnnouncementNode[JsonId]?.ToString() ?? string.Empty;
        var arrSplit = sId.Split('_', 2);
        dTrauerfall[KeyId] = arrSplit.Length > 1 ? arrSplit[1] : string.Empty;

        if (xAnnouncementNode[JsonText] is JsonArray arrAnnouncementText && arrAnnouncementText.Count > 1)
        {
            var sPublish = arrAnnouncementText[1]?.ToString() ?? string.Empty;
            if (sPublish.StartsWith(PublishPrefix, StringComparison.Ordinal))
            {
                dTrauerfall[KeyPublish] = TrimPrefixedValue(sPublish, PublishPrefix);
            }
        }

        if (xTrauerfallData[JsonName] is not null)
        {
            dTrauerfall[JsonName] = xTrauerfallData[JsonName]!.ToString();
        }

        if (arrTrauerfallSections.Count > 0 && arrTrauerfallSections[0] is JsonObject xFirstSection && xFirstSection[JsonText] is JsonArray arrFirstText && arrFirstText.Count > 1)
        {
            var sBirthName = arrFirstText[1]?.ToString() ?? string.Empty;
            if (sBirthName.StartsWith(BirthNamePrefix, StringComparison.Ordinal))
            {
                dTrauerfall[KeyBirthName] = TrimPrefixedValue(sBirthName, BirthNamePrefix);
            }
        }

        foreach (var sKey in TrauerfallSourceKeys)
        {
            if (xTrauerfallData[sKey] is not null)
            {
                dTrauerfall[sKey] = xTrauerfallData[sKey]!.ToString();
            }
        }

        if (xAnnouncementNode[JsonLinks] is JsonArray arrAnnouncementLinks && arrAnnouncementLinks.Count >= 3)
        {
            var sImage = arrAnnouncementLinks[1]?[JsonHref]?.ToString() ?? string.Empty;
            dTrauerfall[KeyImage] = PortedHelpers.GetLocalPath(sImage, sLocalPathRoot);
            var sPdf = arrAnnouncementLinks[2]?[JsonHref]?.ToString() ?? string.Empty;
            dTrauerfall[KeyPdf] = PortedHelpers.GetLocalPath(sPdf, sLocalPathRoot);
            if (xTrauerfallData[sPdf] is JsonObject xPdfObject && xPdfObject[JsonPdfText] is not null)
            {
                dTrauerfall[KeyPdfText] = xPdfObject[JsonPdfText]!.ToString();
            }
            else
            {
                var sPdfFile = dTrauerfall[KeyPdf]?.ToString() ?? string.Empty;
                if (_xFile.Exists(sPdfFile))
                {
                    dTrauerfall[KeyPdfText] = PortedHelpers.PdfText(_xFile.ReadAllBytes(sPdfFile));
                }
            }
        }

        return dTrauerfall;
    }

    private JsonObject LoadTrauerfallData(JsonObject xRoot, JsonObject xSectionNode, string sLocalPathRoot)
    {
        if (xSectionNode[JsonLinks] is not JsonArray arrLinks || arrLinks.Count == 0 || arrLinks[0] is not JsonObject xLink0)
        {
            return new JsonObject();
        }

        var sLinkHref = xLink0[JsonHref]?.ToString() ?? string.Empty;
        if (xRoot[$"{sLinkHref}{InlineAnnouncementSuffix}"] is JsonObject xInline)
        {
            Console.Write(StatusInlineData);
            return xInline;
        }

        var sLocalPath = PortedHelpers.GetLocalPath(sLinkHref, sLocalPathRoot);
        var sFullName = Path.HasExtension(sLocalPath) ? sLocalPath : sLocalPath + JsonExtension;
        if (_xFile.Exists(sFullName))
        {
            Console.Write(StatusCachedData);
            return JsonNode.Parse(_xFile.ReadAllText(sFullName)) as JsonObject ?? new JsonObject();
        }

        Console.Write(StatusMissingData);
        return new JsonObject();
    }

    private static Dictionary<string, object?> CreateAnnouncementRecord(long iTrauerfallId, IReadOnlyDictionary<string, object?> dTrauerfall, string sLocalPath, int iRubrik)
    {
        var sImgPath = GetAnnouncementAssetPath(dTrauerfall, sLocalPath);
        var sPath = Directory.GetParent(sImgPath)?.FullName ?? string.Empty;
        var sNormalizedLocalPath = sPath.Replace(sLocalPath, string.Empty, StringComparison.Ordinal);
        var sProfileBase = Directory.GetParent(Directory.GetParent(sPath)?.FullName ?? string.Empty)?.FullName ?? string.Empty;
        var sProfileImage = dTrauerfall.Cond(KeyProfileImageSource).Replace(sProfileBase, ProfileImageRelativeRoot, StringComparison.Ordinal);
        var (sLastName, sFirstName) = dTrauerfall.Cond(JsonName).SplitName();

        return new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            [KeyIdTrauerfall] = iTrauerfallId,
            [KeyUrl] = dTrauerfall.Cond(KeyUrl),
            [KeyAnnouncement] = int.TryParse(dTrauerfall.Cond(KeyId), out var iId) ? iId : 0,
            [KeyRelease] = ToDbValue(PortedHelpers.Str2Date(dTrauerfall.Cond(KeyPublish))),
            [KeyLocalPath] = sNormalizedLocalPath,
            [KeyPngFile] = Path.GetFileName(dTrauerfall.Cond(KeyImage)),
            [KeyPdfFile] = Path.GetFileName(dTrauerfall.Cond(KeyPdf)),
            [KeyAdditional] = JsonSerializer.Serialize(dTrauerfall, PortedHelpers.JsonOptions),
            [KeyFirstName] = sFirstName,
            [KeyLastName] = sLastName,
            [KeyBirthName] = dTrauerfall.Cond(KeyBirthName),
            [KeyBirth] = ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(dTrauerfall.Cond(KeyBirth)))),
            [KeyDeath] = ToDbValue(PortedHelpers.Str2Date(TrimLeadingTwo(dTrauerfall.Cond(KeyDeath)))),
            [KeyPlace] = dTrauerfall.Cond(KeyPlace),
            [KeyInfo] = dTrauerfall.Cond(KeyPdfText),
            [KeyProfileImage] = sProfileImage,
            [KeyRubrik] = iRubrik
        };
    }

    private static Dictionary<string, object?> CreateLegacyAnnouncementRecord(string sAuftrag, IReadOnlyDictionary<string, object?> dTrauerfall, string sLocalPath)
    {
        var sNormalizedLocalPath = Directory.GetParent(dTrauerfall.Cond(KeyPdf))?.FullName?.Replace(sLocalPath, string.Empty, StringComparison.Ordinal) ?? string.Empty;
        return new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["Auftrag"] = sAuftrag,
            [KeyUrl] = dTrauerfall.Cond(KeyUrl),
            [KeyAnnouncement] = int.TryParse(dTrauerfall.Cond(KeyId), out var iId) ? iId : 0,
            [KeyRelease] = ToDbValue(PortedHelpers.Str2Date(dTrauerfall.Cond(KeyPublish))),
            [KeyLocalPath] = sNormalizedLocalPath,
            [KeyPngFile] = Path.GetFileName(dTrauerfall.Cond(KeyImage)),
            [KeyPdfFile] = Path.GetFileName(dTrauerfall.Cond(KeyPdf)),
            [KeyAdditional] = JsonSerializer.Serialize(dTrauerfall, PortedHelpers.JsonOptions)
        };
    }

    private static string GetAnnouncementAssetPath(IReadOnlyDictionary<string, object?> dTrauerfall, string sLocalPath)
    {
        var sImgPath = dTrauerfall.Cond(KeyImage);
        if (!string.IsNullOrEmpty(sImgPath))
        {
            return sImgPath;
        }

        sImgPath = dTrauerfall.Cond(KeyPdf);
        return string.IsNullOrEmpty(sImgPath) ? sLocalPath : sImgPath;
    }

    private static int ResolveRubrik(IReadOnlyDictionary<string, object?> dCurrent, Dictionary<string, object?> dTrauerfall)
    {
        var iRubrik = dCurrent.TryGetValue(KeyRubrik, out var xCurrentRubrik) && int.TryParse(Convert.ToString(xCurrentRubrik, CultureInfo.InvariantCulture), out var iParsed)
            ? iParsed
            : RubrikTodesanzeigen;

        var iMappedRubrik = GetRubrikOrNull(dTrauerfall.Cond(KeyFilter));
        if (iMappedRubrik.HasValue)
        {
            return iMappedRubrik.Value;
        }

        TryRestoreFilterFromAdditional(dCurrent, dTrauerfall);
        return iRubrik;
    }

    private static void TryRestoreFilterFromAdditional(IReadOnlyDictionary<string, object?> dCurrent, Dictionary<string, object?> dTrauerfall)
    {
        try
        {
            var xJson = JsonNode.Parse(Convert.ToString(dCurrent.GetValueOrDefault(KeyAdditional), CultureInfo.InvariantCulture) ?? string.Empty) as JsonObject;
            if (xJson?[KeyFilter] is not null)
            {
                dTrauerfall[KeyFilter] = xJson[KeyFilter]!.ToString();
            }
        }
        catch
        {
        }
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
        return GetRubrikOrNull(dTrauerfall.Cond(KeyFilter)) ?? RubrikTodesanzeigen;
    }

    private static int? GetRubrikOrNull(string sFilter)
    {
        return sFilter switch
        {
            FilterDanksagungen => RubrikDanksagungen,
            FilterNachrufe => RubrikNachrufe,
            FilterTodesanzeigen => RubrikTodesanzeigen,
            _ => null
        };
    }

    private static bool IsAnnouncementSection(JsonObject xSectionNode)
    {
        var arrTexts = xSectionNode[JsonText] as JsonArray;
        return arrTexts is not null
            && arrTexts.Count > 0
            && (arrTexts[0]?.ToString() ?? string.Empty).StartsWith(AnnouncementPrefix, StringComparison.Ordinal);
    }

    private static bool StartsWithValue(JsonObject xNode, string sKey, string sPrefix)
    {
        return (xNode[sKey]?.ToString() ?? string.Empty).StartsWith(sPrefix, StringComparison.Ordinal);
    }

    private static string TrimPrefixedValue(string sValue, string sPrefix)
    {
        var iStart = sPrefix.Length;
        if (sValue.Length > iStart && sValue[iStart] == ' ')
        {
            iStart += 1;
        }

        return sValue.Length > iStart ? sValue[iStart..] : string.Empty;
    }

    private static void TryUpdateProfileImagePath(JsonObject xAnnouncementNode, string sLocalPathRoot, ref string sProfileImagePath)
    {
        if (!StartsWithValue(xAnnouncementNode, JsonClass, ImageColumnCssPrefix))
        {
            return;
        }

        try
        {
            if (xAnnouncementNode[JsonImages] is JsonArray arrImages && arrImages.Count > 0 && arrImages[0] is JsonObject xImage0)
            {
                var sSource = xImage0[JsonSource]?.ToString() ?? string.Empty;
                if (sSource.Contains(MediaMarker, StringComparison.Ordinal))
                {
                    sProfileImagePath = PortedHelpers.GetLocalPath(sSource.LCropStr(QuerySeparator), sLocalPathRoot);
                }
            }
        }
        catch
        {
        }
    }
}
