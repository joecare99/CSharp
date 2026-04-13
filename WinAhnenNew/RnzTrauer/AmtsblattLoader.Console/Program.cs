using Microsoft.Extensions.DependencyInjection;
using AmtsblattLoader.Console.ViewModels;
using AmtsblattLoader.Console.Views;
using RnzTrauer.Core;

var xServices = new ServiceCollection()
    .AddSingleton<IFile, FileProxy>()
    .AddSingleton<IConfigLoader, ConfigLoader>()
    .AddTransient<ConsoleOutputView>()
    .AddTransient<AmtsblattLoaderConsoleViewModel>()
    .BuildServiceProvider();

var xView = xServices.GetRequiredService<ConsoleOutputView>();

try
{
    var xConfig = new AmtsblattConfig(xServices.GetRequiredService<IConfigLoader>()).Load(Path.Combine(AppContext.BaseDirectory, "Amtsblatt_Cfg.json"));
    var xViewModel = xServices.GetRequiredService<AmtsblattLoaderConsoleViewModel>();
    xViewModel.Run(xConfig);
}
catch (FileNotFoundException ex)
{
    xView.WriteErrorLine(ex.Message);
    xView.WriteErrorLine("Lege eine Datei `Amtsblatt_Cfg.json` neben die EXE. Eine Vorlage liegt als `Amtsblatt_Cfg.sample.json` im Projekt.");
}
