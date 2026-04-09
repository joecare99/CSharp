using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace RnzTrauer.Core;

/// <summary>
/// Provides Selenium-based scraping for the Amtsblatt loader.
/// </summary>
public sealed class AmtsblattWebHandler : IDisposable
{
    private readonly AmtsblattConfig _config;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmtsblattWebHandler"/> class.
    /// </summary>
    public AmtsblattWebHandler(AmtsblattConfig xConfig)
    {
        _config = xConfig;
    }

    /// <summary>
    /// Gets the active Firefox driver instance.
    /// </summary>
    public FirefoxDriver? Driver { get; private set; }

    /// <summary>
    /// Opens the configured start page.
    /// </summary>
    public void InitPage()
    {
        var xOptions = new FirefoxOptions();
        Driver = new FirefoxDriver(xOptions);
        Driver.Navigate().GoToUrl(_config.Url);
        while (Driver.Title != _config.Title)
        {
            Thread.Sleep(500);
        }

        Thread.Sleep(500);
    }

    /// <summary>
    /// Loads the target issue page and triggers the PDF download action.
    /// </summary>
    public (Dictionary<string, Dictionary<string, object?>> Pages, List<Dictionary<string, object?>> Items) GetData1(string sStart)
    {
        var xDriver = Driver ?? throw new InvalidOperationException("The web driver has not been initialized.");
        xDriver.Navigate().GoToUrl(sStart);
        while (xDriver.Title == "RNZ" || string.IsNullOrEmpty(xDriver.Title))
        {
            Thread.Sleep(500);
        }

        var dPages = new Dictionary<string, Dictionary<string, object?>>(StringComparer.Ordinal);
        var arrItems = new List<Dictionary<string, object?>>();
        var sNextUrl = ".";
        var iCounter = 0;
        while (!string.IsNullOrEmpty(sNextUrl) && iCounter < 20)
        {
            Console.WriteLine(xDriver.Url);
            (_, _, sNextUrl) = WorkMainPage(dPages, arrItems);
            iCounter++;
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
    }

    private (List<string> SubPages, string Url, string NextUrl) WorkMainPage(Dictionary<string, Dictionary<string, object?>> dPages, List<Dictionary<string, object?>> arrItems)
    {
        var xDriver = Driver ?? throw new InvalidOperationException("The web driver has not been initialized.");
        var sUrl = xDriver.Url;
        var dPage = new Dictionary<string, object?>
        {
            ["Title"] = xDriver.Title,
            ["url"] = sUrl,
            ["sections"] = new List<Dictionary<string, object?>>(),
            ["content"] = xDriver.PageSource
        };
        dPages[sUrl] = dPage;

        Thread.Sleep(4000);
        foreach (var xButton in xDriver.FindElements(By.TagName("button")))
        {
            if ((xButton.Text ?? string.Empty).StartsWith("Alle akz", StringComparison.Ordinal))
            {
                xButton.Click();
            }
        }

        foreach (var xLink in xDriver.FindElements(By.TagName("a")))
        {
            if ((xLink.Text ?? string.Empty).StartsWith("PDF her", StringComparison.Ordinal))
            {
                xLink.Click();
            }
        }

        return ([], sUrl, string.Empty);
    }
}
