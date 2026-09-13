using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Project.Explorer.Tests;

/// <summary>Tests the UI-neutral explorer contract invariants and state transitions.</summary>
[TestClass]
public sealed class ProjectExplorerContractsTests
{
    [TestMethod]
    public void Item_NormalizesPathAndPreservesHierarchy()
    {
        string rootPath = Path.Combine(Path.GetTempPath(), "ProjectExplorerContracts");
        ProjectExplorerItem file = new("file:readme", "README.md", Path.Combine(rootPath, "README.md"), ProjectExplorerItemKind.File);
        ProjectExplorerItem project = new("project:root", "Root", rootPath, ProjectExplorerItemKind.Project, children: [file]);

        Assert.AreEqual(Path.GetFullPath(rootPath), project.Path);
        Assert.AreEqual(1, project.Children.Count);
        Assert.AreSame(file, project.Children[0]);
    }

    [TestMethod]
    public void State_TracksSelectionAndExpansionTransitions()
    {
        ProjectExplorerState state = new();

        state.Select("folder:src");
        state.SetExpanded("folder:src", true);
        state.SetExpanded("folder:src", false);
        state.Select(null);

        Assert.IsNull(state.SelectedItemId);
        Assert.IsFalse(state.ExpandedItemIds.Contains("folder:src"));
    }

    [TestMethod]
    public void Item_RejectsMissingStableIdentity()
    {
        Assert.Throws<ArgumentException>(
            () => new ProjectExplorerItem("", "Root", Path.GetTempPath(), ProjectExplorerItemKind.Project));
    }
}
