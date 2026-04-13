using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace RnzTrauer.Core;

/// <summary>
/// Creates Firefox-based Selenium web drivers.
/// </summary>
public sealed class FirefoxWebDriverFactory : IWebDriverFactory
{
    /// <inheritdoc />
    public IWebDriver Create()
    {
        var xOptions = new FirefoxOptions();
        return new FirefoxDriver(xOptions);
    }
}
