using RnzTrauer.Console.ViewModels;
using RnzTrauer.Console.Views;
using RnzTrauer.Core;

var xView = new ConsoleOutputView();

try
{
    var xConfig = RnzConfig.Load(Path.Combine(AppContext.BaseDirectory, "RNZ_Config.json"));
    var xViewModel = new RnzTrauerConsoleViewModel(xView);
    xViewModel.Run(xConfig);
}
catch (FileNotFoundException ex)
{
    xView.WriteErrorLine(ex.Message);
    xView.WriteErrorLine("Lege eine Datei `RNZ_Config.json` neben die EXE. Eine Vorlage liegt als `RNZ_Config.sample.json` im Projekt.");
}
