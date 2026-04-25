using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using RnzTrauer.Core;

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
}
