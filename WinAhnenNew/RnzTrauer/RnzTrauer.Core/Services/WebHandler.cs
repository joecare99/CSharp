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

    private const string AnnouncementPrefix = "ANZ";
    private const string DataAttributeName = "data-original";
    private const string HtmlAttrAlt = "alt";
    private const string HtmlAttrClass = "class";
    private const string HtmlAttrHref = "href";
    private const string HtmlAttrId = "id";
    private const string HtmlAttrStyle = "style";
    private const string HtmlAttrTarget = "target";
    private const string HtmlAttrTitle = "title";
    private const string HtmlAttrSrc = "src";
    private const string HtmlClassDetailColumn = "col-sm-6";
    private const string HtmlClassContentColumn = "col-12";
    private const string HtmlClassWideColumn = "col-xl-8";
    private const string HtmlTagA = "a";
    private const string HtmlTagDiv = "div";
    private const string HtmlTagHeading1 = "h1";
    private const string HtmlTagImage = "img";
    private const string HtmlTagSection = "section";
    private const string HtmlTagBody = "body";
    private const string ImageFileJpg = ".jpg";
    private const string ImageFilePng = ".png";
    private const string ImageSignaturePng = "PNG";
    private const string KeyBirth = "Birth";
    private const string KeyContent = "content";
    private const string KeyCreatedBy = "created_by";
    private const string KeyCreatedOn = "created_on";
    private const string KeyDeath = "Death";
    private const string KeyFilter = "filter";
    private const string KeyHref = "href";
    private const string KeyId = "id";
    private const string KeyIdAnz = "id-anz";
    private const string KeyImages = "imgs";
    private const string KeyInfo = "Info";
    private const string KeyLinks = "links";
    private const string KeyMedia = "media";
    private const string KeyName = "name";
    private const string KeyParent = "parent";
    private const string KeyPlace = "Place";
    private const string KeySections = "sections";
    private const string KeyTag = "tag";
    private const string KeyText = "text";
    private const string KeyTitle = "Title";
    private const string KeyUrl = "url";
    private const string KeyVisits = "visits";
    private const string LoginEmailAddressId = "emailAddress";
    private const string LoginFormId = "form";
    private const string LoginPasswordId = "password";
    private const string MediaServerToken = "MEDIASERVER";
    private const string MissingDriverMessage = "The web driver has not been initialized.";
    private const string NextLinkMarker = ">";
    private const string PageBlockItemClass = "c-blockitem";
    private const string PagePathAnzeigen = "/anzeigen";
    private const string PagePathAnzeigenArt = "/anzeigenart-";
    private const string PagePathSeparator = "/";
    private const string PageTitleRnz = "RNZ";
    private const string BirthMarker = "*";
    private const string CommaSeparator = ",";
    private const string PlaceSeparator = "in";
    private const string SpaceSeparator = " ";
    private const string ErrorMarker404 = "404";
    private const string ErrorMarkerNotFound = "not found";
    private const string ErrorMarkerServiceTimeout = "service timeout";
    private const string ErrorMarkerTimeout = "timeout";
    private const string ErrorMarkerServiceUnavailable = "service unavailable";
    private const string ErrorMarkerTemporarilyUnavailable = "temporarily unavailable";
    private const string ErrorMarkerBadGateway = "bad gateway";
    private const string ErrorMarkerGatewayTimeout = "gateway timeout";
    private const string ErrorMarker504GatewayTimeOut = "504 gateway time-out";
    private const string ErrorMarkerServerDidntRespondInTime = "the server didn't respond in time";
    private const string ErrorMarkerServerDidNotRespondInTime = "the server did not respond in time";
    private const string ErrorMarkerRequestTimeout = "request timeout";
    private const string ErrorMarkerConnectionTimeout = "connection timeout";
    private const string ErrorMarkerOperationTimedOut = "operation timed out";
    private const int MaxReloadAttempts = 6;
    private const int NavigationWaitCycles = 20;
    private const int NavigationWaitMilliseconds = 500;

    // UI strings from the RNZ portal.
    private const string UiActionLargeView = "Großansicht";
    private const string UiActionSave = "Speichern";
    private const string UiLabelCreated = "Erstellt";
    private const string UiLabelCreatedOn = "Angelegt";
    private const string UiLabelVisits = "Besuche";
    private const string UiProgressDot = ".";
    private const string UiProgressGetSubPages = "\nGet Subpages:";

    private static readonly string[] AnnouncementTypes = ["nachrufe", "danksagungen", "todesanzeigen", "_"];

    private static readonly Dictionary<string, string[]> _tagAttributes = new(StringComparer.OrdinalIgnoreCase)
    {
        [HtmlTagSection] = [HtmlAttrClass, HtmlAttrId],
        [HtmlTagDiv] = [HtmlAttrClass, HtmlAttrId],
        [HtmlTagImage] = [HtmlAttrClass, HtmlAttrTitle, HtmlAttrAlt, CsSrc, HtmlAttrSrc, HtmlAttrStyle, DataAttributeName],
        [HtmlTagA] = [HtmlAttrTitle, HtmlAttrTarget, HtmlAttrHref]
    };

    private readonly RnzConfig _config;
    private readonly string _sBaseHost;
    private readonly IHttpClientProxy _xHttpClient;
    private readonly IWebDriverFactory _xWebDriverFactory;
    private readonly IProgress<WebHandlerProgress>? _xProgress;
    private readonly HashSet<string> _hsIndexedSubPages = new(StringComparer.Ordinal);
    private readonly Dictionary<string, byte[]> _dIndexedBinaryData = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Dictionary<string, string>> _dIndexedBinaryHeaders = new(StringComparer.Ordinal);

    /// <summary>
    /// Initializes a new instance of the <see cref="WebHandler"/> class.
    /// </summary>
    public WebHandler(RnzConfig xConfig, IHttpClientProxy xHttpClient, IWebDriverFactory xWebDriverFactory, IProgress<WebHandlerProgress>? xProgress = null)
    {
        _config = xConfig;
        _sBaseHost = Uri.TryCreate(_config.Url, UriKind.Absolute, out var xUri)
            ? xUri.GetLeftPart(UriPartial.Authority)
            : string.Empty;
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
        Driver.FindElement(By.Id(LoginEmailAddressId)).SendKeys(_config.User);
        Driver.FindElement(By.Id(LoginPasswordId)).SendKeys(_config.Password);
        Driver.FindElement(By.Id(LoginFormId)).Submit();
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
        IReadOnlyCollection<IWebElement> arrElements;
        try
        {
            arrElements = xWebElement.FindElements(xQuery.By);
        }
        catch (StaleElementReferenceException)
        {
            return arrResult;
        }

        foreach (var xElement in arrElements)
        {
            string sTagName;
            try
            {
                sTagName = xElement.TagName;
            }
            catch (StaleElementReferenceException)
            {
                continue;
            }

            var dItem = new Dictionary<string, object?>
            {
                [KeyTag] = sTagName
            };
            arrResult.Add(dItem);

            if (_tagAttributes.TryGetValue(sTagName, out var arrAttributes))
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
                dItem[KeyText] = (xElement.Text ?? string.Empty).Split(Environment.NewLine, StringSplitOptions.None).ToList<object?>();
            }
            catch
            {
            }
            try
            {
                foreach (var xChildQuery in xQuery.Children)
                {
                    dItem[xChildQuery.Name] = Wdr2List(xElement, xChildQuery);
                }
            }
            catch
            {
            }
        }
        return arrResult;
    }

    /// <summary>
    /// Loads the configured RNZ pages and associated media.
    /// </summary>
    public (Dictionary<string, Dictionary<string, object?>> Pages, List<Dictionary<string, object?>> Items) GetData1(string sStart, int iMaxPage = 30)
    {
        var xDriver = Driver ?? throw new InvalidOperationException(MissingDriverMessage);
        var dPages = new Dictionary<string, Dictionary<string, object?>>(StringComparer.Ordinal);
        var arrItems = new List<Dictionary<string, object?>>();

        foreach (var sAnnouncementType in AnnouncementTypes)
        {
            if (!NavigateWithReloadRetry($"{sStart}{PagePathAnzeigenArt}{sAnnouncementType}"))
            {
                continue;
            }

            var sNextUrl = UiProgressDot;
            var iCounter = 0;
            while (!string.IsNullOrEmpty(sNextUrl) && iCounter < iMaxPage)
            {
                _xProgress?.Report(new WebHandlerProgress(xDriver.Url, true));
                List<string> arrSubPages;
                string sUrl;
                string sNext;
                try
                {
                    (arrSubPages, sUrl, sNext) = WorkMainPage(dPages, arrItems, sAnnouncementType);
                }
                catch
                {
                    (arrSubPages, sUrl, sNext) = WorkMainPage(dPages, arrItems, sAnnouncementType);
                }
                sNextUrl = sNext;

                _xProgress?.Report(new WebHandlerProgress(UiProgressGetSubPages));
                foreach (var sSubPage in arrSubPages)
                {
                    var sSubPageUrl = sSubPage + PagePathAnzeigen;
                    if (_hsIndexedSubPages.Contains(sSubPageUrl))
                    {
                        continue;
                    }

                    if (!NavigateWithReloadRetry(sSubPageUrl))
                    {
                        continue;
                    }

                    WorkSubPage(sUrl, dPages, arrItems);
                    _hsIndexedSubPages.Add(sSubPageUrl);
                }

                _xProgress?.Report(new WebHandlerProgress(string.Empty, true));
                if (!string.IsNullOrEmpty(sNextUrl))
                {
                    if (!NavigateWithReloadRetry(sNextUrl))
                    {
                        sNextUrl = string.Empty;
                    }
                    else
                    {
                        while (xDriver.Url == sUrl)
                        {
                            Thread.Sleep(NavigationWaitMilliseconds);
                            _xProgress?.Report(new WebHandlerProgress(UiProgressDot));
                        }
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
        var xDriver = Driver ?? throw new InvalidOperationException(MissingDriverMessage);
        var sUrl = xDriver.Url;
        var dPage = new Dictionary<string, object?>
        {
            [KeyTitle] = xDriver.Title,
            [KeyUrl] = sUrl,
            [KeyFilter] = sAnnouncementType,
            [KeySections] = new List<Dictionary<string, object?>>(),
            [KeyContent] = PortedHelpers.MakeLocal(xDriver.PageSource, sUrl.Length > 50 ? sUrl[..50] + PagePathSeparator : sUrl + PagePathSeparator)
        };
        var arrSubPages = new List<string>();
        dPages[sUrl] = dPage;

        var arrElements = Wdr2List(xDriver, new WebQuery(string.Empty, By.ClassName(PageBlockItemClass), new WebQuery(KeyLinks, By.TagName(HtmlTagA)), new WebQuery(KeyImages, By.TagName(HtmlTagImage))));
        dPage[KeySections] = arrElements;
        var sNextUrl = string.Empty;

        foreach (var dElement in arrElements)
        {
            var arrText = GetStringList(dElement, KeyText);
            if (arrText.Count > 1 && arrText[0].StartsWith(AnnouncementPrefix, StringComparison.Ordinal))
            {
                var dAnnouncement = new Dictionary<string, object?>
                {
                    [KeyTitle] = arrText[0].Length >= 8 ? arrText[0][8..] : arrText[0],
                    [KeyParent] = sUrl,
                    [KeyText] = arrText.Cast<object?>().ToList()
                };

                if (arrText.Count > 1)
                {
                    dAnnouncement[KeyInfo] = arrText[1];
                }

                var arrLinks = GetDictionaryList(dElement, KeyLinks);
                if (arrLinks.Count > 0)
                {
                    var sHref = Convert.ToString(arrLinks[0].GetValueOrDefault(KeyHref), CultureInfo.InvariantCulture) ?? string.Empty;
                    dAnnouncement[KeyHref] = sHref;
                    arrSubPages.Add(sHref);
                }

                var arrImages = GetDictionaryList(dElement, KeyImages);
                if (arrImages.Count > 0)
                {
                    foreach (var dImage in arrImages)
                    {
                        var dCopy = new Dictionary<string, object?>(dAnnouncement, StringComparer.Ordinal);
                        dCopy[CsSrc] = string.Empty;
                        var sSource = Convert.ToString(dImage.GetValueOrDefault(CsSrc), CultureInfo.InvariantCulture) ?? string.Empty;
                        if (string.IsNullOrEmpty(sSource))
                        {
                            sSource = Convert.ToString(dImage.GetValueOrDefault(HtmlAttrSrc), CultureInfo.InvariantCulture) ?? string.Empty;
                        }

                        var sDataOriginal = Convert.ToString(dImage.GetValueOrDefault(DataAttributeName), CultureInfo.InvariantCulture) ?? string.Empty;
                        if (sSource.Contains(MediaServerToken, StringComparison.Ordinal))
                        {
                            dCopy[CsSrc] = sSource;
                        }
                        else if (sDataOriginal.Contains(MediaServerToken, StringComparison.Ordinal))
                        {
                            dCopy[CsSrc] = sDataOriginal;
                        }

                        _xProgress?.Report(new WebHandlerProgress(UiProgressDot));
                        try
                        {
                            var sMediaSource = Convert.ToString(dCopy[CsSrc], CultureInfo.InvariantCulture) ?? string.Empty;
                            if (!string.IsNullOrEmpty(sMediaSource))
                            {
                                TryLoadBinary(sMediaSource, dCopy);
                                if (sMediaSource.Contains(MediaServerToken, StringComparison.Ordinal))
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
            else if (arrText.Count > 1 && string.Join(SpaceSeparator, arrText).Contains(NextLinkMarker, StringComparison.Ordinal) && string.IsNullOrEmpty(sNextUrl))
            {
                var arrLinks = GetDictionaryList(dElement, KeyLinks);
                foreach (var dLink in arrLinks)
                {
                    var arrLinkText = GetStringList(dLink, KeyText);
                    if (arrLinkText.Count == 1 && arrLinkText[0] == NextLinkMarker)
                    {
                        sNextUrl = Convert.ToString(dLink.GetValueOrDefault(KeyHref), CultureInfo.InvariantCulture) ?? string.Empty;
                    }
                }

                _xProgress?.Report(new WebHandlerProgress(sNextUrl, true));
            }
        }

        return (arrSubPages, sUrl, sNextUrl);
    }

    private void WorkSubPage(string sUrl, Dictionary<string, Dictionary<string, object?>> dPages, List<Dictionary<string, object?>> arrItems)
    {
        var xDriver = Driver ?? throw new InvalidOperationException(MissingDriverMessage);
        var dPage = dPages[sUrl];
        var sSubPageUrl = xDriver.Url;
        if (!dPages.TryGetValue(sSubPageUrl, out var dPage2))
        {
            dPage2 = new Dictionary<string, object?>
            {
                [KeyParent] = new List<object?>()
            };
            dPages[sSubPageUrl] = dPage2;
        }

        Dictionary<string, object?> dParentSection = new(StringComparer.Ordinal);
        var arrPageSections = GetDictionaryList(dPage, KeySections);
        for (var iIndex = 0; iIndex < arrPageSections.Count; iIndex++)
        {
            var dSection = arrPageSections[iIndex];
            var arrLinks = GetDictionaryList(dSection, KeyLinks);
            var sHref = arrLinks.Count > 0 ? Convert.ToString(arrLinks[0].GetValueOrDefault(KeyHref), CultureInfo.InvariantCulture) ?? string.Empty : string.Empty;
            if (!string.IsNullOrEmpty(sHref) && sHref + PagePathAnzeigen == sSubPageUrl)
            {
                dParentSection = dSection;
                arrPageSections[iIndex] = dSection;
            }
        }

        dPage2[KeyTitle] = xDriver.Title;
        dPage2[KeyUrl] = sSubPageUrl;
        if (dPage2[KeyParent] is not List<object?> arrParents)
        {
            arrParents = new List<object?>();
            dPage2[KeyParent] = arrParents;
        }

        arrParents.Add(sUrl);
        dPage2[KeyContent] = PortedHelpers.MakeLocal(xDriver.PageSource, sSubPageUrl + PagePathSeparator);
        var arrMedia = new List<object?>();
        dPage2[KeyMedia] = arrMedia;
        var arrSections = Wdr2List(xDriver, new WebQuery(string.Empty, By.TagName(HtmlTagSection), new WebQuery(KeyLinks, By.TagName(HtmlTagA)), new WebQuery(KeyImages, By.TagName(HtmlTagImage))));
        dPage2[KeySections] = arrSections;

        foreach (var xElement in xDriver.FindElements(By.TagName(HtmlTagHeading1)))
        {
            dPage2[KeyName] = xElement.Text;
        }

        foreach (var xElement in xDriver.FindElements(By.ClassName(HtmlClassDetailColumn)))
        {
            if (xElement.Text.StartsWith(BirthMarker, StringComparison.Ordinal))
            {
                dPage2[KeyBirth] = xElement.Text;
            }
            else if (xElement.Text.Contains('†'))
            {
                var arrParts = xElement.Text.Split(PlaceSeparator, 2, StringSplitOptions.None);
                dPage2[KeyDeath] = arrParts[0];
                dPage2[KeyPlace] = arrParts.Length > 1 ? arrParts[1] : string.Empty;
            }
        }

        _xProgress?.Report(new WebHandlerProgress(UiProgressDot));
        foreach (var dSection in arrSections)
        {
            var sSectionClass = Convert.ToString(dSection.GetValueOrDefault(HtmlAttrClass), CultureInfo.InvariantCulture) ?? string.Empty;
            if (sSectionClass.StartsWith(HtmlClassContentColumn, StringComparison.Ordinal))
            {
                foreach (var sLine in GetStringList(dSection, KeyText))
                {
                    if (sLine.StartsWith(UiLabelCreated, StringComparison.Ordinal))
                    {
                        dPage2[KeyCreatedBy] = sLine.Length > 12 ? sLine[12..] : string.Empty;
                    }
                    else if (sLine.StartsWith(UiLabelCreatedOn, StringComparison.Ordinal))
                    {
                        dPage2[KeyCreatedOn] = sLine.Length > 11 ? sLine[11..] : string.Empty;
                    }
                    else if (sLine.EndsWith(UiLabelVisits, StringComparison.Ordinal))
                    {
                        dPage2[KeyVisits] = sLine.Length > 7 ? sLine[..^7] : string.Empty;
                    }
                }

                foreach (var dImage in GetDictionaryList(dSection, KeyImages))
                {
                    var sHref = Convert.ToString(dImage.GetValueOrDefault(CsSrc), CultureInfo.InvariantCulture) ?? string.Empty;
                    if (string.IsNullOrEmpty(sHref))
                    {
                        sHref = Convert.ToString(dImage.GetValueOrDefault(HtmlAttrSrc), CultureInfo.InvariantCulture) ?? string.Empty;
                    }

                    var sText = Convert.ToString(dImage.GetValueOrDefault(HtmlAttrAlt), CultureInfo.InvariantCulture) ?? string.Empty;
                    if (sHref.Contains(MediaServerToken, StringComparison.Ordinal) && !arrMedia.Contains(sHref))
                    {
                        arrMedia.Add(sHref);
                        var dAnnouncement = new Dictionary<string, object?>
                        {
                            [KeyParent] = sSubPageUrl,
                            [CsSrc] = sHref,
                            [KeyInfo] = sText,
                            [KeyId] = string.Empty
                        };

                        TryLoadBinary(sHref, dAnnouncement);
                        arrItems.Add(dAnnouncement);
                    }
                }
            }
            else if (!sSectionClass.StartsWith(HtmlClassWideColumn, StringComparison.Ordinal))
            {
                foreach (var dImage in GetDictionaryList(dSection, KeyImages))
                {
                    var sSource = Convert.ToString(dImage.GetValueOrDefault(CsSrc), CultureInfo.InvariantCulture) ?? string.Empty;
                    if (string.IsNullOrEmpty(sSource))
                    {
                        sSource = Convert.ToString(dImage.GetValueOrDefault(HtmlAttrSrc), CultureInfo.InvariantCulture) ?? string.Empty;
                    }

                    var sDataOriginal = Convert.ToString(dImage.GetValueOrDefault(DataAttributeName), CultureInfo.InvariantCulture) ?? string.Empty;
                    if (!string.IsNullOrEmpty(sSource) && dPage.ContainsKey(sSource))
                    {
                        dParentSection[KeyIdAnz] = Convert.ToString(dSection.GetValueOrDefault(KeyId), CultureInfo.InvariantCulture) ?? string.Empty;
                        if (dPage[sSource] is Dictionary<string, object?> dPageMedia)
                        {
                            dPageMedia[KeyIdAnz] = dParentSection[KeyIdAnz];
                        }

                        dSection[KeyFilter] = dPage[KeyFilter];
                    }
                    else if (!string.IsNullOrEmpty(sDataOriginal) && dPage.ContainsKey($"{_sBaseHost}{sDataOriginal}"))
                    {
                        dParentSection[KeyIdAnz] = Convert.ToString(dSection.GetValueOrDefault(KeyId), CultureInfo.InvariantCulture) ?? string.Empty;
                        if (dPage[$"{_sBaseHost}{sDataOriginal}"] is Dictionary<string, object?> dPageMedia)
                        {
                            dPageMedia[KeyIdAnz] = dParentSection[KeyIdAnz];
                        }

                        dSection[KeyFilter] = dPage[KeyFilter];
                    }
                }

                foreach (var dLink in GetDictionaryList(dSection, KeyLinks))
                {
                    try
                    {
                        var sHref = Convert.ToString(dLink.GetValueOrDefault(KeyHref), CultureInfo.InvariantCulture) ?? string.Empty;
                        var arrText = GetStringList(dLink, KeyText);
                        if (sHref.Contains(MediaServerToken, StringComparison.Ordinal) && !arrMedia.Contains(sHref))
                        {
                            arrMedia.Add(sHref);
                        }

                        if ((arrText.Count == 1 && arrText[0] == UiActionSave) || (arrText.Count == 1 && arrText[0] == UiActionLargeView))
                        {
                            var dAnnouncement = new Dictionary<string, object?>
                            {
                                [KeyParent] = sSubPageUrl,
                                [CsSrc] = sHref,
                                [KeyId] = Convert.ToString(dSection.GetValueOrDefault(KeyId), CultureInfo.InvariantCulture) ?? string.Empty
                            };

                            if (Equals(dParentSection.GetValueOrDefault(KeyIdAnz), dAnnouncement[KeyId]))
                            {
                                var arrImages = GetDictionaryList(dParentSection, KeyImages);
                                arrImages.Add(new Dictionary<string, object?>(dAnnouncement));
                                dParentSection[KeyImages] = arrImages;
                            }

                            TryLoadBinary(sHref, dAnnouncement);
                            if (sHref.EndsWith(ImageFileJpg, StringComparison.OrdinalIgnoreCase) && dAnnouncement.TryGetValue(CsData, out var xDataObject) && xDataObject is byte[] arrBinary)
                            {
                                var sPrefix = Encoding.ASCII.GetString(arrBinary.Take(10).ToArray());
                                if (sPrefix.Contains(ImageSignaturePng, StringComparison.Ordinal))
                                {
                                    dLink[KeyHref] = sHref.Replace(ImageFileJpg, ImageFilePng, StringComparison.OrdinalIgnoreCase);
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
        if (_dIndexedBinaryData.TryGetValue(sHref, out var arrIndexedData))
        {
            dTarget[CsData] = arrIndexedData;
            dTarget[CsHeader] = _dIndexedBinaryHeaders.TryGetValue(sHref, out var dIndexedHeaders)
                ? new Dictionary<string, string>(dIndexedHeaders, StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            return;
        }

        try
        {
            var xResponse = _xHttpClient.GetAsync(sHref).GetAwaiter().GetResult();
            var arrData = xResponse.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
            var dHeaders = xResponse.Headers.Concat(xResponse.Content.Headers)
                .ToDictionary(h => h.Key, h => string.Join(CommaSeparator, h.Value), StringComparer.OrdinalIgnoreCase);

            dTarget[CsData] = arrData;
            dTarget[CsHeader] = dHeaders;
            _dIndexedBinaryData[sHref] = arrData;
            _dIndexedBinaryHeaders[sHref] = new Dictionary<string, string>(dHeaders, StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            dTarget[CsData] = Array.Empty<byte>();
            dTarget[CsHeader] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _dIndexedBinaryData[sHref] = Array.Empty<byte>();
            _dIndexedBinaryHeaders[sHref] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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

    private bool NavigateWithReloadRetry(string sUrl)
    {
        var xDriver = Driver ?? throw new InvalidOperationException(MissingDriverMessage);
        var iAttempt = 0;
        while (iAttempt < MaxReloadAttempts)
        {
            iAttempt++;
            try
            {
                xDriver.Navigate().GoToUrl(sUrl);
            }
            catch (WebDriverException)
            {
                _xProgress.Report(new WebHandlerProgress($"Navigation attempt {iAttempt} to {sUrl} failed with WebDriverException."));
                Thread.Sleep(3000);
                continue;
            }
            WaitForPageLoadCompletion(xDriver);
            if (HasMeaningfulPageContent(xDriver))
            {
                return true;
            }
            _xProgress.Report(new WebHandlerProgress("O"));
            Thread.Sleep(30000);
        }

        return false;
    }

    private static void WaitForPageLoadCompletion(IWebDriver xDriver)
    {
        var iCycle = 0;
        while ((xDriver.Title == PageTitleRnz || string.IsNullOrEmpty(xDriver.Title)) && iCycle < NavigationWaitCycles)
        {
            Thread.Sleep(NavigationWaitMilliseconds);
            iCycle++;
        }

        Thread.Sleep(NavigationWaitMilliseconds);
    }

    private static bool HasMeaningfulPageContent(IWebDriver xDriver)
    {
        var sTitle = xDriver.Title ?? string.Empty;
        var sBodyText = TryGetBodyText(xDriver);

        if (string.IsNullOrWhiteSpace(sTitle) && string.IsNullOrWhiteSpace(sBodyText))
        {
            return false;
        }

        return !ContainsErrorMarker(sTitle) && !ContainsErrorMarker(sBodyText);
    }

    private static string TryGetBodyText(ISearchContext xSearchContext)
    {
        try
        {
            var arrBodies = xSearchContext.FindElements(By.TagName(HtmlTagBody));
            foreach (var xBody in arrBodies)
            {
                try
                {
                    return xBody.Text ?? string.Empty;
                }
                catch (StaleElementReferenceException)
                {
                }
            }
        }
        catch (WebDriverException)
        {
        }

        return string.Empty;
    }

    private static bool ContainsErrorMarker(string sText)
    {
        if (string.IsNullOrWhiteSpace(sText))
        {
            return false;
        }

        var sNormalized = sText.ToLowerInvariant()
            .Replace('’', '\'')
            .Replace('‑', '-')
            .Replace('–', '-');

        return sNormalized.Contains(ErrorMarker404, StringComparison.Ordinal) && sNormalized.Length < 500
            || sNormalized.Contains(ErrorMarkerNotFound, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerServiceTimeout, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerServiceUnavailable, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerTemporarilyUnavailable, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerBadGateway, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerGatewayTimeout, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarker504GatewayTimeOut, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerServerDidntRespondInTime, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerServerDidNotRespondInTime, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerRequestTimeout, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerConnectionTimeout, StringComparison.Ordinal)
            || sNormalized.Contains(ErrorMarkerOperationTimedOut, StringComparison.Ordinal);
    }
}
