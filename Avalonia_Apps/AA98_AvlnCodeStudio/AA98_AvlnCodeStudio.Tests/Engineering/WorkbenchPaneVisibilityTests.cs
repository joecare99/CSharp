using AA98_AvlnCodeStudio.Diagnostics.UI.ViewModels;
using AA98_AvlnCodeStudio.Planning.Core.Services;
using AA98_AvlnCodeStudio.UI.Components;
using AA98_AvlnCodeStudio.UI.ViewModels;
using Avalonia.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class WorkbenchPaneVisibilityTests
{
    [TestMethod]
    public void WorkspaceSidebar_CollapsesAndRestoresLastWidth()
    {
        MainWindowViewModel viewModel = CreateViewModel();

        Assert.IsTrue(viewModel.IsWorkspacePaneVisible);
        Assert.AreEqual(new GridLength(280), viewModel.WorkspacePaneWidth);
        Assert.AreEqual(new GridLength(8), viewModel.WorkspacePaneSplitterWidth);

        viewModel.WorkspacePaneWidth = new GridLength(360);
        viewModel.IsWorkspacePaneVisible = false;

        Assert.AreEqual(new GridLength(0), viewModel.WorkspacePaneWidth);
        Assert.AreEqual(new GridLength(0), viewModel.WorkspacePaneSplitterWidth);

        viewModel.IsWorkspacePaneVisible = true;

        Assert.AreEqual(new GridLength(360), viewModel.WorkspacePaneWidth);
        Assert.AreEqual(new GridLength(8), viewModel.WorkspacePaneSplitterWidth);
    }

    [TestMethod]
    public void WorkspaceSidebar_IgnoresCollapsedWidthEcho()
    {
        MainWindowViewModel viewModel = CreateViewModel();
        viewModel.WorkspacePaneWidth = new GridLength(340);
        viewModel.IsWorkspacePaneVisible = false;

        viewModel.WorkspacePaneWidth = new GridLength(0);
        viewModel.IsWorkspacePaneVisible = true;

        Assert.AreEqual(new GridLength(340), viewModel.WorkspacePaneWidth);
    }

    private static MainWindowViewModel CreateViewModel()
    {
        IAvaloniaEditorComponent editorComponent = Substitute.For<IAvaloniaEditorComponent>();
        editorComponent.EditorViewModel.Returns(new EditorViewModel());
        return new MainWindowViewModel(
            editorComponent,
            new PlanningExplorerViewModel(new MarkdownPlanningReader()),
            new DiagnosticCollectionViewModel(),
            null,
            null);
    }
}
