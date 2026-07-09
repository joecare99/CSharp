using AA98_AvlnCodeStudio.Base.Components.Commands;
using AppKomponentBaseLib.Context;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Components.Commands;

/// <summary>
/// Verifies the AA98 workbench command contribution contracts.
/// </summary>
[TestClass]
public sealed class WorkbenchCommandContractTests
{
    /// <summary>
    /// Verifies that placement paths are normalized into stable segments.
    /// </summary>
    [TestMethod]
    public void PlacementNormalizesPathSegments()
    {
        var placement = new WorkbenchCommandPlacement(WorkbenchCommandSurface.Menu, " Image / View / ZoomIn ", 42);

        Assert.AreEqual(WorkbenchCommandSurface.Menu, placement.Surface);
        Assert.AreEqual("Image/View/ZoomIn", placement.Path);
        CollectionAssert.AreEqual(new[] { "Image", "View", "ZoomIn" }, (System.Collections.ICollection)placement.PathSegments);
        Assert.AreEqual(42, placement.Order);
    }

    /// <summary>
    /// Verifies that invalid placement paths are rejected.
    /// </summary>
    [TestMethod]
    public void PlacementRejectsMissingPath()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new WorkbenchCommandPlacement(WorkbenchCommandSurface.Menu, string.Empty));
    }

    /// <summary>
    /// Verifies that descriptors preserve stable metadata and placements.
    /// </summary>
    [TestMethod]
    public void DescriptorStoresMetadataAndPlacements()
    {
        var placement = new WorkbenchCommandPlacement(WorkbenchCommandSurface.Toolbar, "Image/Edit", 10);

        var descriptor = new WorkbenchCommandDescriptor(
            "Image.ZoomIn",
            "Zoom In",
            new[] { placement },
            "Increase the active zoom level.",
            "Ctrl++",
            new[] { "EditorSurface", "DocumentTab" });

        Assert.AreEqual("Image.ZoomIn", descriptor.CommandId);
        Assert.AreEqual("Zoom In", descriptor.DisplayTitle);
        Assert.AreEqual("Increase the active zoom level.", descriptor.Description);
        Assert.AreEqual("Ctrl++", descriptor.DefaultGesture);
        Assert.AreEqual(1, descriptor.Placements.Count);
        Assert.AreSame(placement, descriptor.Placements[0]);
        CollectionAssert.AreEqual(new[] { "EditorSurface", "DocumentTab" }, (System.Collections.ICollection)descriptor.ContextKinds);
    }

    /// <summary>
    /// Verifies that descriptors reject missing identifiers and titles.
    /// </summary>
    [TestMethod]
    public void DescriptorRejectsMissingIdentity()
    {
        Assert.ThrowsExactly<ArgumentException>(() => new WorkbenchCommandDescriptor(string.Empty, "Title"));
        Assert.ThrowsExactly<ArgumentException>(() => new WorkbenchCommandDescriptor("Image.ZoomIn", string.Empty));
    }

    /// <summary>
    /// Verifies that descriptors default to no placements when none are specified.
    /// </summary>
    [TestMethod]
    public void DescriptorDefaultsToEmptyPlacements()
    {
        var descriptor = new WorkbenchCommandDescriptor("Image.ZoomOut", "Zoom Out");

        Assert.AreEqual(0, descriptor.Placements.Count);
        Assert.IsNull(descriptor.Description);
        Assert.IsNull(descriptor.DefaultGesture);
        Assert.AreEqual(0, descriptor.ContextKinds.Count);
    }

    /// <summary>
    /// Verifies that popup targets normalize supported context kinds.
    /// </summary>
    [TestMethod]
    public void PopupTargetNormalizesContextKinds()
    {
        var popupTarget = new WorkbenchPopupTarget("Editor.Canvas", new[] { "Canvas", " Canvas ", string.Empty, "Selection" });

        Assert.AreEqual("Editor.Canvas", popupTarget.TargetId);
        CollectionAssert.AreEqual(new[] { "Canvas", "Selection" }, (System.Collections.ICollection)popupTarget.ContextKinds);
    }

    /// <summary>
    /// Verifies that popup contributions can match workbench popup targets.
    /// </summary>
    [TestMethod]
    public void PopupContributionCanMatchPopupTarget()
    {
        var contribution = new TestPopupCommandContribution();
        var popupTarget = new WorkbenchPopupTarget("Editor.Canvas", new[] { "Canvas" });
        var context = new TestWorkbenchCommandContext(new[] { new AppContextTarget("Canvas") });

        Assert.IsTrue(contribution.CanExecute(context));
        Assert.IsTrue(contribution.AppliesTo(context, popupTarget));
    }

    private sealed class TestPopupCommandContribution : IWorkbenchPopupCommandContribution
    {
        public WorkbenchCommandDescriptor Descriptor { get; } = new(
            "Editor.Canvas.ZoomIn",
            "Zoom In",
            new[] { new WorkbenchCommandPlacement(WorkbenchCommandSurface.ContextMenu, "Editor/Canvas", 10) },
            contextKinds: new[] { "Canvas" });

        public bool CanExecute(IWorkbenchCommandContext context)
        {
            return context.Targets.Count > 0;
        }

        public Task ExecuteAsync(IWorkbenchCommandContext context, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public bool AppliesTo(IWorkbenchCommandContext context, WorkbenchPopupTarget popupTarget)
        {
            return popupTarget.ContextKinds.Any(static contextKind => contextKind == "Canvas")
                && context.Targets[0].TargetType == "Canvas";
        }
    }

    private sealed class TestWorkbenchCommandContext : IWorkbenchCommandContext
    {
        public TestWorkbenchCommandContext(IReadOnlyList<AppContextTarget> targets)
        {
            Targets = targets;
        }

        public string? ActiveComponentId => "ImageEditor";

        public string? ActiveDocumentId => "Document-1";

        public IReadOnlyList<AppContextTarget> Targets { get; }

        public IServiceProvider? Services => null;
    }
}
