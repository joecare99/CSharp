using System.Globalization;
using System.Text;
using OpenQA.Selenium;

namespace RnzTrauer.Core;

/// <summary>
/// Provides Selenium-based scraping for the RNZ obituary portal.
/// </summary>
public sealed class WebHandler : IDisposable
{
    /// <summary>
    /// Gets the data payload key used by the ported dictionaries.
    /// </summary>
    public const string CsData = "Data";

    /// <summary>
    /// Gets the header payload key used by the ported dictionaries.
    /// </summary>
    public const string CsHeader = "Header";

    /// <summary>
    /// Gets the media source key used by the ported dictionaries.
    /// </summary>
    public const string CsSrc = "Src";

    private static readonly Dictionary<string, string[]> _tagAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["section"] = ["class", "id"],
        ["div"] = ["class", "id"],
        ["img"] = ["class", "title", "alt", "src", "style", "data-original"],
        ["a"] = ["title", "target", "href"]
    };

    private readonly RnzConfig _config;
    private readonly IHttpClientProxy _xHttpClient;
    private readonly IWebDriverFactory _xWebDriverFactory;
    private readonly IProgress<WebHandlerProgress>? _xProgress;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebHandler"/> class.
    /// </summary>
    public WebHandler(RnzConfig xConfig, IHttpClientProxy xHttpClient, IWebDriverFactory xWebDriverFactory, IProgress<WebHandlerProgress>? xProgress = null)
    {
        _config = xConfig;
        _xHttpClient = xHttpClient ?? throw new ArgumentNullException(nameof(xHttpClient));
        _xWebDriverFactory = xWebDriverFactory ?? throw new ArgumentNullException(nameof(xWebDriverFactory));
        _xProgress = xProgress;
    }

    /// <summary>
    /// Gets the active Firefox driver instance.
    /// </summary>
    public IWebDriver? Driver { get; private set; }

    /// <summary>
    /// Initializes the RNZ login session.
    /// </summary>
    public void InitPage()
    {
        try
        {
            Driver?.Quit();
        }
        catch
        {
        }

        Driver = _xWebDriverFactory.Create();
        Driver.Navigate().GoToUrl(_config.Url);
        Driver.FindElement(By.Id("emailAddress")).SendKeys(_config.User);
        Driver.FindElement(By.Id("password")).SendKeys(_config.Password);
        Driver.FindElement(By.Id("form")).Submit();
        while (Driver.Title == _config.Title || string.IsNullOrEmpty(Driver.Title))
        {
            Thread.Sleep(500);
        }

        Thread.Sleep(500);
    }

    /// <summary>
    /// Recursively captures matching elements into dictionary-based structures.
    /// </summary>
    public List<Dictionary<string, object?>> Wdr2List(ISearchContext xWebElement, WebQuery xQuery)
    {
        var arrResult = new List<Dictionary<string, object?>>();
        foreach (var xElement in xWebElement.FindElements(xQuery.By))
        {
            var dItem = new Dictionary<string, object?>
            {
                ["tag"] = xElement.TagName
            };
            arrResult.Add(dItem);

            if (_tagAttributes.TryGetValue(xElement.TagName, out var arrAttributes))
            {
                foreach (var sAttribute in arrAttributes)
                {
                    try
                    {
                        dItem[sAttribute] = xElement.GetAttribute(sAttribute) ?? string.Empty;
                    }
                    catch
                    {
                        dItem[sAttribute] = string.Empty;
                    }
                }
            }

            try
            {
                dItem["text"] = (xElement.Text ?? string.Empty).Split(Environment.NewLine, StringSplitOptions.None).ToList<object?>();
            }
            catch
            {
            }

            foreach (var xChildQuery in xQuery.Children)
            {
                dItem[xChildQuery.Name] = Wdr2List(xElement, xChildQuery);
            }
        }

        return arrResult;
    }

    /// <summary>
    /// Loads the configured RNZ pages and associated media.
    /// </summary>
    public (Dictionary<string, Dictionary<string, object?>> Pages, List<Dictionary<string, object?>> Items) GetData1(string sStart, int iMaxPage = 30)
    {
        var xDriver = Driver ?? throw new InvalidOperationException("The web driver has not been initialized.");
        var dPages = new Dictionary<string, Dictionary<string, object?>>(StringComparer.Ordinal);
        var arrItems = new List<Dictionary<string, object?>>();

        foreach (var sAnnouncementType in new[] { "nachrufe", "danksagungen", "todesanzeigen", "_" })
        {
            xDriver.Navigate().GoToUrl($"{sStart}/anzeigenart-{sAnnouncementType}");
            while (xDriver.Title == "RNZ" || string.IsNullOrEmpty(xDriver.Title))
            {
                Thread.Sleep(500);
            }

            var sNextUrl = ".";
            var iCounter = 0;
            while (!string.IsNullOrEmpty(sNextUrl) && iCounter < iMaxPage)
            {
                _xProgress?.Report(new WebHandlerProgress(xDriver.Url, true));
                var (arrSubPages, sUrl, sNext) = WorkMainPage(dPages, arrItems, sAnnouncementType);
                sNextUrl = sNext;

                _xProgress?.Report(new WebHandlerProgress("\nGet Subpages:"));
                foreach (var sSubPage in arrSubPages)
                {
                    xDriver.Navigate().GoToUrl(sSubPage + "/anzeigen");
                    Thread.Sleep(5500);
                    WorkSubPage(sUrl, dPages, arrItems);
                }

                _xProgress?.Report(new WebHandlerProgress(string.Empty, true));
                if (!string.IsNullOrEmpty(sNextUrl))
                {
                    xDriver.Navigate().GoToUrl(sNextUrl);
                    while (xDriver.Url == sUrl)
                    {
                        Thread.Sleep(500);
                        _xProgress?.Report(new WebHandlerProgress("."));
                    }
                }

                iCounter++;
            }
        }

        return (dPages, arrItems);
    }

    /// <summary>
    /// Closes the browser session.
    /// </summary>
    public void Close()
    {
        Driver?.Quit();
        Driver = null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Close();
        _xHttpClient.Dispose();
    }

    private (List<string> SubPages, string Url, string NextUrl) WorkMainPage(Dictionary<string, Dictionary<string, object?>> dPages, List<Dictionary<string, object?>> arrItems, string sAnnouncementType)
    {
        var xDriver = Driver ?? throw new InvalidOperationException("The web driver has not been initialized.");
        var sUrl = xDriver.Url;
        var dPage = new Dictionary<string, object?>
        {
            ["Title"] = xDriver.Title,
            ["url"] = sUrl,
            ["filter"] = sAnnouncementType,
            ["sections"] = new List<Dictionary<string, object?>>(),
            ["content"] = PortedHelpers.MakeLocal(xDriver.PageSource, sUrl.Length > 50 ? sUrl[..50] + "/" : sUrl + "/")
        };
        var arrSubPages = new List<string>();
        dPages[sUrl] = dPage;

        var arrElements = Wdr2List(xDriver, new WebQuery(string.Empty, By.ClassName("c-blockitem"), new WebQuery("links", By.TagName("a")), new WebQuery("imgs", By.TagName("img"))));
        dPage["sections"] = arrElements;
        var sNextUrl = string.Empty;

        foreach (var dElement in arrElements)
        {
            var arrText = GetStringList(dElement, "text");
            if (arrText.Count > 1 && arrText[0].StartsWith("ANZ", StringComparison.Ordinal))
            {
                var dAnnouncement = new Dictionary<string, object?>
                {
                    ["Title"] = arrText[0].Length >= 8 ? arrText[0][8..] : arrText[0],
                    ["parent"] = sUrl,
                    ["Text"] = arrText.Cast<object?>().ToList()
                };

                if (arrText.Count > 1)
                {
                    dAnnouncement["Info"] = arrText[1];
                }

                var arrLinks = GetDictionaryList(dElement, "links");
                if (arrLinks.Count > 0)
                {
                    var sHref = Convert.ToString(arrLinks[0].GetValueOrDefault("href"), CultureInfo.InvariantCulture) ?? string.Empty;
                    dAnnouncement["href"] = sHref;
                    arrSubPages.Add(sHref);
                }

                var arrImages = GetDictionaryList(dElement, "imgs");
                if (arrImages.Count > 0)
                {
                    foreach (var dImage in arrImages)
                    {
                        var dCopy = new Dictionary<string, object?>(dAnnouncement, StringComparer.Ordinal);
                        dCopy[CsSrc] = string.Empty;
                        var sSource = Convert.ToString(dImage.GetValueOrDefault("src"), CultureInfo.InvariantCulture) ?? string.Empty;
                        var sDataOriginal = Convert.ToString(dImage.GetValueOrDefault("data-original"), CultureInfo.InvariantCulture) ?? string.Empty;
                        if (sSource.Contains("MEDIASERVER", StringComparison.Ordinal))
                        {
                            dCopy[CsSrc] = sSource;
                        }
                        else if (sDataOriginal.Contains("MEDIASERVER", StringComparison.Ordinal))
                        {
                            dCopy[CsSrc] = sDataOriginal;
                        }

                        _xProgress?.Report(new WebHandlerProgress("."));
                        try
                        {
                            var sMediaSource = Convert.ToString(dCopy[CsSrc], CultureInfo.InvariantCulture) ?? string.Empty;
                            if (!string.IsNullOrEmpty(sMediaSource))
                            {
                                var xResponse = _xHttpClient.GetAsync(sMediaSource).GetAwaiter().GetResult();
                                dCopy[CsData] = xResponse.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                                dCopy[CsHeader] = xResponse.Headers.Concat(xResponse.Content.Headers)
                                    .ToDictionary(h => h.Key, h => string.Join(",", h.Value), StringComparer.OrdinalIgnoreCase);
                                if (sMediaSource.Contains("MEDIASERVER", StringComparison.Ordinal))
                                {
                                    dPage[sMediaSource] = new Dictionary<string, object?>
                                    {
                                        [CsHeader] = (Dictionary<string, string>)dCopy[CsHeader]!
                                    };
                                }
                            }
                        }
                        catch
                        {
                            dCopy[CsData] = Array.Empty<byte>();
                        }

                        arrItems.Add(dCopy);
                    }
                }
                else
                {
                    arrItems.Add(dAnnouncement);
                }
            }
            else if (arrText.Count > 1 && string.Join(" ", arrText).Contains('>') && string.IsNullOrEmpty(sNextUrl))
            {
                var arrLinks = GetDictionaryList(dElement, "links");
                foreach (var dLink in arrLinks)
                {
                    var arrLinkText = GetStringList(dLink, "text");
                    if (arrLinkText.Count == 1 && arrLinkText[0] == ">")
                    {
                        sNextUrl = Convert.ToString(dLink.GetValueOrDefault("href"), CultureInfo.InvariantCulture) ?? string.Empty;
                    }
                }

                _xProgress?.Report(new WebHandlerProgress(sNextUrl, true));
            }
        }

        return (arrSubPages, sUrl, sNextUrl);
    }

    private void WorkSubPage(string sUrl, Dictionary<string, Dictionary<string, object?>> dPages, List<Dictionary<string, object?>> arrItems)
    {
        var xDriver = Driver ?? throw new InvalidOperationException("The web driver has not been initialized.");
        var dPage = dPages[sUrl];
        var sSubPageUrl = xDriver.Url;
        if (!dPages.TryGetValue(sSubPageUrl, out var dPage2))
        {
            dPage2 = new Dictionary<string, object?>
            {
                ["parent"] = new List<object?>()
            };
            dPages[sSubPageUrl] = dPage2;
        }

        Dictionary<string, object?> dParentSection = new(StringComparer.Ordinal);
        var arrPageSections = GetDictionaryList(dPage, "sections");
        for (var iIndex = 0; iIndex < arrPageSections.Count; iIndex++)
        {
            var dSection = arrPageSections[iIndex];
            var arrLinks = GetDictionaryList(dSection, "links");
            var sHref = arrLinks.Count > 0 ? Convert.ToString(arrLinks[0].GetValueOrDefault("href"), CultureInfo.InvariantCulture) ?? string.Empty : string.Empty;
            if (!string.IsNullOrEmpty(sHref) && sHref + "/anzeigen" == sSubPageUrl)
            {
                dParentSection = dSection;
                arrPageSections[iIndex] = dSection;
            }
        }

        dPage2["Title"] = xDriver.Title;
        dPage2["url"] = sSubPageUrl;
        if (dPage2["parent"] is not List<object?> arrParents)
        {
            arrParents = new List<object?>();
            dPage2["parent"] = arrParents;
        }

        arrParents.Add(sUrl);
        dPage2["content"] = PortedHelpers.MakeLocal(xDriver.PageSource, sSubPageUrl + '/');
        var arrMedia = new List<object?>();
        dPage2["media"] = arrMedia;
        var arrSections = Wdr2List(xDriver, new WebQuery(string.Empty, By.TagName("section"), new WebQuery("links", By.TagName("a")), new WebQuery("imgs", By.TagName("img"))));
        dPage2["sections"] = arrSections;

        foreach (var xElement in xDriver.FindElements(By.TagName("h1")))
        {
            dPage2["name"] = xElement.Text;
        }

        foreach (var xElement in xDriver.FindElements(By.ClassName("col-sm-6")))
        {
            if (xElement.Text.StartsWith("*", StringComparison.Ordinal))
            {
                dPage2["Birth"] = xElement.Text;
            }
            else if (xElement.Text.Contains('†'))
            {
                var arrParts = xElement.Text.Split("in", 2, StringSplitOptions.None);
                dPage2["Death"] = arrParts[0];
                dPage2["Place"] = arrParts.Length > 1 ? arrParts[1] : string.Empty;
            }
        }

        _xProgress?.Report(new WebHandlerProgress("."));
        foreach (var dSection in arrSections)
        {
            var sSectionClass = Convert.ToString(dSection.GetValueOrDefault("class"), CultureInfo.InvariantCulture) ?? string.Empty;
            if (sSectionClass.StartsWith("col-12", StringComparison.Ordinal))
            {
                foreach (var sLine in GetStringList(dSection, "text"))
                {
                    if (sLine.StartsWith("Erstellt", StringComparison.Ordinal))
                    {
                        dPage2["created_by"] = sLine.Length > 12 ? sLine[12..] : string.Empty;
                    }
                    else if (sLine.StartsWith("Angelegt", StringComparison.Ordinal))
                    {
                        dPage2["created_on"] = sLine.Length > 11 ? sLine[11..] : string.Empty;
                    }
                    else if (sLine.EndsWith("Besuche", StringComparison.Ordinal))
                    {
                        dPage2["visits"] = sLine.Length > 7 ? sLine[..^7] : string.Empty;
                    }
                }

                foreach (var dImage in GetDictionaryList(dSection, "imgs"))
                {
                    var sHref = Convert.ToString(dImage.GetValueOrDefault("src"), CultureInfo.InvariantCulture) ?? string.Empty;
                    var sText = Convert.ToString(dImage.GetValueOrDefault("alt"), CultureInfo.InvariantCulture) ?? string.Empty;
                    if (sHref.Contains("MEDIASERVER", StringComparison.Ordinal) && !arrMedia.Contains(sHref))
                    {
                        arrMedia.Add(sHref);
                        var dAnnouncement = new Dictionary<string, object?>
                        {
                            ["parent"] = sSubPageUrl,
                            [CsSrc] = sHref,
                            ["Info"] = sText,
                            ["id"] = string.Empty
                        };

                        TryLoadBinary(sHref, dAnnouncement);
                        arrItems.Add(dAnnouncement);
                    }
                }
            }
            else if (!sSectionClass.StartsWith("col-xl-8", StringComparison.Ordinal))
            {
                foreach (var dImage in GetDictionaryList(dSection, "imgs"))
                {
                    var sSource = Convert.ToString(dImage.GetValueOrDefault("src"), CultureInfo.InvariantCulture) ?? string.Empty;
                    var sDataOriginal = Convert.ToString(dImage.GetValueOrDefault("data-original"), CultureInfo.InvariantCulture) ?? string.Empty;
                    if (!string.IsNullOrEmpty(sSource) && dPage.ContainsKey(sSource))
                    {
                        dParentSection["id-anz"] = Convert.ToString(dSection.GetValueOrDefault("id"), CultureInfo.InvariantCulture) ?? string.Empty;
                        if (dPage[sSource] is Dictionary<string, object?> dPageMedia)
                        {
                            dPageMedia["id-anz"] = dParentSection["id-anz"];
                        }

                        dSection["filter"] = dPage["filter"];
                    }
                    else if (!string.IsNullOrEmpty(sDataOriginal) && dPage.ContainsKey($"https://trauer.rnz.de{sDataOriginal}"))
                    {
                        dParentSection["id-anz"] = Convert.ToString(dSection.GetValueOrDefault("id"), CultureInfo.InvariantCulture) ?? string.Empty;
                        if (dPage[$"https://trauer.rnz.de{sDataOriginal}"] is Dictionary<string, object?> dPageMedia)
                        {
                            dPageMedia["id-anz"] = dParentSection["id-anz"];
                        }

                        dSection["filter"] = dPage["filter"];
                    }
                }

                foreach (var dLink in GetDictionaryList(dSection, "links"))
                {
                    try
                    {
                        var sHref = Convert.ToString(dLink.GetValueOrDefault("href"), CultureInfo.InvariantCulture) ?? string.Empty;
                        var arrText = GetStringList(dLink, "text");
                        if (sHref.Contains("MEDIASERVER", StringComparison.Ordinal) && !arrMedia.Contains(sHref))
                        {
                            arrMedia.Add(sHref);
                        }

                        if ((arrText.Count == 1 && arrText[0] == "Speichern") || (arrText.Count == 1 && arrText[0] == "Großansicht"))
                        {
                            var dAnnouncement = new Dictionary<string, object?>
                            {
                                ["parent"] = sSubPageUrl,
                                [CsSrc] = sHref,
                                ["id"] = Convert.ToString(dSection.GetValueOrDefault("id"), CultureInfo.InvariantCulture) ?? string.Empty
                            };

                            if (Equals(dParentSection.GetValueOrDefault("id-anz"), dAnnouncement["id"]))
                            {
                                var arrImages = GetDictionaryList(dParentSection, "imgs");
                                arrImages.Add(new Dictionary<string, object?>(dAnnouncement));
                                dParentSection["imgs"] = arrImages;
                            }

                            TryLoadBinary(sHref, dAnnouncement);
                            if (sHref.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) && dAnnouncement.TryGetValue(CsData, out var xDataObject) && xDataObject is byte[] arrBinary)
                            {
                                var sPrefix = Encoding.ASCII.GetString(arrBinary.Take(10).ToArray());
                                if (sPrefix.Contains("PNG", StringComparison.Ordinal))
                                {
                                    dLink["href"] = sHref.Replace(".jpg", ".png", StringComparison.OrdinalIgnoreCase);
                                }
                            }

                            arrItems.Add(dAnnouncement);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

    private void TryLoadBinary(string sHref, Dictionary<string, object?> dTarget)
    {
        try
        {
            var xResponse = _xHttpClient.GetAsync(sHref).GetAwaiter().GetResult();
            dTarget[CsData] = xResponse.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            dTarget[CsHeader] = xResponse.Headers.Concat(xResponse.Content.Headers)
                .ToDictionary(h => h.Key, h => string.Join(",", h.Value), StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            dTarget[CsData] = Array.Empty<byte>();
            dTarget[CsHeader] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private static List<Dictionary<string, object?>> GetDictionaryList(IReadOnlyDictionary<string, object?> dValues, string sKey)
    {
        if (!dValues.TryGetValue(sKey, out var xValue) || xValue is null)
        {
            return [];
        }

        return xValue switch
        {
            List<Dictionary<string, object?>> arrReady => arrReady,
            IEnumerable<Dictionary<string, object?>> arrEnumerable => arrEnumerable.ToList(),
            _ => []
        };
    }

    private static List<string> GetStringList(IReadOnlyDictionary<string, object?> dValues, string sKey)
    {
        if (!dValues.TryGetValue(sKey, out var xValue) || xValue is null)
        {
            return [];
        }

        return xValue switch
        {
            List<object?> arrList => arrList.Select(v => Convert.ToString(v, CultureInfo.InvariantCulture) ?? string.Empty).ToList(),
            IEnumerable<string> arrStringList => arrStringList.ToList(),
            _ => []
        };
    }
}
