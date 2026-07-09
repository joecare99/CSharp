using AA98_AvlnCodeStudio.Base.UI.Properties;
using AA98_AvlnCodeStudio.Planning.Core.Models;
using AA98_AvlnCodeStudio.Planning.Core.Services;
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
}
