using DetectiveGame.ConsoleApp;
using DetectiveGame.Console.Views;
using DetectiveGame.Engine.Game;
using DetectiveGame.Engine.Game.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using ConsoleLib.CommonControls;
using BaseLib.Interfaces;
using BaseLib.Models;
using ConsoleLib.Interfaces;
using ConsoleLib;

var services = new ServiceCollection();

services.AddSingleton<IGameService,GameService>();
services.AddSingleton<IGameSetup>(sp => sp.GetRequiredService<GameService>());
services.AddTransient<IGameViewModel,GameViewModel>();
services.AddSingleton<GameView>();
services.AddSingleton<IConsole,ConsoleProxy>();
services.AddSingleton<IExtendedConsole,ExtendedConsole>();
services.AddSingleton<IApplication,Application>();

var provider = services.BuildServiceProvider();

var view = provider.GetRequiredService<GameView>();
view.Run();