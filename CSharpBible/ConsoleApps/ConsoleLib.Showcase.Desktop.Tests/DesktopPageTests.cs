using ConsoleLib.Showcase.Desktop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ConsoleLib.Showcase.Desktop.Tests;

[TestClass]
public sealed class DesktopPageTests
{
    [TestMethod]
    public void EmbeddedPageLoadsAndExposesNamedControls()
    {
        var viewModel = new DesktopViewModel();
        var result = new DesktopPage().Load(viewModel);

        Assert.AreEqual("Desktop", result.Root.Text);
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
}
