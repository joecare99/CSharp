// ***********************************************************************
// Assembly         : AA05_CommandParCalc
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
/// The AA05_CommandParCalc namespace.
/// </summary>
namespace AA05_CommandParCalc;

/// <summary>
/// Provides the shared Avalonia application builder.
/// </summary>
public static class AppBuilderFactory
{
    /// <summary>
    /// Builds the shared Avalonia application.
    /// </summary>
    /// <returns>AppBuilder.</returns>
    public static AppBuilder BuildAvaloniaApp()
        => GetAppBuilder();

    /// <summary>
    /// Gets the application builder factory.
    /// </summary>
    /// <returns>A delegate returning the configured <see cref="AppBuilder"/>.</returns>
    public static Func<AppBuilder> GetAppBuilder = CreateAppBuilder;

    private static AppBuilder CreateAppBuilder()
        => AppBuilder.Configure<App>()
            .WithInterFont()
            .LogToTrace();
}
