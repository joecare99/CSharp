using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Selection;

namespace Osb.Core.Tests.Selection;

[TestClass]
public sealed class FamilySelectionRequestTests
{
    [TestMethod]
    public void Constructor_PreservesSelectionInputs()
    {
        var options = new FamilySelectionOptions("20200000", true, "20210000", false);
        var request = new FamilySelectionRequest(12, 34, 56, 78, 3, options);

        Assert.AreEqual(12, request.InitialPersonId);
        Assert.AreEqual(34, request.InitialFamilyId);
        Assert.AreEqual(56, request.MalePersonId);
        Assert.AreEqual(78, request.FemalePersonId);
        Assert.AreEqual((short)3, request.TraversalStep);
        Assert.AreSame(options, request.Options);
    }

    [TestMethod]
    public void EqualValues_CompareEqual()
    {
        var first = CreateRequest();
        var second = CreateRequest();

        Assert.AreEqual(first, second);
    }

    [TestMethod]
    public void Constructor_RejectsNegativeIdentifiersAndTraversalStep()
    {
        var options = new FamilySelectionOptions("20200000", false, "20200000", false);

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new FamilySelectionRequest(-1, 0, 0, 0, 0, options));
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => new FamilySelectionRequest(0, 0, 0, 0, -1, options));
    }

    [TestMethod]
    public void Options_RejectNullOrNonLegacyDateValues()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new FamilySelectionOptions(null, false, "20200000", false));
        Assert.ThrowsExactly<ArgumentException>(
            () => new FamilySelectionOptions("2020-01-01", false, "20200000", false));
        Assert.ThrowsExactly<ArgumentException>(
            () => new FamilySelectionOptions("20200000", false, "2020ABCD", false));
    }

    [TestMethod]
    public void FromLegacyState_MapsOnlySelectionRelevantSlots()
    {
        var legacyOptions = new string?[95];
        legacyOptions[81] = "20200000";
        legacyOptions[82] = "1";
        legacyOptions[83] = "20210000";
        legacyOptions[94] = "0";

        var request = FamilySelectionRequest.FromLegacyState(1, 2, 3, 4, 5, legacyOptions);

        Assert.AreEqual("20200000", request.Options.PersonCutoffDate);
        Assert.IsTrue(request.Options.ExcludeFamiliesAfterCutoff);
        Assert.AreEqual("20210000", request.Options.FamilyCutoffDate);
        Assert.IsFalse(request.Options.ExcludeSponsorOrWitnessOnlyPeople);
    }

    [TestMethod]
    public void FromLegacyState_RejectsMissingOrInvalidSelectionSlots()
    {
        var missingOptions = new string?[94];
        Assert.ThrowsExactly<ArgumentException>(
            () => FamilySelectionRequest.FromLegacyState(0, 0, 0, 0, 0, missingOptions));

        var invalidOptions = new string?[95];
        invalidOptions[81] = "20200000";
        invalidOptions[82] = "yes";
        invalidOptions[83] = "20210000";
        invalidOptions[94] = "0";
        Assert.ThrowsExactly<ArgumentException>(
            () => FamilySelectionRequest.FromLegacyState(0, 0, 0, 0, 0, invalidOptions));
    }

    private static FamilySelectionRequest CreateRequest()
    {
        return new FamilySelectionRequest(
            12,
            34,
            56,
            78,
            3,
            new FamilySelectionOptions("20200000", true, "20210000", false));
    }
}
