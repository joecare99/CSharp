using Db.Core.Abstractions.Sql.Interfaaces;
using Db.Provider.MySql;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Models.Interfaces;
using BaseLib.Models;
using RnzTrauer.Console.Configuration;
using RnzTrauer.Console.ViewModels;
using RnzTrauer.Console.Views;
using RnzTrauer.Core;
using RnzTrauer.Core.Services.Interfaces;
using RnzTrauer.WebDriver.Edge;
using RnzTrauer.WebDriver.Firefox;

try
{
    var xConfig = new RnzConsoleConfigurationLoader().Load();

    var xServices = new ServiceCollection()
        .AddSingleton<IFile,FileProxy>()
        .AddSingleton(xConfig)
        .AddSingleton<IDbConnectionFactory, MySqlDbConnectionFactory>()
        .AddSingleton<IHttpClientProxy, HttpClientProxy>()
        .AddFirefoxWebDriver()
        .AddEdgeWebDriver()
        .AddSingleton<IWebDriverFactory, BrowserWebDriverFactory>()
        .AddTransient<ConsoleOutputView>()
        .AddTransient<RnzTrauerConsoleViewModel>()
        .BuildServiceProvider();

    var xViewModel = xServices.GetRequiredService<RnzTrauerConsoleViewModel>();
    xViewModel.Run(xConfig, args.FirstOrDefault() ?? "");
}
catch (InvalidOperationException ex)
{
    var xView = new ConsoleOutputView();
    xView.WriteErrorLine(ex.Message);
    xView.WriteErrorLine("Lege Standardwerte in App.config unter appSettings ab und hinterlege Geheimnisse per User Secrets, z. B. `dotnet user-secrets set \"RnzConfig:Password\" \"...\"`.");
}
