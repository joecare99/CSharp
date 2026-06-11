using AA40_Wizzard.Model;
using AA40_Wizzard.ViewModels;
using AA40_Wizzard.Views;
using CommunityToolkit.Mvvm.Messaging;

namespace AA40_Wizzard.Tests.Views;

[TestClass]
public class ViewCreationTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
        => TestAppBuilder.EnsureInitialized();

    [TestMethod]
    public void MainWindowAndMainViewCanBeCreatedTest()
    {
        var viewModel = CreateMainWindowViewModel();
        var window = new MainWindow(viewModel);
        var view = new MainView(viewModel);

        window.Show();

        Assert.IsNotNull(window.Content);
        Assert.IsNotNull(view.Content);
    }

    [TestMethod]
    public void WizardViewsCanBeCreatedTest()
    {
        var wizard = CreateMainWindowViewModel().Wizzard;
        var page1 = new Page1View { DataContext = wizard.Page1 };
        var page2 = new Page2View { DataContext = wizard.Page2 };
        var page3 = new Page3View { DataContext = wizard.Page3 };
        var page4 = new Page4View { DataContext = wizard.Page4 };
        var wizardView = new WizzardView { DataContext = wizard };

        Assert.IsNotNull(page1.Content);
        Assert.IsNotNull(page2.Content);
        Assert.IsNotNull(page3.Content);
        Assert.IsNotNull(page4.Content);
        Assert.IsNotNull(wizardView.Content);
    }

    private static MainWindowViewModel CreateMainWindowViewModel()
    {
        var messenger = new WeakReferenceMessenger();
        var model = new WizzardModel(new SystemClock(), NSubstitute.Substitute.For<ILogSink>());
        var content = new FakeWizardContentService();
        var wizard = new WizzardViewModel(
            model,
            messenger,
            content,
            new Page1ViewModel(model, content, messenger),
            new Page2ViewModel(model, content, messenger),
            new Page3ViewModel(model, content, messenger),
            new Page4ViewModel(model, content, messenger));
        return new MainWindowViewModel(content, messenger, wizard);
    }
}
