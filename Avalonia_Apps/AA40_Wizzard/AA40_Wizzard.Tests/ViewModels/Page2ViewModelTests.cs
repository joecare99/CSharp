using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AA40_Wizzard.Tests.ViewModels;

[TestClass]
public class Page2ViewModelTests
{
    [TestMethod]
    public void SelectionReflectsModelStateTest()
    {
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var viewModel = new Page2ViewModel(model, new FakeWizardContentService(), new WeakReferenceMessenger());

        viewModel.SubSelection = new ListEntry(5, "Five");

        Assert.AreEqual(5, model.SubSelection);
        Assert.AreEqual(5, viewModel.SubSelection?.ID);
    }
}
