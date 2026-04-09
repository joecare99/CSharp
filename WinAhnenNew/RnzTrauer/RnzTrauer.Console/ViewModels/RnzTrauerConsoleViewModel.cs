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

    /// <summary>
    /// Initializes a new instance of the <see cref="RnzTrauerConsoleViewModel"/> class.
    /// </summary>
    public RnzTrauerConsoleViewModel(ConsoleOutputView xView)
    {
        _view = xView;
    }

    /// <summary>
    /// Runs the RNZ scraping and import workflow.
    /// </summary>
    public void Run(RnzConfig xConfig)
    {
        _view.WriteLine("Start...");
        using var xWebHandler = new WebHandler(xConfig);
        xWebHandler.InitPage();

        _view.WriteLine("Init...");
        var iOffset = 35;
        var iDayDelta = 0;
        while (iDayDelta <= 800)
        {
            var dtCurrent = DateOnly.FromDateTime(DateTime.Today).AddDays(-(iDayDelta + iOffset));
            iDayDelta += 1;
            var sStart = $"https://trauer.rnz.de/traueranzeigen-suche/erscheinungstag-{dtCurrent.Day:00}-{dtCurrent.Month:00}-{dtCurrent.Year:0000}";
            var (dPages, arrItems) = xWebHandler.GetData1(sStart);

            _view.Write("Compute ");
            for (var iIndex = 0; iIndex < arrItems.Count; iIndex++)
            {
                var dEntry = new Dictionary<string, object?>(arrItems[iIndex], StringComparer.Ordinal);
                try
                {
                    if (dEntry.TryGetValue(WebHandler.CsData, out var xDataObject) && xDataObject is byte[] arrData && arrData.Length >= 10)
                    {
                        var sPrefix = Encoding.ASCII.GetString(arrData.Take(10).ToArray());
                        if (sPrefix.Contains("PDF", StringComparison.Ordinal))
                        {
                            dEntry["pdfText"] = PortedHelpers.PdfText(arrData);
                            arrItems[iIndex] = dEntry;
                            if (dEntry.TryGetValue("parent", out var xParentObject))
                            {
                                var sParent = Convert.ToString(xParentObject) ?? string.Empty;
                                if (dPages.TryGetValue(sParent, out var dParentPage) && dEntry.TryGetValue(WebHandler.CsSrc, out var xSourceObject))
                                {
                                    dParentPage[Convert.ToString(xSourceObject) ?? string.Empty] = new Dictionary<string, object?>
                                    {
                                        ["pdfText"] = dEntry["pdfText"]
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

            _view.Write("\nSave ");
            SavePages(xConfig, dPages);
            _view.Write("\nSave Media:");
            SaveMedia(xConfig, arrItems, dtCurrent);
            _view.WriteLine();
        }

        xWebHandler.Close();

        using var xDataHandler = new DataHandler(xConfig);
        iDayDelta = 0;
        while (iDayDelta <= 14)
        {
            var dtCurrent = DateOnly.FromDateTime(DateTime.Today).AddDays(-(iDayDelta + iOffset));
            _view.WriteLine($"Handle: {dtCurrent}");
            iDayDelta += 1;
            foreach (var sAnnouncementType in new[] { "todesanzeigen", "nachrufe", "danksagungen", "_" })
            {
                _view.WriteLine($"Type: {sAnnouncementType}");
                var iPage = 0;
                while (iPage < 20)
                {
                    iPage += 1;
                    _view.WriteLine($"Page: {iPage}");
                    var sStart = iPage == 1
                        ? $"https://trauer.rnz.de/traueranzeigen-suche/erscheinungstag-{dtCurrent.Day:00}-{dtCurrent.Month:00}-{dtCurrent.Year:0000}/anzeigenart-{sAnnouncementType}"
                        : $"https://trauer.rnz.de/traueranzeigen-suche/erscheinungstag-{dtCurrent.Day:00}-{dtCurrent.Month:00}-{dtCurrent.Year:0000}/anzeigenart-{sAnnouncementType}/seite-{iPage}";
                    var sPath = PortedHelpers.GetLocalPath(sStart, xConfig.LocalPath, dtCurrent);
                    var sJsonFile = Path.HasExtension(sPath)
                        ? Path.ChangeExtension(sPath, ".json")
                        : sPath + ".json";
                    if (File.Exists(sJsonFile))
                    {
                        var xData = JsonNode.Parse(File.ReadAllText(sJsonFile));
                        var arrTrauerfaelle = DataHandler.ExtractTrauerData(xData, xConfig.LocalPath);
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

            if (dPage.TryGetValue("parent", out var xParentObject) && xParentObject is List<object?> arrParents)
            {
                foreach (var xParentValue in arrParents)
                {
                    var sParent = Convert.ToString(xParentValue) ?? string.Empty;
                    var sParentPath = PortedHelpers.GetLocalPath(sParent, xConfig.LocalPath);
                    sParentPath = Path.HasExtension(sParentPath) ? Path.ChangeExtension(sParentPath, ".json") : sParentPath + ".json";
                    if (File.Exists(sParentPath))
                    {
                        try
                        {
                            dParentData[sParent] = JsonNode.Parse(File.ReadAllText(sParentPath)) as JsonObject ?? new JsonObject();
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

            var sLocalPath = PortedHelpers.GetLocalPath(Convert.ToString(dPage["url"]) ?? string.Empty, xConfig.LocalPath);
            var sHtmlPath = Path.HasExtension(sLocalPath) ? sLocalPath : sLocalPath + ".html";
            Directory.CreateDirectory(Path.GetDirectoryName(sHtmlPath)!);
            File.WriteAllText(sHtmlPath, Convert.ToString(dPage["content"]) ?? string.Empty);

            var sJsonPath = Path.ChangeExtension(sHtmlPath, ".json");
            var xPageNode = PortedHelpers.ToJsonObject(dPage);
            if (File.Exists(sJsonPath))
            {
                try
                {
                    var xOldNode = JsonNode.Parse(File.ReadAllText(sJsonPath)) as JsonObject;
                    if (xOldNode is not null)
                    {
                        foreach (var kvValue in xOldNode)
                        {
                            if (kvValue.Key.StartsWith("http", StringComparison.Ordinal) && !xPageNode.ContainsKey(kvValue.Key))
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

            File.WriteAllText(sJsonPath, xPageNode.ToJsonString(PortedHelpers.JsonOptions));

            if (dPage.TryGetValue("parent", out xParentObject) && xParentObject is List<object?> arrParentList)
            {
                foreach (var xParentValue in arrParentList)
                {
                    var sParent = Convert.ToString(xParentValue) ?? string.Empty;
                    var dEntryCopy = new Dictionary<string, object?>(dPage, StringComparer.Ordinal);
                    dEntryCopy.Remove("content");
                    dEntryCopy["localpath"] = sJsonPath;
                    var sUrl = Convert.ToString(dPage["url"]) ?? string.Empty;
                    if (!dParentData[sParent].ContainsKey(sUrl))
                    {
                        dParentData[sParent][sUrl] = PortedHelpers.ToJsonObject(dEntryCopy);
                        xParentChanged = true;
                    }
                    else if (dParentData[sParent][sUrl] is JsonObject xTarget)
                    {
                        foreach (var kvValue in dEntryCopy)
                        {
                            xTarget[kvValue.Key] = PortedHelpers.ToJsonNode(kvValue.Value);
                        }

                        xParentChanged = true;
                    }

                    if (xParentChanged)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(dParentPaths[sParent])!);
                        File.WriteAllText(dParentPaths[sParent], dParentData[sParent].ToJsonString(PortedHelpers.JsonOptions));
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
                var sParent = Convert.ToString(dEntry.GetValueOrDefault("parent")) ?? string.Empty;
                var sParentPath = PortedHelpers.GetLocalPath(sParent, xConfig.LocalPath);
                sParentPath = Path.HasExtension(sParentPath) ? Path.ChangeExtension(sParentPath, ".json") : sParentPath + ".json";
                JsonObject xParentData;
                var xParentChanged = false;
                if (File.Exists(sParentPath))
                {
                    try
                    {
                        xParentData = JsonNode.Parse(File.ReadAllText(sParentPath)) as JsonObject ?? new JsonObject();
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

                if (dEntryCopy.TryGetValue("Header", out var xHeaderObject) && xHeaderObject is Dictionary<string, string> dHeaders)
                {
                    dEntryCopy["Header"] = dHeaders.ToDictionary(k => k.Key, v => (object?)v.Value, StringComparer.OrdinalIgnoreCase);
                }

                if (dEntry.TryGetValue("href", out var xHrefObject))
                {
                    var sHref = Convert.ToString(xHrefObject) ?? string.Empty;
                    var sLocalPath = PortedHelpers.GetLocalPath(sHref, xConfig.LocalPath, dtCurrent);
                    var sDataPath = Path.Combine(sLocalPath, $"data_{dtCurrent:yyyy-MM-dd}.json");
                    Directory.CreateDirectory(Path.GetDirectoryName(sDataPath)!);
                    File.WriteAllText(sDataPath, PortedHelpers.ToJsonObject(dEntryCopy).ToJsonString(PortedHelpers.JsonOptions));
                }

                var sSource = Convert.ToString(dEntry.GetValueOrDefault(WebHandler.CsSrc)) ?? string.Empty;
                if (!string.IsNullOrEmpty(sSource) && dEntry.TryGetValue(WebHandler.CsData, out var xDataObject) && xDataObject is byte[] arrData)
                {
                    var sLocalPath = PortedHelpers.GetLocalPath(sSource, xConfig.LocalPath);
                    var sFilePath = sLocalPath;
                    var sPrefix = Encoding.ASCII.GetString(arrData.Take(10).ToArray());
                    if (sPrefix.Contains("PNG", StringComparison.Ordinal))
                    {
                        sFilePath = Path.ChangeExtension(sFilePath, ".png");
                    }
                    else if (sPrefix.Contains("PDF", StringComparison.Ordinal))
                    {
                        sFilePath = Path.ChangeExtension(sFilePath, ".pdf");
                    }
                    else if (sPrefix.Contains("JFIF", StringComparison.Ordinal))
                    {
                        sFilePath = Path.ChangeExtension(sFilePath, ".jpeg");
                    }
                    else if (sPrefix.Contains("<!DOCTYP", StringComparison.Ordinal))
                    {
                        sFilePath = Path.ChangeExtension(sFilePath, ".html");
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(sFilePath)!);
                    File.WriteAllBytes(sFilePath, arrData);
                    dEntryCopy["localpath"] = sFilePath;
                    xParentData[sSource] = PortedHelpers.ToJsonObject(dEntryCopy);
                    xParentChanged = true;
                }

                if (xParentChanged)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(sParentPath)!);
                    File.WriteAllText(sParentPath, xParentData.ToJsonString(PortedHelpers.JsonOptions));
                }
            }
            catch
            {
            }

            _view.Write('.');
        }
    }
}
