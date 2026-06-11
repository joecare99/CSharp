using System.Collections.Generic;
using System.Globalization;
using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AA40_Wizzard.Tests.ViewModels;

[TestClass]
public class Page4ViewModelTests
{
    [TestMethod]
    public void SummaryUsesLocalizedContentKeysTest()
    {
        var messenger = new WeakReferenceMessenger();
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>())
        {
            MainSelection = 1,
            SubSelection = 2,
            Additional1 = 10,
            Additional2 = 12,
            Additional3 = 13,
        };
        var viewModel = new Page4ViewModel(model, new FakeWizardContentService(), messenger);

        Assert.AreEqual("text:MainSelection1", viewModel.MainSelection);
        Assert.AreEqual("text:SubSelection2", viewModel.SubSelection);
        Assert.AreEqual("text:AdditSelection10", viewModel.Additional1);
    }

    [TestMethod]
    public void ReceiveRaisesSelectionChangesTest()
    {
        var messenger = new WeakReferenceMessenger();
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var viewModel = new Page4ViewModel(model, new FakeWizardContentService(), messenger);
        var changed = new List<string>();
        viewModel.PropertyChanged += (_, args) => changed.Add(args.PropertyName!);

        messenger.Send(new ValueChangedMessage<CultureInfo>(CultureInfo.InvariantCulture));

        CollectionAssert.Contains(changed, nameof(Page4ViewModel.MainSelection));
        CollectionAssert.Contains(changed, nameof(Page4ViewModel.Additional3));
    }
}
