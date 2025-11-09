using Avalonia;
using System;

namespace AA09_DialogBoxes;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
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
    public static Func<AppBuilder> GetAppBuilder { get; set; } = ()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
//            .UseReactiveUI();
}
