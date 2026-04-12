using Microsoft.Extensions.DependencyInjection;
using RnzTrauer.Console.ViewModels;
using RnzTrauer.Console.Views;
using RnzTrauer.Core;

var xServices = new ServiceCollection()
    .AddSingleton<IFile, FileProxy>()
    .AddSingleton<IConfigLoader, ConfigLoader>()
    .AddSingleton<IHttpClientProxy, HttpClientProxy>()
    .AddSingleton<IWebDriverFactory, FirefoxWebDriverFactory>()
    .AddTransient<ConsoleOutputView>()
    .AddTransient<RnzTrauerConsoleViewModel>()
    .BuildServiceProvider();

var xView = xServices.GetRequiredService<ConsoleOutputView>();

try
{
    var xConfig = new RnzConfig(xServices.GetRequiredService<IConfigLoader>()).Load(Path.Combine(AppContext.BaseDirectory, "RNZ_Config.json"));
    var xViewModel = xServices.GetRequiredService<RnzTrauerConsoleViewModel>();
    xViewModel.Run(xConfig, args.FirstOrDefault() ?? "");
}
catch (FileNotFoundException ex)
{
    xView.WriteErrorLine(ex.Message);
    xView.WriteErrorLine("Lege eine Datei `RNZ_Config.json` neben die EXE. Eine Vorlage liegt als `RNZ_Config.sample.json` im Projekt.");
}
