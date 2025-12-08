// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using BaseLib.Helper;
using Brushes.ViewModels;
using Brushes.ViewModels.Interfaces;
using Brushes.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace Brushes;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var sc = new ServiceCollection()
            .AddTransient<SampleViewer>()
            .AddTransient<ISampleViewerViewModel, SampleViewerViewModel>();

        var sp = sc.BuildServiceProvider();
        IoC.Configure(sp);
        var main = IoC.GetRequiredService<SampleViewer>();
        MainWindow = new Window() { Content = main };
        base.OnStartup(e);
        MainWindow.Show();
    }
}