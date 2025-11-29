using Microsoft.Extensions.DependencyInjection;
using Treppen.Base;
using Treppen.WPF.Services;
using Treppen.WPF.ViewModels;

namespace Treppen.WPF;

public class ViewModelLocator
{
    private static ServiceProvider _provider;

    public static void Init()
    {
        var services = new ServiceCollection();
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<IHeightLabyrinth, HeightLabyrinth>();
        services.AddSingleton<IDrawingService, DrawingService>();
        services.AddSingleton<ILabyrinth3dDrawer, Labyrinth3dDrawer>();
        
        _provider = services.BuildServiceProvider();
    }

    public MainWindowViewModel MainWindowViewModel => _provider.GetRequiredService<MainWindowViewModel>();
}
