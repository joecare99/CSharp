// ***********************************************************************
// Assembly: Avln_Sample_Template
// Author     : Mir
// Created     : 01-15-2025
//
// Last Modified By : Mir
// Last Modified On : 01-15-2025
// ***********************************************************************
// <copyright file="App.axaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avln_Sample_Template.Models;
using Avln_Sample_Template.ViewModels;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Avln_Sample_Template;

/// <summary>
/// Interaction logic for App.axaml
/// </summary>
public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Configure Dependency Injection
        var sc = new ServiceCollection()
          .AddSingleton<ITemplateModel, TemplateModel>()
   .AddTransient<TemplateViewModel, TemplateViewModel>();

     var sp = sc.BuildServiceProvider();
  Ioc.Default.ConfigureServices(sp);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
      desktop.MainWindow = new MainWindow
   {
              DataContext = Ioc.Default.GetService<TemplateViewModel>()
         };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
