using ConsoleLib.Showcase.Desktop;
using ConsoleLib.Showcase.Desktop.ViewModels;
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

        Assert.AreEqual("ConsoleLib Desktop", result.Root.Text);
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
        var loader = new ShowcasePageLoader();

        foreach (var page in Enum.GetValues<ShowcasePage>())
        {
            var result = loader.Load(page, CreateViewModel(page));
            using var service = new AttachedRenderService();

            Assert.IsNotNull(result.Root, page.ToString());
            Assert.IsTrue(result.NamedControls.Count > 0, page.ToString());
            service.Attach(result.Root, new Size(80, 25));
            Assert.IsTrue(service.GetSnapshot().Revision > 0, page.ToString());
        }
    }

    private static object CreateViewModel(ShowcasePage page) => page switch
    {
        ShowcasePage.Calendar => new CalendarViewModel(new DateTime(2026, 9, 1)),
        ShowcasePage.Calculator => new CalculatorViewModel(),
        ShowcasePage.Notepad => new NotepadViewModel(),
        ShowcasePage.Characters => new CharactersViewModel(),
        ShowcasePage.Clock => new ClockViewModel(new DateTime(2026, 9, 1, 10, 15, 0)),
        ShowcasePage.Terminal => new TerminalViewModel(),
        _ => new DesktopViewModel()
    };
}
