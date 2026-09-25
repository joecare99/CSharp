using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Profiles;

namespace Osb.Core.Tests.Profiles;

[TestClass]
public sealed class PublicationProfileTests
{
    [TestMethod]
    public void Constructor_AllowsEmptySelectionSet() 
    {
        var profile = new PublicationProfile(
            1,
            "Familienliste",
            PlaceSelectionMode.IncludeOnly,
            Array.Empty<string>(),
            "{name}",
            "{footer}");

        Assert.AreEqual("Familienliste", profile.Name);
        Assert.AreEqual(PlaceSelectionMode.IncludeOnly, profile.InclusionMode);
        Assert.AreEqual(0, profile.SelectedPlaceIds.Count);
    }

    [TestMethod]
    public void Constructor_RejectsInvalidInput()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new PublicationProfile(
            1,
            " ",
            PlaceSelectionMode.IncludeOnly,
            Array.Empty<string>(),
            "{name}",
            "{footer}"));

        Assert.ThrowsExactly<ArgumentException>(() => new PublicationProfile(
            1,
            "Familienliste",
            PlaceSelectionMode.IncludeOnly,
            new[] { "Ort 1", string.Empty },
            "{name}",
            "{footer}"));
    }

    [TestMethod]
    public void Serialize_RoundTripsProfile()
    {
        var profile = new PublicationProfile(
            7,
            "Testprofil",
            PlaceSelectionMode.ExcludeSelected,
            new[] { "A-1", "B-2", "C-3" },
            "Ausgabe für {name}",
            "Ende");

        var serialized = PublicationProfileCodec.Serialize(profile);
        var actual = PublicationProfileCodec.Deserialize(serialized);

        Assert.AreEqual(profile.SchemaVersion, actual.SchemaVersion);
        Assert.AreEqual(profile.Name, actual.Name);
        Assert.AreEqual(profile.InclusionMode, actual.InclusionMode);
        CollectionAssert.AreEqual(profile.SelectedPlaceIds.ToArray(), actual.SelectedPlaceIds.ToArray());
        Assert.AreEqual(profile.TitleTemplate, actual.TitleTemplate);
        Assert.AreEqual(profile.FooterTemplate, actual.FooterTemplate);
    }

    [TestMethod]
    public void Deserialize_RejectsMissingOrInvalidStructure()
    {
        Assert.ThrowsExactly<ArgumentException>(() => PublicationProfileCodec.Deserialize(" "));
        Assert.ThrowsExactly<FormatException>(() => PublicationProfileCodec.Deserialize("schemaVersion=1\nname=Test\nmode=Invalid\n"));
        Assert.ThrowsExactly<FormatException>(() => PublicationProfileCodec.Deserialize("schemaVersion=1\nname=Test\nmode=IncludeOnly\nfooter=END\n"));
    }
}
