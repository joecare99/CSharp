using Avalonia;

namespace AA19_FilterLists;

public static class AppBuilderFactory
{
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .WithInterFont()
            .LogToTrace();
}
