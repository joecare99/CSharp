// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 08-11-2022
//
// Last Modified By : Mir
// Last Modified On : 08-24-2022
// ***********************************************************************
// <copyright file="App.xaml.cs" company="JC-Soft">
//     Copyright © JC-Soft 2022
// </copyright>
// <summary></summary>
// ***********************************************************************
using Microsoft.Extensions.DependencyInjection;
using MVVM_40_Wizzard.Models;
using BaseLib.Interfaces;
using System.Windows;
using MVVM.View.Extension;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Globalization;
using System.Threading;
using System;

namespace MVVM_40_Wizzard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application, IRecipient<ValueChangedMessage<CultureInfo>>
{
    private WindowState? ws;
    private double? tp;
    private double? lf;
    private double? wd;
    private double? hg;

    protected override void OnStartup(StartupEventArgs e)
    {

        IServiceCollection services = new ServiceCollection()
            .AddSingleton<IWizzardModel, WizzardModel>()
            .AddSingleton<IMessenger, WeakReferenceMessenger>()
            .AddTransient<ISysTime, SysTime>()
            .AddSingleton<ILog, SimpleLog>();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        IoC.Configure(serviceProvider);

        base.OnStartup(e);

        IoC.GetRequiredService<IMessenger>().Register<ValueChangedMessage<CultureInfo>>(this);
    }

    public void Receive(ValueChangedMessage<CultureInfo> message)
    {
        Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        if (Current.MainWindow != null)
            Current.MainWindow.Closing += Wnd_Closed;
        Thread.CurrentThread.CurrentCulture = message.Value;
        Thread.CurrentThread.CurrentUICulture = message.Value;
        ws = Current.MainWindow?.WindowState;
        tp = Current.MainWindow?.Top;
        lf = Current.MainWindow?.Left;
        wd = Current.MainWindow?.Width;
        hg = Current.MainWindow?.Height;
        Current.MainWindow?.Close();

    }
    private void Wnd_Closed(object? sender, EventArgs e)
    {
        Current.MainWindow = new MainWindow();
        Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
        Current.MainWindow.Show();
        if (ws != null) Current.MainWindow.WindowState = ws.Value;
        if (tp != null) Current.MainWindow.Top = tp.Value;
        if (lf != null) Current.MainWindow.Left = lf.Value;
        if (wd != null) Current.MainWindow.Width = wd.Value;
        if (hg != null) Current.MainWindow.Height = hg.Value;
    }

}
