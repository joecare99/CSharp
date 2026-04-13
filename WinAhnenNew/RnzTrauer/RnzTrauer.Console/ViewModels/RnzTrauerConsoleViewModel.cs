using System.Text;
using System.Text.Json.Nodes;
using RnzTrauer.Console.Views;
using RnzTrauer.Core;

namespace RnzTrauer.Console.ViewModels;

/// <summary>
/// Coordinates the RNZ console workflow while keeping user-facing output outside the core services.
/// </summary>
public sealed class RnzTrauerConsoleViewModel
{
    private readonly ConsoleOutputView _view;
    private readonly IFile _xFile;
    private readonly IHttpClientProxy _xHttpClient;
    private readonly IWebDriverFactory _xWebDriverFactory;

    // Dictionary keys
    private const string KeyContent = "content";
    private const string KeyHref = "href";
    private const string KeyLocalPath = "localpath";
    private const string KeyParent = "parent";
    private const string KeyPdfText = "pdfText";
    private const string KeyUrl = "url";

    // File extensions
    private const string ExtHtml = ".html";
    private const string ExtJpeg = ".jpeg";
    private const string ExtJson = ".json";
    private const string ExtPdf = ".pdf";
    private const string ExtPng = ".png";

    // Binary file signatures
    private const string SignatureHtmlDoctype = "<!DOCTYP";
    private const string SignatureJfif = "JFIF";
    private const string SignaturePdf = "PDF";
    private const string SignaturePng = "PNG";

    // URL and path segments
    private const string DateFormatDaily = "yyyy-MM-dd";
    private const string DataFilePrefix = "data_";
    private const string HttpSchemePrefix = "http";
    private const string PathAnzeigenArt = "/anzeigenart-";
    private const string PathSeite = "/seite-";
    private const string RnzSearchPath = "/traueranzeigen-suche/erscheinungstag-";

    private static readonly string[] AnnouncementTypes = ["todesanzeigen", "nachrufe", "danksagungen", "_"];

    // UI strings
    private const string UiMsgCompute = "Compute ";
    private const string UiMsgInit = "Init...";
    private const string UiMsgSave = "\nSave ";
    private const string UiMsgSaveMedia = "\nSave Media:";
    private const string UiMsgStart = "Start...";

    /// <summary>
    /// Initializes a new instance of the <see cref="RnzTrauerConsoleViewModel"/> class.
    /// </summary>
    public RnzTrauerConsoleViewModel(ConsoleOutputView xView, IFile xFile, IHttpClientProxy xHttpClient, IWebDriverFactory xWebDriverFactory)
    {
        _view = xView;
        _xFile = xFile ?? throw new ArgumentNullException(nameof(xFile));
        _xHttpClient = xHttpClient ?? throw new ArgumentNullException(nameof(xHttpClient));
        _xWebDriverFactory = xWebDriverFactory ?? throw new ArgumentNullException(nameof(xWebDriverFactory));
    }

