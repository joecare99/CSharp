using AA25_RichTextEdit;
using Avalonia;
using Avalonia.Headless;

public static class TestAppBuilder
{
    private static bool _isInitialized;

    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions());

    public static void EnsureInitialized()
    {
        if (_isInitialized)
        {
            return;
        }

        BuildAvaloniaApp().SetupWithoutStarting();
        _isInitialized = true;
    }
}