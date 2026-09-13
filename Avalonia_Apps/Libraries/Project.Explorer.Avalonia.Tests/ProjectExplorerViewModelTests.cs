using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Project.Explorer;
using Project.Explorer.Avalonia.ViewModels;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Project.Explorer.Avalonia.Tests;

/// <summary>Tests reusable project explorer presentation without a concrete UI host.</summary>
[TestClass]
public sealed class ProjectExplorerViewModelTests
{
    [TestMethod]
    public async Task LoadAsync_RestoresSelectionAndExpansionByStableIdentity()
    {
        ProjectExplorerItem file = new("file:readme", "README.md", Path.GetTempFileName(), ProjectExplorerItemKind.File);
        ProjectExplorerItem folder = new("folder:docs", "docs", Path.GetTempPath(), ProjectExplorerItemKind.Folder, children: [file]);
        IProjectExplorerSource source = Substitute.For<IProjectExplorerSource>();
        source.RefreshAsync(Arg.Any<string>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns(new ProjectExplorerSnapshot(Path.GetTempPath(), [folder]));
        ProjectExplorerState state = new();
        state.Select(file.Id);
        state.SetExpanded(folder.Id, true);
        IProjectExplorerItemOpener opener = Substitute.For<IProjectExplorerItemOpener>();
        ProjectExplorerViewModel viewModel = new(source, state, opener);

        await viewModel.LoadAsync(Path.GetTempPath());

        Assert.AreEqual(file.Id, viewModel.SelectedItem?.Id);
        Assert.IsTrue(viewModel.RootItems[0].IsExpanded);
        await opener.Received(1).OpenAsync(file, Arg.Any<System.Threading.CancellationToken>());
    }

    [TestMethod]
    public async Task OpenAsync_DoesNotActivateFolderOrUnavailableFile()
    {
        IProjectExplorerSource source = Substitute.For<IProjectExplorerSource>();
        IProjectExplorerItemOpener opener = Substitute.For<IProjectExplorerItemOpener>();
        ProjectExplorerViewModel viewModel = new(source, new ProjectExplorerState(), opener);
        ProjectExplorerItem folder = new("folder:root", "root", Path.GetTempPath(), ProjectExplorerItemKind.Folder);
        ProjectExplorerItem unavailable = new("file:missing", "missing.cs", Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N")), ProjectExplorerItemKind.File, isAvailable: false);

        await viewModel.OpenAsync(new ProjectExplorerItemViewModel(folder));
        await viewModel.OpenAsync(new ProjectExplorerItemViewModel(unavailable));

        await opener.DidNotReceive().OpenAsync(Arg.Any<ProjectExplorerItem>(), Arg.Any<System.Threading.CancellationToken>());
    }

    [TestMethod]
    public async Task LoadAsync_ExposesSourceFailure()
    {
        IProjectExplorerSource source = Substitute.For<IProjectExplorerSource>();
        source.RefreshAsync(Arg.Any<string>(), Arg.Any<System.Threading.CancellationToken>())
            .Returns<Task<ProjectExplorerSnapshot>>(_ => throw new IOException("Unavailable root."));
        ProjectExplorerViewModel viewModel = new(source, new ProjectExplorerState(), Substitute.For<IProjectExplorerItemOpener>());

        await viewModel.LoadAsync(Path.GetTempPath());

        Assert.AreEqual("Unavailable root.", viewModel.ErrorMessage);
    }
}
