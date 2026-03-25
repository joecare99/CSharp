using BaseLib.Helper;
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleDisplay.View;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Werner_Flaschbier.Console2.View;
using Werner_Flaschbier_Base.Model;
using Werner_Flaschbier_Base.ViewModels;

namespace Werner_Flaschbier.Console2;

public class Programm
{
    private IWernerGame? game;
    private IVisual? visual;

    private void OnStartUp()
    {
        var sc = new ServiceCollection()
            .AddSingleton<IWernerGame, WernerGame>()
            .AddTransient<IWernerViewModel, WernerViewModel>()
            .AddSingleton<ITileDef, VTileDef>()
            .AddSingleton<IVisual, Visual>()
            .AddSingleton<IConsole, ConsoleProxy>();
        
        var sp = sc.BuildServiceProvider();

        IoC.Configure(sp);
    }

    public static void Main(params string[] args)
    {
        var program = new Programm();
        program.Initialize(args);
        program.Run();
    }

    public void Run()
    {
        while (game!.isRunning)
        {
            visual?.CheckUserAction();
            var delay = game.GameStep();
            Thread.Sleep(delay);
        }
    }

    public void Initialize(string[] args)
    {
        OnStartUp();

        game = IoC.GetRequiredService<IWernerGame>();
        visual = IoC.GetRequiredService<IVisual>();
    }
}
