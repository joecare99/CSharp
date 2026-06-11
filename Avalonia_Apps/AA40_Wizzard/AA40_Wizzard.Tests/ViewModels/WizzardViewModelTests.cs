using System.Collections.Generic;
using System.Globalization;
using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA40_Wizzard.Tests.ViewModels;

[TestClass]
public class WizzardViewModelTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
        => TestAppBuilder.EnsureInitialized();

    [TestMethod]
    public void TabEnablingAndNavigationFollowModelStateTest()
    {
        var messenger = new WeakReferenceMessenger();
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var content = new FakeWizardContentService();
        var viewModel = new WizzardViewModel(
            model,
            messenger,
            content,
            new Page1ViewModel(model, content, messenger),
            new Page2ViewModel(model, content, messenger),
            new Page3ViewModel(model, content, messenger),
            new Page4ViewModel(model, content, messenger));

        Assert.IsFalse(viewModel.Tab2Enabled);
        model.MainSelection = 0;
        Assert.IsTrue(viewModel.Tab2Enabled);
        model.SubSelection = 1;
        model.Additional1 = 10;
        model.Additional2 = 12;
        model.Additional3 = 13;
        Assert.IsTrue(viewModel.Tab4Enabled);

        viewModel.NextTabCommand.Execute(null);
        Assert.AreEqual(1, viewModel.SelectedTab);
        viewModel.PrevTabCommand.Execute(null);
        Assert.AreEqual(0, viewModel.SelectedTab);
    }

    [TestMethod]
    public void ReceiveRefreshesLocalizedTitlesTest()
    {
        var messenger = new WeakReferenceMessenger();
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var content = new FakeWizardContentService();
        var viewModel = new WizzardViewModel(
            model,
            messenger,
            content,
            new Page1ViewModel(model, content, messenger),
            new Page2ViewModel(model, content, messenger),
            new Page3ViewModel(model, content, messenger),
            new Page4ViewModel(model, content, messenger));
        var changed = new List<string>();
        viewModel.PropertyChanged += (_, args) => changed.Add(args.PropertyName!);

        messenger.Send(new ValueChangedMessage<CultureInfo>(CultureInfo.InvariantCulture));

        CollectionAssert.Contains(changed, nameof(WizzardViewModel.Title));
        CollectionAssert.Contains(changed, nameof(WizzardViewModel.Page4Header));
    }
}
