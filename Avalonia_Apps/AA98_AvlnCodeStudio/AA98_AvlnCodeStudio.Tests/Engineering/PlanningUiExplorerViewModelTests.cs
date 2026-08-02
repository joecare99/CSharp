using AA98_AvlnCodeStudio.Base.UI.Properties;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AppKomponentBaseLib.Diagnostics;
using AA98_AvlnCodeStudio.Planning.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

/// <summary>
/// Verifies planning UI explorer behavior for view modes, status line, and generic properties.
/// </summary>
[TestClass]
public class PlanningUiExplorerViewModelTests
{
    /// <summary>
    /// Verifies that loading builds both hierarchy and grouped category data.
    /// </summary>
    [TestMethod]
    public async Task LoadAsync_BuildsHierarchyAndCategoryGroups()
    {
        IPlanningProvider provider = Substitute.For<IPlanningProvider>();
        provider
            .ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(CreateResult()));

        PlanningExplorerViewModel viewModel = new(provider);

        await viewModel.LoadAsync(new PlanningReadRequest
        {
            RepositoryRootPath = "C:\\Repo",
            PlanningRootPath = "DevOps",
        });

        Assert.AreEqual(1, viewModel.RootItems.Count);
        Assert.AreEqual("AA98-E12", viewModel.RootItems[0].Id);
        Assert.IsTrue(viewModel.CategoryGroups.Count >= 2);
        Assert.IsTrue(viewModel.CategoryGroups.Any(static group => group.Name == PlanningItemKind.Epic.ToString()));
        Assert.IsTrue(viewModel.CategoryGroups.Any(static group => group.Name == PlanningItemKind.Feature.ToString()));

