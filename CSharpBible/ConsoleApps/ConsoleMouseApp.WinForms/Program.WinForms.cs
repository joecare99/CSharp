using BaseLib.Interfaces;
using BaseLib.Models;
using CommunityToolkit.Mvvm.DependencyInjection;
using ConsoleLib;
using ConsoleLib.CommonControls;
using ConsoleLib.Interfaces;
using ConsoleLib.WinForms;
using ConsoleMouseApp.ViewModels;
using ConsoleMouseApp.Views;
using Microsoft.Extensions.DependencyInjection;
using BaseLib.Helper;

namespace ConsoleMouseApp;

internal static class ProgramWinForms
{
    [System.STAThread]
    private static void Main(string[] _)
    {
        ServiceProvider sp = new ServiceCollection()
            .AddSingleton<IWidgetSet, WinFormsWidgetSet>()
            .AddSingleton<IConsoleMouseViewModel, ConsoleMouseViewModel>()
            .AddSingleton<Application, ConsoleMouseView>()
            .BuildServiceProvider();

        IoC.Configure(sp);

        Application app = sp.GetRequiredService<Application>();
        app.Visible = true;
        app.Run();
    }
}
