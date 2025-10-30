// ***********************************************************************
// Assembly   : Avln_CustomAnimation
// Author : Mir
// Created     : 01-15-2025
// ***********************************************************************
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_CustomAnimation.Models;
using Avln_CustomAnimation.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Avln_CustomAnimation;

public partial class App : Application
{
 public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

  public override void OnFrameworkInitializationCompleted()
    {
    var sc = new ServiceCollection()
   .AddSingleton<IAnimationModel, AnimationModel>()
 .AddTransient<CustomAnimationViewModel>()
.AddTransient<MainWindowViewModel>();

        var sp = sc.BuildServiceProvider();
        Ioc.Default.ConfigureServices(sp);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
  {
       DataContext = Ioc.Default.GetService<MainWindowViewModel>()
  };
   }

        base.OnFrameworkInitializationCompleted();
    }
}