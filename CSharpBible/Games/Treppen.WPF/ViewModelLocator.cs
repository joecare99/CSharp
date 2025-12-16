using BaseLib.Helper;
using Microsoft.Extensions.DependencyInjection;
using Treppen.Base;
using Treppen.Export.Services.Interfaces;
using Treppen.Print.Services;
using Treppen.WPF.Services;
using Treppen.WPF.Services.Drawing;
using Treppen.WPF.Services.Interfaces;
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
        services.AddKeyedSingleton<IDrawCommandFactory, DrawContextDrawingFactory>("dc");
        services.AddKeyedSingleton<IPrintRenderer, PrintRenderer>("prn");
        
        _provider = services.BuildServiceProvider();
        IoC.Configure(_provider);
    }

    public MainWindowViewModel MainWindowViewModel => _provider.GetRequiredService<MainWindowViewModel>();
}
