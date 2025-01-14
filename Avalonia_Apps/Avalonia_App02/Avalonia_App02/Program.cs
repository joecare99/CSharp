// ***********************************************************************
// Assembly         : Avalonia_App02
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-11-2025
// ***********************************************************************
// <copyright file="Program.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using Avalonia;

/// <summary>
/// The Avalonia_App02 namespace.
/// </summary>
namespace Avalonia_App02;

/// <summary>
/// Class Program. This class cannot be inherited.
/// </summary>
public sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    /// <param name="args">The arguments.</param>
    [STAThread]
    public static void Main(string[] args) 
        => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    /// <summary>
    /// Builds the avalonia application.
    /// </summary>
    /// <returns>AppBuilder.</returns>
    public static AppBuilder BuildAvaloniaApp() //{ get; set; } = ()
        => GetAppBuilder();

    /// <summary>
    /// Builds the avalonia application.
    /// </summary>
    /// <returns>AppBuilder.</returns>
    public static Func<AppBuilder> GetAppBuilder{ get; set; } = ()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

}
