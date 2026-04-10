using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OpenQA.Selenium;
using RnzTrauer.Core;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class WebHandlerTests
{
    [TestMethod]
    public void InitPage_Uses_Injected_DriverFactory_And_Submits_Login()
    {
        var xConfig = new RnzConfig
        {
            Url = "https://example.invalid/login",
            User = "user@example.invalid",
            Password = "secret",
            Title = "RNZ"
        };
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();
        var xEmailElement = Substitute.For<IWebElement>();
        var xPasswordElement = Substitute.For<IWebElement>();
        var xFormElement = Substitute.For<IWebElement>();

        xDriver.Navigate().Returns(xNavigation);
        xDriver.Title.Returns("Loaded");
        xDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("emailAddress").ToString())).Returns(xEmailElement);
        xDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("password").ToString())).Returns(xPasswordElement);
        xDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("form").ToString())).Returns(xFormElement);
        xWebDriverFactory.Create().Returns(xDriver);

        using var xHandler = new WebHandler(xConfig, xHttpClient, xWebDriverFactory);

        xHandler.InitPage();

        _ = xWebDriverFactory.Received(1).Create();
        xNavigation.Received(1).GoToUrl(xConfig.Url);
        xEmailElement.Received(1).SendKeys(xConfig.User);
        xPasswordElement.Received(1).SendKeys(xConfig.Password);
        xFormElement.Received(1).Submit();
        Assert.AreSame(xDriver, xHandler.Driver);
    }

    [TestMethod]
    public void Wdr2List_Reads_Attributes_Text_And_Children()
    {
        var xConfig = new RnzConfig();
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xSearchContext = Substitute.For<ISearchContext>();
        var xParentElement = Substitute.For<IWebElement>();
        var xChildElement = Substitute.For<IWebElement>();

        xSearchContext.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("div").ToString()))
            .Returns(CreateCollection(xParentElement));
        xParentElement.TagName.Returns("div");
        xParentElement.Text.Returns($"Line1{Environment.NewLine}Line2");
        xParentElement.GetAttribute("class").Returns("c-blockitem");
        xParentElement.GetAttribute("id").Returns(_ => throw new InvalidOperationException());
        xParentElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString()))
            .Returns(CreateCollection(xChildElement));

        xChildElement.TagName.Returns("a");
        xChildElement.Text.Returns("Read more");
        xChildElement.GetAttribute("title").Returns("title");
        xChildElement.GetAttribute("target").Returns("_blank");
        xChildElement.GetAttribute("href").Returns("https://example.invalid/item");
        xChildElement.FindElements(Arg.Any<By>()).Returns(CreateCollection());

        using var xHandler = new WebHandler(xConfig, xHttpClient, xWebDriverFactory);

        var lstResult = xHandler.Wdr2List(xSearchContext, new WebQuery(string.Empty, By.TagName("div"), new WebQuery("links", By.TagName("a"))));

        Assert.AreEqual(1, lstResult.Count);
        Assert.AreEqual("div", lstResult[0]["tag"]);
        Assert.AreEqual("c-blockitem", lstResult[0]["class"]);
        Assert.AreEqual(string.Empty, lstResult[0]["id"]);
        CollectionAssert.AreEqual(new object?[] { "Line1", "Line2" }, (System.Collections.ICollection)lstResult[0]["text"]!);
        var lstLinks = (List<Dictionary<string, object?>>)lstResult[0]["links"]!;
        Assert.AreEqual(1, lstLinks.Count);
        Assert.AreEqual("a", lstLinks[0]["tag"]);
        Assert.AreEqual("https://example.invalid/item", lstLinks[0]["href"]);
    }

    [TestMethod]
    public void GetData1_Throws_When_Driver_Has_Not_Been_Initialized()
    {
        using var xHandler = new WebHandler(new RnzConfig(), Substitute.For<IHttpClientProxy>(), Substitute.For<IWebDriverFactory>());

        try
        {
            _ = xHandler.GetData1("https://example.invalid/start");
            Assert.Fail("Expected InvalidOperationException was not thrown.");
        }
        catch (InvalidOperationException xException)
        {
            StringAssert.Contains(xException.Message, "initialized");
        }
    }

    [TestMethod]
    public void GetData1_Loads_Announcement_Media_Using_Injected_Dependencies()
    {
        var xConfig = new RnzConfig
        {
            Url = "https://example.invalid/login",
            User = "user@example.invalid",
            Password = "secret",
            Title = "RNZ"
        };
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xProgress = Substitute.For<IProgress<WebHandlerProgress>>();
        var xDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();
        var xEmailElement = Substitute.For<IWebElement>();
        var xPasswordElement = Substitute.For<IWebElement>();
        var xFormElement = Substitute.For<IWebElement>();
        var xAnnouncementElement = Substitute.For<IWebElement>();
        var xImageElement = Substitute.For<IWebElement>();
        var sCurrentUrl = string.Empty;

        xNavigation.When(xNav => xNav.GoToUrl(Arg.Any<string>()))
            .Do(xCall => sCurrentUrl = xCall.Arg<string>());
        xDriver.Navigate().Returns(xNavigation);
        xDriver.Url.Returns(_ => sCurrentUrl);
        xDriver.Title.Returns("Loaded");
        xDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("emailAddress").ToString())).Returns(xEmailElement);
        xDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("password").ToString())).Returns(xPasswordElement);
        xDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("form").ToString())).Returns(xFormElement);
        xDriver.FindElements(Arg.Any<By>()).Returns(xCall =>
        {
            var xBy = xCall.Arg<By>();
            if (xBy.ToString() == By.ClassName("c-blockitem").ToString())
            {
                return CreateCollection(xAnnouncementElement);
            }

            return CreateCollection();
        });
        xWebDriverFactory.Create().Returns(xDriver);

        xAnnouncementElement.TagName.Returns("div");
        xAnnouncementElement.Text.Returns($"ANZ 12345678{Environment.NewLine}Info");
        xAnnouncementElement.GetAttribute("class").Returns("c-blockitem");
        xAnnouncementElement.GetAttribute("id").Returns("item-1");
        xAnnouncementElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString())).Returns(CreateCollection());
        xAnnouncementElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("img").ToString())).Returns(CreateCollection(xImageElement));

        xImageElement.TagName.Returns("img");
        xImageElement.Text.Returns(string.Empty);
        xImageElement.GetAttribute("class").Returns(string.Empty);
        xImageElement.GetAttribute("title").Returns(string.Empty);
        xImageElement.GetAttribute("alt").Returns("image alt");
        xImageElement.GetAttribute("src").Returns("https://trauer.rnz.de/MEDIASERVER/image.jpg");
        xImageElement.GetAttribute("style").Returns(string.Empty);
        xImageElement.GetAttribute("data-original").Returns(string.Empty);
        xImageElement.FindElements(Arg.Any<By>()).Returns(CreateCollection());

        var xResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Encoding.ASCII.GetBytes("PNG1234567"))
        };
        xResponse.Headers.Add("X-Test", "1");
        xResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
        xHttpClient.GetAsync("https://trauer.rnz.de/MEDIASERVER/image.jpg").Returns(Task.FromResult(xResponse));

        using var xHandler = new WebHandler(xConfig, xHttpClient, xWebDriverFactory, xProgress);
        xHandler.InitPage();

        var (dPages, lstItems) = xHandler.GetData1("https://example.invalid/start", 1);

        Assert.AreEqual(4, dPages.Count);
        Assert.AreEqual(4, lstItems.Count);
        Assert.IsTrue(lstItems.All(dItem => dItem.ContainsKey(WebHandler.CsData)));
        Assert.IsTrue(lstItems.All(dItem => dItem.ContainsKey(WebHandler.CsHeader)));
        Assert.IsTrue(dPages.Values.All(dPage => dPage.ContainsKey("https://trauer.rnz.de/MEDIASERVER/image.jpg")));
        _ = xHttpClient.Received(4).GetAsync("https://trauer.rnz.de/MEDIASERVER/image.jpg");
        xProgress.Received().Report(new WebHandlerProgress("."));
    }

    [TestMethod]
    public void TryLoadBinary_Loads_Data_And_Headers_Via_Reflection()
    {
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent([1, 2, 3])
        };
        xResponse.Headers.Add("X-Test", "1");
        xResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        xHttpClient.GetAsync("https://example.invalid/data.bin").Returns(Task.FromResult(xResponse));
        using var xHandler = CreateHandler(xHttpClient: xHttpClient);
        var dTarget = new Dictionary<string, object?>();

        InvokeNonPublicInstanceMethod(xHandler, "TryLoadBinary", "https://example.invalid/data.bin", dTarget);

        CollectionAssert.AreEqual(new byte[] { 1, 2, 3 }, (byte[])dTarget[WebHandler.CsData]!);
        Assert.IsInstanceOfType<Dictionary<string, string>>(dTarget[WebHandler.CsHeader]);
        var dHeaders = (Dictionary<string, string>)dTarget[WebHandler.CsHeader]!;
        Assert.AreEqual("1", dHeaders["X-Test"]);
    }

    [TestMethod]
    public void TryLoadBinary_Sets_Empty_Result_On_Failure_Via_Reflection()
    {
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        xHttpClient.GetAsync("https://example.invalid/data.bin").Returns<Task<HttpResponseMessage>>(_ => throw new HttpRequestException("failed"));
        using var xHandler = CreateHandler(xHttpClient: xHttpClient);
        var dTarget = new Dictionary<string, object?>();

        InvokeNonPublicInstanceMethod(xHandler, "TryLoadBinary", "https://example.invalid/data.bin", dTarget);

        CollectionAssert.AreEqual(Array.Empty<byte>(), (byte[])dTarget[WebHandler.CsData]!);
        Assert.IsInstanceOfType<Dictionary<string, string>>(dTarget[WebHandler.CsHeader]);
        Assert.AreEqual(0, ((Dictionary<string, string>)dTarget[WebHandler.CsHeader]!).Count);
    }

    [TestMethod]
    public void GetDictionaryList_Returns_Private_List_Result_Via_Reflection()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["links"] = new List<Dictionary<string, object?>>()
            {
                new(StringComparer.Ordinal) { ["href"] = "https://example.invalid" }
            }
        };

        var lstResult = InvokeNonPublicStaticMethod<List<Dictionary<string, object?>>>(typeof(WebHandler), "GetDictionaryList", dValues, "links");
        var lstEmpty = InvokeNonPublicStaticMethod<List<Dictionary<string, object?>>>(typeof(WebHandler), "GetDictionaryList", dValues, "missing");

        Assert.AreEqual(1, lstResult.Count);
        Assert.AreEqual("https://example.invalid", lstResult[0]["href"]);
        Assert.AreEqual(0, lstEmpty.Count);
    }

    [TestMethod]
    public void GetStringList_Returns_Private_List_Result_Via_Reflection()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["text"] = new List<object?> { "Line1", 2, null }
        };

        var lstResult = InvokeNonPublicStaticMethod<List<string>>(typeof(WebHandler), "GetStringList", dValues, "text");
        var lstEmpty = InvokeNonPublicStaticMethod<List<string>>(typeof(WebHandler), "GetStringList", dValues, "missing");

        CollectionAssert.AreEqual(new[] { "Line1", "2", string.Empty }, lstResult);
        Assert.AreEqual(0, lstEmpty.Count);
    }

    [TestMethod]
    public void WorkMainPage_Returns_Page_SubPages_And_NextUrl_Via_Reflection()
    {
        var xDriver = Substitute.For<IWebDriver>();
        var xAnnouncementElement = Substitute.For<IWebElement>();
        var xAnnouncementLink = Substitute.For<IWebElement>();
        var xPagerElement = Substitute.For<IWebElement>();
        var xNextLink = Substitute.For<IWebElement>();
        using var xHandler = CreateHandler();
        SetDriver(xHandler, xDriver);

        xDriver.Url.Returns("https://example.invalid/list");
        xDriver.Title.Returns("Loaded");
        xDriver.PageSource.Returns("<html></html>");
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.ClassName("c-blockitem").ToString()))
            .Returns(CreateCollection(xAnnouncementElement, xPagerElement));

        xAnnouncementElement.TagName.Returns("div");
        xAnnouncementElement.Text.Returns($"ANZ 12345678{Environment.NewLine}Info");
        xAnnouncementElement.GetAttribute("class").Returns("c-blockitem");
        xAnnouncementElement.GetAttribute("id").Returns("ann-1");
        xAnnouncementElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString())).Returns(CreateCollection(xAnnouncementLink));
        xAnnouncementElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("img").ToString())).Returns(CreateCollection());

        xAnnouncementLink.TagName.Returns("a");
        xAnnouncementLink.Text.Returns(string.Empty);
        xAnnouncementLink.GetAttribute("title").Returns(string.Empty);
        xAnnouncementLink.GetAttribute("target").Returns(string.Empty);
        xAnnouncementLink.GetAttribute("href").Returns("https://example.invalid/subpage");
        xAnnouncementLink.FindElements(Arg.Any<By>()).Returns(CreateCollection());

        xPagerElement.TagName.Returns("div");
        xPagerElement.Text.Returns($"Page{Environment.NewLine}>\n");
        xPagerElement.GetAttribute("class").Returns("c-blockitem");
        xPagerElement.GetAttribute("id").Returns("pager-1");
        xPagerElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString())).Returns(CreateCollection(xNextLink));
        xPagerElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("img").ToString())).Returns(CreateCollection());

        xNextLink.TagName.Returns("a");
        xNextLink.Text.Returns(">");
        xNextLink.GetAttribute("title").Returns(string.Empty);
        xNextLink.GetAttribute("target").Returns(string.Empty);
        xNextLink.GetAttribute("href").Returns("https://example.invalid/list?page=2");
        xNextLink.FindElements(Arg.Any<By>()).Returns(CreateCollection());

        var dPages = new Dictionary<string, Dictionary<string, object?>>(StringComparer.Ordinal);
        var lstItems = new List<Dictionary<string, object?>>();

        var tResult = InvokeNonPublicInstanceMethod<(List<string> SubPages, string Url, string NextUrl)>(xHandler, "WorkMainPage", dPages, lstItems, "nachrufe");

        Assert.AreEqual("https://example.invalid/list", tResult.Url);
        Assert.AreEqual("https://example.invalid/list?page=2", tResult.NextUrl);
        CollectionAssert.AreEqual(new[] { "https://example.invalid/subpage" }, tResult.SubPages);
        Assert.AreEqual(1, lstItems.Count);
        Assert.AreEqual("5678", lstItems[0]["Title"]);
        Assert.AreEqual("Info", lstItems[0]["Info"]);
        Assert.AreEqual("nachrufe", dPages[tResult.Url]["filter"]);
    }

    [TestMethod]
    public void WorkSubPage_Extracts_Metadata_And_Media_Via_Reflection()
    {
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xDriver = Substitute.For<IWebDriver>();
        var xTitleElement = Substitute.For<IWebElement>();
        var xBirthElement = Substitute.For<IWebElement>();
        var xDeathElement = Substitute.For<IWebElement>();
        var xInfoSection = Substitute.For<IWebElement>();
        var xMediaImage = Substitute.For<IWebElement>();
        var xResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Encoding.ASCII.GetBytes("PNG1234567"))
        };
        xHttpClient.GetAsync("https://trauer.rnz.de/MEDIASERVER/profile.jpg").Returns(Task.FromResult(xResponse));
        using var xHandler = CreateHandler(xHttpClient: xHttpClient);
        SetDriver(xHandler, xDriver);

        xDriver.Url.Returns("https://example.invalid/subpage/anzeigen");
        xDriver.Title.Returns("SubPage");
        xDriver.PageSource.Returns("<html><body>sub</body></html>");
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("h1").ToString())).Returns(CreateCollection(xTitleElement));
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.ClassName("col-sm-6").ToString())).Returns(CreateCollection(xBirthElement, xDeathElement));
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("section").ToString())).Returns(CreateCollection(xInfoSection));

        xTitleElement.Text.Returns("Max Mustermann");
        xBirthElement.Text.Returns("* 01.01.2000");
        xDeathElement.Text.Returns("† 02.02.2020in Heidelberg");

        xInfoSection.TagName.Returns("section");
        xInfoSection.Text.Returns($"Erstellt von Test{Environment.NewLine}Angelegt am Heute{Environment.NewLine}42 Besuche");
        xInfoSection.GetAttribute("class").Returns("col-12");
        xInfoSection.GetAttribute("id").Returns("sec-1");
        xInfoSection.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString())).Returns(CreateCollection());
        xInfoSection.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("img").ToString())).Returns(CreateCollection(xMediaImage));

        xMediaImage.TagName.Returns("img");
        xMediaImage.Text.Returns(string.Empty);
        xMediaImage.GetAttribute("class").Returns(string.Empty);
        xMediaImage.GetAttribute("title").Returns(string.Empty);
        xMediaImage.GetAttribute("alt").Returns("Profile image");
        xMediaImage.GetAttribute("src").Returns("https://trauer.rnz.de/MEDIASERVER/profile.jpg");
        xMediaImage.GetAttribute("style").Returns(string.Empty);
        xMediaImage.GetAttribute("data-original").Returns(string.Empty);
        xMediaImage.FindElements(Arg.Any<By>()).Returns(CreateCollection());

        var dPages = new Dictionary<string, Dictionary<string, object?>>(StringComparer.Ordinal)
        {
            ["https://example.invalid/list"] = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["filter"] = "nachrufe",
                ["sections"] = new List<Dictionary<string, object?>>()
                {
                    new(StringComparer.Ordinal)
                    {
                        ["links"] = new List<Dictionary<string, object?>>()
                        {
                            new(StringComparer.Ordinal) { ["href"] = "https://example.invalid/subpage" }
                        },
                        ["imgs"] = new List<Dictionary<string, object?>>()
                    }
                }
            }
        };
        var lstItems = new List<Dictionary<string, object?>>();

        InvokeNonPublicInstanceMethod(xHandler, "WorkSubPage", "https://example.invalid/list", dPages, lstItems);

        var dSubPage = dPages["https://example.invalid/subpage/anzeigen"];
        Assert.AreEqual("SubPage", dSubPage["Title"]);
        Assert.AreEqual("Max Mustermann", dSubPage["name"]);
        Assert.AreEqual("* 01.01.2000", dSubPage["Birth"]);
        Assert.AreEqual("† 02.02.2020", dSubPage["Death"]);
        Assert.AreEqual(" Heidelberg", dSubPage["Place"]);
        Assert.AreEqual(" Test", dSubPage["created_by"]);
        Assert.AreEqual(" Heute", dSubPage["created_on"]);
        Assert.AreEqual("42 ", dSubPage["visits"]);
        Assert.AreEqual(1, lstItems.Count);
        Assert.AreEqual("Profile image", lstItems[0]["Info"]);
        Assert.AreEqual("https://example.invalid/subpage/anzeigen", lstItems[0]["parent"]);
    }

    private static ReadOnlyCollection<IWebElement> CreateCollection(params IWebElement[] arrElements)
    {
        return new ReadOnlyCollection<IWebElement>(arrElements.ToList());
    }

    private static WebHandler CreateHandler(IHttpClientProxy? xHttpClient = null, IWebDriverFactory? xWebDriverFactory = null, IProgress<WebHandlerProgress>? xProgress = null)
    {
        return new WebHandler(new RnzConfig(), xHttpClient ?? Substitute.For<IHttpClientProxy>(), xWebDriverFactory ?? Substitute.For<IWebDriverFactory>(), xProgress);
    }

    private static void SetDriver(WebHandler xHandler, IWebDriver xDriver)
    {
        var xSetter = typeof(WebHandler).GetProperty(nameof(WebHandler.Driver), BindingFlags.Instance | BindingFlags.Public)?.GetSetMethod(true);
        Assert.IsNotNull(xSetter);
        xSetter.Invoke(xHandler, [xDriver]);
    }

    private static void InvokeNonPublicInstanceMethod(object xTarget, string sMethodName, params object?[] arrArguments)
    {
        var xMethod = xTarget.GetType().GetMethod(sMethodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(xMethod);

        try
        {
            _ = xMethod.Invoke(xTarget, arrArguments);
        }
        catch (TargetInvocationException xException) when (xException.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(xException.InnerException).Throw();
        }
    }

    private static T InvokeNonPublicInstanceMethod<T>(object xTarget, string sMethodName, params object?[] arrArguments)
    {
        var xMethod = xTarget.GetType().GetMethod(sMethodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(xMethod);

        try
        {
            return (T)xMethod.Invoke(xTarget, arrArguments)!;
        }
        catch (TargetInvocationException xException) when (xException.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(xException.InnerException).Throw();
            throw;
        }
    }

    private static T InvokeNonPublicStaticMethod<T>(Type xType, string sMethodName, params object?[] arrArguments)
    {
        var xMethod = xType.GetMethod(sMethodName, BindingFlags.Static | BindingFlags.NonPublic);
        Assert.IsNotNull(xMethod);
        return (T)xMethod.Invoke(null, arrArguments)!;
    }
}
