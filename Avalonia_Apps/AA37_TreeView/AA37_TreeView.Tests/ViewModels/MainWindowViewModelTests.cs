using AA37_TreeView.Model;
using AA37_TreeView.ViewModels;
using NSubstitute;

namespace AA37_TreeView.Tests.ViewModels;

[TestClass]
public class MainWindowViewModelTests
{
    [TestMethod]
    public void ConstructorStoresTreeViewModelTest()
    {
        var treeViewModel = new BooksTreeViewModel(Substitute.For<IBooksService>());

        var viewModel = new MainWindowViewModel(treeViewModel);

        Assert.AreSame(treeViewModel, viewModel.BooksTree);
    }

    [TestMethod]
    public void ParameterlessConstructorCreatesTreeViewModelTest()
    {
        var viewModel = new MainWindowViewModel();

        Assert.IsNotNull(viewModel.BooksTree);
        Assert.IsInstanceOfType(viewModel.BooksTree, typeof(BooksTreeViewModel));
    }
}
