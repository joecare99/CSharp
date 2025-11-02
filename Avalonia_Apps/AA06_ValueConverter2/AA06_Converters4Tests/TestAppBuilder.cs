// ***********************************************************************
// Assembly         : AA06_Converters_4Tests
// Author           : Joe Care
// Created          : 01-12-2025
//
// Last Modified By : Joe Care
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="TestAppBuilder.cs" company="JC-Soft">
//     Copyright (c) Joe Care All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using AA06_Converters_4;
using Avalonia;
using Avalonia.Headless;

/// <summary>
/// Class TestAppBuilder.
/// </summary>
[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

public class TestAppBuilder
{
    /// <summary>
    /// Builds the avalonia application.
    /// </summary>
    /// <returns>AppBuilder.</returns>
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}