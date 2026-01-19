using AA25_RichTextEdit.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_CommonDialogs.Avalonia;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA25_RichTextEdit;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = default!;

    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = Services.GetRequiredService<MainWindow>();
        }
        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var topLevelAccessor = new TopLevelAccessor();
        services.AddSingleton(topLevelAccessor);
        services.AddSingleton<RichTextEditViewModel>();
        services.AddTransient<MainWindow>();
        services.AddTransient<Views.RichTextEditView>();
        services.AddAvaloniaCommonDialogs(() => topLevelAccessor.Current);
    }
}

public sealed class TopLevelAccessor
{
    public TopLevel? Current { get; set; }
}
