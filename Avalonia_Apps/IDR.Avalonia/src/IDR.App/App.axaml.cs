using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using IDR.App.Services;
using IDR.App.ViewModels;
using IDR.Core.Services;
using IDR.Infrastructure.Disassembly;
using IDR.Infrastructure.KnowledgeBase;
using IDR.Infrastructure.Pe;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IDR.App;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            TopLevel? topLevel = null;
            Services = CreateServices(() => topLevel);
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
            topLevel = desktop.MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static ServiceProvider CreateServices(Func<TopLevel?>? topLevelProvider = null)
    {
        topLevelProvider ??= static () => null;
        ServiceCollection services = new();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<IPeImageLoader, PeImageLoader>();
        services.AddSingleton<IDelphiVersionDetector, DelphiVersionDetector>();
        services.AddSingleton<IKnowledgeBaseProvider, KnowledgeBaseReader>();
        services.AddSingleton<IKnowledgeBasePathResolver>(_ =>
            new KnowledgeBasePathResolver(
                Environment.GetEnvironmentVariable("IDR_KNOWLEDGE_BASE_DIRECTORY")
                    ?? AppContext.BaseDirectory));
        services.AddSingleton<IInstructionDecoder, IcedInstructionDecoder>();
        services.AddSingleton<IAnalysisService, BasicAnalysisService>();
        services.AddSingleton<IFileSelectionService>(
            _ => new AvaloniaFileSelectionService(topLevelProvider));
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });
    }
}
