using AppKomponentBaseLib.Components;
using AppKomponentBaseLib.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AA98_AvlnCodeStudio.Tests.Components.Configuration;

/// <summary>
/// Verifies the shared application-context contracts.
/// </summary>
[TestClass]
public sealed class AppContextContractTests
{
    /// <summary>
    /// Verifies that context targets normalize identifiers and roles.
    /// </summary>
    [TestMethod]
    public void ContextTargetNormalizesRoles()
    {
        var target = new AppContextTarget(" DocumentNode ", " Node-1 ", new[] { "file", " file ", "selection", string.Empty });

        Assert.AreEqual("DocumentNode", target.TargetType);
        Assert.AreEqual("Node-1", target.TargetId);
        CollectionAssert.AreEqual(new[] { "file", "selection" }, (System.Collections.ICollection)target.Roles);
    }

    /// <summary>
    /// Verifies that context targets reject missing target types.
    /// </summary>
    [TestMethod]
    public void ContextTargetRejectsMissingType()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new AppContextTarget(string.Empty));
    }

    /// <summary>
    /// Verifies that context snapshots preserve active component and targets.
    /// </summary>
    [TestMethod]
    public void ContextSnapshotStoresMetadata()
    {
        var component = new AppComponentDescriptor("ImageEditor", "Image Editor");
        var target = new AppContextTarget("Canvas", "MainCanvas", new[] { "surface" });
        var snapshot = new AppContextSnapshot("ImageEditor", component, new[] { target });

        Assert.AreEqual("ImageEditor", snapshot.ActiveComponentId);
        Assert.AreSame(component, snapshot.ActiveComponent);
        Assert.AreEqual(1, snapshot.Targets.Count);
        Assert.AreSame(target, snapshot.Targets[0]);
        Assert.IsNull(snapshot.Services);
    }
}
