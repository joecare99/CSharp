using Microsoft.VisualStudio.TestTools.UnitTesting;
using RnzTrauer.Core;

namespace RnzTrauer.Tests;

[TestClass]
public sealed class PortedHelpersTests
{
    private const string LocalRoot = "\\\\Diskstation\\Daten\\dokumente\\HTML\\trauer.rnz.de";

    [TestMethod]
    public void LCropStr_Returns_Left_Part()
    {
        Assert.AreEqual("123", PortedHelpers.LCropStr("1234567", "4"));
    }

    [DataTestMethod]
    [DataRow("A B", "B", "A")]
    [DataRow("aa bb", "Bb", "Aa")]
    [DataRow(" a b c ", "C", "A B")]
    [DataRow(" aü b-c ", "B-C", "Aü")]
    [DataRow("üa bö-cä", "Bö-Cä", "Üa")]
    [DataRow("üa öb-äc", "Öb-Äc", "Üa")]
    [DataRow(" a von c ", "von C", "A")]
    [DataRow(" aa bb-cc ", "Bb-Cc", "Aa")]
    public void SplitName_Matches_Python_Behaviour(string input, string expectedLastName, string expectedFirstName)
    {
        var (lastName, firstName) = PortedHelpers.SplitName(input);
        Assert.AreEqual(expectedLastName, lastName);
        Assert.AreEqual(expectedFirstName, firstName);
    }

    [DataTestMethod]
    [DataRow("https://trauer.rnz.de/MEDIASERVER/content/LH230/obi_new/2022_11/waltraud-hacker-traueranzeige-697fddce-93c0-4b07-af61-bb046a47cbba.jpg", "\\\\Diskstation\\Daten\\dokumente\\HTML\\trauer.rnz.de\\MEDIASERVER\\content\\LH230\\obi_new\\2022_11\\waltraud-hacker-traueranzeige-697fddce-93c0-4b07-af61-bb046a47cbba.jpg")]
    [DataRow("https://trauer.rnz.de/traueranzeigen-suche/erscheinungstag-01-10-2021", "\\\\Diskstation\\Daten\\dokumente\\HTML\\trauer.rnz.de\\2021\\2021-10-01\\liste-01-10-2021")]
    [DataRow("https://trauer.rnz.de/traueranzeigen-suche/erscheinungstag-01-10-2021/seite-3", "\\\\Diskstation\\Daten\\dokumente\\HTML\\trauer.rnz.de\\2021\\2021-10-01\\liste-01-10-2021-pg-3")]
    [DataRow("https://trauer.rnz.de/traueranzeigen-suche/erscheinungstag-3-8-2021/anzeigenart-danksagungen/seite-3", "\\\\Diskstation\\Daten\\dokumente\\HTML\\trauer.rnz.de\\2021\\2021-08-03\\liste-3-8-2021-d-pg-3")]
    [DataRow("https://trauer.rnz.de/traueranzeige/aktuelle-ausgabe?code=934598745894569734506987456304567", "\\\\Diskstation\\Daten\\dokumente\\HTML\\trauer.rnz.de\\traueranzeige\\22D\\aktuelle-ausgabe")]
    public void GetLocalPath_Matches_Known_Paths(string url, string expected)
    {
        var actual = PortedHelpers.GetLocalPath(url, LocalRoot, new DateOnly(2024, 12, 24));
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetLocalPath_Handles_AktuelleAusgabe_And_Heute()
    {
        var day = new DateOnly(2024, 12, 24);
        Assert.AreEqual($"{LocalRoot}\\2024\\2024-12-24\\liste-24-12-2024", PortedHelpers.GetLocalPath("https://trauer.rnz.de/traueranzeigen-suche/aktuelle-ausgabe", LocalRoot, day));
        Assert.AreEqual($"{LocalRoot}\\2024\\2024-12-24\\liste-heute-pg-2", PortedHelpers.GetLocalPath("https://trauer.rnz.de/traueranzeigen-suche/erscheinungstag-heute/seite-2", LocalRoot, day));
    }

    [DataTestMethod]
    [DataRow("<html><img src=\"/images/picture.jpg\"></html>", "https://www.jc99.de/test/text2", "<html><img src=\"../images/picture.png\"></html>")]
    [DataRow("<html><img src=\"https://www.jc99.de/images/picture.jpg\"></html>", "https://www.jc99.de/test/test/text2", "<html><img src=\"../../images/picture.png\"></html>")]
    [DataRow("<html><a href=\"/test/data\"></html>", "https://www.jc99.de/test/text2", "<html><a href=\"../test/data\"></html>")]
    [DataRow("<html><a href=\"https://www.jc99.de/test/data\"></html>", "https://www.jc99.de/test/test/text2", "<html><a href=\"../../test/data\"></html>")]
    public void MakeLocal_Matches_Python_Behaviour(string source, string reference, string expected)
    {
        Assert.AreEqual(expected, PortedHelpers.MakeLocal(source, reference));
    }
}