    /// <summary>
    /// Runs the RNZ scraping and import workflow.
    /// </summary>
    public void Run(RnzConfig xConfig, string sParam1 = "")
    {
        _view.WriteLine(UiMsgStart);
        var xProgress = new Progress<WebHandlerProgress>(xUpdate =>
        {
            if (xUpdate.WriteLine)
            {
                _view.WriteLine(xUpdate.Text);
            }
            else
            {
                _view.Write(xUpdate.Text);
            }
        });
        using var xWebHandler = new WebHandler(xConfig, _xHttpClient, _xWebDriverFactory, xProgress);
        xWebHandler.InitPage();

        _view.WriteLine(UiMsgInit);
        var sBaseHost = Uri.TryCreate(xConfig.Url, UriKind.Absolute, out var xBaseUri)
            ? xBaseUri.GetLeftPart(UriPartial.Authority)
            : string.Empty;
        var sSearchBaseUrl = $"{sBaseHost}{RnzSearchPath}";
        DateTime today = DateTime.Today;
        var iOffset = DateTime.TryParse(sParam1,out var dtStart)?(today- dtStart).Days : 0; 
        var iDayDelta = 0;
        while (iDayDelta <= 14)
        {
            var dtCurrent = DateOnly.FromDateTime(today).AddDays(-(iDayDelta + iOffset));
            iDayDelta += 1;
            var sStart = $"{sSearchBaseUrl}{dtCurrent.Day:00}-{dtCurrent.Month:00}-{dtCurrent.Year:0000}";
            var (dPages, arrItems) = xWebHandler.GetData1(sStart);

            _view.Write(UiMsgCompute);
            for (var iIndex = 0; iIndex < arrItems.Count; iIndex++)
            {
                var dEntry = new Dictionary<string, object?>(arrItems[iIndex], StringComparer.Ordinal);
                try
                {
                    if (dEntry.TryGetValue(WebHandler.CsData, out var xDataObject) && xDataObject is byte[] arrData && arrData.Length >= 10)
                    {
                        var sPrefix = Encoding.ASCII.GetString(arrData.Take(10).ToArray());
                        if (sPrefix.Contains(SignaturePdf, StringComparison.Ordinal))
                        {
                            dEntry[KeyPdfText] = PortedHelpers.PdfText(arrData);
                            arrItems[iIndex] = dEntry;
                            if (dEntry.TryGetValue(KeyParent, out var xParentObject))
                            {
                                var sParent = Convert.ToString(xParentObject) ?? string.Empty;
                                if (dPages.TryGetValue(sParent, out var dParentPage) && dEntry.TryGetValue(WebHandler.CsSrc, out var xSourceObject))
                                {
                                    dParentPage[Convert.ToString(xSourceObject) ?? string.Empty] = new Dictionary<string, object?>
                                    {
                                        [KeyPdfText] = dEntry[KeyPdfText]
                                    };
                                }
                            }

                            _view.Write('+');
                        }
                        else
                        {
                            _view.Write('.');
                        }
                    }
                }
                catch
                {
                }
            }

            _view.Write(UiMsgSave);
            SavePages(xConfig, dPages);
            _view.Write(UiMsgSaveMedia);
            SaveMedia(xConfig, arrItems, dtCurrent);
            _view.WriteLine();
        }

        xWebHandler.Close();

        using var xDataHandler = new DataHandler(xConfig, _xFile);
        iDayDelta = -7;
        while (iDayDelta <= 30)
        {
            var dtCurrent = DateOnly.FromDateTime(DateTime.Today).AddDays(-(iDayDelta + iOffset));
            _view.WriteLine($"Handle: {dtCurrent}");
            iDayDelta += 1;
            foreach (var sAnnouncementType in AnnouncementTypes)
            {
                _view.WriteLine($"Type: {sAnnouncementType}");
                var iPage = 0;
                while (iPage < 20)
                {
                    iPage += 1;
                    _view.WriteLine($"Page: {iPage}");
                    var sStart = iPage == 1
                        ? $"{sSearchBaseUrl}{dtCurrent.Day:00}-{dtCurrent.Month:00}-{dtCurrent.Year:0000}{PathAnzeigenArt}{sAnnouncementType}"
                        : $"{sSearchBaseUrl}{dtCurrent.Day:00}-{dtCurrent.Month:00}-{dtCurrent.Year:0000}{PathAnzeigenArt}{sAnnouncementType}{PathSeite}{iPage}";
                    var sPath = PortedHelpers.GetLocalPath(sStart, xConfig.LocalPath, dtCurrent);
                    var sJsonFile = Path.HasExtension(sPath)
                        ? Path.ChangeExtension(sPath, ExtJson)
                        : sPath + ExtJson;
                    if (_xFile.Exists(sJsonFile))
                    {
                        var xData = JsonNode.Parse(_xFile.ReadAllText(sJsonFile));
                        var arrTrauerfaelle = xDataHandler.ExtractTrauerData(xData, xConfig.LocalPath);
                        xDataHandler.TrauerDataToDb(arrTrauerfaelle, xConfig.LocalPath);
                    }
                    else
                    {
                        if (iPage == 1)
                        {
                            _view.Write('-');
                        }

                        iPage = 99;
                    }
                }
            }
        }
    }

