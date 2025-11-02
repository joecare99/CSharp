using Avalonia;
using Avalonia.Headless;
using AA15_Labyrinth;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

public static class TestAppBuilder
{
 public static AppBuilder BuildAvaloniaApp() => AppBuilder
 .Configure<App>()
 .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}
