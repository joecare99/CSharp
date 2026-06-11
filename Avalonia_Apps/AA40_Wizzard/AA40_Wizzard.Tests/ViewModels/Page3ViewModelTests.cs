using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using CommunityToolkit.Mvvm.Messaging;

namespace AA40_Wizzard.Tests.ViewModels;

[TestClass]
public class Page3ViewModelTests
{
    [TestMethod]
    public void ClearResetsAllAdditionalSelectionsTest()
    {
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var viewModel = new Page3ViewModel(model, new FakeWizardContentService(), new WeakReferenceMessenger())
        {
            Additional1 = new ListEntry(10, "Ten"),
            Additional2 = new ListEntry(12, "Twelve"),
            Additional3 = new ListEntry(13, "Thirteen"),
        };

        viewModel.ClearCommand.Execute(null);

        Assert.AreEqual(-1, model.Additional1);
        Assert.AreEqual(-1, model.Additional2);
        Assert.AreEqual(-1, model.Additional3);
    }
}
