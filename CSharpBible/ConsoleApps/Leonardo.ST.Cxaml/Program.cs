using BaseLib.Interfaces;
using BaseLib.Models;
using CommunityToolkit.Mvvm.DependencyInjection;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.ExtCon;
using ConsoleLib.Interfaces;
using Leonardo.Models;
using Leonardo.Models.Interfaces;
using Leonardo.ViewModels;
using Leonardo.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using CxamlApplication = ConsoleLib.CommonControls.Application;

namespace Leonardo.ST.Cxaml;

/// <summary>Starts the CXAML adaptation of the Leonardo terminal application.</summary>
public static class Program
{
    /// <summary>Loads the Leonardo markup for a supplied view model.</summary>
    public static CxamlLoadResult CreateView(ILeonardoViewModel viewModel, IApplication? application = null)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        return new LeonardoCxamlView(viewModel, application).Load();
    }

    /// <summary>Provides a self-contained view for existing CXAML sample callers.</summary>
    public static IControl CreateView()
    {
        LeonardoClass model = new(new LeonardoHttpClient(), new Steganography(new ConsoleProxy()), new ConsoleProxy());
        return CreateView(new LeonardoViewModel(model)).Root;
    }

    public static int Main()
    {
        ServiceCollection services = new();
        services.AddSingleton<BaseLib.Interfaces.IConsole, ConsoleProxy>();
        services.AddSingleton<IHttpClient, LeonardoHttpClient>();
        services.AddSingleton<ISteganography>(provider =>
            new Steganography(provider.GetRequiredService<BaseLib.Interfaces.IConsole>()));
        services.AddSingleton<ILeonardoClass, LeonardoClass>();
        services.AddTransient<ILeonardoViewModel, LeonardoViewModel>();
        services.AddTransient<IOpenFileDialog, LeonardoOpenFileDialog>();
        services.AddTransient<ISaveFileDialog, LeonardoSaveFileDialog>();
        services.AddSingleton<IExtendedConsole, ExtendedConsole>();
        services.AddSingleton<IWidgetSet, ConsoleWidgetSet>();
        services.AddSingleton<CxamlApplication>();
        services.AddSingleton<IApplication>(provider => provider.GetRequiredService<CxamlApplication>());

        using ServiceProvider provider = services.BuildServiceProvider(validateScopes: true);
        Ioc.Default.ConfigureServices(provider);

        CxamlApplication application = provider.GetRequiredService<CxamlApplication>();
        application.Dimension = ConsoleFramework.Canvas.ClipRect;
        application.BorderStyle = ConsoleLib.Data.BorderStyle.Single;
        application.ForeColor = ConsoleColor.Gray;
        application.BackColor = ConsoleColor.DarkGray;
        application.BorderColor = ConsoleColor.Green;

        CxamlLoadResult view = CreateView(provider.GetRequiredService<ILeonardoViewModel>(), application);
        application.Add(view.Root);
        application.Visible = true;
        application.Draw();
        application.Run();
        ConsoleFramework.ExtendedConsole?.Stop();
        return 0;
    }
}
