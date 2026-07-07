using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies deterministic planning explorer view-model behavior.
/// </summary>
[TestClass]
public class PlanningExplorerViewModelTests
{
    /// <summary>
    /// Verifies that planning items are loaded into a stable root and child hierarchy.
    /// </summary>
    [TestMethod]
    public async Task LoadAsync_BuildsDeterministicHierarchy()
    {
        IPlanningProvider planningReader = Substitute.For<IPlanningProvider>();
        planningReader
            .ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(CreateHierarchyResult()));

        PlanningExplorerViewModel viewModel = new(planningReader);

        await viewModel.LoadAsync("C:\\Repo");

        Assert.AreEqual(1, viewModel.RootItems.Count);
        PlanningExplorerItemViewModel epic = viewModel.RootItems.Single();
        Assert.AreEqual("AA98-E12", epic.Id);
        Assert.AreEqual(PlanningItemKind.Epic, epic.Kind);
        Assert.AreEqual(1, epic.Children.Count);

        PlanningExplorerItemViewModel feature = epic.Children.Single();
        Assert.AreEqual("AA98-F43", feature.Id);
        Assert.AreEqual("AA98-E12", feature.ParentId);
        Assert.AreEqual(1, feature.Children.Count);

        PlanningExplorerItemViewModel task = feature.Children.Single();
        Assert.AreEqual("AA98-T062", task.Id);
        Assert.AreEqual("AA98-F43", task.ParentId);
        Assert.AreEqual("DevOps\\Tasks\\AA98-T062-Implement-Planning-Explorer-ViewModel.md", task.SourcePath);
    }

    /// <summary>
    /// Verifies that selected item metadata and diagnostics are exposed after loading.
    /// </summary>
    [TestMethod]
    public async Task LoadAsync_ExposesSelectionMetadataAndDiagnostics()
    {
        IPlanningProvider planningReader = Substitute.For<IPlanningProvider>();
        planningReader
            .ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(CreateResultWithDiagnosticsAndOrphan()));

        PlanningExplorerViewModel viewModel = new(planningReader);

        await viewModel.LoadAsync("C:\\Repo");

        Assert.AreEqual(2, viewModel.RootItems.Count);
        Assert.IsNotNull(viewModel.SelectedItem);
        Assert.AreEqual(viewModel.RootItems[0].Id, viewModel.SelectedItemId);

        viewModel.SelectedItem = viewModel.RootItems.Single(static item => item.Id == "AA98-T999");

        Assert.AreEqual("AA98-T999", viewModel.SelectedItemId);
        Assert.AreEqual("Orphan Task", viewModel.SelectedItemTitle);
        Assert.AreEqual(PlanningItemKind.Task, viewModel.SelectedItemKind);
        Assert.AreEqual(PlanningItemStatus.Proposed, viewModel.SelectedItemStatus);
        Assert.AreEqual("DevOps\\Tasks\\AA98-T999-Orphan.md", viewModel.SelectedItemSourcePath);
        Assert.AreEqual("AA98-Bl999", viewModel.SelectedItemParentId);

        Assert.IsTrue(viewModel.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN001"));
        Assert.IsTrue(viewModel.Diagnostics.Any(static diagnostic => diagnostic.Code == "PLN031"));
    }

    private static PlanningReadResult CreateHierarchyResult()
    {
        PlanningItem epic = new()
        {
            Id = "AA98-E12",
            Title = "DevOps Planning Workbench",
            Kind = PlanningItemKind.Epic,
            Status = PlanningItemStatus.InProgress,
            SourcePath = "DevOps\\Epics\\AA98-E12-DevOps-Planning-Workbench.md",
        };

        PlanningItem feature = new()
        {
            Id = "AA98-F43",
            Title = "Repository and Planning Workflows",
            Kind = PlanningItemKind.Feature,
            Status = PlanningItemStatus.InProgress,
            SourcePath = "DevOps\\Features\\AA98-F43-Repository-and-Planning-Workflows.md",
            Parent = new PlanningItemLink
            {
                ItemId = "AA98-E12",
                Kind = PlanningItemKind.Epic,
            },
        };

        PlanningItem task = new()
        {
            Id = "AA98-T062",
            Title = "Implement Planning Explorer ViewModel",
            Kind = PlanningItemKind.Task,
            Status = PlanningItemStatus.Proposed,
            SourcePath = "DevOps\\Tasks\\AA98-T062-Implement-Planning-Explorer-ViewModel.md",
            Parent = new PlanningItemLink
            {
                ItemId = "AA98-F43",
                Kind = PlanningItemKind.Feature,
            },
        };

        PlanningReadResult result = new();
        result.Items.Add(epic);
        result.Items.Add(feature);
        result.Items.Add(task);
        return result;
    }

    private static PlanningReadResult CreateResultWithDiagnosticsAndOrphan()
    {
        PlanningItem epic = new()
        {
            Id = "AA98-E12",
            Title = "DevOps Planning Workbench",
            Kind = PlanningItemKind.Epic,
            Status = PlanningItemStatus.InProgress,
            SourcePath = "DevOps\\Epics\\AA98-E12-DevOps-Planning-Workbench.md",
        };

        PlanningItem orphanTask = new()
        {
            Id = "AA98-T999",
            Title = "Orphan Task",
            Kind = PlanningItemKind.Task,
            Status = PlanningItemStatus.Proposed,
            SourcePath = "DevOps\\Tasks\\AA98-T999-Orphan.md",
            Parent = new PlanningItemLink
            {
                ItemId = "AA98-Bl999",
                Kind = PlanningItemKind.BacklogItem,
            },
        };

        orphanTask.Diagnostics.Add(new Diagnostic
        {
            Code = "PLN031",
            Severity = DiagnosticSeverity.Warning,
            Message = "Missing parent.",
            SourcePath = orphanTask.SourcePath,
        });

        PlanningReadResult result = new();
        result.Items.Add(epic);
        result.Items.Add(orphanTask);
        result.Diagnostics.Add(new Diagnostic
        {
            Code = "PLN001",
            Severity = DiagnosticSeverity.Error,
            Message = "Planning root missing.",
            SourcePath = "DevOps",
        });

        return result;
    }
}
