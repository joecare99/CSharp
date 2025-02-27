// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BaseLib.Helper;
using ImageView.Models.Interfaces;
using ImageView.ViewModels;
using ImageView.ViewModels.Interfaces;
using ImageView.Views;
using ImageViewer.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ImageView;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var sc = new ServiceCollection()
            .AddTransient<MainWindow>()
            .AddTransient<IImageViewerViewModel, ImageViewerViewModel>()
            .AddSingleton<IImageViewerModel, ImageViewerModel>();

        var sp = sc.BuildServiceProvider();
        IoC.Configure(sp);
        MainWindow = IoC.GetRequiredService<MainWindow>();
        base.OnStartup(e);
        MainWindow.Show();
    }

}