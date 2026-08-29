using ConsoleLib;
using ConsoleLib.CommonControls;
using DetectiveGame.ConsoleApp;
using DetectiveGame.Engine.Game;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DetectiveGame.Console.Cxaml.Tests;

[TestClass]
public sealed class DetectiveGameCxamlIntegrationTests
{
    [TestMethod]
    public void View_StartsGameAndExecutesTheExistingSuggestionCommand()
    {
        GameViewModel viewModel = new(new GameService());
        CxamlLoadResult result = Program.CreateView(viewModel);

        GetButton(result, "Start").Click();
        GetButton(result, "Suggest").Click();

        Assert.IsTrue(viewModel.History.Cast<string>().Any(item => item.StartsWith("V:", StringComparison.Ordinal)));
        StringAssert.Contains(result.NamedControls["SuggestionLimitation"].Text, "Person1");
        StringAssert.Contains(result.NamedControls["SuggestionLimitation"].Text, "Weapon1");
        StringAssert.Contains(result.NamedControls["SuggestionLimitation"].Text, "Room1");
    }

    private static Button GetButton(CxamlLoadResult result, string name)
        => result.NamedControls[name] as Button
            ?? throw new AssertFailedException($"The '{name}' detective-game control was not materialized.");
}
