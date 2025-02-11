// // Copyright (c) Microsoft. All rights reserved.
// // Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using Geometry.ViewModels;
using Geometry.ViewModels.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Helper;
using Geometry.Views;

namespace Geometry
{
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
}