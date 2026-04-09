using AmtsblattLoader.Console.ViewModels;
using AmtsblattLoader.Console.Views;
using RnzTrauer.Core;

var xView = new ConsoleOutputView();

try
{
    var xConfig = AmtsblattConfig.Load(Path.Combine(AppContext.BaseDirectory, "Amtsblatt_Cfg.json"));
    var xViewModel = new AmtsblattLoaderConsoleViewModel(xView);
    xViewModel.Run(xConfig);
}
catch (FileNotFoundException ex)
{
    xView.WriteErrorLine(ex.Message);
    xView.WriteErrorLine("Lege eine Datei `Amtsblatt_Cfg.json` neben die EXE. Eine Vorlage liegt als `Amtsblatt_Cfg.sample.json` im Projekt.");
}
