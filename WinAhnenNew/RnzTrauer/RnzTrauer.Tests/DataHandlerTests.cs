using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RnzTrauer.Core;
using RnzTrauer.Core.Services.Interfaces;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class DataHandlerTests
{
    [TestMethod]
    public void BuildTrauerFallIndex_Uses_Repository_Result()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        var dExpected = new Dictionary<string, long>(StringComparer.Ordinal)
        {
            ["/test/url"] = 42
        };
        xRepository.BuildTrauerFallIndex().Returns(dExpected);

        using var xHandler = new DataHandler(xRepository, xFile);

        xHandler.BuildTrauerFallIndex();

        Assert.AreEqual(1, xHandler.TfIdx.Count);
        Assert.AreEqual(42L, xHandler.TfIdx["/test/url"]);
        _ = xRepository.Received(1).BuildTrauerFallIndex();
    }

    [TestMethod]
    public void RepositoryForwarders_Delegate_To_Repository()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        var dList = new List<Dictionary<string, object?>> { new(StringComparer.Ordinal) { ["id"] = 1 } };
        xRepository.TrauerAnzId(11).Returns(dList);
        xRepository.TrauerAnz(22).Returns(dList);
        xRepository.LegacyTrauerAnz("abc").Returns(dList);
        xRepository.TrauerAnzIsNull("field", 3).Returns(dList);
        xRepository.TrauerFallIsNull("field", 4).Returns(dList);
        xRepository.TrauerFallEquals("field", "value", 5).Returns(dList);
        xRepository.TrauerFallById(66).Returns(dList);
        xRepository.TrauerFallByUrl("https://example.invalid").Returns(dList);
        xRepository.AppendTrauerFall(Arg.Any<IReadOnlyDictionary<string, object?>>()).Returns(101L);
        xRepository.AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>()).Returns(102L);
        xRepository.AppendLegacyTAnz(Arg.Any<IReadOnlyDictionary<string, object?>>()).Returns(103L);
        xRepository.UpdateTrauerAnz(Arg.Any<List<Dictionary<string, object?>>>() , Arg.Any<List<Dictionary<string, object?>>>() ).Returns(true);

        using var xHandler = new DataHandler(xRepository, xFile);

        Assert.AreSame(dList, xHandler.TrauerAnzId(11));
        Assert.AreSame(dList, xHandler.TrauerAnz(22));
        Assert.AreSame(dList, xHandler.LegacyTrauerAnz("abc"));
        Assert.AreSame(dList, xHandler.TrauerAnzIsNull("field", 3));
        Assert.AreSame(dList, xHandler.TrauerFallIsNull("field", 4));
        Assert.AreSame(dList, xHandler.TrauerFallEquals("field", "value", 5));
        Assert.AreSame(dList, xHandler.TrauerFallById(66));
        Assert.AreSame(dList, xHandler.TrauerFallByUrl("https://example.invalid"));
        Assert.AreEqual(101L, xHandler.AppendTrauerFall(new Dictionary<string, object?>()));
        Assert.AreEqual(102L, xHandler.AppendTrauerAnz(1L, CreateAnnouncementInput(), @"C:\data"));
        Assert.AreEqual(103L, xHandler.AppendLegacyTAnz("x", CreateAnnouncementInput(), @"C:\data"));

        var dNew = new List<Dictionary<string, object?>> { new(StringComparer.Ordinal) };
        var dOld = new List<Dictionary<string, object?>> { new(StringComparer.Ordinal) };
        xHandler.UpdateTrauerFall(dNew, dOld);
        Assert.IsTrue(xHandler.UpdateTrauerAnz(dNew, dOld));

        _ = xRepository.Received(1).TrauerAnzId(11);
        _ = xRepository.Received(1).TrauerAnz(22);
        _ = xRepository.Received(1).LegacyTrauerAnz("abc");
        _ = xRepository.Received(1).TrauerAnzIsNull("field", 3);
        _ = xRepository.Received(1).TrauerFallIsNull("field", 4);
        _ = xRepository.Received(1).TrauerFallEquals("field", "value", 5);
        _ = xRepository.Received(1).TrauerFallById(66);
        _ = xRepository.Received(1).TrauerFallByUrl("https://example.invalid");
        _ = xRepository.Received(1).AppendTrauerFall(Arg.Any<IReadOnlyDictionary<string, object?>>());
        _ = xRepository.Received(1).AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>());
        _ = xRepository.Received(1).AppendLegacyTAnz(Arg.Any<IReadOnlyDictionary<string, object?>>());
        xRepository.Received(1).UpdateTrauerFall(dNew, dOld);
        _ = xRepository.Received(1).UpdateTrauerAnz(dNew, dOld);
    }

    [TestMethod]
    public void AppendTrauerFall_Maps_Case_Record_For_Repository()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        IReadOnlyDictionary<string, object?>? dCapturedValues = null;
        xRepository
            .AppendTrauerFall(Arg.Any<IReadOnlyDictionary<string, object?>>())
            .Returns(xCall =>
            {
                dCapturedValues = xCall.ArgAt<IReadOnlyDictionary<string, object?>>(0);
                return 77L;
            });
        var dTrauerfall = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["url"] = "https://example.invalid/trauerfall",
            ["created_on"] = "03.04.2024",
            ["Birth"] = "* 01.02.2000",
            ["Death"] = "† 05.06.2024",
            ["Birthname"] = "Beispiel",
            ["Place"] = "Heidelberg",
            ["created_by"] = "tester",
            ["name"] = "Erika Muster"
        };

        using var xHandler = new DataHandler(xRepository, xFile);

        var iResult = xHandler.AppendTrauerFall(dTrauerfall);

        Assert.AreEqual(77L, iResult);
        Assert.IsNotNull(dCapturedValues);
        Assert.AreEqual("https://example.invalid/trauerfall", dCapturedValues!["URL"]);
        Assert.AreEqual(new DateTime(2024, 4, 3), dCapturedValues["Created"]);
        Assert.AreEqual(new DateTime(2000, 2, 1), dCapturedValues["Preread_Birth"]);
        Assert.AreEqual(new DateTime(2024, 6, 5), dCapturedValues["Preread_Death"]);
        Assert.AreEqual("Muster, Erika", dCapturedValues["Fullname"]);
        Assert.AreEqual("Erika", dCapturedValues["Firstname"]);
        Assert.AreEqual("Muster", dCapturedValues["Lastname"]);
        Assert.AreEqual("Beispiel", dCapturedValues["Birthname"]);
        Assert.AreEqual("Heidelberg", dCapturedValues["Place"]);
        Assert.AreEqual("tester", dCapturedValues["Created_by"]);
    }

    [TestMethod]
    public void AppendTrauerAnz_Maps_Announcement_Record_For_Repository()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        IReadOnlyDictionary<string, object?>? dCapturedValues = null;
        xRepository
            .AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>())
            .Returns(xCall =>
            {
                dCapturedValues = xCall.ArgAt<IReadOnlyDictionary<string, object?>>(0);
                return 12L;
            });
        var dTrauerfall = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["url"] = "https://example.invalid/trauerfall",
            ["id"] = "17",
            ["publish"] = "03.04.2024",
            ["img"] = @"C:\data\2024\folder\image.png",
            ["pdf"] = @"C:\data\2024\folder\notice.pdf",
            ["name"] = "Erika Muster",
            ["Birthname"] = "Beispiel",
            ["Birth"] = "* 01.02.2000",
            ["Death"] = "† 05.06.2024",
            ["Place"] = "Heidelberg",
            ["pdfText"] = "PDF content",
            ["profImg"] = @"C:\data\profiles\portrait.png",
            ["filter"] = "danksagungen"
        };

        using var xHandler = new DataHandler(xRepository, xFile);

        var iResult = xHandler.AppendTrauerAnz(55, dTrauerfall, @"C:\data");

        Assert.AreEqual(12L, iResult);
        _ = xRepository.Received(1).AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>());
        Assert.IsNotNull(dCapturedValues);
        Assert.AreEqual(55L, dCapturedValues["idTrauerfall"]);
        Assert.AreEqual(17, dCapturedValues["Announcement"]);
        Assert.AreEqual("https://example.invalid/trauerfall", dCapturedValues["url"]);
        Assert.AreEqual("Erika", dCapturedValues["Firstname"]);
        Assert.AreEqual("Muster", dCapturedValues["Lastname"]);
        Assert.AreEqual("Beispiel", dCapturedValues["Birthname"]);
        Assert.AreEqual("Heidelberg", dCapturedValues["Place"]);
        Assert.AreEqual("PDF content", dCapturedValues["Info"]);
        Assert.AreEqual("image.png", dCapturedValues["pngFile"]);
        Assert.AreEqual("notice.pdf", dCapturedValues["pdfFile"]);
        Assert.AreEqual(@"\2024\folder", dCapturedValues["localpath"]);
        Assert.AreEqual(@"..\..\profiles\portrait.png", dCapturedValues["ProfileImg"]);
        Assert.AreEqual(8060, dCapturedValues["Rubrik"]);
        Assert.AreEqual(new DateTime(2024, 4, 3), dCapturedValues["release"]);
        Assert.AreEqual(new DateTime(2000, 2, 1), dCapturedValues["Birth"]);
        Assert.AreEqual(new DateTime(2024, 6, 5), dCapturedValues["Death"]);
        StringAssert.Contains(Convert.ToString(dCapturedValues["Additional"]), "Erika Muster");
    }

    [TestMethod]
    public void AppendLegacyTAnz_Maps_Legacy_Record_For_Repository()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        IReadOnlyDictionary<string, object?>? dCapturedValues = null;
        xRepository
            .AppendLegacyTAnz(Arg.Any<IReadOnlyDictionary<string, object?>>())
            .Returns(xCall =>
            {
                dCapturedValues = xCall.ArgAt<IReadOnlyDictionary<string, object?>>(0);
                return 19L;
            });
        var dTrauerfall = CreateAnnouncementInput();

        using var xHandler = new DataHandler(xRepository, xFile);

        var iResult = xHandler.AppendLegacyTAnz("4711", dTrauerfall, @"C:\data");

        Assert.AreEqual(19L, iResult);
        Assert.IsNotNull(dCapturedValues);
        Assert.AreEqual("4711", dCapturedValues!["Auftrag"]);
        Assert.AreEqual("https://example.invalid/trauerfall", dCapturedValues["url"]);
        Assert.AreEqual(17, dCapturedValues["Announcement"]);
        Assert.AreEqual(new DateTime(2024, 4, 3), dCapturedValues["release"]);
        Assert.AreEqual(@"\2024\folder", dCapturedValues["localpath"]);
        Assert.AreEqual("image.png", dCapturedValues["pngFile"]);
        Assert.AreEqual("notice.pdf", dCapturedValues["pdfFile"]);
        StringAssert.Contains(Convert.ToString(dCapturedValues["Additional"], CultureInfo.InvariantCulture), "Erika Muster");
    }

    [TestMethod]
    public void SetTrauerAnz_MapsKnownFilter_And_PreservesExistingIdTrauerfall()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        var dCurrent = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["idTrauerfall"] = 99L,
            ["Rubrik"] = 1234,
            ["Additional"] = "{}"
        };
        var dTrauerfall = CreateAnnouncementInput();
        dTrauerfall["filter"] = "nachrufe";

        using var xHandler = new DataHandler(xRepository, xFile);

        xHandler.SetTrauerAnz(dCurrent, dTrauerfall, @"C:\data");

        Assert.AreEqual(99L, dCurrent["idTrauerfall"]);
        Assert.AreEqual(8070, dCurrent["Rubrik"]);
        Assert.AreEqual(17, dCurrent["Announcement"]);
        Assert.AreEqual("Erika", dCurrent["Firstname"]);
        Assert.AreEqual("Muster", dCurrent["Lastname"]);
        Assert.AreEqual("notice.pdf", dCurrent["pdfFile"]);
    }

    [TestMethod]
    public void SetTrauerAnz_RestoresFilterFromAdditional_WhenFilterIsMissing()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        var dCurrent = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["Rubrik"] = 1234,
            ["Additional"] = "{\"filter\":\"danksagungen\"}"
        };
        var dTrauerfall = CreateAnnouncementInput();
        dTrauerfall["filter"] = string.Empty;

        using var xHandler = new DataHandler(xRepository, xFile);

        xHandler.SetTrauerAnz(dCurrent, dTrauerfall, @"C:\data");

        Assert.AreEqual(1234, dCurrent["Rubrik"]);
        Assert.AreEqual(17, dCurrent["Announcement"]);
        Assert.AreEqual("image.png", dCurrent["pngFile"]);
        Assert.AreEqual("danksagungen", dTrauerfall["filter"]);
    }

    [TestMethod]
    public void SetTrauerAnz_IgnoresInvalidAdditionalJson_WhenFilterIsMissing()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        var dCurrent = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["Rubrik"] = 1234,
            ["Additional"] = "{invalid json"
        };
        var dTrauerfall = CreateAnnouncementInput();
        dTrauerfall["filter"] = string.Empty;

        using var xHandler = new DataHandler(xRepository, xFile);

        xHandler.SetTrauerAnz(dCurrent, dTrauerfall, @"C:\data");

        Assert.AreEqual(1234, dCurrent["Rubrik"]);
        Assert.AreEqual(17, dCurrent["Announcement"]);
        Assert.AreEqual(string.Empty, dTrauerfall["filter"]);
    }

    [TestMethod]
    public void ExtractTrauerData_ExtractsInlineAnnouncementSection()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        using var xHandler = new DataHandler(xRepository, xFile);

        var xRoot = JsonNode.Parse(
            """
            {
              "sections": [
                {
                  "text": ["ANZ 1"],
                  "links": [{ "href": "case1" }]
                }
              ],
              "case1/anzeigen": {
                "sections": [
                  {
                    "class": "col-12",
                    "text": ["x", "geb. Muster"],
                    "imgs": [{ "src": "https://example.invalid/MEDIA/profiles/portrait.png?x=1" }]
                  },
                  {
                    "class": "container col-12",
                    "id": "ann_17",
                    "text": ["x", "vom 03.04.2024"],
                    "filter": "danksagungen",
                    "links": [null, { "href": "https://example.invalid/assets/image.png" }, { "href": "pdf.json" }]
                  }
                ],
                "name": "Erika Muster",
                "created_by": "tester",
                "created_on": "01.04.2024",
                "url": "https://example.invalid/trauerfall",
                "Birth": "* 01.02.2000",
                "Death": "† 05.06.2024",
                "Place": "Heidelberg",
                "visits": 7,
                "pdf.json": { "pdfText": "PDF content" }
              }
            }
            """);

        var arrResult = xHandler.ExtractTrauerData(xRoot, @"C:\data");

        Assert.AreEqual(1, arrResult.Count);
        var dResult = arrResult[0];
        Assert.AreEqual("17", dResult["id"]);
        Assert.AreEqual("danksagungen", dResult["filter"]);
        Assert.AreEqual("Erika Muster", dResult["name"]);
        Assert.AreEqual("Muster", dResult["Birthname"]);
        Assert.AreEqual("Heidelberg", dResult["Place"]);
        Assert.AreEqual("PDF content", dResult["pdfText"]);
        StringAssert.Contains(Convert.ToString(dResult["profImg"], CultureInfo.InvariantCulture), "profiles");
    }

    [TestMethod]
    public void ExtractTrauerData_ReturnsEmptyForNonObjectRoot()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();

        using var xHandler = new DataHandler(xRepository, xFile);

        var arrResult = xHandler.ExtractTrauerData(JsonValue.Create(1), @"C:\data");

        Assert.AreEqual(0, arrResult.Count);
    }

    [TestMethod]
    public void ExtractTrauerData_IgnoresNonAnnouncementSections()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        using var xHandler = new DataHandler(xRepository, xFile);

        var xRoot = JsonNode.Parse(
            """
            {
              "sections": [
                {
                  "text": ["Not an announcement"],
                  "links": [{ "href": "case1" }]
                }
              ]
            }
            """);

        var arrResult = xHandler.ExtractTrauerData(xRoot, @"C:\data");

        Assert.AreEqual(0, arrResult.Count);
    }

    [TestMethod]
    public void ExtractTrauerData_UsesCachedAnnouncementFileWhenInlineSectionIsMissing()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        xFile.Exists(Arg.Any<string>()).Returns(true);
        xFile.ReadAllText(Arg.Any<string>()).Returns(
            """
            {
              "sections": [
                {
                  "class": "container col-12",
                  "id": "ann_18",
                  "text": ["x", "vom 04.04.2024"],
                  "filter": "nachrufe",
                  "links": [null, { "href": "https://example.invalid/assets/image.png" }, { "href": "pdf.json" }]
                }
              ],
              "name": "Klaus Beispiel",
              "created_by": "tester",
              "created_on": "02.04.2024",
              "url": "https://example.invalid/trauerfall-2",
              "Birth": "* 01.03.2000",
              "Death": "† 06.06.2024",
              "Place": "Mannheim",
              "pdf.json": { "pdfText": "Cached PDF content" }
            }
            """);

        using var xHandler = new DataHandler(xRepository, xFile);

        var xRoot = JsonNode.Parse(
            """
            {
              "sections": [
                {
                  "text": ["ANZ 2"],
                  "links": [{ "href": "case2" }]
                }
              ]
            }
            """);

        var arrResult = xHandler.ExtractTrauerData(xRoot, @"C:\data");

        Assert.AreEqual(1, arrResult.Count);
        var dResult = arrResult[0];
        Assert.AreEqual("18", dResult["id"]);
        Assert.AreEqual("Klaus Beispiel", dResult["name"]);
        Assert.AreEqual("nachrufe", dResult["filter"]);
        Assert.AreEqual("Cached PDF content", dResult["pdfText"]);
    }

    [TestMethod]
    public void ExtractTrauerData_UsesPdfBytesWhenPdfJsonIsMissing()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        xFile.Exists(Arg.Any<string>()).Returns(true);
        xFile.ReadAllBytes(Arg.Any<string>()).Returns([1, 2, 3, 4]);

        using var xHandler = new DataHandler(xRepository, xFile);

        var xRoot = JsonNode.Parse(
            """
            {
              "sections": [
                {
                  "text": ["ANZ 5"],
                  "links": [{ "href": "case5" }]
                }
              ],
              "case5/anzeigen": {
                "sections": [
                  {
                    "class": "container col-12",
                    "id": "ann_19",
                    "text": ["x", "vom 05.04.2024"],
                    "filter": "danksagungen",
                    "links": [null, { "href": "https://example.invalid/assets/image.png" }, { "href": "pdf.bin" }]
                  }
                ],
                "name": "Paul Beispiel",
                "created_by": "tester",
                "created_on": "03.04.2024",
                "url": "https://example.invalid/trauerfall-3",
                "Birth": "* 01.04.2000",
                "Death": "† 06.06.2024",
                "Place": "Mannheim"
              }
            }
            """);

        var arrResult = xHandler.ExtractTrauerData(xRoot, @"C:\data");

        Assert.AreEqual(1, arrResult.Count);
        Assert.AreEqual(string.Empty, arrResult[0]["pdfText"]);
    }

    [TestMethod]
    public void AppendTrauerAnz_UsesLocalPathWhenImageAndPdfAreMissing()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        IReadOnlyDictionary<string, object?>? dCapturedValues = null;
        xRepository.AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>()).Returns(xCall =>
        {
            dCapturedValues = xCall.ArgAt<IReadOnlyDictionary<string, object?>>(0);
            return 14L;
        });
        var dTrauerfall = CreateAnnouncementInput();
        dTrauerfall["img"] = string.Empty;
        dTrauerfall["pdf"] = string.Empty;

        using var xHandler = new DataHandler(xRepository, xFile);

        var iResult = xHandler.AppendTrauerAnz(55, dTrauerfall, @"C:\data\root\folder");

        Assert.AreEqual(14L, iResult);
        Assert.IsNotNull(dCapturedValues);
        Assert.AreEqual(string.Empty, dCapturedValues!["pngFile"]);
        Assert.AreEqual(string.Empty, dCapturedValues["pdfFile"]);
        Assert.AreEqual(@"C:\data\root", dCapturedValues["localpath"]);
    }

    [TestMethod]
    public void AppendTrauerAnz_UsesTodesanzeigenRubrik_WhenFilterMatches()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        IReadOnlyDictionary<string, object?>? dCapturedValues = null;
        xRepository.AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>()).Returns(xCall =>
        {
            dCapturedValues = xCall.ArgAt<IReadOnlyDictionary<string, object?>>(0);
            return 15L;
        });
        var dTrauerfall = CreateAnnouncementInput();
        dTrauerfall["filter"] = "todesanzeigen";

        using var xHandler = new DataHandler(xRepository, xFile);

        var iResult = xHandler.AppendTrauerAnz(55, dTrauerfall, @"C:\data");

        Assert.AreEqual(15L, iResult);
        Assert.IsNotNull(dCapturedValues);
        Assert.AreEqual(8050, dCapturedValues!["Rubrik"]);
    }

    [TestMethod]
    public void ExtractTrauerData_ReturnsEmptyWhenCachedAnnouncementFileIsMissing()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        xFile.Exists(Arg.Any<string>()).Returns(false);

        using var xHandler = new DataHandler(xRepository, xFile);

        var xRoot = JsonNode.Parse(
            """
            {
              "sections": [
                {
                  "text": ["ANZ 3"],
                  "links": [{ "href": "case3" }]
                }
              ]
            }
            """);

        var arrResult = xHandler.ExtractTrauerData(xRoot, @"C:\data");

        Assert.AreEqual(0, arrResult.Count);
    }

    [TestMethod]
    public void ExtractTrauerData_IgnoresSectionsWithoutLinks()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();

        using var xHandler = new DataHandler(xRepository, xFile);

        var xRoot = JsonNode.Parse(
            """
            {
              "sections": [
                {
                  "text": ["ANZ 4"]
                }
              ]
            }
            """);

        var arrResult = xHandler.ExtractTrauerData(xRoot, @"C:\data");

        Assert.AreEqual(0, arrResult.Count);
    }

    [TestMethod]
    public void TrauerDataToDb_SkipsIncompleteRows_And_UpdatesExistingAnnouncement()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        var dExistingCase = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["idTrauerfall"] = 88L
        };
        var dExistingAnnouncement = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["idTrauerfall"] = 88L,
            ["Rubrik"] = 8050,
            ["Additional"] = "{\"filter\":\"danksagungen\"}"
        };
        var dAnnouncement = CreateAnnouncementInput();
        dAnnouncement["created_on"] = "01.04.2024";
        dAnnouncement["publish"] = "03.04.2024";

        xRepository.TrauerFallByUrl("https://example.invalid/trauerfall").Returns(new List<Dictionary<string, object?>> { dExistingCase });
        xRepository.TrauerAnz(17).Returns(new List<Dictionary<string, object?>> { dExistingAnnouncement });
        xRepository.UpdateTrauerAnz(Arg.Any<List<Dictionary<string, object?>>>() , Arg.Any<List<Dictionary<string, object?>>>() ).Returns(true);

        using var xHandler = new DataHandler(xRepository, xFile);

        xHandler.TrauerDataToDb(
        [
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["url"] = "https://example.invalid/incomplete"
            },
            dAnnouncement
        ], @"C:\data");

        _ = xRepository.Received(1).TrauerFallByUrl("https://example.invalid/trauerfall");
        _ = xRepository.Received(1).TrauerAnz(17);
        _ = xRepository.DidNotReceive().AppendTrauerFall(Arg.Any<IReadOnlyDictionary<string, object?>>());
        _ = xRepository.DidNotReceive().AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>());
        _ = xRepository.Received(1).UpdateTrauerAnz(Arg.Any<List<Dictionary<string, object?>>>() , Arg.Any<List<Dictionary<string, object?>>>() );
    }

    [TestMethod]
    public void TrauerDataToDb_InsertsNewRows_WhenNothingExists()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        var dAnnouncement = CreateAnnouncementInput();
        dAnnouncement["created_on"] = "01.04.2024";
        dAnnouncement["publish"] = "03.04.2024";
        xRepository.TrauerFallByUrl(Arg.Any<string>()).Returns(new List<Dictionary<string, object?>>());
        xRepository.TrauerAnz(Arg.Any<int>()).Returns(new List<Dictionary<string, object?>>());
        xRepository.AppendTrauerFall(Arg.Any<IReadOnlyDictionary<string, object?>>()).Returns(201L);
        xRepository.AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>()).Returns(202L);

        using var xHandler = new DataHandler(xRepository, xFile);

        using var xWriter = new StringWriter();
        var xOriginal = System.Console.Out;
        System.Console.SetOut(xWriter);
        try
        {
            xHandler.TrauerDataToDb([dAnnouncement], @"C:\data");
        }
        finally
        {
            System.Console.SetOut(xOriginal);
        }

        _ = xRepository.Received(1).TrauerFallByUrl("https://example.invalid/trauerfall");
        _ = xRepository.Received(1).TrauerAnz(17);
        _ = xRepository.Received(1).AppendTrauerFall(Arg.Any<IReadOnlyDictionary<string, object?>>());
        _ = xRepository.Received(1).AppendTrauerAnz(Arg.Any<IReadOnlyDictionary<string, object?>>());
        _ = xRepository.DidNotReceive().UpdateTrauerAnz(Arg.Any<List<Dictionary<string, object?>>>() , Arg.Any<List<Dictionary<string, object?>>>() );
        StringAssert.Contains(xWriter.ToString(), "+");
    }

    private static Dictionary<string, object?> CreateAnnouncementInput()
    {
        return new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["url"] = "https://example.invalid/trauerfall",
            ["id"] = "17",
            ["publish"] = "03.04.2024",
            ["img"] = @"C:\data\2024\folder\image.png",
            ["pdf"] = @"C:\data\2024\folder\notice.pdf",
            ["name"] = "Erika Muster",
            ["Birthname"] = "Beispiel",
            ["Birth"] = "* 01.02.2000",
            ["Death"] = "† 05.06.2024",
            ["Place"] = "Heidelberg",
            ["pdfText"] = "PDF content",
            ["profImg"] = @"C:\data\profiles\portrait.png",
            ["filter"] = "danksagungen"
        };
    }

    [TestMethod]
    public void ExtractTrauerData_DoesNotSetProfileImage_When_ImageSource_Is_Not_Media()
    {
        var xRepository = Substitute.For<ITrauerDataRepository>();
        var xFile = Substitute.For<IFile>();
        using var xHandler = new DataHandler(xRepository, xFile);

        var xRoot = JsonNode.Parse(
            """
            {
              "sections": [
                {
                  "text": ["ANZ 6"],
                  "links": [{ "href": "case6" }]
                }
              ],
              "case6/anzeigen": {
                "sections": [
                  {
                    "class": "col-12",
                    "text": ["x"],
                    "imgs": [{ "src": "https://example.invalid/assets/profile.png" }]
                  },
                  {
                    "class": "container col-12",
                    "id": "ann_26",
                    "text": ["x", "vom 06.04.2024"],
                    "filter": "nachrufe",
                    "links": [null, { "href": "https://example.invalid/assets/image.png" }, { "href": "pdf.json" }]
                  }
                ],
                "name": "Lena Beispiel",
                "created_by": "tester",
                "created_on": "06.04.2024",
                "url": "https://example.invalid/trauerfall-6",
                "Birth": "* 01.01.1990",
                "Death": "† 01.01.2024",
                "Place": "Heidelberg",
                "pdf.json": { "pdfText": "PDF content" }
              }
            }
            """);

        var arrResult = xHandler.ExtractTrauerData(xRoot, @"C:\data");

        Assert.AreEqual(1, arrResult.Count);
        Assert.AreEqual(string.Empty, Convert.ToString(arrResult[0]["profImg"], CultureInfo.InvariantCulture));
    }
}
