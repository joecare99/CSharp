using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;

namespace AA40_Wizzard.Tests.ViewModels;

[TestClass]
public class Page1ViewModelTests
{
    [TestMethod]
    public void SelectionAndContentReflectModelStateTest()
    {
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var viewModel = new Page1ViewModel(model, new FakeWizardContentService(), new WeakReferenceMessenger());

        viewModel.MainSelection = new ListEntry(3, "Three");

        Assert.AreEqual(3, model.MainSelection);
        Assert.IsNotNull(viewModel.Image);
        Assert.IsInstanceOfType<TextBlock>(viewModel.DocumentPreview);
        Assert.AreEqual("docpreview:3", ((TextBlock)viewModel.DocumentPreview!).Text);
    }

    [TestMethod]
    public void ExternalSelectionChangeRefreshesSelectionAndContentTest()
    {
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var viewModel = new Page1ViewModel(model, new FakeWizardContentService(), new WeakReferenceMessenger());

        model.MainSelection = 1;
        model.MainSelection = 3;

        Assert.AreEqual(3, viewModel.MainSelection?.ID);
        Assert.IsInstanceOfType<TextBlock>(viewModel.DocumentPreview);
        Assert.AreEqual("docpreview:3", ((TextBlock)viewModel.DocumentPreview!).Text);
        Assert.IsNotNull(viewModel.Image);
    }

    [TestMethod]
    public void ClearResetsMainSelectionTest()
    {
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var viewModel = new Page1ViewModel(model, new FakeWizardContentService(), new WeakReferenceMessenger())
        {
            MainSelection = new ListEntry(1, "One"),
        };

        viewModel.ClearCommand.Execute(null);

        Assert.AreEqual(-1, model.MainSelection);
        Assert.IsNull(viewModel.MainSelection);
        Assert.IsNull(viewModel.Image);
        Assert.IsNull(viewModel.DocumentPreview);
    }
}