    private void SavePages(RnzConfig xConfig, Dictionary<string, Dictionary<string, object?>> dPages)
    {
        foreach (var dPage in dPages.Values)
        {
            var dParentData = new Dictionary<string, JsonObject>(StringComparer.Ordinal);
            var dParentPaths = new Dictionary<string, string>(StringComparer.Ordinal);
            var xParentChanged = false;

            if (dPage.TryGetValue(KeyParent, out var xParentObject) && xParentObject is List<object?> arrParents)
            {
                foreach (var xParentValue in arrParents)
                {
                    var sParent = Convert.ToString(xParentValue) ?? string.Empty;
                    var sParentPath = PortedHelpers.GetLocalPath(sParent, xConfig.LocalPath);
                    sParentPath = Path.HasExtension(sParentPath) ? Path.ChangeExtension(sParentPath, ExtJson) : sParentPath + ExtJson;
                    if (_xFile.Exists(sParentPath))
                    {
                        try
                        {
                            dParentData[sParent] = JsonNode.Parse(_xFile.ReadAllText(sParentPath)) as JsonObject ?? new JsonObject();
                        }
                        catch
                        {
                            dParentData[sParent] = new JsonObject();
                            xParentChanged = true;
                        }
                    }
                    else
                    {
                        dParentData[sParent] = new JsonObject();
                        xParentChanged = true;
                    }

                    dParentPaths[sParent] = sParentPath;
                }
            }

            var sLocalPath = PortedHelpers.GetLocalPath(Convert.ToString(dPage[KeyUrl]) ?? string.Empty, xConfig.LocalPath);
            var sHtmlPath = Path.HasExtension(sLocalPath) ? sLocalPath : sLocalPath + ExtHtml;
            Directory.CreateDirectory(Path.GetDirectoryName(sHtmlPath)!);
            _xFile.WriteAllText(sHtmlPath, Convert.ToString(dPage[KeyContent]) ?? string.Empty);

            var sJsonPath = Path.ChangeExtension(sHtmlPath, ExtJson);
            var xPageNode = PortedHelpers.ToJsonObject(dPage);
            if (_xFile.Exists(sJsonPath))
            {
                try
                {
                    var xOldNode = JsonNode.Parse(_xFile.ReadAllText(sJsonPath)) as JsonObject;
                    if (xOldNode is not null)
                    {
                        foreach (var kvValue in xOldNode)
                        {
                            if (kvValue.Key.StartsWith(HttpSchemePrefix, StringComparison.Ordinal) && !xPageNode.ContainsKey(kvValue.Key))
                            {
                                xPageNode[kvValue.Key] = kvValue.Value?.DeepClone();
                            }
                        }
                    }
                }
                catch
                {
                    _view.Write('-');
                }
            }

            _xFile.WriteAllText(sJsonPath, xPageNode.ToJsonString(PortedHelpers.JsonOptions));

            if (dPage.TryGetValue(KeyParent, out xParentObject) && xParentObject is List<object?> arrParentList)
            {
                foreach (var xParentValue in arrParentList)
                {
                    var sParent = Convert.ToString(xParentValue) ?? string.Empty;
                    var dEntryCopy = new Dictionary<string, object?>(dPage, StringComparer.Ordinal);
                    dEntryCopy.Remove(KeyContent);
                    dEntryCopy[KeyLocalPath] = sJsonPath;
                    var sUrl = Convert.ToString(dPage[KeyUrl]) ?? string.Empty;
                    if (!dParentData[sParent].ContainsKey(sUrl))
                    {
                        dParentData[sParent][sUrl] = PortedHelpers.ToJsonObject(dEntryCopy);
                        xParentChanged = true;
                    }
                    else if (dParentData[sParent][sUrl] is JsonObject xTarget)
                    {
                        foreach (var kvValue in dEntryCopy)
                        {
                            xTarget[kvValue.Key] = kvValue.Value.ToJsonNode();
                        }

                        xParentChanged = true;
                    }

                    if (xParentChanged)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dParentPaths[sParent])!);
                        _xFile.WriteAllText(dParentPaths[sParent], dParentData[sParent].ToJsonString(PortedHelpers.JsonOptions));
                    }
                }
            }

            _view.Write('.');
        }
    }

    private void SaveMedia(RnzConfig xConfig, List<Dictionary<string, object?>> arrItems, DateOnly dtCurrent)
    {
        foreach (var dEntry in arrItems)
        {
            try
            {
                var dEntryCopy = new Dictionary<string, object?>(dEntry, StringComparer.Ordinal);
                dEntryCopy.Remove(WebHandler.CsData);
                var sParent = Convert.ToString(dEntry.GetValueOrDefault(KeyParent)) ?? string.Empty;
                var sParentPath = PortedHelpers.GetLocalPath(sParent, xConfig.LocalPath);
                sParentPath = Path.HasExtension(sParentPath) ? Path.ChangeExtension(sParentPath, ExtJson) : sParentPath + ExtJson;
                JsonObject xParentData;
                var xParentChanged = false;
                if (_xFile.Exists(sParentPath))
                {
                    try
                    {
                        xParentData = JsonNode.Parse(_xFile.ReadAllText(sParentPath)) as JsonObject ?? new JsonObject();
                    }
                    catch
                    {
                        xParentData = new JsonObject();
                        xParentChanged = true;
                    }
                }
                else
                {
                    xParentData = new JsonObject();
                    xParentChanged = true;
                }

                if (dEntryCopy.TryGetValue(WebHandler.CsHeader, out var xHeaderObject) && xHeaderObject is Dictionary<string, string> dHeaders)
                {
                    dEntryCopy[WebHandler.CsHeader] = dHeaders.ToDictionary(k => k.Key, v => (object?)v.Value, StringComparer.OrdinalIgnoreCase);
                }

                if (dEntry.TryGetValue(KeyHref, out var xHrefObject))
                {
                    var sHref = Convert.ToString(xHrefObject) ?? string.Empty;
                    var sLocalPath = PortedHelpers.GetLocalPath(sHref, xConfig.LocalPath, dtCurrent);
                    var sDataPath = Path.Combine(sLocalPath, $"{DataFilePrefix}{dtCurrent.ToString(DateFormatDaily)}{ExtJson}");
                    Directory.CreateDirectory(Path.GetDirectoryName(sDataPath)!);
                    _xFile.WriteAllText(sDataPath, PortedHelpers.ToJsonObject(dEntryCopy).ToJsonString(PortedHelpers.JsonOptions));
                }

                var sSource = Convert.ToString(dEntry.GetValueOrDefault(WebHandler.CsSrc)) ?? string.Empty;
                if (!string.IsNullOrEmpty(sSource) && dEntry.TryGetValue(WebHandler.CsData, out var xDataObject) && xDataObject is byte[] arrData)
                {
                    var sLocalPath = PortedHelpers.GetLocalPath(sSource, xConfig.LocalPath);
                    var sFilePath = sLocalPath;
                    var sPrefix = Encoding.ASCII.GetString(arrData.Take(10).ToArray());
                    if (sPrefix.Contains(SignaturePng, StringComparison.Ordinal))
                    {
                        sFilePath = Path.ChangeExtension(sFilePath, ExtPng);
                    }
                    else if (sPrefix.Contains(SignaturePdf, StringComparison.Ordinal))
                    {
                        sFilePath = Path.ChangeExtension(sFilePath, ExtPdf);
                    }
                    else if (sPrefix.Contains(SignatureJfif, StringComparison.Ordinal))
                    {
                        sFilePath = Path.ChangeExtension(sFilePath, ExtJpeg);
                    }
                    else if (sPrefix.Contains(SignatureHtmlDoctype, StringComparison.Ordinal))
                    {
                        sFilePath = Path.ChangeExtension(sFilePath, ExtHtml);
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(sFilePath)!);
                    _xFile.WriteAllBytes(sFilePath, arrData);
                    dEntryCopy[KeyLocalPath] = sFilePath;
                    xParentData[sSource] = PortedHelpers.ToJsonObject(dEntryCopy);
                    xParentChanged = true;
                }

                if (xParentChanged)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(sParentPath)!);
                    _xFile.WriteAllText(sParentPath, xParentData.ToJsonString(PortedHelpers.JsonOptions));
                }
            }
            catch
            {
            }

            _view.Write('.');
        }
    }
}
