using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using OpenQA.Selenium;
using RnzTrauer.Core;
using RnzTrauer.Core.Services.Interfaces;
using BaseLib.Helper;

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
            Password = "secret".ToSecureString(),
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
        xPasswordElement.Received(1).SendKeys(new System.Net.NetworkCredential(string.Empty, xConfig.Password).Password);
        xFormElement.Received(1).Submit();
        Assert.AreSame(xDriver, xHandler.Driver);
    }

    [TestMethod]
    public void InitPage_Quits_Previous_Driver_And_Waits_For_Title_Change()
    {
        var xConfig = new RnzConfig
        {
            Url = "https://example.invalid/login",
            User = "user@example.invalid",
            Password = "secret".ToSecureString(),
            Title = "RNZ"
        };
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xOldDriver = Substitute.For<IWebDriver>();
        var xNewDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();
        var xEmailElement = Substitute.For<IWebElement>();
        var xPasswordElement = Substitute.For<IWebElement>();
        var xFormElement = Substitute.For<IWebElement>();
        var iTitleReads = 0;

        xOldDriver.When(xDriver => xDriver.Quit()).Do(_ => throw new InvalidOperationException("quit failed"));
        xNewDriver.Navigate().Returns(xNavigation);
        xNewDriver.Title.Returns(_ => ++iTitleReads == 1 ? "RNZ" : "Loaded");
        xNewDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("emailAddress").ToString())).Returns(xEmailElement);
        xNewDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("password").ToString())).Returns(xPasswordElement);
        xNewDriver.FindElement(Arg.Is<By>(xBy => xBy.ToString() == By.Id("form").ToString())).Returns(xFormElement);
        xWebDriverFactory.Create().Returns(xNewDriver);

        using var xHandler = new WebHandler(xConfig, xHttpClient, xWebDriverFactory);
        SetDriver(xHandler, xOldDriver);

        xHandler.InitPage();

        _ = xWebDriverFactory.Received(1).Create();
        Assert.AreEqual(1, xOldDriver.ReceivedCalls().Count(xCall => xCall.GetMethodInfo().Name == nameof(IWebDriver.Quit)));
        xNavigation.Received(1).GoToUrl(xConfig.Url);
        xEmailElement.Received(1).SendKeys(xConfig.User);
        xPasswordElement.Received(1).SendKeys(new System.Net.NetworkCredential(string.Empty, xConfig.Password).Password);
        xFormElement.Received(1).Submit();
        Assert.AreSame(xNewDriver, xHandler.Driver);
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
    public void Wdr2List_Skips_Stale_Elements_And_Handles_Throwing_Text_And_Children()
    {
        var xConfig = new RnzConfig();
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xSearchContext = Substitute.For<ISearchContext>();
        var xStaleElement = Substitute.For<IWebElement>();
        var xTextThrowingElement = Substitute.For<IWebElement>();
        var xChildThrowingElement = Substitute.For<IWebElement>();

        xSearchContext.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("div").ToString()))
            .Returns(CreateCollection(xStaleElement, xTextThrowingElement, xChildThrowingElement));

        xStaleElement.TagName.Returns(_ => throw new StaleElementReferenceException());

        xTextThrowingElement.TagName.Returns("div");
        xTextThrowingElement.GetAttribute("class").Returns("c-blockitem");
        xTextThrowingElement.GetAttribute("id").Returns("text-throw");
        xTextThrowingElement.Text.Returns(_ => throw new StaleElementReferenceException());
        xTextThrowingElement.FindElements(Arg.Any<By>()).Returns(CreateCollection());

        xChildThrowingElement.TagName.Returns("div");
        xChildThrowingElement.GetAttribute("class").Returns("c-blockitem");
        xChildThrowingElement.GetAttribute("id").Returns("child-throw");
        xChildThrowingElement.Text.Returns("Line1");
        xChildThrowingElement.FindElements(Arg.Any<By>()).Returns(_ => throw new StaleElementReferenceException());

        using var xHandler = new WebHandler(xConfig, xHttpClient, xWebDriverFactory);

        var lstResult = xHandler.Wdr2List(xSearchContext, new WebQuery(string.Empty, By.TagName("div"), new WebQuery("links", By.TagName("a"))));

        Assert.AreEqual(2, lstResult.Count);
        Assert.AreEqual("text-throw", lstResult[0]["id"]);
        Assert.IsFalse(lstResult[0].ContainsKey("text"));
        Assert.AreEqual("child-throw", lstResult[1]["id"]);
        Assert.IsTrue(lstResult[1].ContainsKey("links"));
        Assert.AreEqual(0, ((List<Dictionary<string, object?>>)lstResult[1]["links"]!).Count);
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
            Password = "secret".ToSecureString(),
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
        _ = xHttpClient.Received(1).GetAsync("https://trauer.rnz.de/MEDIASERVER/image.jpg");
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
            },
            ["https://example.invalid/subpage/anzeigen"] = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["parent"] = "invalid-parent-shape"
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

    [TestMethod]
    public void WorkSubPage_Propagates_Ids_And_Rewrites_Jpg_To_Png_Via_Reflection()
    {
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xDriver = Substitute.For<IWebDriver>();
        var xTitleElement = Substitute.For<IWebElement>();
        var xBirthElement = Substitute.For<IWebElement>();
        var xDeathElement = Substitute.For<IWebElement>();
        var xSectionElement = Substitute.For<IWebElement>();
        var xSectionImage = Substitute.For<IWebElement>();
        var xSectionLink = Substitute.For<IWebElement>();
        var xResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Encoding.ASCII.GetBytes("PNG1234567"))
        };
        xHttpClient.GetAsync("https://example.invalid/MEDIASERVER/photo.jpg").Returns(Task.FromResult(xResponse));
        using var xHandler = CreateHandler(xHttpClient: xHttpClient);
        SetDriver(xHandler, xDriver);

        xDriver.Url.Returns("https://example.invalid/subpage/anzeigen");
        xDriver.Title.Returns("SubPage");
        xDriver.PageSource.Returns("<html><body>sub</body></html>");
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("h1").ToString())).Returns(CreateCollection(xTitleElement));
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.ClassName("col-sm-6").ToString())).Returns(CreateCollection(xBirthElement, xDeathElement));
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("section").ToString())).Returns(CreateCollection(xSectionElement));

        xTitleElement.Text.Returns("Max Mustermann");
        xBirthElement.Text.Returns("* 01.01.2000");
        xDeathElement.Text.Returns("† 02.02.2020 in Heidelberg");

        xSectionElement.TagName.Returns("div");
        xSectionElement.Text.Returns($"Hinweis{Environment.NewLine}Mehr Text");
        xSectionElement.GetAttribute("class").Returns("row");
        xSectionElement.GetAttribute("id").Returns("19");
        xSectionElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString())).Returns(CreateCollection(xSectionLink));
        xSectionElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("img").ToString())).Returns(CreateCollection(xSectionImage));

        xSectionImage.TagName.Returns("img");
        xSectionImage.Text.Returns(string.Empty);
        xSectionImage.GetAttribute("class").Returns(string.Empty);
        xSectionImage.GetAttribute("title").Returns(string.Empty);
        xSectionImage.GetAttribute("alt").Returns("Image");
        xSectionImage.GetAttribute("src").Returns("https://example.invalid/MEDIASERVER/photo.jpg");
        xSectionImage.GetAttribute("style").Returns(string.Empty);
        xSectionImage.GetAttribute("data-original").Returns(string.Empty);
        xSectionImage.FindElements(Arg.Any<By>()).Returns(CreateCollection());

        xSectionLink.TagName.Returns("a");
        xSectionLink.Text.Returns("Speichern");
        xSectionLink.GetAttribute("title").Returns(string.Empty);
        xSectionLink.GetAttribute("target").Returns(string.Empty);
        xSectionLink.GetAttribute("href").Returns("https://example.invalid/MEDIASERVER/photo.jpg");
        xSectionLink.FindElements(Arg.Any<By>()).Returns(CreateCollection());

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
                },
                ["https://example.invalid/MEDIASERVER/photo.jpg"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    [WebHandler.CsHeader] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                }
            }
        };
        var lstItems = new List<Dictionary<string, object?>>();

        InvokeNonPublicInstanceMethod(xHandler, "WorkSubPage", "https://example.invalid/list", dPages, lstItems);

        var dSubPage = dPages["https://example.invalid/subpage/anzeigen"];
        var lstParentSections = (List<Dictionary<string, object?>>)dPages["https://example.invalid/list"]["sections"]!;
        Assert.AreEqual("19", lstParentSections[0]["id-anz"]);
        Assert.AreEqual("19", dPages["https://example.invalid/list"]["https://example.invalid/MEDIASERVER/photo.jpg"] is Dictionary<string, object?> dMedia ? dMedia["id-anz"] : string.Empty);
        var lstSections = (List<Dictionary<string, object?>>)dSubPage["sections"]!;
        var lstLinks = (List<Dictionary<string, object?>>)lstSections[0]["links"]!;
        Assert.AreEqual("https://example.invalid/MEDIASERVER/photo.png", lstLinks[0]["href"]);
        Assert.AreEqual(1, lstItems.Count);
    }

    [TestMethod]
    public void ContainsErrorMarker_Detects_Only_Real_Error_Texts()
    {
        var xResult = InvokeNonPublicStaticMethod<bool>(typeof(WebHandler), "ContainsErrorMarker", "504 gateway time-out");

        Assert.IsTrue(xResult);
        xResult = InvokeNonPublicStaticMethod<bool>(typeof(WebHandler), "ContainsErrorMarker", "Service Unavailable");

        Assert.IsTrue(xResult);
        xResult = InvokeNonPublicStaticMethod<bool>(typeof(WebHandler), "ContainsErrorMarker", "<html><head><script>window.setTimeout(function(){}, 1000);</script></head><body>ok</body></html>");

        Assert.IsFalse(xResult);
    }

    [TestMethod]
    public void TryGetBodyText_Returns_Empty_When_FindElements_Throws()
    {
        var xSearchContext = Substitute.For<ISearchContext>();
        xSearchContext.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("body").ToString()))
            .Returns(_ => throw new WebDriverException("failed"));

        var sResult = InvokeNonPublicStaticMethod<string>(typeof(WebHandler), "TryGetBodyText", xSearchContext);

        Assert.AreEqual(string.Empty, sResult);
    }

    [TestMethod]
    public void NavigateWithReloadRetry_Retries_After_WebDriverException_And_Succeeds()
    {
        var xConfig = new RnzConfig();
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xDriver = Substitute.For<IWebDriver>();
        var xNavigation = Substitute.For<INavigation>();
        var xBody = Substitute.For<IWebElement>();
        var xProgress = Substitute.For<IProgress<WebHandlerProgress>>();
        var iCalls = 0;

        xDriver.Navigate().Returns(xNavigation);
        xDriver.Title.Returns("Loaded");
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("body").ToString())).Returns(CreateCollection(xBody));
        xBody.Text.Returns("content");
        xNavigation.When(xNav => xNav.GoToUrl(Arg.Any<string>())).Do(_ =>
        {
            iCalls++;
            if (iCalls == 1)
            {
                throw new WebDriverException("temporary");
            }
        });

        using var xHandler = new WebHandler(xConfig, xHttpClient, xWebDriverFactory, xProgress);
        SetDriver(xHandler, xDriver);

        var xResult = InvokeNonPublicInstanceMethod<bool>(xHandler, "NavigateWithReloadRetry", "https://example.invalid/list");

        Assert.IsTrue(xResult);
        Assert.AreEqual(2, iCalls);
        xProgress.Received().Report(Arg.Any<WebHandlerProgress>());
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

    [TestMethod]
    public void TryLoadBinary_CacheHit_Reuses_Same_ByteArray_Instance_Via_Reflection()
    {
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent([1, 2, 3])
        };
        xResponse.Headers.Add("X-Test", "1");
        xHttpClient.GetAsync("https://example.invalid/cached.bin").Returns(Task.FromResult(xResponse));
        using var xHandler = CreateHandler(xHttpClient: xHttpClient);

        var dTarget1 = new Dictionary<string, object?>();
        var dTarget2 = new Dictionary<string, object?>();

        InvokeNonPublicInstanceMethod(xHandler, "TryLoadBinary", "https://example.invalid/cached.bin", dTarget1);
        InvokeNonPublicInstanceMethod(xHandler, "TryLoadBinary", "https://example.invalid/cached.bin", dTarget2);

        var arrData1 = (byte[])dTarget1[WebHandler.CsData]!;
        var arrData2 = (byte[])dTarget2[WebHandler.CsData]!;
        Assert.IsTrue(ReferenceEquals(arrData1, arrData2));
        _ = xHttpClient.Received(1).GetAsync("https://example.invalid/cached.bin");
    }

    [TestMethod]
    public void TryLoadBinary_CacheHit_Returns_Copied_Header_Dictionary_Via_Reflection()
    {
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent([1, 2, 3])
        };
        xResponse.Headers.Add("X-Test", "1");
        xHttpClient.GetAsync("https://example.invalid/cached-headers.bin").Returns(Task.FromResult(xResponse));
        using var xHandler = CreateHandler(xHttpClient: xHttpClient);

        var dTarget1 = new Dictionary<string, object?>();
        var dTarget2 = new Dictionary<string, object?>();

        InvokeNonPublicInstanceMethod(xHandler, "TryLoadBinary", "https://example.invalid/cached-headers.bin", dTarget1);
        var dHeaders1 = (Dictionary<string, string>)dTarget1[WebHandler.CsHeader]!;
        dHeaders1["X-Test"] = "modified";

        InvokeNonPublicInstanceMethod(xHandler, "TryLoadBinary", "https://example.invalid/cached-headers.bin", dTarget2);
        var dHeaders2 = (Dictionary<string, string>)dTarget2[WebHandler.CsHeader]!;

        Assert.IsFalse(ReferenceEquals(dHeaders1, dHeaders2));
        Assert.AreEqual("1", dHeaders2["X-Test"]);
    }

    [TestMethod]
    public void HasMeaningfulPageContent_Returns_False_For_Error_Text_In_Body()
    {
        var xDriver = Substitute.For<IWebDriver>();
        var xBody = Substitute.For<IWebElement>();

        xDriver.Title.Returns("Loaded");
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("body").ToString())).Returns(CreateCollection(xBody));
        xBody.Text.Returns("504 gateway time-out");

        var xResult = InvokeNonPublicStaticMethod<bool>(typeof(WebHandler), "HasMeaningfulPageContent", xDriver);

        Assert.IsFalse(xResult);
    }

    [TestMethod]
    public void TryLoadBinary_Mutating_Returned_Cached_ByteArray_Affects_Followup_Reads_Via_Reflection()
    {
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent([1, 2, 3])
        };
        xHttpClient.GetAsync("https://example.invalid/mutable-cache.bin").Returns(Task.FromResult(xResponse));
        using var xHandler = CreateHandler(xHttpClient: xHttpClient);

        var dTarget1 = new Dictionary<string, object?>();
        var dTarget2 = new Dictionary<string, object?>();

        InvokeNonPublicInstanceMethod(xHandler, "TryLoadBinary", "https://example.invalid/mutable-cache.bin", dTarget1);
        var arrData1 = (byte[])dTarget1[WebHandler.CsData]!;
        arrData1[0] = 9;

        InvokeNonPublicInstanceMethod(xHandler, "TryLoadBinary", "https://example.invalid/mutable-cache.bin", dTarget2);
        var arrData2 = (byte[])dTarget2[WebHandler.CsData]!;

        Assert.AreEqual(9, arrData2[0]);
    }

    [TestMethod]
    public void GetDictionaryList_Handles_Enumerable_Dictionary_Path_Via_Reflection()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["links"] = (IEnumerable<Dictionary<string, object?>>)new[]
            {
                new Dictionary<string, object?>(StringComparer.Ordinal) { ["href"] = "https://example.invalid/e" }
            }
        };

        var lstResult = InvokeNonPublicStaticMethod<List<Dictionary<string, object?>>>(typeof(WebHandler), "GetDictionaryList", dValues, "links");

        Assert.AreEqual(1, lstResult.Count);
        Assert.AreEqual("https://example.invalid/e", lstResult[0]["href"]);
    }

    [TestMethod]
    public void GetStringList_Handles_Enumerable_String_Path_Via_Reflection()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["text"] = (IEnumerable<string>)new[] { "A", "B" }
        };

        var lstResult = InvokeNonPublicStaticMethod<List<string>>(typeof(WebHandler), "GetStringList", dValues, "text");

        CollectionAssert.AreEqual(new[] { "A", "B" }, lstResult);
    }

    [TestMethod]
    public void HasMeaningfulPageContent_Returns_False_When_Title_And_Body_Are_Empty()
    {
        var xDriver = Substitute.For<IWebDriver>();
        var xBody = Substitute.For<IWebElement>();

        xDriver.Title.Returns(string.Empty);
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("body").ToString())).Returns(CreateCollection(xBody));
        xBody.Text.Returns(string.Empty);

        var xResult = InvokeNonPublicStaticMethod<bool>(typeof(WebHandler), "HasMeaningfulPageContent", xDriver);

        Assert.IsFalse(xResult);
    }

    [TestMethod]
    public void TryGetBodyText_Ignores_Stale_Body_And_Returns_Empty()
    {
        var xSearchContext = Substitute.For<ISearchContext>();
        var xBody = Substitute.For<IWebElement>();
        xSearchContext.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("body").ToString())).Returns(CreateCollection(xBody));
        xBody.Text.Returns(_ => throw new StaleElementReferenceException());

        var sResult = InvokeNonPublicStaticMethod<string>(typeof(WebHandler), "TryGetBodyText", xSearchContext);

        Assert.AreEqual(string.Empty, sResult);
    }

    [TestMethod]
    public void WorkSubPage_Uses_DataOriginal_Media_Branch_And_Catches_Invalid_Link()
    {
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xDriver = Substitute.For<IWebDriver>();
        var xTitleElement = Substitute.For<IWebElement>();
        var xBirthElement = Substitute.For<IWebElement>();
        var xDeathElement = Substitute.For<IWebElement>();
        var xSectionElement = Substitute.For<IWebElement>();
        var xSectionImage = Substitute.For<IWebElement>();
        var xBrokenLink = Substitute.For<IWebElement>();
        var xResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(Encoding.ASCII.GetBytes("PNG1234567"))
        };

        xHttpClient.GetAsync("https://example.invalid/MEDIASERVER/from-data-original.jpg").Returns(Task.FromResult(xResponse));
        using var xHandler = CreateHandler(xHttpClient: xHttpClient);
        SetDriver(xHandler, xDriver);

        xDriver.Url.Returns("https://example.invalid/sub/anzeigen");
        xDriver.Title.Returns("SubTitle");
        xDriver.PageSource.Returns("<html></html>");
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("h1").ToString())).Returns(CreateCollection(xTitleElement));
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.ClassName("col-sm-6").ToString())).Returns(CreateCollection(xBirthElement, xDeathElement));
        xDriver.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("section").ToString())).Returns(CreateCollection(xSectionElement));

        xTitleElement.Text.Returns("Max Mustermann");
        xBirthElement.Text.Returns("* 01.01.2000");
        xDeathElement.Text.Returns("† 02.02.2020 in Heidelberg");

        xSectionElement.TagName.Returns("div");
        xSectionElement.Text.Returns("Abschnitt");
        xSectionElement.GetAttribute("class").Returns("row");
        xSectionElement.GetAttribute("id").Returns("55");
        xSectionElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("img").ToString())).Returns(CreateCollection(xSectionImage));
        xSectionElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString())).Returns(CreateCollection(xBrokenLink));

        xSectionImage.TagName.Returns("img");
        xSectionImage.Text.Returns(string.Empty);
        xSectionImage.GetAttribute("class").Returns(string.Empty);
        xSectionImage.GetAttribute("title").Returns(string.Empty);
        xSectionImage.GetAttribute("alt").Returns("image");
        xSectionImage.GetAttribute("src").Returns(string.Empty);
        xSectionImage.GetAttribute("style").Returns(string.Empty);
        xSectionImage.GetAttribute("data-original").Returns("/MEDIASERVER/from-data-original.jpg");
        xSectionImage.FindElements(Arg.Any<By>()).Returns(CreateCollection());

        xBrokenLink.TagName.Returns("a");
        xBrokenLink.GetAttribute("href").Returns((string?)null);
        xBrokenLink.FindElements(Arg.Any<By>()).Returns(CreateCollection());
        xBrokenLink.Text.Returns(_ => throw new InvalidOperationException("to-string failed"));

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
                            new(StringComparer.Ordinal) { ["href"] = "https://example.invalid/sub" }
                        },
                        ["imgs"] = new List<Dictionary<string, object?>>()
                    }
                },
                ["/MEDIASERVER/from-data-original.jpg"] = new Dictionary<string, object?>(StringComparer.Ordinal)
                {
                    [WebHandler.CsHeader] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                }
            },
            ["https://example.invalid/sub/anzeigen"] = new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["parent"] = "invalid-parent-shape"
            }
        };
        var lstItems = new List<Dictionary<string, object?>>();

        InvokeNonPublicInstanceMethod(xHandler, "WorkSubPage", "https://example.invalid/list", dPages, lstItems);

        var dSubPage = dPages["https://example.invalid/sub/anzeigen"];
        Assert.IsTrue(dSubPage["parent"] is List<object?>);
        Assert.AreEqual("nachrufe", ((List<Dictionary<string, object?>>)dSubPage["sections"]!)[0]["filter"]);
    }

    private sealed class ThrowingToStringValue
    {
        public override string ToString()
        {
            throw new InvalidOperationException("to-string failed");
        }
    }

    [TestMethod]
    public void GetDictionaryList_Returns_Empty_For_Unsupported_Value_Type_Via_Reflection()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["links"] = 42
        };

        var lstResult = InvokeNonPublicStaticMethod<List<Dictionary<string, object?>>>(typeof(WebHandler), "GetDictionaryList", dValues, "links");

        Assert.AreEqual(0, lstResult.Count);
    }

    [TestMethod]
    public void GetStringList_Returns_Empty_For_Unsupported_Value_Type_Via_Reflection()
    {
        IReadOnlyDictionary<string, object?> dValues = new Dictionary<string, object?>
        {
            ["text"] = 42
        };

        var lstResult = InvokeNonPublicStaticMethod<List<string>>(typeof(WebHandler), "GetStringList", dValues, "text");

        Assert.AreEqual(0, lstResult.Count);
    }

    [TestMethod]
    public void Wdr2List_Ignores_Throwing_Child_Query_And_Returns_Element()
    {
        var xConfig = new RnzConfig();
        var xHttpClient = Substitute.For<IHttpClientProxy>();
        var xWebDriverFactory = Substitute.For<IWebDriverFactory>();
        var xSearchContext = Substitute.For<ISearchContext>();
        var xElement = Substitute.For<IWebElement>();

        xSearchContext.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("div").ToString()))
            .Returns(CreateCollection(xElement));

        xElement.TagName.Returns("div");
        xElement.GetAttribute("class").Returns("c-blockitem");
        xElement.GetAttribute("id").Returns("item-1");
        xElement.Text.Returns("Line");
        xElement.FindElements(Arg.Is<By>(xBy => xBy.ToString() == By.TagName("a").ToString()))
            .Returns(_ => throw new InvalidOperationException("child query failed"));

        using var xHandler = new WebHandler(xConfig, xHttpClient, xWebDriverFactory);

        var lstResult = xHandler.Wdr2List(xSearchContext, new WebQuery(string.Empty, By.TagName("div"), new WebQuery("links", By.TagName("a"))));

        Assert.AreEqual(1, lstResult.Count);
        Assert.AreEqual("item-1", lstResult[0]["id"]);
        Assert.IsFalse(lstResult[0].ContainsKey("links"));
    }
}
