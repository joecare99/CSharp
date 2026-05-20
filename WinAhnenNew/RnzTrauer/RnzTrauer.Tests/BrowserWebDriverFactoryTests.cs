using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OpenQA.Selenium;
using RnzTrauer.Core;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class BrowserWebDriverFactoryTests
{
    [TestMethod]
    public void Create_Uses_Edge_Factory_When_Config_Is_Edge()
    {
        var xConfig = new RnzConfig
        {
            Browser = BrowserType.Edge
        };
        var xFirefoxFactory = Substitute.For<IFirefoxWebDriverFactory>();
        var xEdgeFactory = Substitute.For<IEdgeWebDriverFactory>();
        var xEdgeDriver = Substitute.For<IWebDriver>();
        xEdgeFactory.Create().Returns(xEdgeDriver);

        var xFactory = new BrowserWebDriverFactory(xConfig, xFirefoxFactory, xEdgeFactory);

        var xResult = xFactory.Create();

        Assert.AreSame(xEdgeDriver, xResult);
        _ = xEdgeFactory.Received(1).Create();
        _ = xFirefoxFactory.DidNotReceive().Create();
    }

    [TestMethod]
    public void Create_Uses_Firefox_Factory_When_Config_Is_Default()
    {
        var xConfig = new RnzConfig();
        var xFirefoxFactory = Substitute.For<IFirefoxWebDriverFactory>();
        var xEdgeFactory = Substitute.For<IEdgeWebDriverFactory>();
        var xFirefoxDriver = Substitute.For<IWebDriver>();
        xFirefoxFactory.Create().Returns(xFirefoxDriver);

        var xFactory = new BrowserWebDriverFactory(xConfig, xFirefoxFactory, xEdgeFactory);

        var xResult = xFactory.Create();

        Assert.AreSame(xFirefoxDriver, xResult);
        _ = xFirefoxFactory.Received(1).Create();
        _ = xEdgeFactory.DidNotReceive().Create();
    }
}
