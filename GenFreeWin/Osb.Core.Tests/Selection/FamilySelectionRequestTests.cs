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
