using Calc32.Models;
using Calc32.ViewModels;
using ConsoleLib;
using ConsoleLib.CommonControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calc32Cons.Cxaml.Tests;

[TestClass]
public sealed class Calc32CxamlIntegrationTests
{
    [TestMethod]
    public void View_BindsExistingCalculatorCommandsWithConfiguredParameters()
    {
        CalculatorViewModel viewModel = new(new CalculatorClass());

        CxamlLoadResult result = Program.CreateView(viewModel);

        GetButton(result, "Number1").Click();
        GetButton(result, "Number2").Click();
        GetButton(result, "Add").Click();
        GetButton(result, "Number3").Click();
        GetButton(result, "Equals").Click();

        Assert.AreEqual(15, viewModel.Accumulator);
        Assert.AreEqual("15", result.NamedControls["Accumulator"].Text);
        Assert.AreEqual("=", result.NamedControls["Operation"].Text);
    }

    private static Button GetButton(CxamlLoadResult result, string name)
        => result.NamedControls[name] as Button
            ?? throw new AssertFailedException($"The '{name}' calculator control was not materialized.");
}
