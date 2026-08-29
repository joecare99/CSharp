using ConsoleLib.CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Leonardo.ST.Cxaml.Tests;

[TestClass]
public sealed class LeonardoCxamlIntegrationTests
{
    [TestMethod]
    public void View_UsesTheExistingLeonardoCommandsAndTerminalAdapter()
    {
        var root = Program.CreateView();
        Panel panel = root as Panel
            ?? throw new AssertFailedException("The Leonardo CXAML root must be a panel.");

        Assert.IsInstanceOfType<Terminal>(panel.Children.Single(control => control.Text == string.Empty));
        Assert.IsNotNull(((Button)panel.Children.Single(control => control.Text == "Encode")).Command);
        Assert.IsNotNull(((Button)panel.Children.Single(control => control.Text == "Decode")).Command);
        Assert.IsNotNull(((Button)panel.Children.Single(control => control.Text == "Generate")).Command);
        Assert.IsNotNull(((Button)panel.Children.Single(control => control.Text == "Test")).Command);
    }
}
