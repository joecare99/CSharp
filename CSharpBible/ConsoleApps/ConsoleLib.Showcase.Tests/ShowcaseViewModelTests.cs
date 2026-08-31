using ConsoleLib.Showcase.Services;
using ConsoleLib.Showcase.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Threading.Tasks;

namespace ConsoleLib.Showcase.Tests;

[TestClass]
public sealed class ShowcaseViewModelTests
{
    [TestMethod]
    public void Constructor_SelectsControlsSection()
    {
        var viewModel = new ShowcaseViewModel(Substitute.For<IShowcaseTerminalService>());

        Assert.AreEqual("Controls", viewModel.SelectedSection?.Name);
        Assert.AreEqual(4, viewModel.Sections.Count);
    }

    [TestMethod]
    public void Commands_UpdateGalleryState()
    {
        var viewModel = new ShowcaseViewModel(Substitute.For<IShowcaseTerminalService>());

        viewModel.ToggleEffectsCommand.Execute(null);
        viewModel.AdvanceProgressCommand.Execute(null);

        Assert.IsTrue(viewModel.EffectsRunning);
        Assert.AreEqual(10, viewModel.Progress);
    }

    [TestMethod]
    public async Task LaunchTerminalCommand_ReportsProbeOutput()
    {
        var terminal = Substitute.For<IShowcaseTerminalService>();
        terminal.RunProbeAsync(default).Returns(Task.FromResult("probe ok"));
        var viewModel = new ShowcaseViewModel(terminal);

        await viewModel.LaunchTerminalCommand.ExecuteAsync(null);

        Assert.AreEqual("probe ok", viewModel.Status);
    }
}