        viewModel.ShowCategoryViewCommand.Execute(null);
        Assert.IsTrue(viewModel.IsCategoryMode);
        StringAssert.Contains(viewModel.ExplorerStatusText, "Category");
    }

    /// <summary>
    /// Verifies that properties expose read-only and editable fields and update selected item metadata.
    /// </summary>
    [TestMethod]
    public async Task SelectedItem_ExposesEditablePropertiesAndUpdatesTitle()
    {
        IPlanningProvider provider = Substitute.For<IPlanningProvider>();
        provider
            .ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(CreateResult()));

        PlanningExplorerViewModel viewModel = new(provider);

        await viewModel.LoadAsync(new PlanningReadRequest
        {
            RepositoryRootPath = "C:\\Repo",
            PlanningRootPath = "DevOps",
        });

        IPropertyItem? titleProperty = viewModel.Properties.SingleOrDefault(static property => property.Name == "Title");
        IPropertyItem? idProperty = viewModel.Properties.SingleOrDefault(static property => property.Name == "Id");

        Assert.IsNotNull(titleProperty);
        Assert.IsNotNull(idProperty);
        Assert.IsTrue(titleProperty!.IsEditable);
        Assert.IsTrue(idProperty!.IsReadOnly);

        titleProperty.Value = "Updated Epic Title";

        Assert.AreEqual("Updated Epic Title", viewModel.SelectedItemTitle);
        StringAssert.Contains(viewModel.ExplorerStatusText, "Selected: AA98-E12");
    }

    /// <summary>
    /// Verifies that an empty planning result clears explorer state without selecting an item.
    /// </summary>
    [TestMethod]
    public async Task LoadAsync_EmptyResult_LeavesExplorerEmptyAndUnselected()
    {
        IPlanningProvider provider = Substitute.For<IPlanningProvider>();
        provider
            .ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(new PlanningReadResult()));

        PlanningExplorerViewModel viewModel = new(provider);

        await viewModel.LoadAsync(new PlanningReadRequest());

        Assert.AreEqual(0, viewModel.RootItems.Count);
        Assert.AreEqual(0, viewModel.CategoryGroups.Count);
        Assert.AreEqual(0, viewModel.Diagnostics.Count);
        Assert.IsNull(viewModel.SelectedItem);
        StringAssert.Contains(viewModel.StatusText, "Loaded 0 planning items. Diagnostics: 0.");
        StringAssert.Contains(viewModel.ExplorerStatusText, "Selected: none");
    }

    /// <summary>
    /// Verifies that read and item diagnostics remain visible when planning data is incomplete.
    /// </summary>
    [TestMethod]
    public async Task LoadAsync_DiagnosticsAreReported_ExposesDiagnosticsAndKeepsValidItemsBrowsable()
    {
        IPlanningProvider provider = Substitute.For<IPlanningProvider>();
        provider
            .ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(CreateDiagnosticResult()));

        PlanningExplorerViewModel viewModel = new(provider);

        await viewModel.LoadAsync(new PlanningReadRequest());

        Assert.AreEqual(1, viewModel.RootItems.Count);
        Assert.AreEqual("AA98-T064", viewModel.SelectedItemId);
        Assert.AreEqual(2, viewModel.Diagnostics.Count);
        CollectionAssert.AreEquivalent(
            new[] { "PLN001", "PLN002" },
            viewModel.Diagnostics.Select(static diagnostic => diagnostic.Code).ToArray());
        StringAssert.Contains(viewModel.StatusText, "Loaded 1 planning items. Diagnostics: 2.");
    }

    /// <summary>
    /// Verifies that reloading restores the latest provider state and discards local document edits.
    /// </summary>
    [TestMethod]
    public async Task ReloadAsync_DiscardsLocalDocumentEditsAndReloadsProviderState()
    {
        IPlanningProvider provider = Substitute.For<IPlanningProvider>();
        PlanningReadResult initialResult = CreateResult();
        PlanningReadResult reloadedResult = CreateResult();
        reloadedResult.Items[0].DocumentText = "# AA98-E12 Reloaded Document";
        provider
            .ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(initialResult), Task.FromResult(reloadedResult));

        PlanningExplorerViewModel viewModel = new(provider);
        await viewModel.LoadAsync(new PlanningReadRequest
        {
            RepositoryRootPath = "C:\\Repo",
            PlanningRootPath = "DevOps",
        });
        viewModel.SelectedItemDocumentText = "# AA98-E12 Local Edit";

        await viewModel.ReloadCommand.ExecuteAsync(null);

        Assert.AreEqual("# AA98-E12 Reloaded Document", viewModel.SelectedItemDocumentText);
        StringAssert.Contains(viewModel.StatusText, "Local changes were discarded.");
        await provider.Received(2).ReadAsync(
            Arg.Is<PlanningReadRequest>(request =>
                request.RepositoryRootPath == "C:\\Repo" &&
                request.PlanningRootPath == "DevOps"),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Verifies that saving the selected item writes its edited document and metadata through the provider contract.
    /// </summary>
    [TestMethod]
    public async Task SaveSelectedItemAsync_WritesSelectedItemThroughProvider()
    {
        IPlanningProvider provider = Substitute.For<IPlanningProvider>();
        PlanningReadResult readResult = CreateResult();
        readResult.RepositoryRootPath = "C:\\Repo";
        readResult.PlanningRootPath = "C:\\Repo\\DevOps";
        provider
            .ReadAsync(Arg.Any<PlanningReadRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(readResult));
        provider
            .WriteAsync(Arg.Any<PlanningWriteRequest>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult(new PlanningWriteResult()));

        PlanningExplorerViewModel viewModel = new(provider);
        await viewModel.LoadAsync(new PlanningReadRequest
        {
            RepositoryRootPath = "C:\\Repo",
            PlanningRootPath = "DevOps",
        });
        viewModel.SelectedItemDocumentText = "# AA98-E12 Updated Document";
        viewModel.SelectedItem!.Status = PlanningItemStatus.Completed;

        await viewModel.SaveSelectedItemCommand.ExecuteAsync(null);

        await provider.Received(1).WriteAsync(
            Arg.Is<PlanningWriteRequest>(request =>
                request.RepositoryRootPath == "C:\\Repo" &&
                request.PlanningRootPath == "DevOps" &&
                request.Items.Single().Id == "AA98-E12" &&
                request.Items.Single().DocumentText == "# AA98-E12 Updated Document" &&
                request.Items.Single().Status == PlanningItemStatus.Completed &&
                request.ExpectedDocumentTexts["DevOps\\Epics\\AA98-E12-DevOps-Planning-Workbench.md"] == "# AA98-E12 DevOps Planning Workbench"),
            Arg.Any<CancellationToken>());
        Assert.AreEqual("Saved AA98-E12.", viewModel.StatusText);
    }

    private static PlanningReadResult CreateResult()
    {
        PlanningItem epic = new()
        {
            Id = "AA98-E12",
            Title = "DevOps Planning Workbench",
            Kind = PlanningItemKind.Epic,
            Status = PlanningItemStatus.InProgress,
            SourcePath = "DevOps\\Epics\\AA98-E12-DevOps-Planning-Workbench.md",
            DocumentText = "# AA98-E12 DevOps Planning Workbench",
        };

        PlanningItem feature = new()
        {
            Id = "AA98-F43",
            Title = "Repository and Planning Workflows",
            Kind = PlanningItemKind.Feature,
            Status = PlanningItemStatus.InProgress,
            SourcePath = "DevOps\\Features\\AA98-F43-Repository-and-Planning-Workflows.md",
            DocumentText = "# AA98-F43 Repository and Planning Workflows",
            Parent = new PlanningItemLink
            {
                ItemId = "AA98-E12",
                Kind = PlanningItemKind.Epic,
            },
        };

        PlanningReadResult result = new();
        result.Items.Add(epic);
        result.Items.Add(feature);
        return result;
    }

    private static PlanningReadResult CreateDiagnosticResult()
    {
        PlanningItem task = new()
        {
            Id = "AA98-T064",
            Title = "Add Planning UI Tests",
            Kind = PlanningItemKind.Task,
            Status = PlanningItemStatus.Proposed,
            SourcePath = "DevOps\\Tasks\\AA98-T064-Add-Planning-UI-Tests.md",
        };
        task.Diagnostics.Add(new Diagnostic
        {
            Code = "PLN002",
            Message = "The parent reference could not be resolved.",
            Severity = DiagnosticSeverity.Warning,
            SourcePath = task.SourcePath,
        });

        PlanningReadResult result = new();
        result.Diagnostics.Add(new Diagnostic
        {
            Code = "PLN001",
            Message = "A planning document is incomplete.",
            Severity = DiagnosticSeverity.Warning,
        });
        result.Items.Add(task);
        return result;
    }
}
