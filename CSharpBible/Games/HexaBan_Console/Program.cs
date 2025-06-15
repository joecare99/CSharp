using HexaBan.Models.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HexaBan_Console;

internal class Program
{
    static void Main(string[] args)
    {
        // Dependency Injection Setup
        var services = new ServiceCollection();
        services.AddSingleton<IHexaBanEngine, HexaBanEngine2>();
        services.AddSingleton<IHexaBanLevelRepository, HexaBanLevelRepository>();
        services.AddSingleton<HexaBanViewModel>();
        services.AddSingleton<HexaBanConsoleView>();
        var provider = services.BuildServiceProvider();

        var viewModel = provider.GetRequiredService<HexaBanViewModel>();
        var view = provider.GetRequiredService<HexaBanConsoleView>();

        // Main loop
        while (true)
        {
            Console.Clear();
            view.Render((f, b) => { Console.ForegroundColor = f;Console.BackgroundColor = b; },Console.Write);
            Console.ResetColor();
            Console.WriteLine("WASD zum Bewegen, Q zum Beenden:");
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.Q)
                break;
            viewModel.HandleInput(key);
        }
    }
}
