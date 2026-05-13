using System;
using OpenQA.Selenium;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.Core;

/// <summary>
/// Selects the configured Selenium browser factory for RNZ scraping.
/// </summary>
public sealed class BrowserWebDriverFactory : IWebDriverFactory
{
    private readonly RnzConfig _config;
    private readonly IFirefoxWebDriverFactory _firefoxFactory;
    private readonly IEdgeWebDriverFactory _edgeFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="BrowserWebDriverFactory"/> class.
    /// </summary>
    public BrowserWebDriverFactory(RnzConfig xConfig, IFirefoxWebDriverFactory xFirefoxFactory, IEdgeWebDriverFactory xEdgeFactory)
    {
        _config = xConfig ?? throw new ArgumentNullException(nameof(xConfig));
        _firefoxFactory = xFirefoxFactory ?? throw new ArgumentNullException(nameof(xFirefoxFactory));
        _edgeFactory = xEdgeFactory ?? throw new ArgumentNullException(nameof(xEdgeFactory));
    }

    /// <inheritdoc />
    public IWebDriver Create() => _config.Browser switch
    {
        BrowserType.Edge => _edgeFactory.Create(),
        _ => _firefoxFactory.Create()
    };
}
