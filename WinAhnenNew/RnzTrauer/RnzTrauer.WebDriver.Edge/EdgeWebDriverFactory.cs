using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.Core;

/// <summary>
/// Creates Edge-based Selenium web drivers.
/// </summary>
public sealed class EdgeWebDriverFactory : IEdgeWebDriverFactory
{
    public static Func<EdgeOptions, IWebDriver> DefaultCreateDriver { get; set; } = xOptions => new EdgeDriver(xOptions);

    private readonly Func<EdgeOptions, IWebDriver> _xCreateDriver;

    /// <summary>
    /// Initializes a new instance of the <see cref="EdgeWebDriverFactory"/> class.
    /// </summary>
    public EdgeWebDriverFactory()
        : this(DefaultCreateDriver)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EdgeWebDriverFactory"/> class with a custom driver creator.
    /// </summary>
    public EdgeWebDriverFactory(Func<EdgeOptions, IWebDriver> xCreateDriver)
    {
        _xCreateDriver = xCreateDriver ?? throw new ArgumentNullException(nameof(xCreateDriver));
    }

    /// <inheritdoc />
    public IWebDriver Create()
    {
        var xOptions = new EdgeOptions();
        return _xCreateDriver(xOptions);
    }
}
