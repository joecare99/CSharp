using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.ExtCon;
using ConsoleLib.Interfaces;
using DetectiveGame.ConsoleApp;
using DetectiveGame.Engine.Game;
using DetectiveGame.Engine.Game.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DetectiveGame.Console.Cxaml;

/// <summary>Starts the declarative terminal view with the existing detective-game services.</summary>
public static class Program
{
    /// <summary>Loads the view for a supplied game view model.</summary>
    public static CxamlLoadResult CreateView(
        IGameViewModel viewModel,
        IConsole? console = null,
        IApplication? application = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        return new DetectiveGameCxamlView(viewModel, console, application).Load();
    }

    /// <summary>Provides a self-contained view for existing CXAML sample callers.</summary>
    public static IControl CreateView()
        => CreateView(new GameViewModel(new GameService())).Root;

    public static int Main()
    {
        ServiceCollection services = new();
        services.AddSingleton<IGameService, GameService>();
        services.AddSingleton<IGameSetup>(provider => (IGameSetup)provider.GetRequiredService<IGameService>());
        services.AddTransient<IGameViewModel, GameViewModel>();
        services.AddSingleton<IConsole, ConsoleProxy>();
        services.AddSingleton<IExtendedConsole, ExtendedConsole>();
        services.AddSingleton<IWidgetSet, ConsoleWidgetSet>();
        services.AddSingleton<Application>();
        services.AddSingleton<IApplication>(provider => provider.GetRequiredService<Application>());

        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        Application application = provider.GetRequiredService<Application>();
        IConsole console = provider.GetRequiredService<IConsole>();
        application.Dimension = new System.Drawing.Rectangle(0, 0, Math.Max(80, console.WindowWidth), 50);
        application.BackColor = ConsoleColor.Black;
        application.ForeColor = ConsoleColor.Gray;

        CxamlLoadResult view = CreateView(provider.GetRequiredService<IGameViewModel>(), console, application);
        application.Add(view.Root);
        application.Visible = true;
        application.Draw();
        application.Run();
        return 0;
    }
}
