using System.Collections.Generic;
using System.Globalization;
using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA40_Wizzard.Tests.ViewModels;

[TestClass]
public class MainWindowViewModelTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
        => TestAppBuilder.EnsureInitialized();

    [TestMethod]
    public void ConstructorExposesWizardAndLocalizedSourcesTest()
    {
        var messenger = new WeakReferenceMessenger();
        var viewModel = new MainWindowViewModel(new FakeWizardContentService(), messenger, CreateWizardViewModel(messenger));

        Assert.IsNotNull(viewModel.Wizzard);
        Assert.AreEqual("text:Title", viewModel.Title);
        Assert.AreEqual("text:WizzardView", viewModel.WizzardViewSource);
        Assert.AreEqual("text:WizzardViewModel", viewModel.WizzardViewModelSource);
    }

    [TestMethod]
    public void ReceiveRefreshesLocalizedPropertiesTest()
    {
        var messenger = new WeakReferenceMessenger();
        var viewModel = new MainWindowViewModel(new FakeWizardContentService(), messenger, CreateWizardViewModel(messenger));
        var changed = new List<string>();
        viewModel.PropertyChanged += (_, args) => changed.Add(args.PropertyName!);

        messenger.Send(new ValueChangedMessage<CultureInfo>(CultureInfo.InvariantCulture));

        CollectionAssert.Contains(changed, nameof(MainWindowViewModel.Title));
        CollectionAssert.Contains(changed, nameof(MainWindowViewModel.Description));
    }

    private static WizzardViewModel CreateWizardViewModel(WeakReferenceMessenger messenger)
    {
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var content = new FakeWizardContentService();
        return new WizzardViewModel(
            model,
            messenger,
            content,
            new Page1ViewModel(model, content, messenger),
            new Page2ViewModel(model, content, messenger),
            new Page3ViewModel(model, content, messenger),
            new Page4ViewModel(model, content, messenger));
    }
}
