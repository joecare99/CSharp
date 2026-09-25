using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Osb.Core.Selection;

namespace Osb.Core.Tests.Selection;

[TestClass]
public sealed class FamilySelectionScopeTests
{
    [TestMethod]
    public void Constructor_NormalizesAndPreservesSelectionState()
    {
        var scope = new FamilySelectionScope(
            12,
            34,
            new[] { 9, 5, 5, 7 },
            new[] { 100, 90, 90 },
            SelectionInclusionMode.IncludeOnly);

        CollectionAssert.AreEqual(new[] { 5, 7, 9 }, scope.SelectedPersonIds.ToArray());
        CollectionAssert.AreEqual(new[] { 90, 100 }, scope.SelectedFamilyIds.ToArray());
        Assert.AreEqual(12, scope.InitialPersonId);
        Assert.AreEqual(34, scope.InitialFamilyId);
    }

    [TestMethod]
    public void InclusionMode_MapsToSelectionChecks()
    {
        var scope = new FamilySelectionScope(
            0,
            0,
            new[] { 4, 7 },
            new[] { 10 },
            SelectionInclusionMode.ExcludeSelected);

        Assert.IsFalse(scope.IsPersonSelected(4));
        Assert.IsTrue(scope.IsPersonSelected(5));
        Assert.IsFalse(scope.IsFamilySelected(10));
        Assert.IsTrue(scope.IsFamilySelected(11));
    }

    [TestMethod]
    public void Constructor_RejectsNegativeIds()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FamilySelectionScope(
            -1,
            0,
            Array.Empty<int>(),
            Array.Empty<int>(),
            SelectionInclusionMode.IncludeOnly));

        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new FamilySelectionScope(
            0,
            -1,
            Array.Empty<int>(),
            Array.Empty<int>(),
            SelectionInclusionMode.IncludeOnly));
    }
}
