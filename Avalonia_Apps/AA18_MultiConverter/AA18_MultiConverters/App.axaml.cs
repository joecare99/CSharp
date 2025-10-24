using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AA18_MultiConverter;

public partial class App : Application
{
 public static IServiceProvider Services { get; private set; } = default!;

 public override void Initialize()
 {
 AvaloniaXamlLoader.Load(this);
 }

 public override void OnFrameworkInitializationCompleted()
 {
 var services = new ServiceCollection();
 ConfigureServices(services);
 Services = services.BuildServiceProvider();

 if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
 {
 var mainWindow = Services.GetRequiredService<MainWindow>();
 desktop.MainWindow = mainWindow;
 }

 base.OnFrameworkInitializationCompleted();
 }

 private static void ConfigureServices(IServiceCollection services)
 {
 // ViewModels
 services.AddTransient<ViewModels.DateDifViewModel>();

 // Views
 services.AddTransient<MainWindow>();
 services.AddTransient<View.DateDifView>();
 }
}
