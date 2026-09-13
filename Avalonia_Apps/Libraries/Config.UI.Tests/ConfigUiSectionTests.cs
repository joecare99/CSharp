using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Config.UI.Tests;

/// <summary>
/// Verifies Config.UI section metadata invariants.
/// </summary>
[TestClass]
public sealed class ConfigUiSectionTests
{
    [TestMethod]
    public void Constructor_ValidMetadata_PreservesLocalizedValues()
    {
        ConfigUiSection section = new("Database", "Database settings", "Configure the connection.", 2);

        Assert.AreEqual("Database", section.Name);
        Assert.AreEqual("Database settings", section.DisplayName);
        Assert.AreEqual("Configure the connection.", section.Description);
        Assert.AreEqual(2, section.SortOrder);
    }

    [TestMethod]
    public void Constructor_MissingName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ConfigUiSection("", "Display"));
    }

    [TestMethod]
    public void Constructor_MissingDisplayName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ConfigUiSection("Name", ""));
    }
}
