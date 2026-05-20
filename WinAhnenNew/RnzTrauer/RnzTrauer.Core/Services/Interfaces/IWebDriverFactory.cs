using OpenQA.Selenium;

namespace RnzTrauer.Core.Services.Interfaces;

/// <summary>
/// Creates Selenium web driver instances.
/// </summary>
public interface IWebDriverFactory
{
    /// <summary>
    /// Creates a new web driver instance.
    /// </summary>
    IWebDriver Create();
}
