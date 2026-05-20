using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using RnzTrauer.Core;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class AmtsblattWebHandlerTests
{
    [TestMethod]
    public void FirefoxWebDriverFactory_Create_Uses_Injected_Creator()
    {
        var xDriver = Substitute.For<IWebDriver>();
        FirefoxOptions? xCapturedOptions = null;
        var xFactory = new FirefoxWebDriverFactory(xOptions =>
        {
            xCapturedOptions = xOptions;
            return xDriver;
        });

        var xResult = xFactory.Create();

        Assert.AreSame(xDriver, xResult);
        Assert.IsNotNull(xCapturedOptions);
    }

    [TestMethod]
    public void EdgeWebDriverFactory_Create_Uses_Injected_Creator()
    {
        var xDriver = Substitute.For<IWebDriver>();
        EdgeOptions? xCapturedOptions = null;
        var xFactory = new EdgeWebDriverFactory(xOptions =>
        {
            xCapturedOptions = xOptions;
            return xDriver;
        });

        var xResult = xFactory.Create();

        Assert.AreSame(xDriver, xResult);
        Assert.IsNotNull(xCapturedOptions);
    }

    [TestMethod]
    public void InitPage_Uses_Injected_DriverFactory_And_Sleeps_Until_Title_Matches()
    {
        var xConfig = new AmtsblattConfig
        {
            Url = "https://example.invalid/start",
            Title = "Loaded"
        };
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();
        var iTitleReads = 0;
        var iSleepCalls = 0;

        xWebDriverFactory.Create().Returns(xDriver);
        xDriver.Navigate().Returns(xNavigation);
        xDriver.Title.Returns(_ => ++iTitleReads == 1 ? "RNZ" : "Loaded");

        using var xHandler = new AmtsblattWebHandler(xConfig, xWebDriverFactory, _ => iSleepCalls++);

        xHandler.InitPage();

        _ = xWebDriverFactory.Received(1).Create();
        xNavigation.Received(1).GoToUrl("https://example.invalid/start");
        Assert.AreEqual(2, iSleepCalls);
        Assert.AreSame(xDriver, xHandler.Driver);
    }

    [TestMethod]
    public void GetData1_Throws_When_Driver_Not_Initialized()
    {
        using var xHandler = new AmtsblattWebHandler(new AmtsblattConfig(), Substitute.For<IWebDriverFactory>(), _ => { });

        try
        {
            _ = xHandler.GetData1("https://example.invalid");
            Assert.Fail("Expected InvalidOperationException was not thrown.");
        }
        catch (InvalidOperationException xException)
        {
            StringAssert.Contains(xException.Message, "initialized");
        }
    }

    [TestMethod]
    public void GetData1_Loads_Page_And_Clicks_Matching_Controls()
    {
        var xConfig = new AmtsblattConfig
        {
            Url = "https://example.invalid/start",
            Title = "Loaded"
        };
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();
        var xButtonAccept = Substitute.For<IWebElement>();
        var xButtonIgnore = Substitute.For<IWebElement>();
        var xLinkPdf = Substitute.For<IWebElement>();
        var xLinkIgnore = Substitute.For<IWebElement>();
        var sCurrentUrl = "https://example.invalid/start";

        xWebDriverFactory.Create().Returns(xDriver);
        xDriver.Navigate().Returns(xNavigation);
        xNavigation.When(xNav => xNav.GoToUrl(Arg.Any<string>())).Do(xCall => sCurrentUrl = xCall.Arg<string>());
        xDriver.Url.Returns(_ => sCurrentUrl);
        xDriver.PageSource.Returns("<html></html>");
        xDriver.Title.Returns("Loaded");

        xButtonAccept.Text.Returns("Alle akzeptieren");
        xButtonIgnore.Text.Returns("Weiter");
        xLinkPdf.Text.Returns("PDF herunterladen");
        xLinkIgnore.Text.Returns("Start");

        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("button").ToString())).Returns([xButtonAccept, xButtonIgnore]);
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString())).Returns([xLinkPdf, xLinkIgnore]);

        using var xHandler = new AmtsblattWebHandler(xConfig, xWebDriverFactory, _ => { });
        xHandler.InitPage();

        var (dPages, lstItems) = xHandler.GetData1("https://example.invalid/issue");

        xNavigation.Received(1).GoToUrl("https://example.invalid/issue");
        xButtonAccept.Received(1).Click();
        xButtonIgnore.DidNotReceive().Click();
        xLinkPdf.Received(1).Click();
        xLinkIgnore.DidNotReceive().Click();
        Assert.AreEqual(1, dPages.Count);
        Assert.AreEqual(0, lstItems.Count);
        Assert.AreEqual("Loaded", dPages["https://example.invalid/issue"]["Title"]);
    }

    [TestMethod]
    public void Close_Quits_Driver_And_Resets_Property()
    {
        var xConfig = new AmtsblattConfig
        {
            Url = "https://example.invalid/start",
            Title = "Loaded"
        };
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();

        xWebDriverFactory.Create().Returns(xDriver);
        xDriver.Navigate().Returns(xNavigation);
        xDriver.Title.Returns("Loaded");

        using var xHandler = new AmtsblattWebHandler(xConfig, xWebDriverFactory, _ => { });
        xHandler.InitPage();

        xHandler.Close();

        xDriver.Received(1).Quit();
        Assert.IsNull(xHandler.Driver);
    }

    [TestMethod]
    public void FirefoxWebDriverFactory_Default_Constructor_Uses_DefaultCreateDriver_Provider()
    {
        var xDriver = Substitute.For<IWebDriver>();
        Func<OpenQA.Selenium.Firefox.FirefoxOptions, IWebDriver> xOriginal = FirefoxWebDriverFactory.DefaultCreateDriver;

        try
        {
            FirefoxWebDriverFactory.DefaultCreateDriver = _ => xDriver;

            var xFactory = new FirefoxWebDriverFactory();
            var xResult = xFactory.Create();

            Assert.AreSame(xDriver, xResult);
        }
        finally
        {
            FirefoxWebDriverFactory.DefaultCreateDriver = xOriginal;
        }
    }

    [TestMethod]
    public void EdgeWebDriverFactory_Default_Constructor_Uses_DefaultCreateDriver_Provider()
    {
        var xDriver = Substitute.For<IWebDriver>();
        Func<OpenQA.Selenium.Edge.EdgeOptions, IWebDriver> xOriginal = EdgeWebDriverFactory.DefaultCreateDriver;

        try
        {
            EdgeWebDriverFactory.DefaultCreateDriver = _ => xDriver;

            var xFactory = new EdgeWebDriverFactory();
            var xResult = xFactory.Create();

            Assert.AreSame(xDriver, xResult);
        }
        finally
        {
            EdgeWebDriverFactory.DefaultCreateDriver = xOriginal;
        }
    }

    [TestMethod]
    public void AmtsblattWebHandler_Default_Constructor_Uses_DefaultWebDriverFactoryProvider()
    {
        var xDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();
        var xFactory = Substitute.For<IWebDriverFactory>();
        Func<IWebDriverFactory> xOriginal = AmtsblattWebHandler.DefaultWebDriverFactoryProvider;

        try
        {
            xFactory.Create().Returns(xDriver);
            xDriver.Navigate().Returns(xNavigation);
            xDriver.Title.Returns("Loaded");
            AmtsblattWebHandler.DefaultWebDriverFactoryProvider = () => xFactory;

            using var xHandler = new AmtsblattWebHandler(new AmtsblattConfig
            {
                Url = "https://example.invalid/start",
                Title = "Loaded"
            });

            xHandler.InitPage();

            _ = xFactory.Received(1).Create();
            xNavigation.Received(1).GoToUrl("https://example.invalid/start");
        }
        finally
        {
            AmtsblattWebHandler.DefaultWebDriverFactoryProvider = xOriginal;
        }
    }

    [TestMethod]
    public void GetData1_Waits_When_Title_Is_Rnz_Before_Processing_Page()
    {
        var xConfig = new AmtsblattConfig
        {
            Url = "https://example.invalid/start",
            Title = "Loaded"
        };
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();
        var iSleepCalls = 0;
        var iReads = 0;

        xWebDriverFactory.Create().Returns(xDriver);
        xDriver.Navigate().Returns(xNavigation);
        xNavigation.When(xNav => xNav.GoToUrl(Arg.Any<string>())).Do(_ => { });
        xDriver.Url.Returns("https://example.invalid/issue");
        xDriver.PageSource.Returns("<html></html>");
        xDriver.FindElements(Arg.Any<By>()).Returns([]);
        xDriver.Title.Returns(_ => ++iReads == 1 ? "Loaded" : iReads == 2 ? "RNZ" : "Issue Loaded");

        using var xHandler = new AmtsblattWebHandler(xConfig, xWebDriverFactory, _ => iSleepCalls++);
        xHandler.InitPage();

        var (dPages, _) = xHandler.GetData1("https://example.invalid/issue");

        Assert.IsTrue(iSleepCalls >= 2);
        Assert.AreEqual(1, dPages.Count);
    }
}
