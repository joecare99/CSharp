using BaseLib.Interfaces;
using BaseLib.Models;
using Calc32.Models;
using Calc32.Models.Interfaces;
using Calc32.ViewModels;
using Calc32.ViewModels.Interfaces;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.ExtCon;
using ConsoleLib.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Calc32Cons.Cxaml;

/// <summary>Starts the CXAML calculator using the existing calculator model and view model.</summary>
public static class Program
{
    /// <summary>Loads the calculator markup for a supplied application view model.</summary>
    public static CxamlLoadResult CreateView(ICalculatorViewModel viewModel, IApplication? application = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        return new CalcCxamlView(viewModel, application).Load();
    }

    /// <summary>Provides a self-contained view for existing CXAML sample callers.</summary>
    public static IControl CreateView()
        => CreateView(new CalculatorViewModel(new CalculatorClass())).Root;

    public static int Main()
    {
        ServiceCollection services = new();
        services.AddSingleton<ICalculatorClass, CalculatorClass>();
        services.AddTransient<ICalculatorViewModel, CalculatorViewModel>();
        services.AddTransient<IConsole, ConsoleProxy>();
        services.AddSingleton<IExtendedConsole, ExtendedConsole>();
        services.AddSingleton<IWidgetSet, ConsoleWidgetSet>();
        services.AddSingleton<Application>();
        services.AddSingleton<IApplication>(provider => provider.GetRequiredService<Application>());

        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        Application application = provider.GetRequiredService<Application>();
        application.Dimension = ConsoleFramework.Canvas.ClipRect;
        application.ForeColor = ConsoleColor.DarkBlue;
        application.BackColor = ConsoleColor.Black;
        application.BorderColor = ConsoleColor.Blue;

        CxamlLoadResult view = CreateView(provider.GetRequiredService<ICalculatorViewModel>(), application);
        application.Add(view.Root);
        application.Visible = true;
        application.Draw();
        application.Run();
        ConsoleFramework.ExtendedConsole?.Stop();
        return 0;
    }
}
