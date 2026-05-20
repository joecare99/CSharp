using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.Core;

/// <summary>
/// Creates Firefox-based Selenium web drivers.
/// </summary>
public sealed class FirefoxWebDriverFactory : IFirefoxWebDriverFactory
{
    public static Func<FirefoxOptions, IWebDriver> DefaultCreateDriver { get; set; } = xOptions => new FirefoxDriver(xOptions);

    private readonly Func<FirefoxOptions, IWebDriver> _xCreateDriver;

    /// <summary>
    /// Initializes a new instance of the <see cref="FirefoxWebDriverFactory"/> class.
    /// </summary>
    public FirefoxWebDriverFactory()
        : this(DefaultCreateDriver)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirefoxWebDriverFactory"/> class with a custom driver creator.
    /// </summary>
    public FirefoxWebDriverFactory(Func<FirefoxOptions, IWebDriver> xCreateDriver)
    {
        _xCreateDriver = xCreateDriver ?? throw new ArgumentNullException(nameof(xCreateDriver));
    }

    /// <inheritdoc />
    public IWebDriver Create()
    {
        var xOptions = new FirefoxOptions();
        return _xCreateDriver(xOptions);
    }
}
