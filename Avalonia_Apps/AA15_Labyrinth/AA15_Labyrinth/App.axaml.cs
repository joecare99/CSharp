using Microsoft.Extensions.DependencyInjection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using AA15_Labyrinth.Model;
using AA15_Labyrinth.ViewModels;
using AA15_Labyrinth.Views;
using BaseLib.Models.Interfaces;
using BaseLib.Models;

namespace AA15_Labyrinth;

public partial class App : Application
{
 public ServiceProvider? Services { get; private set; }

 public override void Initialize() => AvaloniaXamlLoader.Load(this);

 public override void OnFrameworkInitializationCompleted()
 {
 if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
 {
 var sc = new ServiceCollection();
 sc.AddSingleton<IRandom, CRandom>();
 sc.AddSingleton<ILabyrinthGenerator, LabyrinthGenerator>();
 sc.AddTransient<ILabyrinthViewModel, LabyrinthViewModel>();
 sc.AddTransient<MainWindow>();
 Services = sc.BuildServiceProvider();

 var window = Services.GetRequiredService<MainWindow>();
 desktop.MainWindow = window;
 }
 base.OnFrameworkInitializationCompleted();
 }
}
