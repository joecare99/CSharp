using System;
using Avalonia;

namespace AA37_TreeView;

/// <summary>
/// Provides the shared Avalonia application builder.
/// </summary>
public static class AppBuilderFactory
{
    /// <summary>
    /// Builds the shared Avalonia application.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => GetAppBuilder();

    /// <summary>
    /// Gets or sets the current app builder factory delegate.
    /// </summary>
    public static Func<AppBuilder> GetAppBuilder { get; set; } = CreateAppBuilder;

    private static AppBuilder CreateAppBuilder()
        => AppBuilder.Configure<App>()
            .WithInterFont()
            .LogToTrace();
}
