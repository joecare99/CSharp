using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace BaseLib.Show.Tests;

/// <summary>
/// Verifies localized text resolution for the showcase application.
/// </summary>
[TestClass]
public class ShowcaseTextTests
{
    /// <summary>
    /// Ensures that the neutral resource values stay English for unsupported cultures.
    /// </summary>
    [TestMethod]
    public void Get_UsesNeutralEnglishValues_WhenCultureHasNoSpecificTranslation()
    {
        ShowcaseText testee = new();

        string result = testee.Get("Menu_RunAll", CultureInfo.GetCultureInfo("fr-FR"));

        Assert.AreEqual("Run all demos", result);
    }

    /// <summary>
    /// Ensures that German UI text is returned when the German culture is requested.
    /// </summary>
    [TestMethod]
    public void Get_ReturnsGermanTranslation_WhenGermanCultureIsRequested()
    {
        ShowcaseText testee = new();

        string result = testee.Get("Menu_RunAll", CultureInfo.GetCultureInfo("de-DE"));

        Assert.AreEqual("Alle Demos ausführen", result);
    }
}
