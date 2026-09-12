using ConsoleLib.Showcase.Desktop;
using ConsoleLib.Showcase.Apps;
using ConsoleLib.Showcase.Apps.Calendar;
using ConsoleLib.Showcase.Apps.Calculator;
using ConsoleLib.Showcase.Apps.Notepad;
using ConsoleLib.Showcase.Apps.Characters;
using ConsoleLib.Showcase.Apps.Clock;
using ConsoleLib.Showcase.Apps.Terminal;
using ConsoleLib.Rendering;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace ConsoleLib.Showcase.Desktop.Tests;

[TestClass]
public sealed class DesktopPageTests
{
    [TestMethod]
    public void EmbeddedPageLoadsAndExposesNamedControls()
    {
        var viewModel = new DesktopViewModel();
        var result = new DesktopPage().Load(viewModel);

        Assert.AreEqual(string.Empty, result.Root.Text);
        Assert.IsTrue(result.NamedControls.ContainsKey("Calendar"));
        Assert.IsTrue(result.NamedControls.ContainsKey("Calculator"));
        Assert.IsTrue(result.NamedControls.ContainsKey("Notepad"));
        Assert.IsTrue(result.NamedControls.ContainsKey("Characters"));
    }

    [TestMethod]
    public void PageCommandsUpdateBoundStatus()
    {
        var viewModel = new DesktopViewModel();
        var result = new DesktopPage().Load(viewModel);

        result.NamedControls["Calendar"].Click();

        Assert.AreEqual("Calendar requested.", viewModel.Status);
    }

    [TestMethod]
    public void LoadedPageProducesCanonicalFrameAndRefreshesOnBindingChange()
    {
        var viewModel = new DesktopViewModel();
        var result = new DesktopPage().Load(viewModel);
        using var service = new AttachedRenderService();

        service.Attach(result.Root, new Size(80, 25));
        var first = service.GetSnapshot();
        viewModel.Status = "Updated status";
        var second = service.GetSnapshot();

        Assert.IsTrue(second.Revision > first.Revision);
        Assert.IsTrue(second.GetCell(2, 3).Character == 'U');
    }

    [TestMethod]
    public void EveryShowcasePageLoadsFromEmbeddedCXaml()
    {
        var modules = new IShowcaseAppModule[]
        {
            new CalendarAppModule(), new CalculatorAppModule(), new NotepadAppModule(),
            new CharactersAppModule(), new ClockAppModule(), new TerminalAppModule()
        };
        foreach (var module in modules)
        {
            var context = new ShowcaseAppRegistrationContext(new EmptyServiceProvider());
            module.Register(context);
            var descriptor = context.Apps.Values.Single();
            var result = descriptor.LoadPage(new EmptyServiceProvider(), descriptor.CreateViewModel(new EmptyServiceProvider()));
            using var service = new AttachedRenderService();

            Assert.IsNotNull(result.Root, descriptor.Id);
            Assert.IsTrue(result.NamedControls.Count > 0, descriptor.Id);
            service.Attach(result.Root, new Size(80, 25));
            Assert.IsTrue(service.GetSnapshot().Revision > 0, descriptor.Id);
        }
    }

    private sealed class EmptyServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}
